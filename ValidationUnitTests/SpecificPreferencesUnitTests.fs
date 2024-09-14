module SpecificPreferencesUnitTests

open ExcelTimetableParser.Timetable
open TeacherPreferences
open TeacherPreferences.SpecificPreferencesValidation
open TeacherPreferences.TeacherPreferences

open NUnit.Framework

[<SetUp>]
let Setup () = ()

[<Test>]
let ``Test Audience Preference`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    let teacher =
        { Name =
            { Name = "Ivan"
              Surname = "Ivanov"
              Patronymic = Some "Ivanovich" }
          Department = 1
          Preferences = [ SpecificPreference("Group1", ("Math", Lecture), (Audience(100, Mandatory))) ] }

    let errors = validateSpecificPreferences teacher "Ivanov I.I." timetable

    Assert.IsNotEmpty(errors)

[<Test>]
let ``Test Right Audience Preference`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 100
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    let teacher =
        { Name =
            { Name = "Ivan"
              Surname = "Ivanov"
              Patronymic = Some "Ivanovich" }
          Department = 1
          Preferences = [ SpecificPreference("Group1", ("Math", Lecture), (Audience(100, Mandatory))) ] }

    let errors = validateSpecificPreferences teacher "Ivanov I.I." timetable

    Assert.IsEmpty(errors)

[<Test>]
let ``Test Desirable Audience Preference`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Tuesday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Physics"
          Teachers = [ "Petrov P.P." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    let teacher =
        { Name =
            { Name = "Petr"
              Surname = "Petrov"
              Patronymic = Some "Petrovich" }
          Department = 2
          Preferences = [ SpecificPreference("Group1", ("Physics", Lecture), (Audience(100, Desirable))) ] }

    let errors = validateSpecificPreferences teacher "Petrov P.P." timetable

    Assert.IsNotEmpty(errors)
    Assert.AreEqual("Petrov P. P.: аудитория 100 желательна для предмета \"Physics\" группы Group1", errors.Head)

[<Test>]
let ``Test Not Desirable Audience`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Wednesday
          StartTime = (12, 0)
          EndTime = (13, 30) }

    timetable.Add(
        (time1, "Group2"),
        { StartTime = (12, 0)
          EndTime = (13, 30)
          Subject = "Web"
          Teachers = [ "Sidorov S.S." ]
          Audience = 202
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Practical }
    )

    let teacher =
        { Name =
            { Name = "Sergey"
              Surname = "Sidorov"
              Patronymic = Some "Sergeevich" }
          Department = 3
          Preferences = [ SpecificPreference("Group2", ("Web", Practical), (Audience(202, NotDesirable))) ] }

    let errors = validateSpecificPreferences teacher "Sidorov S.S." timetable

    Assert.IsNotEmpty(errors)
    Assert.AreEqual("Sidorov S. S.: аудитория 202 не желательна для предмета \"Web\" группы Group2", errors.Head)

