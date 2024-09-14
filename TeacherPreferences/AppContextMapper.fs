module TeacherPreferences.AppContextMapper

open TeacherPreferences
open System.Collections.Generic

let mapDayOfWeek (dayOfWeek: TeacherPreferencesDataModel.DayOfWeek) : DayOfWeekSlot =
    match dayOfWeek with
    | TeacherPreferencesDataModel.DayOfWeek.Monday -> Monday
    | TeacherPreferencesDataModel.DayOfWeek.Tuesday -> Tuesday
    | TeacherPreferencesDataModel.DayOfWeek.Wednesday -> Wednesday
    | TeacherPreferencesDataModel.DayOfWeek.Thursday -> Thursday
    | TeacherPreferencesDataModel.DayOfWeek.Friday -> Friday
    | TeacherPreferencesDataModel.DayOfWeek.Saturday -> Saturday
    | TeacherPreferencesDataModel.DayOfWeek.AllWeek -> AllWeek
    | _ -> failwith "Неизвестный день недели"


let mapPriority (priority: TeacherPreferencesDataModel.Priority) : Priority =
    match priority with
    | TeacherPreferencesDataModel.Priority.Mandatory -> Mandatory
    | TeacherPreferencesDataModel.Priority.Desirable -> Desirable
    | TeacherPreferencesDataModel.Priority.Neutral -> Neutral
    | TeacherPreferencesDataModel.Priority.NotDesirable -> NotDesirable
    | TeacherPreferencesDataModel.Priority.Avoidable -> Avoidable
    | _ -> failwith "Неизвестный приоритет"

let mapSpecificPreference (sp: TeacherPreferencesDataModel.SpecificPreference) : SpecificPreference =
    match sp.PreferenceType with
    | TeacherPreferencesDataModel.SpecificPreferenceType.UniteGroups -> UniteGroups(mapPriority sp.Priority)
    | TeacherPreferencesDataModel.SpecificPreferenceType.UniteClasses -> UniteClasses(mapPriority sp.Priority)
    | TeacherPreferencesDataModel.SpecificPreferenceType.AlternateBySubgroups ->
        AlternateBySubgroups(mapPriority sp.Priority)
    | TeacherPreferencesDataModel.SpecificPreferenceType.UniteSubgroups -> UniteSubgroups(mapPriority sp.Priority)
    | TeacherPreferencesDataModel.SpecificPreferenceType.OneClassInTwoWeeks ->
        OneClassInTwoWeeks(mapPriority sp.Priority)
    | TeacherPreferencesDataModel.SpecificPreferenceType.InOneDay -> InOneDay(mapPriority sp.Priority)
    | _ -> failwith "Неизвестный тип предпочтений по курсу"

let mapClassType (classType: TeacherPreferencesDataModel.ClassType) : TeacherPreferences.TeacherPreferences.ClassType =
    match classType with
    | TeacherPreferencesDataModel.ClassType.Lecture -> TeacherPreferences.TeacherPreferences.ClassType.Lecture
    | TeacherPreferencesDataModel.ClassType.Laboratory -> TeacherPreferences.TeacherPreferences.ClassType.Laboratory
    | TeacherPreferencesDataModel.ClassType.Seminar -> TeacherPreferences.TeacherPreferences.ClassType.Seminar
    | TeacherPreferencesDataModel.ClassType.Practical -> TeacherPreferences.TeacherPreferences.ClassType.Practical
    | TeacherPreferencesDataModel.ClassType.None -> TeacherPreferences.TeacherPreferences.ClassType.NoClassType
    | _ -> failwith "Неизвестный тип занятий"

let mapEquipment
    (equipment: TeacherPreferencesDataModel.AudienceEquipment)
    : TeacherPreferences.TeacherPreferences.AudienceEquipment =
    let priority = mapPriority equipment.Priority

    match equipment.EquipmentType with
    | TeacherPreferencesDataModel.AudienceEquipmentType.Blackboard -> Blackboard
    | TeacherPreferencesDataModel.AudienceEquipmentType.Projector -> Projector
    | TeacherPreferencesDataModel.AudienceEquipmentType.Computer -> Computer
    | TeacherPreferencesDataModel.AudienceEquipmentType.ComputerAudience -> ComputerAudience
    | TeacherPreferencesDataModel.AudienceEquipmentType.TimeService -> TimeService
    | TeacherPreferencesDataModel.AudienceEquipmentType.WhiteBoard -> WhiteBoard
    | TeacherPreferencesDataModel.AudienceEquipmentType.Capacity ->
        let intValueOption =
            if equipment.IntValue.HasValue then
                Some equipment.IntValue.Value
            else
                None

        match intValueOption with
        | Some value -> Capacity(value)
        | None -> failwith "Не определена вместимость"
    | TeacherPreferencesDataModel.AudienceEquipmentType.NumberOfBoards ->
        let intValueOption =
            if equipment.IntValue.HasValue then
                Some equipment.IntValue.Value
            else
                None

        match intValueOption with
        | Some value -> NumberOfBoards(value)
        | None -> failwith "Не определено количество досок"
    | _ -> failwith "Неизвестный тип оборудования"

