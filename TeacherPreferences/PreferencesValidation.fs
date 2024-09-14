module TeacherPreferences.PreferencesValidation

open TeacherPreferencesDataModel
open ExcelTimetableParser.Timetable
open TeacherPreferences
open System.Collections.Generic
open AppContextMapper
open GeneralPreferencesValidation
open SpecificPreferencesValidation
open TimePreferencesValidation
open System

type ValidationResult =
    | Valid
    | Invalid of string list


let convertTeachers
    (teacherDict: IDictionary<int, TeacherPreferencesDataModel.Teacher>)
    (teachers: TeacherPreferencesDataModel.Teacher list)
    : Teacher list =
    teachers |> List.map (mapTeacher teacherDict)


let abbreviateTeacherName (teacher: TeacherName) =
    let initials =
        match teacher.Patronymic with
        | Some patr when not (String.IsNullOrEmpty(patr)) -> $"{teacher.Name.[0]}.{patr.[0]}."
        | _ -> $"{teacher.Name.[0]}."

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
    | None -> [ $"Преподаватель {abbreviateTeacherName teacher.Name} не найден в расписании." ]

let validate (preferences: ApplicationContext) (timetable: Timetable) : ValidationResult =
    let dataService = new DataService(preferences)
    let teachers = dataService.GetTeachers()

    let teacherDict: IDictionary<int, TeacherPreferencesDataModel.Teacher> =
        teachers |> Seq.map (fun t -> t.Id, t) |> dict

    let teacherList =
        teachers
        |> Seq.toList
        |> convertTeachers teacherDict

    let errors =
        teacherList
        |> List.collect (fun teacher -> findTeacherAndValidate teacher timetable)

    if List.isEmpty errors then
        Valid
    else
        Invalid errors
