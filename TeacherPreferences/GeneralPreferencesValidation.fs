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
                        [ $"У преподавателя {teacher.Name.Surname} количество
                            дней в расписании превышает желаемое: ({maxDays})." ]
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
                [ $"Преподаватель {teacher.Name.Surname} проводит меньше минимального желаемого количества дней ({minDays})." ]
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
                [ $"Преподаватель {teacher.Name.Surname} должен иметь не менее {minFreeDays} свободных дней в неделю, но имеет только {freeDays}." ]
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
            if classesPerDay |> Seq.exists (fun count -> count > maxClasses) && priority = Mandatory then
                [ $"Преподаватель {teacher.Name.Surname} имеет больше {maxClasses} пар в день." ]
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
            if classesPerDay |> Seq.exists (fun count -> count < minClasses) && priority = Mandatory then
                [ $"Преподаватель {teacher.Name.Surname} имеет меньше {minClasses} пар в день." ]
            else
                []

        | GeneralPreference (Compactness priority) ->
            let teacherClassesByDay =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((time, _), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains abbreviatedName)
                |> Seq.groupBy (fun kvp ->
                    let (time, _) = kvp.Key
                    time.Weekday)

            let daysWithoutGaps =
                teacherClassesByDay
                |> Seq.choose (fun (weekday, classes) ->
                    let sortedClasses =
                        classes
                        |> Seq.sortBy (fun kvp ->
                            let (time, _) = kvp.Key
                            time.StartTime)
                    let noGapsInDay =
                        sortedClasses
                        |> Seq.pairwise
                        |> Seq.forall (fun (kvp1, kvp2) ->
                            let (time1, _) = kvp1.Key
                            let (time2, _) = kvp2.Key
                            let endTime1 = time1.EndTime
                            let startTime2 = time2.StartTime
                            endTime1 >= startTime2) 
                    if noGapsInDay then
                        Some weekday
                    else
                        None)

            if Seq.isEmpty daysWithoutGaps then
                []
            elif priority = Mandatory then
                let daysList = daysWithoutGaps |> Seq.map (fun wd -> wd.ToString()) |> String.concat ", "
                [ $"Преподаватель {teacher.Name.Surname} не должен иметь окна между занятиями,
                    но они найдены в следующие дни: {daysList}." ]
            else
                []


        | GeneralPreference (Gaps priority) ->
            let teacherClassesByDay =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((time, _), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains abbreviatedName)
                |> Seq.groupBy (fun kvp ->
                    let (time, _) = kvp.Key
                    time.Weekday)

            let daysWithoutGaps =
                teacherClassesByDay
                |> Seq.choose (fun (weekday, classes) ->
                    let sortedClasses =
                        classes
                        |> Seq.sortBy (fun kvp ->
                            let (time, _) = kvp.Key
                            time.StartTime)
                    let noGapsInDay =
                        sortedClasses
                        |> Seq.pairwise
                        |> Seq.forall (fun (kvp1, kvp2) ->
                            let (time1, _) = kvp1.Key
                            let (time2, _) = kvp2.Key
                            let endTime1 = time1.EndTime
                            let startTime2 = time2.StartTime
                            endTime1 < startTime2) 
                    if noGapsInDay then
                        Some weekday
                    else
                        None)

            if Seq.isEmpty daysWithoutGaps then
                []
            elif priority = Mandatory then
                let daysList = daysWithoutGaps |> Seq.map translateDayOfWeek |> String.concat ", "
                [ $"Преподаватель {teacher.Name.Surname} должен иметь окна между занятиями, но они не найдены в следующие дни: {daysList}." ]
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
                    let ((time, _), slot) = kvp.Key, kvp.Value
                    slot.Teachers |> List.contains otherAbbreviatedName)

            let intersectionsFound =
                teacherClasses
                |> Seq.exists (fun (KeyValue((time, _), _)) ->
                    otherTeacherClasses
                    |> Seq.exists (fun (KeyValue((otherTime, _), _)) ->
                        time.Weekday = otherTime.Weekday &&
                        time.StartTime = otherTime.StartTime &&
                        time.EndTime = otherTime.EndTime))


            if not intersectionsFound then
                let errorMessage =
                    match priority with
                    | Mandatory -> 
                        $"Преподаватель {teacher.Name.Surname} должен пересекаться по времени с преподавателем {otherTeacherName.Surname}, но это не выполнено."
                    | Desirable -> 
                        $"Желательно, чтобы преподаватель {teacher.Name.Surname} пересекался по времени с преподавателем {otherTeacherName.Surname}, но это не выполнено."
                    | _ -> ""
                [ errorMessage ]
            else
                []


        | _ -> []
    )