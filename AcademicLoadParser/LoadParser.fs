module LoadParser

open System
open System.IO

open FParsec
open NPOI.SS.UserModel
open NPOI.XSSF.UserModel

open System.Text.RegularExpressions

let curricula =
    [ 5751, [ "М02"; "М03" ]
      5665, [ "М04" ]
      5506, [ "М06" ]
      5666, [ "М07" ]
      5890, [ "М08" ]
      5775, [ "М09" ]
      5213, [ "Б04"; "Б05"; "Б06" ] // Прикладная математика, программирование и искусственный интеллект с 2022
      5004, [ "Б04"; "Б05"; "Б06" ] // Прикладная математика и информатика до 2021 включительно
      5162, [ "Б04"; "Б08"; "Б09"; "Б10" ]
      5080, [ "Б11"; "Б15"; "Б17" ]
      5008, [ "Б12" ]
      5001, [ "Б13" ]
      5161, [ "Б14" ]
      5212, [ "Б16"; "Б18" ]
      5088, [ "С01" ]
      5089, [ "С02" ]
      5012, [ "С03" ] ]

type Course =
    { Semester: int
      CourseCode: string
      CourseName: string
      CourseType: string
      ActivityType: string
      Teacher: string
      Groups: string list
      Department: string
      AcademicHours: int }

let parseCourseCodeAndName (row: IRow) =
    let course = row.GetCell(3).ToString()

    let courseParser =
        pipe2 (manyMinMaxSatisfy 6 6 (isDigit)) (spaces >>. restOfLine false) (fun number title -> number, title)

    match run courseParser course with
    | Success (result, _, _) -> result
    | _ ->
        raise
        <| new System.Exception("Неверный формат записи кода и названия курса")


let groupPattern = @"\d{2}\.[СБМ]\d{2}-мм"

let isMatchGroup (input: string) =
    let regex = new Regex(groupPattern)
    regex.IsMatch(input)

let groupFromCurricula groups =
    groups
    |> List.fold
        (fun acc (year, curriculum) ->
            match List.tryFind (fun (curriculumNumber, _) -> curriculumNumber = curriculum) curricula with
            | Some (_, g) ->
                let newGroups = List.map (fun x -> String.Concat(string year, ".", x, "-мм")) g
                newGroups @ acc
            | None -> acc)
        []

let parseCurriculum (curriculum) =
    let curriculumParser =
        pipe3 (pint32 .>> pchar '/') (pint32 .>> pchar '/') pint32 (fun number1 number2 _ -> number1, number2)

    let curriculum =
        match run (sepBy1 curriculumParser (pchar ',' .>> spaces)) curriculum with
        | Success (res, _, _) -> groupFromCurricula res
        | _ ->
            raise
            <| new System.Exception("Неверный формат записи учебного плана")

    curriculum

let parseContingent (contigent) =
    let regex = new Regex(groupPattern)

    let res =
        regex.Matches(contigent)
        |> Seq.cast<Match>
        |> Seq.map (fun matchObj -> matchObj.Value)
        |> Seq.distinct
        |> Seq.toList

    res

let parseGroup (row: IRow) =
    let contingent = row.GetCell(14).ToString()
    let curriculum = row.GetCell(15).ToString()

    let groups =
        match isMatchGroup contingent with
        | true -> parseContingent contingent
        | false -> parseCurriculum curriculum

    groups

let parseSemester (row: IRow) =
    let semester = row.GetCell(0).ToString()

    let semesterParser =
        pipe2 (pstring "Семестр" >>. spaces) (pint32) (fun _ number -> number)

    match run semesterParser semester with
    | Success (result, _, _) -> result
    | _ ->
        raise
        <| new System.Exception("Неверный формат указания семестра")


let parseTeacher (row: IRow) =
    let teacher = row.GetCell(8).ToString()

    let teacherParser = manyTill anyChar (pchar ',')

    match run teacherParser teacher with
    | Success (result, _, _) -> System.String.Concat(result)
    | _ ->
        raise
        <| new System.Exception("Неверный формат записи имени преподавателя")

let parseAcademicHours (row: IRow) =
    let teacher = row.GetCell(11).ToString()
    let mutable intValue = 0

    match Int32.TryParse(teacher, &intValue) with
    | true -> intValue
    | false ->
        raise
        <| new System.Exception("Неверный формат записи академических часов")


let parseCourse (row: IRow) =
    let semester = parseSemester row
    let courseCode, courseName = parseCourseCodeAndName row
    let courseType = row.GetCell(5).ToString()
    let activityType = row.GetCell(7).ToString()
    let teacher = parseTeacher row
    let department = row.GetCell(9).ToString()
    let group = parseGroup row
    let hours = parseAcademicHours row

    { Semester = semester
      CourseCode = courseCode
      CourseName = courseName
      CourseType = courseType
      ActivityType = activityType
      Teacher = teacher
      Groups = group
      AcademicHours = hours
      Department = department }

let openWorkbook (filePath: string) =
    try
        use fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read)
        new XSSFWorkbook(fileStream)
    with
    | :? FileNotFoundException as ex ->
        printfn "Файл не найден: %s" ex.FileName
        exit -1
    | :? DirectoryNotFoundException ->
        printfn
            "Ошибка: Не удается найти указанный путь '%s'. Пожалуйста, проверьте, что путь корректен и файл существует."
            filePath

        exit -1
    | ex ->
        printfn "Произошла ошибка: %s" ex.Message
        exit -1

let readExcelFile (workbook: XSSFWorkbook) =
    let mutable courses: Course list = []

    for sheetIndex = 0 to workbook.NumberOfSheets - 1 do
        let sheet = workbook.GetSheetAt(sheetIndex)

        for rowIndex = 1 to sheet.LastRowNum - 1 do
            let row = sheet.GetRow(rowIndex)

            let course =
                try
                    parseCourse row
                with
                | _ as ex ->
                    printfn "Ошибка в строке %d: %s" rowIndex ex.Message
                    exit -2

            match course.CourseCode with
            | "900000" -> courses <- courses
            | _ -> courses <- course :: courses

    courses

let parseAcademicLoadTeachers (filePath: string) =
    let workbook = openWorkbook filePath
    let courses = readExcelFile workbook

    let teacherInfos =
        courses
        |> List.map (fun course ->
            (course.Teacher, course.Department, course.CourseName, course.ActivityType, course.Groups))

    teacherInfos

[<EntryPoint>]
let main args = 0
