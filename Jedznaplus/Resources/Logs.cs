using System;
using System.IO;
using System.Web;

namespace Jedznaplus.Resources
{
    public static class Logs
    {
        public static void SaveLog(string content)
        {
            string logName = DateTime.Now.ToString("yyyyMMdd") + ".txt";

            using (var fs = new FileStream(Path.Combine(HttpContext.Current.Server.MapPath(ConstantStrings.LogsPath), logName), FileMode.Append, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine(content);
            }
        }
    }
}