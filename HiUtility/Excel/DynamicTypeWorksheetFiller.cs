using ClosedXML.Excel;

namespace MrHihi.HiUtility.Excel;

class DynamicTypeWorksheetFiller: WorksheetFiller, IWorksheetFiller
{
    public DynamicTypeWorksheetFiller(IXLWorksheet worksheet, IEnumerable<object> sheetData): base(worksheet, sheetData)
    {
    }

    public List<string> SetWorksheetColumns()
    {
        var result = new List<string>();
        var firstRow = _sheetData.First() as IDictionary<string, object>;
        if (firstRow == null) return result;

        int col = 1;
        foreach (var key in firstRow.Keys)
        {
            _worksheet.Cell(1, col).Value = key;
            col++;
            result.Add(key);
        }
        return result;
    }

    public void SetWorksheetData()
    {
        int row = 2;
        foreach (var item in _sheetData)
        {
            var dict = item as IDictionary<string, object>;
            if (dict == null) continue;

            int col = 1;
            foreach (var key in dict.Keys)
            {
                
                var value = dict[key];
                _worksheet.Cell(row, col).Value = GetValue(value);

                col++;
            }
            row++;
        }
    }
}