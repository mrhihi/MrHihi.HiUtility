using System.Diagnostics;
using MrHihi.HiUtility.Text;
using MrHihi.HiUtility.Excel;
using MrHihi.HiUtility.Net;

namespace MrHihi.HiUtility.Test;

public class UnitTest1
{
    public UnitTest1()
    {
        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
    }

    [Fact]
    public void Test_Base64()
    {
        string a = "SuperUser:p@ssw0rd";
        string s = a.ToBase64();
        string t = s.GetStrFromBase64();
        Assert.Equal(a, t);
    }
    class User
    {
        public required string Name { get; set; }
        public required int Age { get; set; }
    }

    [Fact]
    public async void Test_Excel()
    {
        var data = new Dictionary<string, IEnumerable<object>>()
        {
            {
                "Sheet1", new List<User>()
                {
                    new User { Name = "SuperUser", Age = 18 },
                    new User { Name = "Admin", Age = 20 }
                }
            }
        };

        using(var excel = new ExcelTool(data, (wb, ws, props) => {
            ws.RangeUsed().SetAutoFilter();
            
        }))
        {
            Stream stream = await excel.Export();
            Assert.NotNull(stream);
            using (var fileStream = new FileStream("../../../data/output.xlsx", FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        };
    }

    [Fact]
    public void Test_SendMail()
    {
        Assert.Throws<MimeKit.ParseException>(() => {
            var smtpInfo = new MailUtil.SmtpInfo
            {
                SmtpServer = "[your smtp server]",
                SmtpPort = 25
            };

            var m = new MailUtil.MessageInfo()
            {
                Sender = "[your@email]",
                Reciver = new List<string> { "[your@email]" },
                Subject = "Test Mail",
                Content = "Hello World"
            };
            MailUtil.SendMail(smtpInfo, m);
        });
    }

}