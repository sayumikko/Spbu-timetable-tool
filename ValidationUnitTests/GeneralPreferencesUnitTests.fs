module GeneralPreferencesUnitTests

open ExcelTimetableParser.Timetable
open TeacherPreferences
open TeacherPreferences.GeneralPreferencesValidation
open TeacherPreferences.TeacherPreferences

open NUnit.Framework

[<SetUp>]
let Setup () = ()

[<Test>]
let ``Test MaxDaysPerWeek Preference`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Tuesday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time3 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Wednesday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time4 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Thursday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time5 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Friday
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

    timetable.Add(
        (time2, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time3, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time4, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time5, "Group1"),
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
          Preferences = [ GeneralPreference(MaxDaysPerWeek(4, Mandatory)) ] }


    let errors = validateGeneralPreferences teacher "Ivanov I.I." timetable
    Assert.IsNotEmpty(errors)

[<Test>]
let ``Test MaxClassesPerDay Preference`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (11, 15)
          EndTime = (12, 50) }

    let time3 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (13, 40)
          EndTime = (15, 15) }

    let time4 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (15, 25)
          EndTime = (17, 0) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time2, "Group1"),
        { StartTime = (11, 15)
          EndTime = (12, 50)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time3, "Group1"),
        { StartTime = (13, 40)
          EndTime = (15, 15)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time4, "Group1"),
        { StartTime = (15, 25)
          EndTime = (17, 0)
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
          Preferences = [ GeneralPreference(MaxClassesPerDay(3, Mandatory)) ] }

    let errors = validateGeneralPreferences teacher "Ivanov I.I." timetable
    Assert.IsNotEmpty(errors)

[<Test>]
let ``Test MinDaysPerWeek Preference`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Tuesday
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

    timetable.Add(
        (time2, "Group1"),
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
          Preferences = [ GeneralPreference(MinDaysPerWeek(3, Mandatory)) ] }

    let errors = validateGeneralPreferences teacher "Ivanov I.I." timetable
    Assert.IsNotEmpty(errors)

[<Test>]
let ``Test MinClassesPerDay Preference`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (11, 15)
          EndTime = (12, 50) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time2, "Group1"),
        { StartTime = (11, 15)
          EndTime = (12, 50)
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
          Preferences = [ GeneralPreference(MinClassesPerDay(3, Mandatory)) ] }

    let errors = validateGeneralPreferences teacher "Ivanov I.I." timetable
    Assert.IsNotEmpty(errors)


[<Test>]
let ``Test FreeDaysPerWeek Preference`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Tuesday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time3 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Wednesday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time4 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Thursday
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

    timetable.Add(
        (time2, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time3, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time4, "Group1"),
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
          Preferences = [ GeneralPreference(FreeDaysPerWeek(2, Mandatory)) ] }

    let errors = validateGeneralPreferences teacher "Ivanov I.I." timetable
    Assert.IsEmpty(errors)

[<Test>]
let ``Test Compactness Preference`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (13, 40)
          EndTime = (15, 15) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time2, "Group1"),
        { StartTime = (13, 40)
          EndTime = (15, 15)
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
          Preferences = [ GeneralPreference(Compactness Mandatory) ] }

    let errors = validateGeneralPreferences teacher "Ivanov I.I." timetable
    Assert.IsNotEmpty(errors)

[<Test>]
let ``Test Gaps Preference`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (15, 30)
          EndTime = (15, 15) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time2, "Group1"),
        { StartTime = (15, 40)
          EndTime = (17, 15)
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
          Preferences = [ GeneralPreference(Gaps Mandatory) ] }

    let errors = validateGeneralPreferences teacher "Ivanov I.I." timetable
    Assert.IsEmpty(errors)

[<Test>]
let ``Test IntersectTimeWithTeacher Preference`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (13, 40)
          EndTime = (15, 15) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I."; "Petrov P.P." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time2, "Group1"),
        { StartTime = (13, 40)
          EndTime = (15, 15)
          Subject = "Physics"
          Teachers = [ "Petrov P.P." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    let teacher =
        { Name =
            { Name = "Ivan"
              Surname = "Ivanov"
              Patronymic = Some "Ivanovich" }
          Department = 1
          Preferences =
            [ GeneralPreference(
                  IntersectTimeWithTeacher(
                      { Name = "Petr"
                        Surname = "Petrov"
                        Patronymic = Some "Petrovich" },
                      Mandatory
                  )
              ) ] }

    let errors = validateGeneralPreferences teacher "Ivanov I.I." timetable
    Assert.IsEmpty(errors)

[<Test>]
let ``Test IntersectTimeWithTeacher Preference Fails`` () =
    let timetable = ttEmpty ()

    let time1 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (9, 30)
          EndTime = (11, 5) }

    let time2 =
        { Weekday = ExcelTimetableParser.Timetable.DayOfWeek.Monday
          StartTime = (13, 40)
          EndTime = (15, 15) }

    timetable.Add(
        (time1, "Group1"),
        { StartTime = (9, 30)
          EndTime = (11, 5)
          Subject = "Math"
          Teachers = [ "Ivanov I.I." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    timetable.Add(
        (time2, "Group1"),
        { StartTime = (13, 40)
          EndTime = (15, 15)
          Subject = "Physics"
          Teachers = [ "Petrov P.P." ]
          Audience = 101
          ClassType = ExcelTimetableParser.Timetable.ActivityType.Lecture }
    )

    let teacher =
        { Name =
            { Name = "Ivan"
              Surname = "Ivanov"
              Patronymic = Some "Ivanovich" }
          Department = 1
          Preferences =
            [ GeneralPreference(
                  IntersectTimeWithTeacher(
                      { Name = "Petr"
                        Surname = "Petrov"
                        Patronymic = Some "Petrovich" },
                      Mandatory
                  )
              ) ] }

    let errors = validateGeneralPreferences teacher "Ivanov I.I." timetable
    Assert.IsNotEmpty(errors)
