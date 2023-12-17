module Main

open System

open LoadParser

let processValidation () =
    Console.Write("Введите путь для файла с педагогической нагрузкой: ")
    let loadFilePath = Console.ReadLine()
    let workbook = openWorkbook loadFilePath
    let courses = readExcelFile workbook

    // Console.Write("Введите путь для файла с пожеланиями преподавателей:")
    // let wishFilePath = Console.ReadLine()

    // Console.Write("Введите путь для файла с составленным расписанием:")
    // let wishTimetablePath = Console.ReadLine()    

    0

let validation = processValidation ()