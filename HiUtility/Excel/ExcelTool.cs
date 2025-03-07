using ClosedXML.Excel;

namespace MrHihi.HiUtility.Excel;

public class ExcelTool: IDisposable
{
    XLWorkbook _workbook;
    IDictionary<string, IEnumerable<object>> _data;
    Action<XLWorkbook?, IXLWorksheet, IList<string>>? _postProcessing;

    public ExcelTool(IDictionary<string, IEnumerable<object>> data, Action<XLWorkbook?, IXLWorksheet, IList<string>>? postProcessing)
    {
        _workbook = new XLWorkbook();
        _data = data;
        _postProcessing = postProcessing;
    }

    private void _export()
    {
        foreach(var key in _data.Keys)
        {
            IWorksheetFiller? filler = null;
            try
            {
                var sheetData = _data[key];
                var worksheet = _workbook?.Worksheets.Add(key);
                if (worksheet == null) continue;
                if (sheetData.First() is IDictionary<string, object>)
                {
                    filler = new DynamicTypeWorksheetFiller(worksheet, sheetData);
                }
                else
                {
                    filler = new StrongTypeWorksheetFiller(worksheet, sheetData);
                }

                // 填充 Excel 表頭
                var colNames = filler.SetWorksheetColumns();

                // 填充 Excel 數據
                filler.SetWorksheetData();

                if (_postProcessing != null)
                {
                    _postProcessing(_workbook, worksheet, colNames);
                }

            }
            catch (Exception ex)
            {
                var errMsg = $"key: {key}, filler: {filler?.GetType().FullName}, ex: {ex.Message}";
                Console.WriteLine(errMsg);
                throw new Exception(errMsg, ex);
            }
        }
    }

    public async Task<Stream> Export()
    {
        MemoryStream stream = new MemoryStream();
        await Task.Run(() => {
            _export();
            _workbook?.SaveAs(stream);
        });
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }


    public async Task Export(string outFilePath)
    {
        await Task.Run(() => {
            _export();
            _workbook?.SaveAs(outFilePath);
        });
    }

    public void Dispose()
    {
        _workbook?.Dispose();
    }
}

