module TeacherPreferences.PreferencesValidation

open TeacherPreferencesDataModel
open ExcelTimetableParser.Timetable
open TeacherPreferences
open System.Collections.Generic
open AppContextMapper
open GeneralPreferencesValidation
open SpecificPreferencesValidation
open TimePreferencesValidation

type ValidationResult = 
    | Valid
    | Invalid of string list


let convertTeachers (teacherDict: IDictionary<int, TeacherPreferencesDataModel.Teacher>) (teachers: TeacherPreferencesDataModel.Teacher list): Teacher list =
    teachers |> List.map (mapTeacher teacherDict)


let abbreviateTeacherName (teacher: TeacherName) =
    let initials = 
        match teacher.Patronymic with
        | Some patr -> $"{teacher.Name.[0]}.{patr.[0]}."
        | None -> $"{teacher.Name.[0]}."
    $"{teacher.Surname} {initials}"


let findTeacherInTimetable (teacher: TeacherName) (timetable: Timetable) =
    let abbreviatedName = abbreviateTeacherName teacher
    let timetableTeachers =
        timetable.Values
        |> Seq.collect (fun slot -> slot.Teachers)
        |> Seq.distinct
    if Seq.contains abbreviatedName timetableTeachers then
        Some abbreviatedName
    else
        None


let findTeacherAndValidate (teacher: Teacher) (timetable: Timetable) : string list =
    let timetableTeacher = findTeacherInTimetable teacher.Name timetable
    match timetableTeacher with
    | Some abbreviatedName ->
        let timeErrors = validateTime teacher abbreviatedName timetable
        let specificErrors = validateSpecificPreferences teacher abbreviatedName timetable
        let generalErrors = validateGeneralPreferences teacher abbreviatedName timetable
        
        timeErrors @ specificErrors @ generalErrors
    | None -> 
        [ $"Преподаватель {teacher.Name.Surname} не найден в расписании." ]

let timetableToString (timetable: Timetable) : string list =
    timetable
    |> Seq.map (fun kvp ->
        let (time, group) = kvp.Key
        let slot = kvp.Value
        let timeString = time.ToString()
        let subject = slot.Subject
        let teachers = String.concat ", " slot.Teachers
        let audience = slot.Audience
        let classType = slot.ClassType
        
        $"{timeString}, Group: {group}, Subject: {subject}, Teachers: {teachers}, Audience: {audience}, Class Type: {classType}"
    )
    |> Seq.toList

let validate (preferences: ApplicationContext) (timetable: Timetable): ValidationResult =
    let dataService = new DataService(preferences)
    let teachers = dataService.GetTeachers()

    let teacherDict: IDictionary<int, TeacherPreferencesDataModel.Teacher> = 
        teachers 
        |> Seq.map (fun t -> t.Id, t)
        |> dict

    let teacherList = teachers |> Seq.toList |> convertTeachers teacherDict

    let errors =
        teacherList
        |> List.collect (fun teacher ->
            findTeacherAndValidate teacher timetable
        )

    let timetableString = timetableToString timetable
    // Invalid timetableString // Выведем расписание в формате строк
    if List.isEmpty errors then Valid else Invalid errors      