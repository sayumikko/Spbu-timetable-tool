module TimeUnitTests

open ExcelTimetableParser.Timetable
open TeacherPreferences
open TeacherPreferences.TimePreferencesValidation
open TeacherPreferences.TeacherPreferences

open NUnit.Framework

[<Test>]
let ``Test Mandatory Time Slot Not Found`` () =
    let timetable = ttEmpty ()

    let teacher =
        { Name =
            { Name = "Ivan"
              Surname = "Ivanov"
              Patronymic = Some "Ivanovich" }
          Department = 1
          Preferences = [ Time((Monday, ((9, 30), (11, 5))), Mandatory) ] }

    let errors = validateTime teacher "Ivanov I.I." timetable

    Assert.IsNotEmpty(errors)

    Assert.AreEqual(
        "Преподаватель Ivanov I. I. должен иметь пары в диапазоне времени с 09:30 до 11:05 в Monday, но они не найдены.",
        errors.Head
    )

[<Test>]
let ``Test Desirable Time Slot Not Found`` () =
    let timetable = ttEmpty ()

    let teacher =
        { Name =
            { Name = "Petr"
              Surname = "Petrov"
              Patronymic = Some "Petrovich" }
          Department = 2
          Preferences = [ Time((Tuesday, ((15, 25), (17, 0))), Desirable) ] }

    let errors = validateTime teacher "Petrov P.P." timetable

    Assert.IsNotEmpty(errors)

    Assert.AreEqual(
        "Преподаватель Petrov P. P. хотел бы иметь пары в диапазоне времени с 15:25 до 17:00 в Tuesday, но они не найдены.",
        errors.Head
    )

[<Test>]
let ``Test Avoidable Time Slot Found`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Wednesday
          StartTime = (13, 40)
          EndTime = (15, 25) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (13, 40)
          EndTime = (15, 25)
          Subject = "Math"
          Teachers = [ "Sidorov S.S." ]
          Audience = 100
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    let teacher =
        { Name =
            { Name = "Sergey"
              Surname = "Sidorov"
              Patronymic = Some "Sergeevich" }
          Department = 3
          Preferences = [ Time((Wednesday, ((13, 40), (15, 25))), Avoidable) ] }

    let errors = validateTime teacher "Sidorov S.S." timetable

    Assert.IsNotEmpty(errors)

    Assert.AreEqual(
        "Преподаватель Sidorov S. S. не должен иметь пары в диапазоне времени с 13:40 до 15:25 в Wednesday, но они найдены.",
        errors.Head
    )

[<Test>]
let ``Test Mandatory Time Slot Found`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Physics"
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
          Preferences = [ Time((Monday, ((9, 30), (11, 5))), Mandatory) ] }

    let errors = validateTime teacher "Ivanov I.I." timetable

    Assert.IsEmpty(errors)

[<Test>]
let ``Test Desirable Time Slot Found`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Tuesday
          StartTime = (15, 25)
          EndTime = (17, 0) }

    timetable.Add(
        (time1, "Group2"),
        { StartTime = (15, 25)
          EndTime = (17, 0)
          Subject = "Informatics"
          Teachers = [ "Petrov P.P." ]
          Audience = 102
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Seminar }
    )

    let teacher =
        { Name =
            { Name = "Petr"
              Surname = "Petrov"
              Patronymic = Some "Petrovich" }
          Department = 2
          Preferences = [ Time((Tuesday, ((15, 25), (17, 0))), Desirable) ] }

    let errors = validateTime teacher "Petrov P.P." timetable

    Assert.IsEmpty(errors)

[<Test>]
let ``Test NotDesirable Time Slot Not Found`` () =
    let timetable = ttEmpty ()

    let teacher =
        { Name =
            { Name = "Alexey"
              Surname = "Alexeev"
              Patronymic = None }
          Department = 4
          Preferences = [ Time((Thursday, ((10, 0), (12, 0))), NotDesirable) ] }

    let errors = validateTime teacher "Alexeev A." timetable

    Assert.IsEmpty(errors)

[<Test>]
let ``Test NotDesirable Time Slot Found`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Thursday
          StartTime = (10, 0)
          EndTime = (12, 0) }

    timetable.Add(
        (time1, "Group3"),
        { StartTime = (10, 0)
          EndTime = (12, 0)
          Subject = "Informatics"
          Teachers = [ "Alexeev A." ]
          Audience = 103
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    let teacher =
        { Name =
            { Name = "Alexey"
              Surname = "Alexeev"
              Patronymic = None }
          Department = 4
          Preferences = [ Time((Thursday, ((10, 0), (12, 0))), NotDesirable) ] }

    let errors = validateTime teacher "Alexeev A." timetable

    Assert.IsNotEmpty(errors)

    Assert.AreEqual(
        "Преподаватель Alexeev A.  не хотел бы иметь пары в диапазоне времени с 10:00 до 12:00 в Thursday, но они найдены.",
        errors.Head
    )
