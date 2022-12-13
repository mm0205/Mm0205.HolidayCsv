using FluentAssertions;

namespace Mm0205.HolidayCsv.Test;

[TestClass]
public class HolidayTest
{
    [TestMethod]
    public void TestConstructor()
    {
        var date = new DateOnly(2022, 12, 3);
        const string name = "テスト祝日";

        var sut = Holiday.from(date, name);

        Holiday.date(sut).Should().Be(date);
        Holiday.name(sut).Should().Be(name);

    }

    [TestMethod]
    [DynamicData(nameof(ValidDateTexts))]
    public void TestTryParseShouldSucceed(string text, int y, int m, int d, string expectedName)
    {
        var result = Holiday.parse(text);
        result.IsOk.Should().BeTrue();
        Holiday.date(result.ResultValue).Should().Be(new DateOnly(y, m, d));
        Holiday.name(result.ResultValue).Should().Be(expectedName);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidDateTexts))]
    public void TestTryParseShouldFail(string text)
    {
        var result = Holiday.parse(text);
        result.IsError.Should().BeTrue();
    }

    public static IEnumerable<object[]> ValidDateTexts
    {
        get
        {
            return new[]
            {
                new object[] { "0001/1/1,1年1月1日", 1, 1, 1, "1年1月1日" },
                new object[] { "1955/1/1,元日", 1955, 1, 1, "元日" },
                new object[] { "1955/11/23,勤労感謝の日", 1955, 11, 23, "勤労感謝の日" },
                new object[] { "9999/12/31,テスト", 9999, 12, 31, "テスト" },
            };
        }
    }

    public static IEnumerable<object[]> InvalidDateTexts
    {
        get
        {
            return new[]
            {
                new object[] { "0000/12/31,1年1月1日" },
                new object[] { "10000/1/1,元日" },
                new object[] { "1955/1123,勤労感謝の日" },
                new object[] { "195511/23,勤労感謝の日" },
                new object[] { "1955/11/23" },
                new object[] { "1955123," },
            };
        }
    }
}