[<Test>]
let ``Test Unite Groups Mandatory`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Thursday
          StartTime = (10, 0)
          EndTime = (11, 30) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Thursday
          StartTime = (10, 0)
          EndTime = (11, 30) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (10, 0)
          EndTime = (11, 30)
          Subject = "Graphs"
          Teachers = [ "Ivanov I.I." ]
          Audience = 301
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Seminar }
    )

    timetable.Add(
        (time2, "Group2"),
        { StartTime = (10, 0)
          EndTime = (11, 30)
          Subject = "Graphs"
          Teachers = [ "Ivanov I.I." ]
          Audience = 301
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Seminar }
    )

    let teacher =
        { Name =
            { Name = "Ivan"
              Surname = "Ivanov"
              Patronymic = Some "Ivanovich" }
          Department = 1
          Preferences = [ SpecificPreference("Group1", ("Graphs", Seminar), UniteGroups(Mandatory)) ] }

    let errors = validateSpecificPreferences teacher "Ivanov I.I." timetable

    Assert.IsEmpty(errors)

[<Test>]
let ``Test Unite Groups Not Desirable`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Friday
          StartTime = (14, 0)
          EndTime = (15, 30) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Friday
          StartTime = (14, 0)
          EndTime = (15, 30) }

    timetable.Add(
        (time1, "Group3"),
        { StartTime = (14, 0)
          EndTime = (15, 30)
          Subject = "Math"
          Teachers = [ "Kuznetsov K.K." ]
          Audience = 401
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Laboratory }
    )

    timetable.Add(
        (time2, "Group4"),
        { StartTime = (14, 0)
          EndTime = (15, 30)
          Subject = "Math"
          Teachers = [ "Kuznetsov K.K." ]
          Audience = 401
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Laboratory }
    )

    let teacher =
        { Name =
            { Name = "Konstantin"
              Surname = "Kuznetsov"
              Patronymic = Some "Konstantinovich" }
          Department = 4
          Preferences = [ SpecificPreference("Group3", ("Math", Laboratory), UniteGroups(NotDesirable)) ] }

    let errors = validateSpecificPreferences teacher "Kuznetsov K.K." timetable

    Assert.IsNotEmpty(errors)

    Assert.AreEqual(
        "Kuznetsov K. K.: нежелательно объединять группы для предмета \"Math\" с группой Group3, но они объединены.",
        errors.Head
    )

[<Test>]
let ``Test Consecutive Classes Mandatory`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (11, 15)
          EndTime = (12, 45) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Informatics"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time2, "Group1"),
        { StartTime = (11, 15)
          EndTime = (12, 45)
          Subject = "Informatics"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    let teacher =
        { Name =
            { Name = "Ivan"
              Surname = "Ivanov"
              Patronymic = Some "Ivanovich" }
          Department = 1
          Preferences = [ SpecificPreference("Group1", ("Informatics", Lecture), UniteClasses(Mandatory)) ] }

    let errors = validateSpecificPreferences teacher "Ivanov I.I." timetable

    Assert.IsEmpty(errors)

[<Test>]
let ``Test Non Consecutive Classes Desirable`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (8, 0)
          EndTime = (9, 30) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (11, 0)
          EndTime = (12, 30) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (8, 0)
          EndTime = (9, 30)
          Subject = "Astronomy"
          Teachers = [ "Sokolov S.S." ]
          Audience = 203
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Practical }
    )

    timetable.Add(
        (time2, "Group1"),
        { StartTime = (11, 0)
          EndTime = (12, 30)
          Subject = "Astronomy"
          Teachers = [ "Sokolov S.S." ]
          Audience = 203
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Practical }
    )

    let teacher =
        { Name =
            { Name = "Sergey"
              Surname = "Sokolov"
              Patronymic = Some "Sergeevich" }
          Department = 5
          Preferences = [ SpecificPreference("Group1", ("Astronomy", Practical), UniteClasses(Desirable)) ] }

    let errors = validateSpecificPreferences teacher "Sokolov S.S." timetable

    Assert.IsNotEmpty(errors)
    Assert.AreEqual("Занятия \"Astronomy\" группы Group1 желательно должны идти подряд, но есть разрыв.", errors.Head)

