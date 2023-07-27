using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using BankApplication.Extension;
using BankApplication.Interfaces;
using BankApplication.Requests;
using BankApplication.Responses;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


namespace BankApplication.Usecases
{
    public class BankOperation : IBankOperation
    {
        #region Initilization
        private readonly IConfiguration _configuration;
        private readonly IXmlHelper _xmlHelper;
        private readonly IValidateInput _validateInput;
        private readonly ILogger _logger;
        public BankOperation(IConfiguration configuration, IXmlHelper xmlHelper, IValidateInput validateInput, ILogger logger)
        {
            _configuration = configuration;
            _xmlHelper = xmlHelper;
            _validateInput = validateInput;
            _logger = logger;
        }
        #endregion

        #region Public Methods
        public async Task<BaseResponse<dynamic>> InvokeBankingOperations(bool printOption)
        {
            #region Object Initilization

            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; dynamic? data = null;

            #endregion

            try
            {

                #region Invoke Banking Operation   

                #region Invoke Interest Process

                var logs = await _xmlHelper.GetAllInterestLogDetails();

                bool isInterestApplied = logs.Where(x => x.Date == Convert.ToDateTime(DateTime.Now.Date).AddDays(-1) && x.IsAppliedInterest == "0").Any();

                if (!isInterestApplied)
                {
                    _ = Task.Run(async () => await InterestCalculation());
                }

                #endregion

                #region Menu display
                if (printOption is true)
                {
                    PrintMenu(false);
                }
                #endregion

                #region Input Process

                var response = await ProcessUserInputs();

                status = ResponseStatus.Success;
                data = response.Data;
                message = response.Message;
                if (!string.IsNullOrEmpty(message))
                {
                        Console.WriteLine(response.Message);                  
                                   }
                #endregion

                #endregion

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to invoke banking operation. Method: {nameof(InvokeBankingOperations)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
            }
            return new BaseResponse<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            };
        }

        #endregion

        #region Private Methods

        private static void PrintMenu(bool? action = false)
        {

            #region Object Initilization

            string[] options = { Constants.INPUT_TRANSACTION, Constants.DEFINE_INTEREST_RULES, Constants.PRINT_STATEMENT, Constants.QUIT };

            #endregion

            #region Logic to perform

            if (action == false)
            {

                Console.WriteLine(Constants.HELP_MSG + "\n");
            }
            else
            {
                Console.WriteLine(Constants.HELP_MSG1 + "\n");
            }

            foreach (String option in options)
            {
                Console.WriteLine(option);
            }
            Console.Write("Please select your option : ");

            #endregion

        }

