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


namespace BankApplication
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            #region ObjectInitialization

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, true);

            IConfiguration config = builder.Build();

            IXmlHelper xmlhelper = new XmlHelper();

            IValidateInput validateInput=new ValidateInputs(config,xmlhelper);

            BankOperation bankOperation = new BankOperation(config, xmlhelper,validateInput); bool showMenu = true; bool printOption = true;

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
                Logger.writeLog($"Something went wrong when trying to get all details from rate table. Method: {nameof(Program)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
            }
        }


    }

}
