using System.Reflection;
using ClosedXML.Excel;

namespace MrHihi.HiUtility.Excel;

public class StrongTypeWorksheetFiller: WorksheetFiller, IWorksheetFiller
{
    Type _type;
    PropertyInfo[] _properties;
    public StrongTypeWorksheetFiller(IXLWorksheet worksheet, IEnumerable<object> sheetData): base(worksheet, sheetData)
    {
        _type = _sheetData.GetType().GetGenericArguments()[0];
        _properties = _type.GetProperties();
    }

    public List<string> SetWorksheetColumns()
    {
        var result = new List<string>();

        for (int i = 1; i <= _properties.Length; i++)
        {
            var prop = _properties[i - 1];
            var att = prop.GetCustomAttribute<ExcelColumnAttribute>(true);
            if (att != null && (att.Width <= 0 || att.Ignored)) continue;

            _worksheet.Column(i).Width = att?.Width??0;
            _worksheet.Cell(1, i).Value = prop.Name;
            result.Add(prop.Name);
        }
        return result;
    }

    public void SetWorksheetData()
    {
        int row = 2;
        foreach (var item in _sheetData)
        {
            for (int i = 1; i <= _properties.Length; i++)
            {
                var prop = _properties[i - 1];
                try 
                {
                    var att = prop.GetCustomAttribute<ExcelColumnAttribute>(true);
                    if (att != null && (att.Width <= 0 || att.Ignored)) continue;

                    var value = prop.GetValue(item, null);
                    if (att != null && att.ZeroToEmpty && value?.ToString() == "0")
                    {
                        value = "";
                    }
                    _worksheet.Cell(row, i).Value = GetValue(value??"");
                }
                catch (Exception ex)
                {
                    var errMsg = $"row: {row}, prop: {prop.Name}, Error: {ex.Message}";
                    Console.WriteLine(errMsg);
                    throw new Exception(errMsg, ex);
                }
            }
            row++;
        }
    }

}