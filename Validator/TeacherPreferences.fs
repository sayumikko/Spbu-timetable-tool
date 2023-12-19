module TeacherPreferences

type Prioirity =
    | Mandatory
    | Desirable
    | Neutral
    | NotDesirable
    | Avoidable

type DayOfWeek =
    | Monday
    | Tuesday
    | Wednesday
    | Thursday
    | Friday
    | Saturday

type AudienceEquipment =
    | Projector
    | WhiteBoard
    | NumberOfBoards of int
    | ComputerAudience
    | Computer
    | TimeService // "Служба времени", запрос кафедры астрономии/астрофизики
    | Capacity of int

type AudienceNumber = int
type Time = int * int // Часы и минуты
type StartTime = Time
type FinalTime = Time

type Audience = AudienceNumber * Prioirity
type TimeSlot = StartTime * FinalTime * Prioirity
type DayOfWeekSlot = (DayOfWeek * Prioirity) option * TimeSlot list option

type GeneralPreferences =
    | MaxDaysPerWeek of int
    | MinDaysPerWeek of int
    | MaxClassesPerDay of int
    | MinClassesPerDay of int
    | Gaps of (IsNeeded * Prioirity)

and IsNeeded =
    | IsNeeded
    | Neutral
    | Avoidable

type SpecificPreference =
    { Target: PreferenceTarget
      DesiredOptions: DesiredOption list }

and PreferenceTarget =
    | BySubject of string
    | ByGroup of string

and DesiredOption =
    | Audience of Audience
    | Equipment of AudienceEquipment
    | Time of DayOfWeekSlot

type TeacherPreference =
    | GeneralPreference of GeneralPreferences
    | SpecificOptions of DesiredOption list
    | SubjectOrGroupSpecificPreference of SpecificPreference

type Teacher =
    { Name: string
      Preferences: TeacherPreference list }
