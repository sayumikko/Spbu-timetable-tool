module TeacherPreferencesPP

open TeacherPreferences

let prettyPrintPreferences (teacher: Teacher) =
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
        | MaxDaysPerWeek(number, priority) ->
            let priority = prettyPrintPriority priority
            printfn "%s ставить не более %d дней в неделю" priority number
        | MaxClassesPerDay(number, priority) ->
            let priority = prettyPrintPriority priority
            printfn "%s ставить не более %d пар в день" priority number
        | MinClassesPerDay(number, priority) ->
            let priority = prettyPrintPriority priority
            printfn "%s ставить не менее %d дней в неделю" priority number
        | MinDaysPerWeek(number, priority) ->
            let priority = prettyPrintPriority priority
            printfn "%s ставить не менее %d пар в день" priority number
        | FreeDaysPerWeek(number, priority) ->
            let priority = prettyPrintPriority priority
            printfn "%s оставить следующее количество свободных дней в неделю: %d" priority number
        | Compactness priority ->
            let priority = prettyPrintPriority priority
            printfn "%s ставить пары компактно" priority
        | IntersectTimeWithTeacher(teacher, priority) ->
            let priority = prettyPrintPriority priority

            printfn
                "%s пересекать время пар с преподавателем %s %s %s"
                priority
                teacher.Name.Surname
                teacher.Name.Name
                teacher.Name.Patronymic
        | Gaps is_needed ->
            match is_needed with
            | Mandatory -> printfn "  Окна между парами необходимы"
            | Desirable -> printfn "  Окна между парами желательны"
            | Neutral -> printfn "  Все равно на окна между парами"
            | NotDesirable -> printfn "  Окна между парами нежелательны"
            | Avoidable -> printfn "  Окна между парами крайне недопустимы"

    let prettyPrintSpecificPreferences preference =
        let prettyPrintDayOfWeek dayOfWeek =
            match dayOfWeek with
            | Monday -> " понедельник"
            | Tuesday -> "о вторник"
            | Wednesday -> " среду"
            | Thursday -> " четверг"
            | Friday -> " пятницу"
            | Saturday -> " субботу"

        let prettyPrintTimeSlot time =
            match time with
            | ((startHour, startMinutes), (finalTime, finalMinutes), priority) ->
                let priority = prettyPrintPriority priority
                printfn "%s ставить с %d:%d до %d:%d" priority startHour startMinutes finalTime finalMinutes

        match preference with
        | Audience(number, priority) ->
            match priority with
            | Mandatory -> printfn "  Необходима %d аудитория" number
            | Desirable -> printfn "  Желательна %d аудитория" number
            | Neutral -> printfn "  Нет пожеланий на аудитории"
            | NotDesirable -> printfn "  Нежелательна %d аудитория" number
            | Avoidable -> printfn "  Недопустима %d аудитория" number
        | Equipment equipment ->
            match equipment with
            | Projector -> printfn "  Желателен проектор"
            | WhiteBoard -> printfn "  Желательна маркерная доска"
            | NumberOfBoards number -> printfn "  Желаемое количество досок: %d" number
            | ComputerAudience -> printfn "  Желательна компьютерная аудитория"
            | Computer -> printfn "  Необходим компьютер"
            | TimeService -> printfn "  Желательна служба времени"
            | Capacity capacity -> printfn "  Желательна аудитория вместимостью %d" capacity
            | Blackboard -> printfn "  Желательна доска с мелом"
        | Time(dayOfWeek, timeSlot) ->
            match dayOfWeek, timeSlot with
            | Some(dayOfWeek, priority), Some(time) ->
                let dayOfWeek = prettyPrintDayOfWeek dayOfWeek
                let priority = prettyPrintPriority priority
                printf "  %s ставить пары в%s" dayOfWeek priority
                List.iter prettyPrintTimeSlot time
            | Some(dayOfWeek, priority), None ->
                let dayOfWeek = prettyPrintDayOfWeek dayOfWeek
                let priority = prettyPrintPriority priority
                printfn "%s ставить пары в%s, пожеланий по времени нет" priority dayOfWeek
            | None, Some(time) -> List.iter prettyPrintTimeSlot time
            | None, None -> printfn "Нет предпочтений по времени и дню недели"
        | UniteGroups priority ->
            let priority = prettyPrintPriority priority
            printfn "%s объединение групп" priority
        | UniteClasses priority ->
            let priority = prettyPrintPriority priority
            printfn "%s проводить сдвоенные пары" priority
        | AlternateBySubgroups priority ->
            let priority = prettyPrintPriority priority
            printfn "%s чередование подгрупп" priority
        | OneClassInTwoWeeks priority ->
            let priority = prettyPrintPriority priority
            printfn "%s ставить пары раз в две недели" priority
        | UniteSubgroups priority ->
            let priority = prettyPrintPriority priority
            printfn "%s объединить подгруппы" priority
        | InOneDay priority ->
            let priority = prettyPrintPriority priority
            printfn "%s ставить все пары в один день" priority

    let prettyPrintSubjectOrGroupPreferences target preferences =
        let prettyPrintClassType classtype =
            match classtype with
            | Lecture -> "Лекция"
            | Seminar -> "Практическое занятие"
            | Laboratory -> "Лабораторная работа"

        match target with
        | BySubject(subject, Some(classtype)) ->
            let classtype = prettyPrintClassType classtype
            printfn "  %s для предмета %s:" classtype (colorizeText subject 64)
        | ByGroup(group, Some(classtype)) ->
            let classtype = prettyPrintClassType classtype
            printfn "  %s для групп(ы) %s:" classtype (colorizeText group 64)
        | BySubject(subject, None) -> printfn "  Для предмета %s:" (colorizeText subject 64)
        | ByGroup(group, None) -> printfn "  Для групп(ы) %s:" (colorizeText group 64)

        List.iter
            (fun preference ->
                printf "    "
                prettyPrintSpecificPreferences preference)
            preferences

    let printDepartment department =
        match department with
        | Astromony -> "астрономии"
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
        | ProbabilityheoryAndMathematicalStatistics -> "теории вероятностей и математической статистики"
        | PhysicalMechanics -> "физической механики"
        | ForeignLanguages -> "иностранных языков в сфере математических наук"
        | TheoryOfElasticity -> "теории упругости"

    printfn
        "Пожелания преподавателя %s %s %s, кафедра %s:"
        (colorizeText teacher.Name.Surname 31)
        (colorizeText teacher.Name.Name 31)
        (colorizeText teacher.Name.Patronymic 31)
        (colorizeText (printDepartment teacher.Department) 31)

    List.iter
        (fun preference ->
            match preference with
            | GeneralPreference preference ->
                prettyPrintGeneralPreferences preference
                printfn ""
            | SpecificPreference preference ->
                prettyPrintSpecificPreferences preference
                printfn ""
            | SubjectOrGroupSpecificPreference(target, preferences) ->
                prettyPrintSubjectOrGroupPreferences target preferences)
        teacher.Preferences

    printfn ""
