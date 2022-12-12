using System.Text;

namespace Mm0205.HolidayCsv.Test;

[TestClass]
public class Initialization
{

    [AssemblyInitialize]
    public static void MyTestInitialize(TestContext testContext)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}