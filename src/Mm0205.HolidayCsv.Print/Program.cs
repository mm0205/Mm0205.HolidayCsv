
using Mm0205.HolidayCsv.Old;

var loader = new HolidayCsvLoader();
var result = await loader.LoadFromWebAsync();
if (result.IsFailed)
{
    await Console.Error.WriteLineAsync("祝日CSVの読み込みに失敗しました");
    await Console.Error.WriteLineAsync(result.ToString());
    return;
}

foreach (var holiday in result.Value)
{
    Console.WriteLine($"{holiday.Date.ToString("yyyy-MM-dd")}, {holiday.Name}");
}


