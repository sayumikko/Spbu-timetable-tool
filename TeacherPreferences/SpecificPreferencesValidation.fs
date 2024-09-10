module TeacherPreferences.SpecificPreferencesValidation

open ExcelTimetableParser.Timetable
open TeacherPreferences

let stringContainsIgnoreCase (substring: string) (str: string) : bool =
    str.IndexOf(substring, System.StringComparison.OrdinalIgnoreCase) >= 0

let convertActivityTypeToClassType (activityType: ClassType) : ActivityType =
    match activityType with
    | Lecture -> ExcelTimetableParser.Timetable.Lecture
    | Practical -> ExcelTimetableParser.Timetable.Practical
    | Seminar -> ExcelTimetableParser.Timetable.Seminar
    | Laboratory -> ExcelTimetableParser.Timetable.Laboratory
    | NoClassType -> ExcelTimetableParser.Timetable.NoClassType

let formatTeacherName (teacher: Teacher) =
    let patronymic = 
        match teacher.Name.Patronymic with
        | Some patr -> patr[0].ToString() + "."
        | None -> ""
    $"{teacher.Name.Surname} {teacher.Name.Name[0]}. {patronymic}"


let validateSpecificPreferences (teacher: Teacher) (abbreviatedName: string) (timetable: Timetable) =
    teacher.Preferences
    |> List.collect (function
        | SpecificPreference (group, (subject, classType), Audience (audience, priority)) ->
            let matchingSlots =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((_, timetableGroup), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains abbreviatedName 
                    && timetableGroup = group 
                    && stringContainsIgnoreCase subject slot.Subject
                    && slot.Audience = audience
                    && slot.ClassType = convertActivityTypeToClassType classType)

            match priority with 
            | Mandatory when Seq.isEmpty matchingSlots -> [ $"{formatTeacherName teacher}: аудитория {audience} обязательна для предмета \"{subject}\" группы {group}" ]
            | Desirable when Seq.isEmpty matchingSlots -> [ $"{formatTeacherName teacher}: аудитория {audience} желательна для предмета \"{subject}\" группы {group}" ]
            | NotDesirable when not (Seq.isEmpty matchingSlots) -> [ $"{formatTeacherName teacher}: аудитория {audience} не желательна для предмета \"{subject}\" группы {group}" ]
            | Avoidable when not (Seq.isEmpty matchingSlots) -> [ $"{formatTeacherName teacher}: аудитория {audience} недопустима для предмета \"{subject}\" группы {group}" ]
            | _ -> []

        | SpecificPreference (group, (subject, classType), Equipment (equipment, priority)) -> [] // Не хватает информации о том, какая аудитория, чем оборудована

        | SpecificPreference (group, (subject, classType), UniteGroups (priority)) ->
            let matchingSlots =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((_, timetableGroup), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains abbreviatedName 
                    && stringContainsIgnoreCase subject slot.Subject
                    && slot.ClassType = convertActivityTypeToClassType classType
                    && timetableGroup <> group) // Так как сравниваем разные группы

            let conflictingSlots =
                matchingSlots
                |> Seq.filter (fun kvp ->
                    let time, _ = kvp.Key
                    timetable
                    |> Seq.exists (fun innerKvp ->
                        let otherTime, otherGroup = innerKvp.Key
                        let otherSlot = innerKvp.Value
                        otherGroup = group
                        && otherTime = time
                        && otherSlot.Subject = subject
                        && otherSlot.ClassType = convertActivityTypeToClassType classType))

            match priority with 
            | Mandatory when Seq.isEmpty conflictingSlots -> [ $"{formatTeacherName teacher}: необходимо объединить группы для предмета \"{subject}\" с группой {group}, но они не проходят одновременно." ]
            | Desirable when Seq.isEmpty conflictingSlots -> [ $"{formatTeacherName teacher}: желательно объединить группы для предмета \"{subject}\" с группой {group}, но они не проходят одновременно." ]
            | NotDesirable when not (Seq.isEmpty conflictingSlots) -> [ $"{formatTeacherName teacher}: нежелательно объединять группы для предмета \"{subject}\" с группой {group}, но они объединены." ]
            | Avoidable when not (Seq.isEmpty conflictingSlots) -> [ $"{formatTeacherName teacher}: недопустимо объединять группы для предмета \"{subject}\" с группой {group}, но они объединены." ]
            | _ -> []

        | SpecificPreference (group, (subject, classType), UniteClasses (priority)) ->
            let matchingSlotsForGroup =
                timetable
                |> Seq.filter (fun kvp ->
                    let (time, timetableGroup) = kvp.Key
                    let slot = kvp.Value
                    timetableGroup = group 
                    && slot.Teachers |> List.contains abbreviatedName
                    && stringContainsIgnoreCase subject slot.Subject
                    && slot.ClassType = convertActivityTypeToClassType classType)
                |> Seq.sortBy (fun kvp -> let (time, _) = kvp.Key in time.StartTime)

            let conflictingSlots =
                matchingSlotsForGroup
                |> Seq.pairwise
                |> Seq.choose (fun (kvp1, kvp2) ->
                    let ((time1, _), _) = kvp1.Key, kvp1.Value
                    let ((time2, _), _) = kvp2.Key, kvp2.Value
            
                    let (endHour1, endMinute1) = time1.EndTime
                    let (startHour2, startMinute2) = time2.StartTime

                    let areConsecutive = 
                        (startHour2 = endHour1 && startMinute2 = endMinute1 + 10)
                        || (startHour2 = endHour1 + 1 && startMinute2 = (endMinute1 + 10) % 60)

                    match priority with
                    | Mandatory when not areConsecutive ->
                        Some ($"Занятия \"{subject}\" группы {group} должны идти подряд, но есть разрыв.")
                    | Desirable when not areConsecutive ->
                        Some ($"Занятия \"{subject}\" группы {group} желательно должны идти подряд, но есть разрыв.")
                    | NotDesirable when areConsecutive ->
                        Some ($"Занятия \"{subject}\" группы {group} не должны идти подряд, но идут подряд.")
                    | Avoidable when areConsecutive ->
                        Some ($"Занятия \"{subject}\" группы {group} должны быть раздельными, но идут подряд.")
                    | _ -> None)

            conflictingSlots |> Seq.toList 

        | SpecificPreference (group, (subject, classType), AlternateBySubgroups (priority)) -> []
        | SpecificPreference (group, (subject, classType), UniteSubgroups (priority)) -> []
        | SpecificPreference (group, (subject, classType), OneClassInTwoWeeks (priority)) -> []

        | SpecificPreference (group, (subject, classType), InOneDay (priority)) ->
            let matchingSlotsForGroup =
                timetable
                |> Seq.filter (fun kvp ->
                    let (time, timetableGroup) = kvp.Key
                    let slot = kvp.Value
                    timetableGroup = group 
                    && slot.Teachers |> List.contains abbreviatedName
                    && stringContainsIgnoreCase subject slot.Subject
                    && slot.ClassType = convertActivityTypeToClassType classType)

            let distinctDays =
                matchingSlotsForGroup
                |> Seq.map (fun kvp -> 
                    let (time, _) = kvp.Key
                    time.Weekday)
                |> Seq.distinct
                |> Seq.length

            match priority with
            | Mandatory when distinctDays > 1 ->
                [ $"Занятия \"{subject}\" группы {group} должны быть в один день, но распределены по разным дням." ]
            | Desirable when distinctDays > 1 ->
                [ $"Занятия \"{subject}\" группы {group} желательно должны быть в один день, но распределены по разным дням." ]
            | NotDesirable when distinctDays = 1 ->
                [ $"Нежелательно проводить занятия \"{subject}\" группы {group} в один день, но они проходят в один день." ]
            | Avoidable when distinctDays = 1 ->
                [ $"Занятия \"{subject}\" группы {group} не должны быть в один день, но все проходят в один день." ]
            | _ -> []
        | _ -> []
    )