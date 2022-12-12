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

        var sut = new Holiday
        {
            Date = date,
            Name = name
        };

        sut.Date.Should().Be(date);
        sut.Name.Should().Be(name);

    }

    [TestMethod]
    [DynamicData(nameof(ValidDateTexts))]
    public void TestTryParseShouldSucceed(string text, int y, int m, int d, string expectedName)
    {
        var result = Holiday.TryParse(text);
        result.IsSuccess.Should().BeTrue();
        result.Value.Date.Should().Be(new DateOnly(y, m, d));
        result.Value.Name.Should().Be(expectedName);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidDateTexts))]
    public void TestTryParseShouldFail(string text)
    {
        var result = Holiday.TryParse(text);
        result.IsFailed.Should().BeTrue();
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