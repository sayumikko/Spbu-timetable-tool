module TeacherPreferences.GeneralPreferencesValidation

open ExcelTimetableParser.Timetable
open TeacherPreferences

let abbreviateTeacherName (teacher: TeacherName) =
    let initials =
        match teacher.Patronymic with
        | Some patr -> $"{teacher.Name.[0]}.{patr.[0]}."
        | None -> $"{teacher.Name.[0]}."

    $"{teacher.Surname} {initials}"

let translateDayOfWeek (weekday: ExcelTimetableParser.Timetable.DayOfWeek) =
    match weekday with
    | ExcelTimetableParser.Timetable.DayOfWeek.Monday -> "Понедельник"
    | ExcelTimetableParser.Timetable.DayOfWeek.Tuesday -> "Вторник"
    | ExcelTimetableParser.Timetable.DayOfWeek.Wednesday -> "Среда"
    | ExcelTimetableParser.Timetable.DayOfWeek.Thursday -> "Четверг"
    | ExcelTimetableParser.Timetable.DayOfWeek.Friday -> "Пятница"
    | ExcelTimetableParser.Timetable.DayOfWeek.Saturday -> "Суббота"
    | ExcelTimetableParser.Timetable.DayOfWeek.Sunday -> "Воскресенье"


let validateGeneralPreferences (teacher: Teacher) (abbreviatedName: string) (timetable: Timetable) =
    teacher.Preferences
    |> List.collect (function
        | GeneralPreference (MaxDaysPerWeek (maxDays, priority)) ->
            let daysTaught =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((_, _), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains abbreviatedName)
                |> Seq.distinctBy (fun kvp ->
                    let (time, _) = kvp.Key
                    time.Weekday)
                |> Seq.length

            if daysTaught > maxDays && priority = Mandatory then
                [ $"У преподавателя {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} количество дней в расписании превышает необходимое: ({maxDays})." ]
            elif daysTaught > maxDays && priority = Desirable then
                [ $"У преподавателя {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} количество дней в расписании превышает желаемое: ({maxDays})." ]
            else
                []


        | GeneralPreference (MinDaysPerWeek (minDays, priority)) ->
            let daysTaught =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((time, _), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains abbreviatedName)
                |> Seq.distinctBy (fun kvp ->
                    let (time, _) = kvp.Key
                    time.Weekday)
                |> Seq.length

            if daysTaught < minDays && priority = Mandatory then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} проводит меньше минимального необходимого количества дней: ({minDays})." ]
            elif daysTaught < minDays && priority = Desirable then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} проводит меньше желаемого минимального количества дней: ({minDays})." ]
            else
                []


        | GeneralPreference (FreeDaysPerWeek (minFreeDays, priority)) ->
            let daysTaught =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((_, _), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains abbreviatedName)
                |> Seq.distinctBy (fun kvp ->
                    let (time, _) = kvp.Key
                    time.Weekday)
                |> Seq.length

            let totalDaysInWeek = 6
            let freeDays = totalDaysInWeek - daysTaught

            if freeDays < minFreeDays && priority = Mandatory then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} должен иметь не менее {minFreeDays} свободных дней." ]
            elif freeDays < minFreeDays && priority = Desirable then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} хотел бы иметь не менее {minFreeDays} свободных дней." ]
            elif freeDays > minFreeDays && priority = NotDesirable then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} не хотел бы иметь меньше {minFreeDays} свободных дней." ]
            elif freeDays > minFreeDays && priority = Avoidable then
                [ $"Преподавателю {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} недопустимо иметь меньше {minFreeDays} свободных дней." ]
            else
                []

        | GeneralPreference (MaxClassesPerDay (maxClasses, priority)) ->
            let classesPerDay =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((time, _), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains abbreviatedName)
                |> Seq.groupBy (fun kvp -> let (time, _) = kvp.Key in time.Weekday)
                |> Seq.map (fun (_, slots) -> Seq.length slots)

            if classesPerDay
               |> Seq.exists (fun count -> count > maxClasses)
               && priority = Mandatory then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} имеет больше {maxClasses} пар в день." ]
            elif classesPerDay
                 |> Seq.exists (fun count -> count > maxClasses)
                 && priority = Desirable then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} желательно имеет не больше {maxClasses} пар в день." ]
            else
                []


        | GeneralPreference (MinClassesPerDay (minClasses, priority)) ->
            let classesPerDay =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((time, _), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains abbreviatedName)
                |> Seq.groupBy (fun kvp -> let (time, _) = kvp.Key in time.Weekday)
                |> Seq.map (fun (_, slots) -> Seq.length slots)

            if classesPerDay
               |> Seq.exists (fun count -> count < minClasses)
               && priority = Mandatory then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} имеет меньше {minClasses} пар в день." ]
            elif classesPerDay
                 |> Seq.exists (fun count -> count < minClasses)
                 && priority = Desirable then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} хотел бы иметь не меньше {minClasses} пар в день." ]
            else
                []

        | GeneralPreference (Compactness priority) ->
            let maxAllowedGapMinutes = 50

            let teacherClassesByDay =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((_, _), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains abbreviatedName)
                |> Seq.groupBy (fun kvp ->
                    let (time, _) = kvp.Key
                    time.Weekday)

            let daysWithGaps =
                teacherClassesByDay
                |> Seq.choose (fun (weekday, classes) ->
                    let sortedClasses =
                        classes
                        |> Seq.sortBy (fun kvp ->
                            let (time, _) = kvp.Key
                            time.StartTime)

                    let hasGaps =
                        sortedClasses
                        |> Seq.pairwise
                        |> Seq.exists (fun (kvp1, kvp2) ->
                            let (time1, _) = kvp1.Key
                            let (time2, _) = kvp2.Key
                            let endTime1 = time1.EndTime
                            let startTime2 = time2.StartTime

                            let hours1, minutes1 = endTime1
                            let hours2, minutes2 = startTime2
                            let totalMinutes1 = hours1 * 60 + minutes1
                            let totalMinutes2 = hours2 * 60 + minutes2

                            totalMinutes2 - totalMinutes1 > maxAllowedGapMinutes)

                    if hasGaps then Some weekday else None)

            if
                not (Seq.isEmpty daysWithGaps)
                && priority = Mandatory
            then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} не должен иметь больших окон между занятиями." ]
            elif
                not (Seq.isEmpty daysWithGaps)
                && priority = Desirable
            then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} не хотел бы иметь больших окон между занятиями." ]
            elif Seq.isEmpty daysWithGaps
                 && priority = NotDesirable then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} не хотел бы иметь окна между занятиями." ]
            elif Seq.isEmpty daysWithGaps && priority = Avoidable then
                [ $"Преподавателю {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} крайне нежелательно иметь окна между занятиями." ]
            else
                []


        | GeneralPreference (Gaps priority) ->
            let minRequiredGapMinutes = 90

            let teacherClassesByDay =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((time, _), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains abbreviatedName)
                |> Seq.groupBy (fun kvp ->
                    let (time, _) = kvp.Key
                    time.Weekday)

            let daysWithGaps =
                teacherClassesByDay
                |> Seq.choose (fun (weekday, classes) ->
                    let sortedClasses =
                        classes
                        |> Seq.sortBy (fun kvp ->
                            let (time, _) = kvp.Key
                            time.StartTime)

                    let hasSufficientGaps =
                        sortedClasses
                        |> Seq.pairwise
                        |> Seq.exists (fun (kvp1, kvp2) ->
                            let (time1, _) = kvp1.Key
                            let (time2, _) = kvp2.Key
                            let endTime1 = time1.EndTime
                            let startTime2 = time2.StartTime

                            let hours1, minutes1 = endTime1
                            let hours2, minutes2 = startTime2
                            let totalMinutes1 = hours1 * 60 + minutes1
                            let totalMinutes2 = hours2 * 60 + minutes2

                            totalMinutes2 - totalMinutes1
                            >= minRequiredGapMinutes)

                    if hasSufficientGaps then
                        Some weekday
                    else
                        None)

            if Seq.isEmpty daysWithGaps && priority = Mandatory then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} должен иметь окна между занятиями." ]
            elif Seq.isEmpty daysWithGaps && priority = Desirable then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} хотел бы иметь окна между занятиями." ]
            elif
                not (Seq.isEmpty daysWithGaps)
                && priority = NotDesirable
            then
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} не хотел бы иметь окон между занятиями." ]
            elif
                not (Seq.isEmpty daysWithGaps)
                && priority = Avoidable
            then
                [ $"Преподавателю {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} недопустимо иметь окна между занятиями." ]
            else
                []


        | GeneralPreference (IntersectTimeWithTeacher (otherTeacherName, priority)) ->
            let otherAbbreviatedName = abbreviateTeacherName otherTeacherName

            let teacherClasses =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((_, _), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains abbreviatedName)

            let otherTeacherClasses =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((_, _), slot) = kvp.Key, kvp.Value

                    slot.Teachers
                    |> List.contains otherAbbreviatedName)

            let intersectionsFound =
                teacherClasses
                |> Seq.exists (fun (KeyValue ((time, _), _)) ->
                    otherTeacherClasses
                    |> Seq.exists (fun (KeyValue ((otherTime, _), _)) ->
                        time.Weekday = otherTime.Weekday
                        && time.StartTime = otherTime.StartTime
                        && time.EndTime = otherTime.EndTime))

            match priority with
            | Mandatory when not intersectionsFound ->
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} должен пересекаться по времени с преподавателем {otherTeacherName.Surname} {abbreviateTeacherName otherTeacherName}." ]
            | Desirable when not intersectionsFound ->
                [ $"Желательно, чтобы преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} пересекался по времени с преподавателем {otherTeacherName.Surname} {abbreviateTeacherName otherTeacherName}." ]
            | NotDesirable when intersectionsFound ->
                [ $"Преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} не хотел бы пересекаться по времени с преподавателем {otherTeacherName.Surname} {abbreviateTeacherName otherTeacherName}." ]
            | Avoidable when intersectionsFound ->
                [ $"Недопустимо, чтобы преподаватель {teacher.Name.Surname} {abbreviateTeacherName teacher.Name} пересекался по времени с преподавателем {otherTeacherName.Surname} {abbreviateTeacherName otherTeacherName}." ]
            | _ -> []

        | _ -> [])
