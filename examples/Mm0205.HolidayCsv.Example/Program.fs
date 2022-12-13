open System
open System.CommandLine
open System.Net.Http
open System.Text
open System.Threading.Tasks
open Mm0205.HolidayCsv

let rootCommand = RootCommand("Mm0205.HolidayCsv example")

module Printer =
    let printHoliday x =
        printfn "date=%s name=%s" ((Holiday.date x).ToString("yyyy/MM/dd")) (Holiday.name x)

    let printHolidays = List.iter printHoliday

    let printError e =
        let msg =
            match e with
            | InvalidHolidayText s -> $"祝日日付エラー: {s}"
            | InvalidDateValue s -> $"CSVレコードフォーマットエラー: {s}"

        printfn $"{msg}"

    let printResult result =
        result |> Result.map printHolidays |> Result.mapError printError |> ignore


module WebExample =
    let private httpClient = new HttpClient()

    let downloadAndReadCsvAsync (uri: string) =
        let uri =
            if String.IsNullOrWhiteSpace uri then
                Uri("https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv")
            else
                Uri(uri)

        task {
            use! responseMessage = httpClient.GetAsync(uri)
            use! stream = responseMessage.Content.ReadAsStreamAsync()
            let! result = HolidayCsvReader.read stream None
            Printer.printResult result
        }
        :> Task


    let addCommand (rootCommand: RootCommand) =
        let webCommand = Command("web", "Read holiday csv from web")
        let uriOption = System.CommandLine.Option<string>("--uri", "URI of Holiday CSV.")
        webCommand.AddOption uriOption
        webCommand.SetHandler(downloadAndReadCsvAsync, uriOption)
        rootCommand.AddCommand webCommand

module LocalFileExample =

    let getEncoding (encodingName: string) =
        if String.IsNullOrWhiteSpace encodingName then
            None
        else
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
            Encoding.GetEncoding encodingName |> Some

    let readAndPrint (filePath: string) (encoding: string) =
        let encoding = getEncoding encoding

        task {
            use stream = System.IO.File.OpenRead filePath
            let! result = HolidayCsvReader.read stream encoding
            Printer.printResult result
        }
        :> Task

    let addCommand (rootCommand: RootCommand) =
        let localCommand = Command("local", "Read local holiday csv file")

        let filePathArg =
            Argument<string>("holiday csv path", description = "path to local holiday csv file")

        let encodingOption =
            Option<string>("--encoding", description = "csv file encoding. default Shift_JIS")

        localCommand.AddArgument filePathArg
        localCommand.AddOption encodingOption
        localCommand.SetHandler(readAndPrint, filePathArg, encodingOption)
        rootCommand.AddCommand localCommand


module Program =
    [<EntryPoint>]
    let main args =
        WebExample.addCommand rootCommand
        LocalFileExample.addCommand rootCommand

        rootCommand.InvokeAsync(args)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore

        0
