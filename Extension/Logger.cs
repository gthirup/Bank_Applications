using BankApplication.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Extension
{
    public class Logger:ILogger
    {
        public void writeLog(string strValue)
        {
            try
            {
               
                var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                
                string? LogFilePath = builder.Build().GetSection("DataSource").GetSection("LogFilePath").Value;
                
                string? startupPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName);
                
                string? fileName = "Log" + "-" + DateTime.UtcNow.Date.ToString("yyyyMMdd");
               
                string? path = startupPath + LogFilePath + fileName;
                
                if (!Directory.Exists(startupPath + LogFilePath))
                {
                    Directory.CreateDirectory(startupPath + LogFilePath);
                }
                StreamWriter sw;
                if (!File.Exists(path))
                { sw = File.CreateText(path); }
                else
                { sw = File.AppendText(path); }
                LogWrite(strValue, sw);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
            }

        }
        private static void LogWrite(string logMessage, StreamWriter w)
        {
            w.WriteLine("{0}", logMessage);

            w.WriteLine("----------------------------------------");
        }
    }
}
