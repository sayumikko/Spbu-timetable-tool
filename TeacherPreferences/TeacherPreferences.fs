module TeacherPreferences.TeacherPreferences

type Priority =
    | Mandatory
    | Desirable
    | Neutral
    | NotDesirable
    | Avoidable

type DayOfWeekSlot =
    | Monday
    | Tuesday
    | Wednesday
    | Thursday
    | Friday
    | Saturday
    | AllWeek

type AudienceEquipment =
    | Projector
    | WhiteBoard
    | NumberOfBoards of int
    | ComputerAudience
    | Computer
    | TimeService // "Служба времени", запрос кафедры астрономии/астрофизики
    | Capacity of int
    | Blackboard

type Department = int

type ClassType =
    | Lecture
    | Seminar
    | Practical
    | Laboratory
    | NoClassType

type HoursAndMinutes = int * int
type StartTime = HoursAndMinutes
type EndTime = HoursAndMinutes

type Group = string
type Subject = string
type TimeSlot = StartTime * EndTime
type Time = (DayOfWeekSlot * TimeSlot) * Priority

type SpecificPreference =
    | Audience of int * Priority
    | Equipment of AudienceEquipment * Priority
    | UniteGroups of Priority
    | UniteClasses of Priority
    | AlternateBySubgroups of Priority
    | UniteSubgroups of Priority
    | OneClassInTwoWeeks of Priority
    | InOneDay of Priority

type TeacherPreference =
    | GeneralPreference of GeneralPreferences
    | SpecificPreference of Group * (Subject * ClassType) * SpecificPreference
    | Time of Time

and GeneralPreferences =
    | MaxDaysPerWeek of int * Priority
    | MinDaysPerWeek of int * Priority
    | FreeDaysPerWeek of int * Priority
    | MaxClassesPerDay of int * Priority
    | MinClassesPerDay of int * Priority
    | Compactness of Priority
    | Gaps of Priority
    | IntersectTimeWithTeacher of TeacherName * Priority


and TeacherName =
    { Name: string
      Surname: string
      Patronymic: string option }

and Teacher =
    { Name: TeacherName
      Department: Department
      Preferences: TeacherPreference list }