[<Test>]
let ``Test Mandatory InOneDay Preference with Multiple Days`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Wednesday
          StartTime = (11, 15)
          EndTime = (12, 45) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Mathematics"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time2, "Group1"),
        { StartTime = (11, 15)
          EndTime = (12, 45)
          Subject = "Mathematics"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    let teacher =
        { Name =
            { Name = "Ivan"
              Surname = "Ivanov"
              Patronymic = Some "Ivanovich" }
          Department = 1
          Preferences = [ SpecificPreference("Group1", ("Mathematics", Lecture), InOneDay(Mandatory)) ] }

    let errors = validateSpecificPreferences teacher "Ivanov I.I." timetable

    Assert.IsNotEmpty(errors)

    Assert.AreEqual(
        "Занятия \"Mathematics\" группы Group1 должны быть в один день, но распределены по разным дням.",
        errors.Head
    )

[<Test>]
let ``Test Desirable InOneDay Preference with Multiple Days`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Tuesday
          StartTime = (10, 0)
          EndTime = (11, 35) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Thursday
          StartTime = (11, 50)
          EndTime = (13, 25) }

    timetable.Add(
        (time1, "Group2"),
        { StartTime = (10, 0)
          EndTime = (11, 35)
          Subject = "Physics"
          Teachers = [ "Petrov P.P." ]
          Audience = 202
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Practical }
    )

    timetable.Add(
        (time2, "Group2"),
        { StartTime = (11, 50)
          EndTime = (13, 25)
          Subject = "Physics"
          Teachers = [ "Petrov P.P." ]
          Audience = 202
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Practical }
    )

    let teacher =
        { Name =
            { Name = "Petr"
              Surname = "Petrov"
              Patronymic = Some "Petrovich" }
          Department = 2
          Preferences = [ SpecificPreference("Group2", ("Physics", Practical), InOneDay(Desirable)) ] }

    let errors = validateSpecificPreferences teacher "Petrov P.P." timetable

    Assert.IsNotEmpty(errors)

    Assert.AreEqual(
        "Занятия \"Physics\" группы Group2 желательно должны быть в один день, но распределены по разным дням.",
        errors.Head
    )

[<Test>]
let ``Test Not Desirable InOneDay Preference with Same Day`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Friday
          StartTime = (9, 0)
          EndTime = (10, 30) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Friday
          StartTime = (11, 0)
          EndTime = (12, 30) }

    timetable.Add(
        (time1, "Group3"),
        { StartTime = (9, 0)
          EndTime = (10, 30)
          Subject = "Astronomy"
          Teachers = [ "Sidorov S.S." ]
          Audience = 203
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Laboratory }
    )

    timetable.Add(
        (time2, "Group3"),
        { StartTime = (11, 0)
          EndTime = (12, 30)
          Subject = "Astronomy"
          Teachers = [ "Sidorov S.S." ]
          Audience = 203
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Laboratory }
    )

    let teacher =
        { Name =
            { Name = "Sergey"
              Surname = "Sidorov"
              Patronymic = Some "Sergeevich" }
          Department = 3
          Preferences = [ SpecificPreference("Group3", ("Astronomy", Laboratory), InOneDay(NotDesirable)) ] }

    let errors = validateSpecificPreferences teacher "Sidorov S.S." timetable

    Assert.IsNotEmpty(errors)

    Assert.AreEqual(
        "Нежелательно проводить занятия \"Astronomy\" группы Group3 в один день, но они проходят в один день.",
        errors.Head
    )

[<Test>]
let ``Test Avoidable InOneDay Preference with Same Day`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (8, 30)
          EndTime = (10, 0) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (10, 10)
          EndTime = (11, 40) }

    timetable.Add(
        (time1, "Group4"),
        { StartTime = (8, 30)
          EndTime = (10, 0)
          Subject = "Math"
          Teachers = [ "Kuznetsov K.K." ]
          Audience = 301
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Seminar }
    )

    timetable.Add(
        (time2, "Group4"),
        { StartTime = (10, 10)
          EndTime = (11, 40)
          Subject = "Math"
          Teachers = [ "Kuznetsov K.K." ]
          Audience = 301
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Seminar }
    )

    let teacher =
        { Name =
            { Name = "Konstantin"
              Surname = "Kuznetsov"
              Patronymic = Some "Konstantinovich" }
          Department = 4
          Preferences = [ SpecificPreference("Group4", ("Math", Seminar), InOneDay(Avoidable)) ] }

    let errors = validateSpecificPreferences teacher "Kuznetsov K.K." timetable

    Assert.IsNotEmpty(errors)

    Assert.AreEqual(
        "Занятия \"Math\" группы Group4 не должны быть в один день, но все проходят в один день.",
        errors.Head
    )
