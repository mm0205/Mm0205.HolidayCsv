namespace Mm0205.HolidayCsv

open System
open System.Text.RegularExpressions

/// <summary>
/// 休日のパースエラー。
/// </summary>
type HolidayParseError =

    /// <summary>
    /// 日付フォーマットエラー。
    /// <br/>
    /// 日付が有効な範囲外の場合など。
    /// </summary>
    | InvalidDateValue of string

    /// <summary>
    /// レコードフォーマットエラー。
    /// <br/>
    /// レコードのフォーマットが"yyyy/M/d,名称"ではないなど。
    /// </summary>
    | InvalidHolidayText of string

/// <summary>
/// 休日データ。
/// </summary>
type HolidayT =
    private { date: DateOnly; name: string }

module Holiday =
    let private holidayRegex =
        Regex(@"^(\d{4})/(\d{1,2})/(\d{1,2}),(.*?)\s*$", RegexOptions.Compiled)

    let private dateFrom year month day =
        let dateText = $"{year}/{month}/{day}"

        match DateOnly.TryParseExact(dateText, "yyyy/M/d") with
        | true, d -> Ok d
        | _ -> Error <| InvalidDateValue dateText

    let private matchToHoliday (m: Match) =
        let year = m.Groups[1].Value in
        let month = m.Groups[2].Value in
        let day = m.Groups[3].Value in
        let name = m.Groups[4].Value in

        dateFrom year month day |> Result.map (fun x -> { date = x; name = name })

    /// <summary>
    /// 祝日データの日付を取得する。
    /// </summary>
    /// <param name="holiday">祝日データ。</param>
    /// <returns>日付。</returns>
    let date holiday = holiday.date

    /// <summary>
    /// 祝日データの名称を取得する。
    /// </summary>
    /// <param name="holiday">祝日データ。</param>
    /// <returns>名称。</returns>
    let name holiday = holiday.name

    /// <summary>
    /// 日付と名称から祝日データを作成する。
    /// </summary>
    /// <param name="date">日付。</param>
    /// <param name="name">名称。<c>null</c>の場合、<see cref="ArgumentNullException"/>をthrowする。</param>
    /// <returns>祝日データ。</returns>
    let from date name =
        if name = null then
            ArgumentNullException(nameof name) |> raise
            
        { date = date; name = name }

    /// <summary>
    /// 祝日レコード文字列から祝日データを作成する。
    /// </summary>
    /// <param name="str">祝日レコード文字列。"yyyy/M/d,名称"</param>
    /// <returns>祝日データ。</returns>
    let parse str =
        match holidayRegex.Match str with
        | m when m.Success -> matchToHoliday m
        | _ -> Error <| InvalidHolidayText str
