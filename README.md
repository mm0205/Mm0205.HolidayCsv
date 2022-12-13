# Mm0205.HolidayCsv

祝日CSVの読み込み用ライブラリ。


## インストール

```shell
dotnet add Mm0205.HolidayCsv
```

## 使い方

```csharp
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
using var ms = new MemoryStream(Encoding.GetEncoding("Shift_JIS").GetBytes(TestCsvText));
var holidaysResult = await HolidayCsvReader.read(ms, null);

holidaysResult.IsOk.Should().BeTrue();
holidaysResult.ResultValue.Length.Should().Be(3);
```