        private async Task<BaseResponse<dynamic>> ProcessUserInputs()
        {
            #region Object Initilization

            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; string? option = null; string? input = string.Empty;
            dynamic? data = null;
            #endregion

            #region Process user Input
            try
            {

                var userInputs = GetUserInput();

                option = userInputs.Status == ResponseStatus.Success ? userInputs.Data : string.Empty;

                if (!string.IsNullOrEmpty(option))
                {
                    var userInput = option.ToUpper();

                    switch (userInput)
                    {
                        case MenuOptions.IT:

                            var inputTransresponse = await InvokeInputTransaction(userInput);
                            status = inputTransresponse.Status;
                            message = inputTransresponse.Message;
                            data = inputTransresponse.Data;
                            break;

                        case MenuOptions.DR:

                            var RulesResponse = await InvokeDefineRuleProcess(userInput);
                            status = RulesResponse.Status;
                            message = RulesResponse.Message;
                            data = RulesResponse.Data;
                            break;

                        case MenuOptions.PS:

                            var printResponse = await InvokePrintStatementProcess(userInput);
                            status = printResponse.Status;
                            message = printResponse.Message;
                            data = printResponse.Data;
                            break;

                        case MenuOptions.QT:
                            Environment.Exit(1);
                            message = "";
                            status = ResponseStatus.Success;

                            break;

                        default:
                            message = "Please enter an initial character value for the otipns";
                            status = ResponseStatus.NotAllowed;
                            break;
                    }

                }
                else
                {
                    Console.Clear();
                    data = false;
                    status = userInputs.Status; message = userInputs?.Message;
                }

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to get statement. Method: {nameof(ProcessUserInputs)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }

            return new BaseResponse<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion
        }

        private async Task<BaseResponse<dynamic>> InvokeInputTransaction(string userInput)
        {
            #region Object Initialization

            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; dynamic data = false;

            #endregion

            #region Invoke Input Transaction process
            try
            {
                Console.Clear();

                Console.WriteLine(Constants.TRANSACTION_FORMAT);

                string? input = Console.ReadLine();

                if (!(string.IsNullOrEmpty(input)))
                {
                    var isValidated = await _validateInput.ValidateProcess(input, userInput);

                    if (isValidated.Status == ResponseStatus.Success)
                    {
                        Console.WriteLine("");
                        var inputResponse = await InputTransactionProcess(input);

                        if (inputResponse.Status == ResponseStatus.Success)
                        {
                            Console.WriteLine("Account: " + inputResponse.Data?.FirstOrDefault()?.AccNumber);

                            Console.WriteLine("Date" + "\t\t\t\t|" + "Txn Id" + "\t\t\t\t|" + "Type" + "\t\t\t|" + "Account" + "\t\t|" + "Amount");

                            foreach (var list in inputResponse.Data)
                            {
                                Console.WriteLine(list.Date + "\t\t\t" + list.TxnId + "\t\t\t" + list.Type + "\t\t\t" + list.AccNumber + "\t\t\t" + list.Amount);
                            }

                            data = true; //Setting up menu option

                            status = ResponseStatus.Success;
                        }
                        else
                        {
                            message = inputResponse.Message;

                            status = inputResponse.Status;

                            data = false; //Setting up menu option
                        }
                    }
                    else
                    {
                        message = isValidated.Message;
                        status = ResponseStatus.Failure;
                        data = false; //Setting up menu option
                    }
                }
                else
                {
                    Console.Clear();
                    data = false; //Setting up menu option
                }
            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to inovke input transaction. Method: {nameof(InvokeInputTransaction)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }

            return new BaseResponse<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion
        }

        private async Task<BaseResponse<dynamic>> InvokeDefineRuleProcess(string userInput)
        {
            #region Object Initilization

            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; dynamic data = false;

            #endregion

            #region Invoke Define Rule Process
            try
            {

                Console.Clear();

                Console.WriteLine(Constants.DEFINE_RULE_FORMAT);

                string? input = Console.ReadLine();

                if (!(string.IsNullOrEmpty(input)))
                {
                    var isValidated = await _validateInput.ValidateProcess(input, userInput);

                    if (isValidated.Status == ResponseStatus.Success)
                    {
                        Console.WriteLine("");

                        var inputResponse = await DefineRuleProcess(input);

                        if (inputResponse.Status == ResponseStatus.Success)
                        {
                            Console.WriteLine("Date" + "\t\t\t|" + "Rule Id" + "\t\t|" + "Rate %");

                            foreach (var list in inputResponse.Data)
                            {
                                Console.WriteLine(list.Date + "\t\t" + list.RuleId + "\t\t\t" + list.Rate);
                            }

                            data = true; //Setting up menu option

                            status = ResponseStatus.Success;
                        }

                        else
                        {
                            message = inputResponse.Message;
                            status = inputResponse.Status;
                            data = false; //Setting up menu option
                        }
                    }
                    else
                    {
                        message = isValidated.Message;
                        status = ResponseStatus.Failure;
                        data = false; //Setting up menu option
                    }
                }
                else
                {
                    data = false;
                }
            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to invoke define rule process. Method: {nameof(InvokeDefineRuleProcess)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }
            return new BaseResponse<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion
        }

        private async Task<BaseResponse<dynamic>> InvokePrintStatementProcess(string userInput)
        {
            #region Object Initialization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; dynamic data = false;
            #endregion

            #region Invoke PrintStmt Process
            try
            {

                Console.Clear();

                Console.WriteLine(Constants.PRINT_FORMAT);

                string input = Console.ReadLine();

                if (!(string.IsNullOrEmpty(input)))
                {
                    var isValidated = await _validateInput.ValidateProcess(input, userInput);

                    if (isValidated.Status == ResponseStatus.Success)
                    {
                        Console.WriteLine("");

                        var printResponse = await PrintProcess(input);

                        if (printResponse.Status == ResponseStatus.Success)
                        {

                            string[] inputs = SplitInputs(input.Trim(), '|');

                            if (printResponse.Data?.Count() > 0)
                            {
                                Console.WriteLine("Account: " + inputs[0]);

                                Console.WriteLine("Date" + "\t\t|" + "Txn Id" + "\t\t\t|" + "Type" + "\t\t\t|" + "Amount" + "\t\t\t\t\t|" + "Balance" + "\n");

                                foreach (var list in printResponse.Data)
                                {
                                    Console.WriteLine("\n" + list.TransactionDate + "\t" + list.TxnId + "\t\t" + list.Type + "\t\t\t" + list.TransactionAmount + "\t\t\t\t\t" + list.CurrentBalance);
                                }
                            }
                            else
                            {
                                message = "No data found";
                            }

                            data = true; //Setting up menu option
                            status = ResponseStatus.Success;
                        }
                        else
                        {
                            message = printResponse.Message;
                            status = printResponse.Status;
                            data = false; //Setting up menu option

                        }
                    }
                    else
                    {
                        message = isValidated.Message;
                        status = ResponseStatus.Failure;
                        data = false; //Setting up menu option
                    }
                }
                else
                {
                    data = false; //Setting up menu option
                }
            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to inovke print statement. Method: {nameof(InvokePrintStatementProcess)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }
            return new BaseResponse<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion
        }

        private async Task<BaseResponse<List<TransactionDetails>>> InputTransactionProcess(string input)
        {
            #region Object Initilization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; List<TransactionDetails>? data = null; decimal amt = 0;
            TransactionDetails txnDtl = new TransactionDetails(); string? currentBal = string.Empty; int accountIdentity = 0;
            AccountDetails accountDtl = new AccountDetails();
            List<TransactionDetails> accountDetails = new List<TransactionDetails>();
            #endregion

            #region Transaction Process
            try
            {
                var details = SplitInputs(input, '|');

                string dat = details[0].ToString(); var account = details[1]; var transType = details[2]; var amount = details[3];

                var accList = await _xmlHelper.GetAllAccountDetials();

                var isExists = accList is not null ? accList?.Where(x => x.AccountNumber.ToUpper() == account.ToUpper()).ToList() : null;

                var transactions = await _xmlHelper.GetAllTransactionDetials();

                if (isExists?.Count() > 0)
                {
                    currentBal = isExists?.FirstOrDefault()?.CurrentBalance;

                    accountIdentity = Convert.ToInt32(isExists?.FirstOrDefault()?.AccountId);

                    if (!string.IsNullOrEmpty(currentBal) && (transType.ToUpper() == Constants.ACCOUNT_TYPE_W && (Convert.ToDecimal(currentBal) > Convert.ToDecimal(amount))))
                    {
                        amt = Convert.ToDecimal(currentBal) - Convert.ToDecimal(amount);

                        var updateAccount = await _xmlHelper.UpdateAccounts(accountIdentity.ToString(), amt.ToString());

                        if (updateAccount.Status == ResponseStatus.Success)
                        {
                            transactions = transactions.Where(x => x.AccNumber.ToUpper() == account.ToUpper()).ToList();
                            var txn_Id = ExtensionLogic.BuildTxnId(transactions, dat);
                            txnDtl.Date = dat;
                            txnDtl.TxnId = txn_Id;
                            txnDtl.Id = accountIdentity.ToString();
                            txnDtl.Amount = amount;
                            txnDtl.Type = transType.ToUpper();
                            txnDtl.AccNumber = account.ToUpper();
                            txnDtl.Balance = amt.ToString();

                            var statementResponse = await _xmlHelper.UpsertTransactionDetailsInXML(txnDtl);

                            if (statementResponse.Status == ResponseStatus.Success)
                            {
                                status = ResponseStatus.Success;
                            }
                        }


                    }
                    else if (transType.ToUpper() == Constants.ACCOUNT_TYPE_D)
                    {
                        amt = Convert.ToDecimal(amount) + Convert.ToDecimal(currentBal);

                        var updateAccount = await _xmlHelper.UpdateAccounts(accountIdentity.ToString(), amt.ToString());

                        if (updateAccount.Status == ResponseStatus.Success)
                        {
                            var accId = GetLastIdentityNumber(accList);
                            transactions = transactions.Where(x => x.AccNumber.ToUpper() == account.ToUpper()).ToList();
                            var txn_Id = ExtensionLogic.BuildTxnId(transactions, dat);
                            txnDtl.Date = dat;
                            txnDtl.TxnId = txn_Id;
                            txnDtl.Id = accountIdentity.ToString();
                            txnDtl.Amount = amount;
                            txnDtl.Type = transType.ToUpper();
                            txnDtl.AccNumber = account.ToUpper();
                            txnDtl.Balance = amt.ToString();
                            var statementResponse = await _xmlHelper.UpsertTransactionDetailsInXML(txnDtl);

                            if (statementResponse.Status == ResponseStatus.Success)
                            {
                                status = ResponseStatus.Success;
                            }

                        }

                    }
                    else
                    {
                        status = ResponseStatus.Failure;
                        message = "Your account balance is low. you cannot debit";
                    }

                }
                else
                {
                    if (transType.ToUpper() == Constants.ACCOUNT_TYPE_D)
                    {
                        transactions = transactions.Where(x => x.AccNumber.ToUpper() == account.ToUpper()).ToList();
                        var txn_Id = ExtensionLogic.BuildTxnId(transactions, dat);

                        #region Add data into account detail table

                        var accId = GetLastIdentityNumber(accList);
                        accountDtl.Date = dat;
                        accountDtl.AccountId = accId.ToString();
                        accountDtl.AccountNumber = account.ToUpper();
                        accountDtl.CurrentBalance = amount;
                        accountDtl.OpenBalance = amount;

                        var response = await _xmlHelper.UpsertAccountDetailsInXML(accountDtl);

                        if (response.Status == ResponseStatus.Success)
                        {
                            txnDtl.Date = dat;
                            txnDtl.TxnId = txn_Id;
                            txnDtl.Id = accId.ToString();
                            txnDtl.Amount = amount;
                            txnDtl.Type = transType.ToUpper();
                            txnDtl.AccNumber = account.ToUpper();
                            txnDtl.Balance = amount.ToString();

                            var statementResponse = await _xmlHelper.UpsertTransactionDetailsInXML(txnDtl);

                            if (statementResponse.Status == ResponseStatus.Success)
                            {
                                status = ResponseStatus.Success;
                            }
                        }
                        else
                        {
                            status = ResponseStatus.Failure;
                            message = "Cannot create account. Please try again later";
                        }
                    }
                    else
                    {
                        status = ResponseStatus.NotAllowed;
                        message = "There is no account for this account number, you can not withdraw. Please deposit the amount,account will be created.";
                    }

                    #endregion


                }

                if (status == ResponseStatus.Success)
                {
                    var transactionDetails = await _xmlHelper.GetAllTransactionDetials();
                    var transForInput = transactionDetails.Where(c => c.AccNumber.ToUpper() == account.ToUpper()).ToList();
                    data = transForInput;
                }


            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to do input transaction. Method: {nameof(InputTransactionProcess)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }
            return new BaseResponse<List<TransactionDetails>>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion
        }

        private async Task<BaseResponse<List<RateRequest>>> DefineRuleProcess(string input)
        {
            #region Object Initialization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; List<RateRequest> data = null;
            RateRequest rateDtl = new RateRequest(); string currentBal = string.Empty;
            #endregion

            #region Define Rule Process
            try
            {
                var details = SplitInputs(input.Trim(), '|');

                string dat = details[0].ToString(); var ruleId = details[1]; var rate = details[2];

                var rateList = await _xmlHelper.GetAllRateDetails();

                var isExists = rateList.Where(x => x.Date == dat).ToList();

                rateDtl.Date = dat;
                rateDtl.RuleId = ruleId;
                rateDtl.Rate = rate;

                var updateRateDetails = isExists?.Count() > 0 ? await _xmlHelper.UpdateRules(rateDtl) : await _xmlHelper.UpsertRatesInXML(rateDtl);

                status = updateRateDetails.Status;

                if (status == ResponseStatus.Success)
                {
                    var rateDetails = await _xmlHelper.GetAllRateDetails();

                    data = rateDetails;
                }

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to invoke define rule process. Method: {nameof(InvokeDefineRuleProcess)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }
            return new BaseResponse<List<RateRequest>>()
            {
                Status = status,
                Data = data,
                Message = message
            };

            #endregion
        }


        private async Task<BaseResponse<List<PrintStatementResponse>>> PrintProcess(string input)
        {
            #region Object Initialization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; List<PrintStatementResponse> data = null;
            RateRequest rateDtl = new RateRequest(); string currentBal = string.Empty; DateTime dt = new DateTime();
            #endregion
            #region Print Process
            try
            {
                var details = SplitInputs(input.Trim(), '|');
                string account = details[0].ToString();
                string month = details[1];
                var accountDetails = await _xmlHelper.GetAllAccountDetials();
                var transactionDetails = await _xmlHelper.GetAllTransactionDetials();
                var responseData = (from acc in accountDetails.ToList()
                                    join tran in transactionDetails.ToList()
                                    on acc.AccountId equals tran.Id into statement
                                    from dtls in statement.DefaultIfEmpty()
                                    select new PrintStatementResponse()
                                    {
                                        AccountId = acc.AccountId,
                                        AccountNumber = acc.AccountNumber,
                                        OpenBalance = acc.OpenBalance,
                                        CurrentBalance = dtls == null ? null : dtls.Balance,
                                        TransactionDate = dtls == null ? null : dtls.Date,
                                        TxnId = dtls == null ? null : dtls.TxnId,
                                        Type = dtls == null ? null : dtls.Type,
                                        TransactionAmount = dtls == null ? null : dtls.Amount
                                    }).ToList();

                responseData = responseData.Where(x => DateTime.ParseExact(x.TransactionDate, "yyyyMMdd", null).ToString("MM") == DateTime.ParseExact(month, "MM", null).ToString("MM") && x.AccountNumber.ToUpper() == account.ToUpper()).ToList();

                data = responseData;

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to get statement. Method: {nameof(PrintProcess)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }
            return new BaseResponse<List<PrintStatementResponse>>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion
        }

        private async Task<BaseResponse<dynamic>> InterestCalculation()
        {
            #region Object Initiliazation
            List<AccountDetails> accDtl = new List<AccountDetails>(); List<RateRequest>? ratePercentForday = new List<RateRequest>();

            AccountRequest accountDtl = new AccountRequest(); string? rateOfInterest = string.Empty; DateTime now = DateTime.Now;

            List<DateTime?>? dates = new List<DateTime?>();

            #endregion

            #region Interest Calculation

            try
            {
                #region Finding rate rule for the day

                var logDtls = await _xmlHelper.GetAllInterestLogDetails();

               
                if (logDtls != null && logDtls?.FirstOrDefault()?.Date != DateTime.Now.Date)
                {
                    if (logDtls?.Count() > 0)
                    {
                        dates = Enumerable.Range(1, int.MaxValue)
                                                        .Select(index => new DateTime?(Convert.ToDateTime(logDtls?.FirstOrDefault()?.Date).Date.AddDays(index)))
                                                        .TakeWhile(date => date < DateTime.Now.Date)
                                                        .ToList();
                    }

                    if (dates?.Count() > 0)
                    {
                        var rateDetails = await _xmlHelper.GetAllRateDetails();

                        var accountDetails = await _xmlHelper.GetAllAccountDetials();


                        if (rateDetails?.Count() > 0)
                        {
                            if (accountDetails?.Count() > 0)
                            {
                                foreach (var i in dates)
                                {

                                    ratePercentForday = rateDetails?.Where(r => DateTime.ParseExact(r.Date.ToString(), @"yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).Date <= i.Value.Date).ToList();

                                    if (ratePercentForday?.Count() != 0)
                                    {
                                        var maxDate = ratePercentForday?.Select(x => DateTime.ParseExact(x.Date.ToString(), @"yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)).Max();

                                        ratePercentForday = ratePercentForday?.Where(r => DateTime.ParseExact(r.Date.ToString(), @"yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).Date == maxDate).ToList();

                                        rateOfInterest = ratePercentForday?.FirstOrDefault()?.Rate;

                                        foreach (var acc in accountDetails)
                                        {
                                            accountDtl = new AccountRequest();

                                            accountDtl.Interest = Math.Round((Convert.ToDecimal(acc.CurrentBalance) * (Convert.ToDecimal(rateOfInterest) / 100) * 1), 2).ToString();

                                            accountDtl.AccountId = acc.AccountId;

                                            await _xmlHelper.UpdateInterestInAccount(accountDtl, false);
                                        }
                                        await _xmlHelper.UpdateLogDetails(Convert.ToDateTime(i).ToString("dd-MM-yyyy"));

                                    }


                                }
                                _logger.writeLog($"Info:Interest calculated for the account on the end of the balance.");
                            }
                            else
                            {
                                _logger.writeLog($"Info: There is no accounts in table. Please create account.");
                            }
                        }
                        else
                        {
                            _logger.writeLog($"Info: There is no rate interest rule in table. Please define the rules.");
                        }
                    }
                    #endregion

                    #region End of the month interest credited
                    var date = (DateTime.Now.Date).AddDays(-1);
                    var startDate = new DateTime(date.Year, date.Month, 1);

                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    if (date == endDate)
                    {
                        Logger.writeLog($"Interest crediting process started at end of month.");
                        await _xmlHelper.UpdateInterestInAccount(null, true);
                    }
                   #endregion

                }

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to apply the interest calculation. Method: {nameof(InterestCalculation)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
            }

            return null;

            #endregion
        }

        private BaseResponse<string> GetUserInput()
        {
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; string? option = null;

            try
            {
                option = Console.ReadLine();
            }
            catch (System.FormatException)
            {
                message = "Please enter an initial character value for the otipns";
                _logger.writeLog($"Something went wrong when trying to get user input. Method: {nameof(GetUserInput)},Error: {message}, StackTrace: {message}");
                status = ResponseStatus.NotAllowed;
            }
            catch (Exception)
            {
                message = "An unexpected error happened. Please try again";
                _logger.writeLog($"Something went wrong when trying to get user input. Method: {nameof(GetUserInput)},Error: {message}, StackTrace: {message}");
                status = ResponseStatus.Failure;
            }
            return new BaseResponse<string>()
            {
                Status = status,
                Data = option,
                Message = message
            };

        }

        #endregion

        #region Static Methods

        static int? GetLastIdentityNumber(List<AccountDetails> acclist)
        {

            int? txnnId = (acclist.ToList())?.Max(v => (int?)Convert.ToInt32(v.AccountId));

            if (txnnId is null)
                txnnId = 0;

            int? currentTxnId = txnnId + 1;

            return currentTxnId;
        }
       

        static string[] SplitInputs(string Inputs, char splitBy)
        {
            string[] result = Inputs.Trim().Split(splitBy);

            return result;
        }

        #endregion

    }
}
