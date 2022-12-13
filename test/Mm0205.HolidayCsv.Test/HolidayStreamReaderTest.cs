using System.Text;
using FluentAssertions;

namespace Mm0205.HolidayCsv.Test;

[TestClass]
public class HolidayStreamReaderTest
{
    private const string TestCsvText = @"
国民の祝日・休日月日,国民の祝日・休日名称
1955/1/1,元日
1955/1/15,成人の日
1955/3/21,春分の日
";

    [TestMethod]
    public async Task TestLoadFromStreamAsync()
    {
        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(TestCsvText));
        var holidaysResult = await HolidayCsvReader.read(ms, Encoding.UTF8);

        holidaysResult.IsOk.Should().BeTrue();
        holidaysResult.ResultValue.Length.Should().Be(3);
    }

    [TestMethod]
    public async Task TestLoadFromSJisStreamAsync()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using var ms = new MemoryStream(Encoding.GetEncoding("Shift_JIS").GetBytes(TestCsvText));
        var holidaysResult = await HolidayCsvReader.read(ms, null);

        holidaysResult.IsOk.Should().BeTrue();
        holidaysResult.ResultValue.Length.Should().Be(3);
    }
}