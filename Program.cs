using System.Net.NetworkInformation;
using Microsoft.Extensions.Configuration;
using BankApplication.Requests;
using BankApplication.Usecases;
using BankApplication.Interfaces;
using System.Data;
using Microsoft.Extensions.Logging;
using BankApplication.Responses;
using BankApplication.Extension;
using BankApplication.DataAccess;
using BankApplication.UseCases;
using ILogger = BankApplication.Interfaces.ILogger;

namespace BankApplication
{
    public class Program
    {     
        public static async Task Main(string[] args)
        {
            #region ObjectInitialization

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, true);

            ILogger logger = new Logger();

            IConfiguration config = builder.Build();

            IXmlHelper xmlhelper = new XmlHelper(logger);

            IValidateInput validateInput=new ValidateInputs(config,xmlhelper, logger);

            BankOperation bankOperation = new BankOperation(config, xmlhelper,validateInput, logger); bool showMenu = true; bool printOption = true;

            #endregion

            try
            {
                #region Calling Logical Method

                Console.WriteLine(Constants.WELCOME_MSG + "\n" + Constants.ASTERIK + "\n");

                while (showMenu)
                {
                    var response = await bankOperation.InvokeBankingOperations(printOption);
                    Display.PrintMenu(response.Data);
                    printOption = false;

                }

                #endregion


            }
            catch (Exception ex)
            {
                logger.writeLog($"Something went wrong when trying to get all details from rate table. Method: {nameof(Program)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
            }
        }


    }

}
