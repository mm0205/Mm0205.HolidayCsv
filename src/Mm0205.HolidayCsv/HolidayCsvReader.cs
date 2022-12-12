using System.IO.Abstractions;
using System.Text;
using FluentResults;

namespace Mm0205.HolidayCsv;

/// <summary>
/// 祝日CSVを読み込む。<br/>
/// 祝日CSVに関しては以下のページを参照。<br/>
/// <a href="https://www8.cao.go.jp/chosei/shukujitsu/gaiyou.html">国民の祝日について - 内閣府</a><br/>
/// <a href="https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv">CSV本体</a>
/// </summary>
public class HolidayCsvReader
{
    private static readonly Uri CsvUri = new("https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv");

    private static readonly HttpClient _httpClient = new();
    private readonly IFileSystem _fileSystem;

    public HolidayCsvReader(IFileSystem? fileSystem = null)
    {
        _fileSystem = fileSystem ?? new FileSystem();
    }

    public async Task<Result<IEnumerable<Holiday>>> LoadFromWebAsync(
        Uri? csvUri = null,
        Encoding? encoding = null,
        CancellationToken cancellationToken = default
    )
    {
        csvUri ??= CsvUri;
        await using var stream = await _httpClient.GetStreamAsync(csvUri, cancellationToken);
        return await LoadFromStreamAsync(stream, encoding, cancellationToken);
    }

    public async Task<Result<IEnumerable<Holiday>>> LoadFromFileAsync(
        string filePath,
        Encoding? encoding = null,
        CancellationToken cancellationToken = default
    )
    {
        await using var fs = _fileSystem.File.OpenRead(filePath);
        return await LoadFromStreamAsync(fs, encoding, cancellationToken);
    }

    public async Task<Result<IEnumerable<Holiday>>> LoadFromStreamAsync(
        Stream stream,
        Encoding? encoding = null,
        CancellationToken cancellationToken = default
    )
    {
        encoding ??= GetDefaultEncoding();
        using var reader = new HolidayStreamReader(stream, encoding);
        return await reader.ReadAllAsync(cancellationToken);
    }

    private static Encoding GetDefaultEncoding()
    {
        RegisterEncodingProvider();
        // 祝日CSVのデフォルトエンコーディングはShift_JISとする。
        return Encoding.GetEncoding("Shift_JIS");
    }

    private static void RegisterEncodingProvider()
    {
        // 同一のProviderを引数にしてEncoding.RegisterProviderを複数回呼び出した場合、
        // 2回目以降の呼び出しは無視される。
        // このため、特に呼び出し済み等のチェックを行わずに呼び出して問題無いはず。
        // https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding.registerprovider?view=net-7.0
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}