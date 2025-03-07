namespace MrHihi.HiUtility.Excel;

[AttributeUsage(AttributeTargets.Property)]
public class ExcelColumnAttribute: Attribute
{
    public int Width { get; set; } = 0;
    public bool Ignored { get; set; } = false;
    public bool ZeroToEmpty { get; set; } = false;

    public ExcelColumnAttribute(int width = 0, bool ignored = false, bool zeroToEmpty = false)
    {
        Width = width;
        Ignored = ignored;
        ZeroToEmpty = zeroToEmpty;
    }
}
