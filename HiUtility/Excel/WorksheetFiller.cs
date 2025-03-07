using ClosedXML.Excel;

namespace MrHihi.HiUtility.Excel;

public class WorksheetFiller
{
    protected IXLWorksheet _worksheet { get; private set; }
    protected IEnumerable<object> _sheetData { get; private set; }
    public WorksheetFiller(IXLWorksheet worksheet, IEnumerable<object> sheetData)
    {
        _worksheet = worksheet;
        _sheetData = sheetData;
    }

    protected dynamic? GetValue(object value)
    {
        if (value is int) return (int)value;
        if (value is double) return (double)value;
        if (value is decimal) return (decimal)value;
        if (value is float) return (float)value;
        return value?.ToString()??"";
    }
}