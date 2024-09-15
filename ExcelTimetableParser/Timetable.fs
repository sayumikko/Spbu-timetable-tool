module ExcelTimetableParser.Timetable

open System.Collections.Generic

type DayOfWeek =
    | Monday
    | Tuesday
    | Wednesday
    | Thursday
    | Friday
    | Saturday
    | Sunday

type ActivityType =
    | Lecture
    | Seminar
    | Practical
    | Laboratory
    | NoClassType

type AudienceNumber = int
type HoursAndMinutes = int * int
type StartTime = HoursAndMinutes
type EndTime = HoursAndMinutes

type TeacherName = string

type TimetableSlot =
    { StartTime: StartTime
      EndTime: EndTime
      Subject: string
      Teachers: TeacherName list
      Audience: AudienceNumber
      ClassType: ActivityType }

type Group = string

type Time =
    { Weekday: DayOfWeek
      StartTime: StartTime
      EndTime: EndTime }
    override x.ToString() =
        let weekdayToStringShort =
            function
            | Monday -> "Пн."
            | Tuesday -> "Вт."
            | Wednesday -> "Ср."
            | Thursday -> "Чт."
            | Friday -> "Пт."
            | Saturday -> "Сб."
            | Sunday -> "Вс."

        let timeToString (t: HoursAndMinutes) =
            let (hh, mm) = t
            $"{hh}:{mm}"

        $"{weekdayToStringShort x.Weekday} {timeToString x.StartTime} - {timeToString x.EndTime}"

type Timetable = Dictionary<Time * Group, TimetableSlot>

let ttEmpty () : Timetable =
    Dictionary<Time * Group, TimetableSlot>()
