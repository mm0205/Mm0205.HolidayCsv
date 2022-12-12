using System.IO.Abstractions.TestingHelpers;
using System.Text;
using FluentAssertions;

namespace Mm0205.HolidayCsv.Test;

[TestClass]
public class HolidayCsvLoaderTest
{
    private const string TestCsvText = @"
国民の祝日・休日月日,国民の祝日・休日名称
1955/1/1,元日
1955/1/15,成人の日
1955/3/21,春分の日
";

    [TestMethod]
    public async Task TestLoadFromFileAsync()
    {

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { "holidays.csv", new MockFileData(TestCsvText, Encoding.GetEncoding("Shift_JIS")) }
        });

        var sut = new HolidayCsvReader(fileSystem);
        var holidaysResult = await sut.LoadFromFileAsync("holidays.csv");

        holidaysResult.IsSuccess.Should().BeTrue();

    }

    [TestMethod]
    public async Task TestLoadFromStreamAsync()
    {

        var sut = new HolidayCsvReader();
        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(TestCsvText));
        var holidaysResult = await sut.LoadFromStreamAsync(ms, Encoding.UTF8);

        holidaysResult.IsSuccess.Should().BeTrue();

    }

    [TestMethod]
    [Ignore("Webアクセスが必要なためSkipする")]
    public async Task LoadFromWebAsyncTest()
    {
         var sut = new HolidayCsvReader();
         var holidaysResult = await sut.LoadFromWebAsync();
 
         holidaysResult.IsSuccess.Should().BeTrue();       
    }
}