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
    | Blackboard

type Department =
    | Astromony
    | Astrophysics
    | Algebra
    | Geometry
    | ComputationalMathematics
    | DifferentialEquations
    | Informatics
    | InformationAndAnalyticalSystems
    | Hydroaeromechanics
    | OperationsResearch
    | MathematicalAnalysis
    | MathematicalPhysics
    | CelestialMechanics
    | ParallelAlgorithms
    | AppliedCybernetics
    | SystemProgramming
    | StatisticalModeling
    | TheoreticalAndAppliedMechanics
    | TheoreticalCybernetics
    | ProbabilityheoryAndMathematicalStatistics
    | PhysicalMechanics
    | ForeignLanguages
    | TheoryOfElasticity

type ClassType = 
    | Lecture
    | Laboratory
    | Seminar

type AudienceNumber = int
type HoursAndMinutes = int * int // Часы и минуты
type StartTime = HoursAndMinutes
type FinalTime = HoursAndMinutes

type Audience = AudienceNumber * Prioirity
type TimeSlot = StartTime * FinalTime * Prioirity
type DayOfWeekSlot = DayOfWeek * Prioirity
type Time = DayOfWeekSlot option * TimeSlot list option

type PreferenceTarget =
    | BySubject of string * ClassType option
    | ByGroup of string * ClassType option

type SpecificPreference =
    | Audience of Audience
    | Equipment of AudienceEquipment
    | Time of Time
    | UniteGroups of Prioirity
    | UniteClasses of Prioirity
    | AlternateBySubgroups of Prioirity
    | UniteSubgroups of Prioirity
    | OneClassInTwoWeeks of Prioirity
    | InOneDay of Prioirity

type TeacherPreference =
    | GeneralPreference of GeneralPreferences
    | SpecificPreference of SpecificPreference
    | SubjectOrGroupSpecificPreference of PreferenceTarget * SpecificPreference list

and GeneralPreferences =
    | MaxDaysPerWeek of int * Prioirity
    | MinDaysPerWeek of int * Prioirity
    | FreeDaysPerWeek of int * Prioirity
    | MaxClassesPerDay of int * Prioirity
    | MinClassesPerDay of int * Prioirity
    | Compactness of Prioirity
    | IntersectTimeWithTeacher of Teacher * Prioirity
    | Gaps of Prioirity

and Teacher =
    { Name: TeacherName
      Department: Department
      Preferences: TeacherPreference list }

and TeacherName =
    { Name: string
      Surname: string
      Patronymic: string }
