module ExcelTimetableParser.TimetableParser

open System.Collections
open System.IO
open System

open System.Text.RegularExpressions
open ClosedXML.Excel
open Timetable

type ParsingResult =
    { Timetable: Timetable
      Errors: string list }

module ExcelParser =
    let enumeratorLength enumerator =
        let rec inner initial (enumerator: IEnumerator) =
            match enumerator.MoveNext() with
            | true -> inner (initial + 1) enumerator
            | false -> initial

        inner 0 enumerator

    let rec fromIEnumerator<'T> (enumerator: IEnumerator) =
        seq {
            if enumerator.MoveNext() then
                enumerator.Current :?> 'T
                yield! fromIEnumerator enumerator
        }
        |> Seq.toList

    let (|Group|_|) (str: string) =
        let success, result = Int32.TryParse(str)
        if success then Some(result) else None

    let listOfMatches (matches: MatchCollection) =
        matches.GetEnumerator()
        |> fromIEnumerator<Match>
        |> Seq.map string
        |> Seq.toList

    let (|Date|_|) (str: string) =
        if str.ToLower().Trim() = "день" then
            Some()
        else
            None

    let (|Time|_|) (str: string) =
        if str.ToLower().Trim() = "время" then
            Some()
        else
            None

    let cellsToSeq (cells: IXLCells) =
        cells.GetEnumerator() |> fromIEnumerator<IXLCell>

    // Считаем ячейку значимой ттт она имеет текст и ширина/высота > 5
    let cellIsMeaningful (cell: IXLCell) =
        let height = cell.WorksheetRow().Height
        let width = cell.WorksheetColumn().Width
        let value = cell.Value.ToString()
        value <> "" && height > 3 && width > 3


    let straightenString (str: string) =
        let str = str.ReplaceLineEndings(" ")
        let splitted = str.Split()
        let splitted = Seq.filter (fun (x: string) -> x.Length > 0) splitted
        let str = String.Join(" ", splitted)
        str

    // Пытаемся прочитать заголовок, там должны быть группы и колонки для даты и времени
    // Пусть 2 - минимальное число ячеек с текстом для заголовка, мб его в константу
    let rec parseHeader startRow (worksheet: IXLWorksheet) =
        let potentialHeaderRow = worksheet.Row startRow

        let usedCells =
            potentialHeaderRow.CellsUsed()
            |> cellsToSeq
            |> Seq.filter cellIsMeaningful

        if Seq.length usedCells < 2 then
            parseHeader (startRow + 1) worksheet
        else
            (startRow, potentialHeaderRow)


    // Находим лекторов Фамилия И.О. или Фамилия ИО
    let tryFetchLecturer (str: string) =
        let pattern =
            @"([А-Я][а-я]* [А-Я]\.[А-Я]\.)|([А-Я][а-я]* [А-Я]\.[А-Я])|([А-Я][а-я]* [А-Я][А-Я]\ )"

        let matches = Regex.Matches(str, pattern, RegexOptions.IgnoreCase)
        matches

    // Аудитория это 4-значное число
    // TODO: Скорректировать
    let tryFetchAuditorium (str: string) =
        let pattern = @"([0-9][0-9][0-9][0-9])|([0-9][0-9][0-9])|(0[0-9])"
        let matches = Regex.Matches(str, pattern, RegexOptions.IgnoreCase)
        matches


    // TODO: Удалить все что в скобках мб
    let tryFetchTitle = id

    let weekdayOfString (s: string) (errors: ResizeArray<string>) : DayOfWeek =
        let alphas = Regex("[а-я]*")

        let processed =
            alphas.Matches(s.ToLower())
            |> listOfMatches
            |> List.head

        match processed.ToLower() with
        | "понедельник" -> Monday
        | "вторник" -> Tuesday
        | "среда" -> Wednesday
        | "четверг" -> Thursday
        | "пятница" -> Friday
        | "суббота" -> Saturday
        | _ ->
            errors.Add($"Не удалось распознать {s} как день недели. Было вставлено воскресенье")
            Sunday

    let convertStringToTime (weekDay: string) (str: string) (cell: IXLCell) (errors: ResizeArray<string>) =
        // Converts xx:yy to xx, yy
        let convTime (str: string) =
            let [| hh; mm |] = str.Split(":")
            (Int32.Parse(hh), Int32.Parse(mm))

        let weekDay = weekdayOfString weekDay errors
        let timePattern = @"[0-9][0-9]:[0-9][0-9]"
        let matches = Regex.Matches(str, timePattern, RegexOptions.IgnoreCase)

        match matches |> listOfMatches with
        | [ s; e ] ->
            Some
                { Weekday = weekDay
                  StartTime = (convTime s)
                  EndTime = (convTime e) }
        | [ s ] ->
            Some
                { Weekday = weekDay
                  StartTime = (convTime s)
                  EndTime = (convTime s) }
        | _ ->
            errors.Add($"Не выходит распознать время из ячейки {cell.Address.ToString()} {str}")
            None


    let fetchActivity (str: string) =
        let mutable result = NoClassType

        if Regex
            .Matches(
                str,
                @"(\(пр\. з\.\))|(прак)",
                RegexOptions.IgnoreCase
            )
            .Count > 0 then
            result <- Practical

        if Regex
            .Matches(
                str,
                @"\(лекц\.\)|(лекц)",
                RegexOptions.IgnoreCase
            )
            .Count > 0 then
            result <- Lecture

        result

    let someOr none option =
        match option with
        | Some x -> x
        | None -> none


    // Обрабатываем одну запись о лекции
    let processLecture
        (cell: IXLCell)
        (str: string)
        (weekDay: string)
        (timeString: string)
        (errors: ResizeArray<string>)
        : TimetableSlot option =
        let teachers = tryFetchLecturer str

        if teachers.Count = 0 then
            errors.Add($"Преподаватель не найден для {str}")

        let teachers = teachers |> listOfMatches

        let auditoriums = tryFetchAuditorium str

        if auditoriums.Count = 0 then
            errors.Add($"Аудитория не найдена для {str}")

        let auditoriums =
            auditoriums
            |> listOfMatches
            |> Seq.map Int32.Parse
            |> Seq.distinct

        let auditorium = auditoriums |> Seq.tryHead |> someOr 0

        if auditoriums |> Seq.length > 1 then
            errors.Add($"Было найдено несколько аудиторий %A{auditoriums}. Была выбрана первая: {auditorium}")

        let lectureTitle = tryFetchTitle str

        match convertStringToTime weekDay timeString cell errors with
        | Some time ->
            let activity = fetchActivity str

            Some
                { StartTime = time.StartTime
                  EndTime = time.EndTime
                  Subject = lectureTitle
                  Teachers = teachers
                  Audience = auditorium
                  ClassType = activity }
        | None ->
            errors.Add($"Ошибка обработки времени для ячейки {cell.Address.ToString()}")
            None

    let mutable weekdayString = ""
    let mutable timeString = ""

    let processRow (tt: Timetable) (headerRow: IXLRow) (row: IXLRow) (errors: ResizeArray<string>) =
        let worksheet = row.Worksheet

        // Большая ячейка -> все маленькие ячейки с ее содержанием
        let flattenSubCells (cells: IXLCell seq) =
            let subCells (cell: IXLCell) =
                if cell.IsMerged() then
                    let subCells = cell.MergedRange().Unmerge().Cells() |> cellsToSeq

                    let updated =
                        subCells
                        |> Seq.map (fun cell -> cell.SetValue(cell.Value))

                    updated
                else
                    [ cell ]

            Seq.map subCells cells |> Seq.concat

        // Если это группа, добавляем
        let processCell (cell: IXLCell) =
            assert (cell.AsRange().ColumnCount() = 1)
            let cellValue = cell.Value.ToString()
            let headerRange = cell.WorksheetColumn().Intersection(headerRow)
            assert (headerRange.NumberOfCells > 0)

            let headerValue =
                worksheet.Cell(headerRange.FirstAddress)
                |> (fun cell -> cell.Value.ToString())

            match headerValue, cellValue with
            | Date, nonempty when nonempty <> "" -> weekdayString <- cellValue
            | Time, _ -> timeString <- cellValue
            | Group _, "" -> ()
            | Group g, _ ->
                let straightened = straightenString cellValue

                match processLecture cell straightened weekdayString timeString errors with
                | Some lectureEntry ->
                    match convertStringToTime weekdayString timeString cell errors with
                    | Some time ->
                        tt.TryAdd((time, g.ToString()), lectureEntry)
                        |> ignore
                    | None -> errors.Add($"Ошибка обработки времени для ячейки {cell.Address.ToString()}")
                | None -> errors.Add($"Ошибка обработки лекции для ячейки {cell.Address.ToString()}")
            | _ -> ()



        let cellSeq =
            row.Cells()
            |> cellsToSeq
            |> Seq.filter cellIsMeaningful
            |> flattenSubCells
            |> Seq.toList

        Seq.iter processCell cellSeq

    let processWorksheet (worksheet: IXLWorksheet) (errors: ResizeArray<string>) : Timetable * string list =
        let tt = ttEmpty ()

        // Ищем заголовок: Первая строка
        // Заголовок обязан содержать более двух ячеек с ненулевым содержанием
        // И также соответствующим критериям cellIsMeaningful
        let headerRowNumber, headerRow = parseHeader 1 worksheet

        // Берем все строки после заголовка и обрабатываем
        let rowsEnumerator = worksheet.Rows().GetEnumerator()
        let rows = rowsEnumerator |> fromIEnumerator
        let rows = Seq.skip headerRowNumber rows
        Seq.iter (fun row -> processRow tt headerRow row errors) rows

        // Возвращаем заполненное расписание и список ошибок
        (tt, List.ofSeq errors)


    let processWorksheets (workbook: IXLWorkbook) (errors: ResizeArray<string>) : Timetable * string list =
        let finalTimetable = ttEmpty ()

        let expectedSheetNames =
            [ "1 курс"
              "2 курс"
              "3 курс"
              "4 курс"
              "5 курс"
              "6 курс" ]

        for sheet in workbook.Worksheets do
            let isValidSheetName =
                expectedSheetNames
                |> List.exists (fun name -> name.Equals(sheet.Name, StringComparison.OrdinalIgnoreCase))

            if isValidSheetName then
                let timetable, sheetErrors = processWorksheet sheet errors

                for KeyValue (key, value) in timetable do
                    finalTimetable.[key] <- value

                errors.AddRange(sheetErrors)
            else
                errors.Add(sprintf "Найден лист с неожиданным названием '%s'. Он будет пропущен" sheet.Name)

        (finalTimetable, List.ofSeq errors)

    let processFile (filePath: string) : Result<ParsingResult, string> =
        try
            use workbook = new XLWorkbook(filePath)
            let errors = ResizeArray<string>()
            let timetable, parsingErrors = processWorksheets workbook errors

            Ok
                { Timetable = timetable
                  Errors = List.ofSeq parsingErrors }
        with
        | :? Exception as ex -> Error ex.Message