let mapCourse (course: TeacherPreferencesDataModel.Course) : (Group * (Subject * ClassType) * SpecificPreference list) =
    let specificPreferences =
        course.SpecificPreferences
        |> Seq.map mapSpecificPreference
        |> Seq.toList

    let audiences =
        course.Audiences
        |> Seq.map (fun a -> Audience(a.Number, mapPriority a.Priority))
        |> Seq.toList

    let equipments =
        course.AudienceEquipments
        |> Seq.map (fun ae -> Equipment(mapEquipment ae, mapPriority ae.Priority))
        |> Seq.toList

    let classType = mapClassType course.ClassType
    (course.Group, (course.SubjectName, classType), specificPreferences @ audiences @ equipments)


let mapTimeSlot (timeSlot: TeacherPreferencesDataModel.TimeSlot) : Time =
    let priority = mapPriority timeSlot.Priority

    let dayOfWeekSlot: DayOfWeekSlot =
        if timeSlot.DayOfWeek.HasValue then
            mapDayOfWeek timeSlot.DayOfWeek.Value
        else
            AllWeek

    let timeSlot: TimeSlot =
        match timeSlot.StartTime.HasValue, timeSlot.EndTime.HasValue with
        | true, true ->
            let startTime = timeSlot.StartTime.Value
            let endTime = timeSlot.EndTime.Value

            (startTime.Hours, startTime.Minutes), (endTime.Hours, endTime.Minutes)

        | _ -> ((9, 30), (18, 45))

    ((dayOfWeekSlot, timeSlot), priority)

let mapGeneralPreference
    (teacherDict: IDictionary<int, TeacherPreferencesDataModel.Teacher>)
    (gp: TeacherPreferencesDataModel.GeneralPreference)
    : TeacherPreference =
    let priority = mapPriority gp.Priority

    match gp.PreferenceType with
    | TeacherPreferencesDataModel.GeneralPreferenceType.Compactness -> GeneralPreference(Compactness priority)
    | TeacherPreferencesDataModel.GeneralPreferenceType.MaxDaysPerWeek ->
        let intValueOption =
            if gp.IntValue.HasValue then
                Some gp.IntValue.Value
            else
                None

        match intValueOption with
        | Some value -> GeneralPreference(MaxDaysPerWeek(value, priority))
        | None -> failwith "MaxDaysPerWeek требует целочисленного значения"
    | TeacherPreferencesDataModel.GeneralPreferenceType.MinDaysPerWeek ->
        let intValueOption =
            if gp.IntValue.HasValue then
                Some gp.IntValue.Value
            else
                None

        match intValueOption with
        | Some value -> GeneralPreference(MinDaysPerWeek(value, priority))
        | None -> failwith "MinDaysPerWeek требует целочисленного значения"
    | TeacherPreferencesDataModel.GeneralPreferenceType.FreeDaysPerWeek ->
        let intValueOption =
            if gp.IntValue.HasValue then
                Some gp.IntValue.Value
            else
                None

        match intValueOption with
        | Some value -> GeneralPreference(FreeDaysPerWeek(value, priority))
        | None -> failwith "FreeDaysPerWeek требует целочисленного значения"
    | TeacherPreferencesDataModel.GeneralPreferenceType.MaxClassesPerDay ->
        let intValueOption =
            if gp.IntValue.HasValue then
                Some gp.IntValue.Value
            else
                None

        match intValueOption with
        | Some value -> GeneralPreference(MaxClassesPerDay(value, priority))
        | None -> failwith "MaxClassesPerDay требует целочисленного значения"
    | TeacherPreferencesDataModel.GeneralPreferenceType.MinClassesPerDay ->
        let intValueOption =
            if gp.IntValue.HasValue then
                Some gp.IntValue.Value
            else
                None

        match intValueOption with
        | Some value -> GeneralPreference(MinClassesPerDay(value, priority))
        | None -> failwith "MinClassesPerDay требует целочисленного значения"
    | TeacherPreferencesDataModel.GeneralPreferenceType.Gaps -> GeneralPreference(Gaps priority)
    | TeacherPreferencesDataModel.GeneralPreferenceType.IntersectTimeWithTeacher ->
        let intersectWithTeacherName =
            if
                gp.TeacherRefId.HasValue
                && teacherDict.ContainsKey(gp.TeacherRefId.Value)
            then
                let teacher = teacherDict.[gp.TeacherRefId.Value]

                { Name = teacher.Name
                  Surname = teacher.Surname
                  Patronymic = Some(teacher.Patronymic) }
            else
                failwith "Невозможно найти учителя по указанному TeacherRefId"

        GeneralPreference(IntersectTimeWithTeacher(intersectWithTeacherName, priority))
    | _ -> failwith "Неизвестный тип предпочтений"


let mapTeacher
    (teacherDict: IDictionary<int, TeacherPreferencesDataModel.Teacher>)
    (teacher: TeacherPreferencesDataModel.Teacher)
    : Teacher =

    let generalPreferences =
        teacher.GeneralPreferences
        |> Seq.map (mapGeneralPreference teacherDict)
        |> Seq.toList

    let specificPreferences =
        teacher.Courses
        |> Seq.map mapCourse
        |> Seq.collect (fun (group, classInfo, prefs) ->
            prefs
            |> List.map (fun pref -> SpecificPreference(group, classInfo, pref)))
        |> Seq.toList

    let timePreferences =
        teacher.TimeSlots
        |> Seq.map mapTimeSlot
        |> Seq.map (fun time -> TeacherPreference.Time time)
        |> Seq.toList

    { Name =
        { Name = teacher.Name
          Surname = teacher.Surname
          Patronymic = Some(teacher.Patronymic) }
      Department = teacher.DepartmentId
      Preferences =
        generalPreferences
        @ specificPreferences @ timePreferences }
