using System.Text;

namespace MrHihi.HiUtility.Text;

public static class TextExtensions
{
    static Encoding DEFAULT_ENCODING = System.Text.Encoding.UTF8;
    public static string ToBase64(this string s, Encoding? encoding = null)
    {
        var ec = encoding ?? DEFAULT_ENCODING;
        byte[] inputBytes = ec.GetBytes(s);
        string base64Str = Convert.ToBase64String(inputBytes);
        return base64Str;
    }

    public static Stream Base64ToStream(this string base64)
    {
        var bytes = Convert.FromBase64String(base64);
        return new MemoryStream(bytes);
    }

    public static string GetStrFromBase64(this string s, Encoding? encoding = null)
    {
        var ec = encoding ?? DEFAULT_ENCODING;
        byte[] inputBytes = Convert.FromBase64String(s);
        string deBase64Str = ec.GetString(inputBytes);
        return deBase64Str;
    }

    public static string Repeat(this string value, int count)
    {
        return new StringBuilder().Insert(0, value, count).ToString();
    }

    public static List<(string Source, string CompareTarget, int SourceLine, int CompareTargetLine)> CompareTextLine(this string source, string compareTarget)
    {
        var result = new List<(string Source, string CompareTarget, int SourceLine, int CompareTargetLine)>();

        // 讀取檔案的所有行
        string[] file1Lines = source.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        string[] file2Lines = compareTarget.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        int index1 = 0, index2 = 0;

        // 比對兩個檔案
        while (index1 < file1Lines.Length && index2 < file2Lines.Length)
        {
            string line1 = file1Lines[index1];
            string line2 = file2Lines[index2];

            // 如果行不相同，顯示差異並嘗試對齊後續行
            if (line1 != line2)
            {
                result.Add((line1, line2, index1+1, index2+1));

                // 找出接下來相同的行，嘗試對齊
                bool aligned = false;

                // 嘗試對齊 file1Lines 的後續行
                for (int tempIndex1 = index1 + 1; tempIndex1 < file1Lines.Length && !aligned; tempIndex1++)
                {
                    if (file1Lines[tempIndex1] == line2)
                    {
                        index1 = tempIndex1; // 調整指標，對齊
                        aligned = true;
                    }
                }

                // 嘗試對齊 file2Lines 的後續行
                if (!aligned)
                {
                    for (int tempIndex2 = index2 + 1; tempIndex2 < file2Lines.Length && !aligned; tempIndex2++)
                    {
                        if (file2Lines[tempIndex2] == line1)
                        {
                            index2 = tempIndex2; // 調整指標，對齊
                            aligned = true;
                        }
                    }
                }

                // 如果無法對齊，則同時前進一行繼續比對
                if (!aligned)
                {
                    index1++;
                    index2++;
                }
            }
            else
            {
                // 如果行相同，繼續比較下一行
                index1++;
                index2++;
            }
        }

        // 處理 file1 多出來的行
        while (index1 < file1Lines.Length)
        {
            result.Add((file1Lines[index1], "", index1 + 1, 0));
            index1++;
        }

        // 處理 file2 多出來的行
        while (index2 < file2Lines.Length)
        {
            result.Add(("", file1Lines[index2], 0, index2 + 1));
            index2++;
        }
        return result;
    }
}