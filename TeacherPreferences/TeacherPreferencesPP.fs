module TeacherPreferences.TeacherPreferencesPP

open TeacherPreferences

let formatTime (hour, minute) = sprintf "%02d:%02d" hour minute

let prettyPrintPreferences (teacher: Teacher) : string =
    let colorizeText (text: string) (colorCode: int) =
        sprintf "\x1B[38;5;%dm%s\x1B[0m" colorCode text

    let prettyPrintPriority priority =
        match priority with
        | Mandatory -> "  Необходимо"
        | Desirable -> "  Желательно"
        | Neutral -> "  Нейтральное отношение к тому, чтобы "
        | NotDesirable -> "  Нежелательно"
        | Avoidable -> "  Недопустимо"

    let prettyPrintGeneralPreferences preference =
        match preference with
        | MaxDaysPerWeek (number, priority) ->
            let priority = prettyPrintPriority priority
            sprintf "%s ставить не более %d дней в неделю\n" priority number
        | MinDaysPerWeek (number, priority) ->
            let priority = prettyPrintPriority priority
            sprintf "%s ставить не менее %d дней в неделю\n" priority number
        | FreeDaysPerWeek (number, priority) ->
            let priority = prettyPrintPriority priority
            sprintf "%s оставить следующее количество свободных дней в неделю: %d\n" priority number
        | MaxClassesPerDay (number, priority) ->
            let priority = prettyPrintPriority priority
            sprintf "%s ставить не более %d пар в день\n" priority number
        | MinClassesPerDay (number, priority) ->
            let priority = prettyPrintPriority priority
            sprintf "%s ставить не менее %d пар в день\n" priority number
        | Compactness priority ->
            let priority = prettyPrintPriority priority
            sprintf "%s ставить пары компактно\n" priority
        | IntersectTimeWithTeacher (teacherName, priority) ->
            let priority = prettyPrintPriority priority

            let patronymic =
                match teacherName.Patronymic with
                | Some patr -> patr
                | None -> ""

            sprintf
                "%s пересекать время пар с преподавателем %s %s %s\n"
                priority
                teacherName.Surname
                teacherName.Name
                patronymic
        | Gaps priority ->
            let priority = prettyPrintPriority priority
            sprintf "%s наличие окон между парами\n" priority

    let prettyPrintSpecificPreferences preference =
        match preference with
        | Audience (number, priority) ->
            let priority = prettyPrintPriority priority
            sprintf "%s использовать аудиторию %d\n" priority number
        | Equipment (equipment, priority) ->
            let priority = prettyPrintPriority priority

            match equipment with
            | Projector -> sprintf "    %s проектор\n" priority
            | WhiteBoard -> sprintf "    %s маркерная доска\n" priority
            | NumberOfBoards number -> sprintf "    %s количество досок: %d\n" priority number
            | ComputerAudience -> sprintf "    %s компьютерная аудитория\n" priority
            | Computer -> sprintf "    %s компьютер\n" priority
            | TimeService -> sprintf "    %s служба времени\n" priority
            | Capacity capacity -> sprintf "    %s аудитория вместимостью %d\n" priority capacity
            | Blackboard -> sprintf "   %s доска с мелом\n" priority
        | UniteGroups priority ->
            let priority = prettyPrintPriority priority
            sprintf "%s объединение групп\n" priority
        | UniteClasses priority ->
            let priority = prettyPrintPriority priority
            sprintf "%s проводить сдвоенные пары\n" priority
        | AlternateBySubgroups priority ->
            let priority = prettyPrintPriority priority
            sprintf "%s чередование подгрупп\n" priority
        | OneClassInTwoWeeks priority ->
            let priority = prettyPrintPriority priority
            sprintf "%s ставить пары раз в две недели\n" priority
        | UniteSubgroups priority ->
            let priority = prettyPrintPriority priority
            sprintf "%s объединить подгруппы\n" priority
        | InOneDay priority ->
            let priority = prettyPrintPriority priority
            sprintf "%s ставить все пары в один день\n" priority

    let prettyPrintTime (dayOfWeekSlot, timeSlot) priority =
        let prettyPrintDayOfWeek dayOfWeek =
            match dayOfWeek with
            | Monday -> " понедельник"
            | Tuesday -> " вторник"
            | Wednesday -> " среду"
            | Thursday -> " четверг"
            | Friday -> " пятницу"
            | Saturday -> " субботу"
            | AllWeek -> " всю неделю"

        let prettyPrintPriority priority =
            match priority with
            | Mandatory -> "Необходимо"
            | Desirable -> "Желательно"
            | Neutral -> "Нейтральное отношение"
            | NotDesirable -> "Нежелательно"
            | Avoidable -> "Недопустимо"

        let prettyPrintTimeSlot (startTime, endTime) =
            sprintf "с %s до %s" (formatTime startTime) (formatTime endTime)

        let dayOfWeekStr = prettyPrintDayOfWeek dayOfWeekSlot
        let timeSlotStr = prettyPrintTimeSlot timeSlot
        let priorityStr = prettyPrintPriority priority

        sprintf "  %s ставить пары в%s %s\n" priorityStr dayOfWeekStr timeSlotStr


    let prettyPrintSubjectOrGroupPreferences (groupOption, subjectOption, preference) =
        let prettyPrintClassType classtype =
            match classtype with
            | Lecture -> "Лекция"
            | Seminar -> "Семинарское занятие"
            | Laboratory -> "Лабораторная работа"
            | Practical -> "Практическое занятие"
            | NoClassType -> ""

        let targetStr =
            match groupOption, subjectOption with
            | group, (subject, classtype) ->
                sprintf
                    "  %s для групп(ы) %s по предмету %s:\n"
                    (prettyPrintClassType classtype)
                    (colorizeText group 64)
                    (colorizeText subject 64)

        let preferencesStr = prettyPrintSpecificPreferences preference
        targetStr + preferencesStr

    let patronymic =
        match teacher.Name.Patronymic with
        | Some patr -> patr
        | None -> ""

    let header =
        sprintf
            "Пожелания преподавателя %s %s %s, кафедра %s:\n"
            (colorizeText teacher.Name.Surname 31)
            (colorizeText teacher.Name.Name 31)
            (colorizeText patronymic 31)
            (colorizeText ((string) teacher.Department) 31)

    let preferencesStr =
        teacher.Preferences
        |> List.map (fun preference ->
            match preference with
            | GeneralPreference generalPreference -> prettyPrintGeneralPreferences generalPreference
            | SpecificPreference (groupOption, subjectOption, specificPreference) ->
                prettyPrintSubjectOrGroupPreferences (groupOption, subjectOption, specificPreference)
            | Time (dayOfWeek, timeslot) -> prettyPrintTime dayOfWeek timeslot)

        |> String.concat ""

    header + preferencesStr
