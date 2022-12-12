using System.Text.RegularExpressions;
using FluentResults;

namespace Mm0205.HolidayCsv;

/// <summary>
/// 祝日・休日
/// </summary>
public partial class Holiday
{
    private const string RegexDateGroupName = "date";
    private const string RegexNameGroupName = "name";
    private const string DateFormat = "yyyy/M/d";

    [GeneratedRegex("^(?<" + RegexDateGroupName + ">\\d{4}/\\d{1,2}/\\d{1,2}),(?<" + RegexNameGroupName + ">.*)$",
        RegexOptions.Compiled)]
    private static partial Regex HolidayTextRegex();

    /// <summary>
    /// 日付。
    /// </summary>
    public required DateOnly Date { get; init; }

    /// <summary>
    /// 祝日・休日名称。
    /// </summary>
    public required string Name { get; init; }

    public static Result<Holiday> TryParse(string? text, int? atLine = null)
    {
        if (text is null)
        {
            return Result.Fail(
                CreateError(
                    string.Format(Resources.Holiday.Error_Required, nameof(text)),
                    atLine
                )
            );
        }

        var match = HolidayTextRegex().Match(text);
        if (!match.Success)
        {
            return Result.Fail(CreateError(Resources.Holiday.Error_InvalidHolidayText, atLine));
        }

        var dateGroup = match.Groups[RegexDateGroupName];
        var nameGroup = match.Groups[RegexNameGroupName];

        if (!DateOnly.TryParseExact(
                dateGroup.Value.AsSpan(),
                DateFormat.AsSpan(),
                out var date))
        {
            return Result.Fail(CreateError(Resources.Holiday.Error_InvalidDateFormat, atLine));
        }

        return new Holiday
        {
            Date = date,
            Name = nameGroup.Value
        };
    }

    private static Error CreateError(string message, int? atLine = null)
    {
        var error = new Error(message);
        return atLine is not null
            ? error.WithMetadata(nameof(atLine), atLine)
            : error;
    }
}