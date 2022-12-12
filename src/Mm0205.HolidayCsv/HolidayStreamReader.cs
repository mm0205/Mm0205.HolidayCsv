using System.Text;
using FluentResults;

namespace Mm0205.HolidayCsv;

internal class HolidayStreamReader : IDisposable
{
    private const string HeaderText = "国民の祝日・休日月日,国民の祝日・休日名称";

    private readonly StreamReader _streamReader;

    public HolidayStreamReader(Stream stream, Encoding encoding)
    {
        _streamReader = new StreamReader(stream, encoding: encoding, leaveOpen: true);
    }

    public void Dispose()
    {
        _streamReader.Dispose();
    }

    public async Task<Result<IEnumerable<Holiday>>> ReadAllAsync(CancellationToken cancellationToken)
    {
        var holidays = new List<Holiday>();
        var result = new Result<IEnumerable<Holiday>>();
        result.WithValue(holidays);


        var lineNumber = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_streamReader.EndOfStream)
            {
                return result;
            }

            lineNumber++;

            var line = await _streamReader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (line == HeaderText)
            {
                continue;
            }

            var holidayResult = Holiday.TryParse(line, atLine: lineNumber);
            if (holidayResult.IsFailed)
            {
                result.WithErrors(holidayResult.Errors);
            }
            else
            {
                holidays.Add(holidayResult.Value);
            }
        }

        return result;
    }
}