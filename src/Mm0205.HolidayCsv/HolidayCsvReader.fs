namespace Mm0205.HolidayCsv

open System
open System.IO
open System.Text

/// <summary>
/// 休日CSVのReader。
/// <br/>
/// <br/>
/// CSVのフォーマットは、
/// <a href="https://www8.cao.go.jp/chosei/shukujitsu/gaiyou.html">「国民の祝日」について</a>
/// を参照。<br/>
/// <br/>
/// CSV本体は<a href="https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv">このリンク</a>
/// から取得。
/// </summary>
module HolidayCsvReader =

    /// <summary>
    /// デフォルトのEncoding(Shift_JIS)を取得する。
    /// </summary>
    let private getOrDefaultEncoding =
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
        Option.defaultValue (Encoding.GetEncoding("Shift_JIS"))
        

    /// <summary>
    /// 祝日CSVをStreamから読み込む。
    /// </summary>
    /// <param name="stream">祝日CSVストリーム。</param>
    /// <param name="encoding">エンコーディング。<c>null</c>の場合はデフォルトのEncodingを使用する。</param>
    /// <returns>
    /// 祝日のリストまたはエラー。
    /// </returns>
    let read (stream: Stream) (encoding: Encoding option) =

        let read2 () =
            let encoding = getOrDefaultEncoding encoding
            let mutable holidays = []

            task {
                use sr = new StreamReader(stream, encoding = encoding)
                while not sr.EndOfStream do
                    let! holiday = sr.ReadLineAsync()
                    holidays <- List.append holidays [ holiday ]

                return holidays
            }

        let isHolidayText s =
            if String.IsNullOrEmpty(s) then false
            elif s = "国民の祝日・休日月日,国民の祝日・休日名称" then false
            else true

        let toResultOfList (acc: Result<HolidayT list, HolidayParseError>) (x: Result<HolidayT, HolidayParseError>) =
            match acc with
            | Error e -> Error e
            | Ok list ->
                match x with
                | Error e -> Error e
                | Ok x -> List.append list [ x ] |> Ok

        task {
            let! list = read2 ()

            return
                list
                |> List.filter isHolidayText
                |> List.map Holiday.parse
                |> List.fold toResultOfList (Ok [])
        }
