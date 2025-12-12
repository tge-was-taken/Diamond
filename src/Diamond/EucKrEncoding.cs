using System.Text;

namespace Diamond;

public static class EucKrEncoding
{
    private static readonly Encoding instance;
    public static Encoding Instance => instance;
    static EucKrEncoding()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        instance = Encoding.GetEncoding("EUC-KR");
    }
}
