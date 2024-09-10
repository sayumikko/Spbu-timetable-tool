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

    let prettyPrintTime (dayOfWeekOption) (timeSlotOption) =
        let prettyPrintDayOfWeek dayOfWeek =
            match dayOfWeek with
            | Monday -> " понедельник"
            | Tuesday -> " вторник"
            | Wednesday -> " среду"
            | Thursday -> " четверг"
            | Friday -> " пятницу"
            | Saturday -> " субботу"
            | AllWeek -> " всю неделю"

        let prettyPrintTimeSlot time =
            match time with
            | ((startHour, startMinutes), (finalTime, finalMinutes), priority) ->
                let priority = prettyPrintPriority priority

                sprintf
                    "  %s ставить с %s до %s\n"
                    priority
                    (formatTime (startHour, startMinutes))
                    (formatTime (finalTime, finalMinutes))

        let dayOfWeekStr =
            match dayOfWeekOption with
            | Some (dayOfWeek, priority) ->
                let dayOfWeekStr = prettyPrintDayOfWeek dayOfWeek
                let priority = prettyPrintPriority priority
                sprintf "    Необходимо ставить пары в%s\n" dayOfWeekStr
            | None -> ""

        let timeSlotsStr =
            match timeSlotOption with
            | Some (times) ->
                times
                |> List.map prettyPrintTimeSlot
                |> String.concat ""
            | None -> ""

        dayOfWeekStr + timeSlotsStr

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

    let printDepartment department =
        match department with
        | Astronomy -> "астрономии"
        | Astrophysics -> "астрофизики"
        | Algebra -> "высшей алгебры и теории чисел"
        | Geometry -> "высшей геометрии"
        | ComputationalMathematics -> "вычислительной математики"
        | DifferentialEquations -> "дифференциальных уравнений"
        | Informatics -> "информатики"
        | InformationAndAnalyticalSystems -> "информационно-аналитических систем"
        | Hydroaeromechanics -> "гидроаэромеханики"
        | OperationsResearch -> "исследования операций"
        | MathematicalAnalysis -> "математического анализа"
        | MathematicalPhysics -> "математической физики"
        | CelestialMechanics -> "небесной механики"
        | ParallelAlgorithms -> "параллельных алгоритмов"
        | AppliedCybernetics -> "прикладной кибернетики"
        | SystemProgramming -> "системного программирования"
        | StatisticalModeling -> "статистического моделирования"
        | TheoreticalAndAppliedMechanics -> "теоретической и прикладной механики"
        | TheoreticalCybernetics -> "теоретической кибернетики"
        | ProbabilityTheoryAndMathematicalStatistics -> "теории вероятностей и математической статистики"
        | PhysicalMechanics -> "физической механики"
        | ForeignLanguages -> "иностранных языков в сфере математических наук"
        | TheoryOfElasticity -> "теории упругости"

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
            (colorizeText (printDepartment teacher.Department) 31)

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
