using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApplication.Requests;
using Microsoft.Extensions.Configuration;

namespace BankApplication.Extension
{
    public static class ExtensionLogic
    {
        public static string BuildTxnId(List<TransactionDetails> acclist, string? date)
        {

            String prefix = String.Empty;
            acclist=acclist.Where(c=>c.Date== date).ToList();
            int? txnnId = (acclist.ToList())?.Count();
            if (txnnId is null)
                txnnId = 0;
            int? currentTxnId = txnnId + 1;
            prefix = date + "-";
            return prefix + currentTxnId.ToString().PadLeft(3, '0');

        }
    }
   
    public static class Display
    {
        public static void PrintMenu(bool? action = false)
        {
            #region Object Initilization
            string[] options = { Constants.INPUT_TRANSACTION, Constants.DEFINE_INTEREST_RULES, Constants.PRINT_STATEMENT, Constants.QUIT };
            #endregion
            #region Logic to perform
            if (action == false)
            {
                Console.WriteLine(Constants.WELCOME_MSG + "\n" + Constants.ASTERIK + "\n");
                Console.WriteLine("\n" + Constants.HELP_MSG + "\n");
            }
            else
            {
                Console.WriteLine("\n" + Constants.HELP_MSG1 + "\n");
            }
            foreach (String option in options)
            {
                Console.WriteLine(option);
            }
            Console.Write("Please select your option : ");
            #endregion
        }
    }
}
