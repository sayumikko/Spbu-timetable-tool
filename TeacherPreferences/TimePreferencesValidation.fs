module TeacherPreferences.TimePreferencesValidation

open ExcelTimetableParser.Timetable
open TeacherPreferences

let formatTeacherName (teacher: TeacherName) =
    let patronymic =
        match teacher.Patronymic with
        | Some patr -> patr[ 0 ].ToString() + "."
        | None -> ""

    $"{teacher.Surname} {teacher.Name[0]}. {patronymic}"

let timeWithinRange (start1: StartTime) (end1: EndTime) (start2: StartTime) (end2: EndTime) =
    start1 < end2 && start2 < end1

let convertDayOfWeek
    (day: TeacherPreferences.TeacherPreferences.DayOfWeekSlot)
    : ExcelTimetableParser.Timetable.DayOfWeek list =
    match day with
    | TeacherPreferences.TeacherPreferences.Monday -> [ ExcelTimetableParser.Timetable.Monday ]
    | TeacherPreferences.TeacherPreferences.Tuesday -> [ ExcelTimetableParser.Timetable.Tuesday ]
    | TeacherPreferences.TeacherPreferences.Wednesday -> [ ExcelTimetableParser.Timetable.Wednesday ]
    | TeacherPreferences.TeacherPreferences.Thursday -> [ ExcelTimetableParser.Timetable.Thursday ]
    | TeacherPreferences.TeacherPreferences.Friday -> [ ExcelTimetableParser.Timetable.Friday ]
    | TeacherPreferences.TeacherPreferences.Saturday -> [ ExcelTimetableParser.Timetable.Saturday ]
    | TeacherPreferences.TeacherPreferences.AllWeek ->
        [ ExcelTimetableParser.Timetable.Monday
          ExcelTimetableParser.Timetable.Tuesday
          ExcelTimetableParser.Timetable.Wednesday
          ExcelTimetableParser.Timetable.Thursday
          ExcelTimetableParser.Timetable.Friday
          ExcelTimetableParser.Timetable.Saturday ]

let formatTime (hours, minutes) = $"{hours:D2}:{minutes:D2}"

let validateTime (teacher: Teacher) (abbreviatedName: string) (timetable: Timetable) =
    teacher.Preferences
    |> List.collect (function
        | Time ((dayOfWeekSlot, (startTime, endTime)), priority) ->
            let excelDays = convertDayOfWeek dayOfWeekSlot

            let matchingSlots =
                timetable
                |> Seq.filter (fun kvp ->
                    let ((time, _), slot) = kvp.Key, kvp.Value

                    slot.Teachers |> List.contains abbreviatedName
                    && excelDays |> List.contains time.Weekday
                    && timeWithinRange time.StartTime time.EndTime startTime endTime)

            let errors =
                match priority with
                | Mandatory when Seq.isEmpty matchingSlots ->
                    excelDays
                    |> List.map (fun d ->
                        $"Преподаватель {formatTeacherName teacher.Name} должен иметь пары в диапазоне времени с {formatTime startTime} до {formatTime endTime} в {d}, но они не найдены.")

                | Desirable when Seq.isEmpty matchingSlots ->
                    excelDays
                    |> List.map (fun d ->
                        $"Преподаватель {formatTeacherName teacher.Name} хотел бы иметь пары в диапазоне времени с {formatTime startTime} до {formatTime endTime} в {d}, но они не найдены.")

                | Avoidable when not (Seq.isEmpty matchingSlots) ->
                    excelDays
                    |> List.map (fun d ->
                        $"Преподаватель {formatTeacherName teacher.Name} не должен иметь пары в диапазоне времени с {formatTime startTime} до {formatTime endTime} в {d}, но они найдены.")

                | NotDesirable when not (Seq.isEmpty matchingSlots) ->
                    excelDays
                    |> List.map (fun d ->
                        $"Преподаватель {formatTeacherName teacher.Name} не хотел бы иметь пары в диапазоне времени с {formatTime startTime} до {formatTime endTime} в {d}, но они найдены.")

                | _ -> []

            errors
        | _ -> [])
