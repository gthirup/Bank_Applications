using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BankApplication.Extension;
using BankApplication.Interfaces;
using BankApplication.Requests;
using BankApplication.Responses;
using Grpc.Core;
using Microsoft.Extensions.Configuration;

namespace BankApplication.UseCases
{
    public class ValidateInputs : IValidateInput
    {
        #region Initilization

        private readonly IConfiguration _configuration;
        private readonly IXmlHelper _xmlHelper;
        private readonly ILogger _logger;
        public ValidateInputs(IConfiguration configuration, IXmlHelper xmlHelper, ILogger logger)
        {
            _configuration = configuration;
            _xmlHelper = xmlHelper;
            _logger = logger;
        }
        #endregion

        #region Public Methods

        public async Task<BaseResponse<string>> ValidateProcess(string input, string option)
        {
            #region Object Initialization

            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; string? data = null; List<string> msg = new List<string>();

            DateTime dt;

            string[]? inputs = new string[] { };

            #endregion

            #region ValidateInputs

            try
            {
                if (option.ToUpper() == Constants.INPUT)
                {
                    inputs = SplitInputs(input.Trim(), '|');

                    if (inputs.Length >= 4)
                    {
                        var isInputValid = CheckInputFormatForInputTransaction(inputs);

                        if ((isInputValid.Status == ResponseStatus.Success && (string.IsNullOrEmpty(isInputValid.Message))))
                        {
                            status = ResponseStatus.Success;
                        }
                        else
                        {
                            status = ResponseStatus.Failure;
                            msg.Add(isInputValid.Message);
                        }
                    }
                    else
                    {
                        status = ResponseStatus.Failure;
                        message = "Input is not valid format.";
                        msg.Add(message);
                    }
                }
                else if (option.ToUpper() == Constants.RULES)
                {
                    inputs = SplitInputs(input, '|');

                    if (input.Length >= 3)
                    {
                        var date = inputs[0]; var ruleId = inputs[1]; var rate = inputs[2];

                        #region Rule Date check

                        var isRuleDateValid = CheckDate(date);

                        if (!isRuleDateValid)
                        {
                            message = "Date is not valid. it should be YYYYMMdd format";
                            msg.Add(message);
                            status = ResponseStatus.Failure;
                        }
                        status = IsdatePastDate(date) == true ? ResponseStatus.Success : ResponseStatus.NotAllowed;
                        #endregion

                        #region Rule id check

                        bool isRuleIdValid = IsAlphaNum(ruleId);

                        if (!isRuleIdValid)
                        {
                            message = "Rule id should be alphanumeric and its startswith Rule";
                            msg.Add(message); status = ResponseStatus.Failure;
                        }
                        #endregion

                        #region rate check                                           

                        var isRateValid = IsDoubleNumber(rate);

                        if (isRateValid is true)
                        {

                            if (!(Convert.ToDecimal(rate) > 0 && Convert.ToDecimal(rate) <= 100))
                            {
                                status = ResponseStatus.Failure;

                                message = "Rate is not valid.Rate should be greater than xero and less than 100";

                                msg.Add(message);
                            }
                        }
                        else
                        {
                            status = ResponseStatus.Failure; message = "Rate is not valid.Rate should be the decimal number"; msg.Add(message);
                        }
                        #endregion
                    }

                    else
                    {
                        status = ResponseStatus.Failure;
                        message = "Input is not valid format.";
                        msg.Add(message);
                    }

                }
                else if (option.ToUpper() == Constants.PRINT)
                {
                    inputs = SplitInputs(input.Trim(), '|');

                    if (inputs.Length >= 2)
                    {
                        var isMonthValid = inputs[1].ToCharArray().All(c => Char.IsNumber(c));
                        if (isMonthValid is true)
                        {
                            status = DateTime.TryParseExact(inputs[1], "MM", CultureInfo.InvariantCulture,
                                    DateTimeStyles.None, out dt) == true ?
                                    ResponseStatus.Success : ResponseStatus.Failure;
                            message = status == ResponseStatus.Failure ? "Month is invalid." : string.Empty;
                            msg.Add(message);

                        }
                        else
                        {
                            status = ResponseStatus.Failure; message = "Month should be in number.";
                            msg.Add(message);
                        }

                    }
                    else
                    {
                        status = ResponseStatus.Failure;
                        message = "Month should be correct format.";
                        msg.Add(message);
                    }
                }

                string[] s = msg.Select(jv => (string)jv).ToArray();

                message = string.Join("\n", s);

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to validate the inputs. Method: {nameof(ValidateProcess)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }
            return new BaseResponse<string>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion
        }
       
        private BaseResponse<dynamic> CheckInputFormatForInputTransaction(string[] inputs)
        {
            #region Object Initialization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; List<string> msg = new List<string>();

            DateTime dt; bool isValidFormat; bool isValid = true; bool isAccValid = false; double amt = 0; bool isAccNumvalid = false;
            #endregion

            #region Validate Input Format
            try
            {
                string dat = inputs[0].ToString(); var account = inputs[1]; var transType = inputs[2]; var amount = inputs[3];

                if (!string.IsNullOrEmpty(dat))
                {
                    isValid = CheckDate(dat);

                    if (!isValid)
                    {
                        status = ResponseStatus.Failure;
                        message = "Invalid date format. Date should be in YYYYMMdd format.";
                        msg.Add(message);

                    }
                    else
                    {

                        status = IsdateFutureDate(dat) == true ? ResponseStatus.Success : ResponseStatus.NotAllowed;
                        status = IsdatePastDate(dat) == true ? ResponseStatus.Success : ResponseStatus.NotAllowed;
                        if (status != ResponseStatus.Success)
                        {
                            message = "You cannot do transaction on this date. Date should not be the future/past date";
                            msg.Add(message);
                        }

                    }
                }
                else
                {
                    status = ResponseStatus.Failure;
                    message = "Date should be empty";
                    msg.Add(message);
                }
                if (!string.IsNullOrEmpty(account))
                {
                    var splitedData = account.Substring(0, 2);

                    if (account.ToUpper().StartsWith("AC"))
                    {
                        var checkAcc = account.Split(splitedData);

                        isAccValid = IsAlphaNum(account);

                        if (isAccValid)
                        {
                            if (checkAcc.Length > 0)
                            {
                                isAccNumvalid = checkAcc[1].ToCharArray().All(c => Char.IsNumber(c));
                            }
                            if (isAccNumvalid)
                            {
                                status = ResponseStatus.Failure;
                            }
                            else
                            {
                                status = ResponseStatus.Failure;
                                message = "Account Number should be alphanumeric and its startswith AC. For example it should be (AC001)";
                                msg.Add(message);
                            }

                        }
                        else
                        {
                            status = ResponseStatus.Failure;
                            message = "Account Number should be alphanumeric";
                            msg.Add(message);
                        }


                    }
                    else
                    {
                        status = ResponseStatus.Failure;
                        message = "Account Number should with startswith AC and followed by number";
                        msg.Add(message);
                    }
                }
                else
                {
                    status = ResponseStatus.Failure;
                    message = "Account number must not be empty";
                    msg.Add(message);
                }
                if (!string.IsNullOrEmpty(amount))
                {
                    var isAmountValid = IsDoubleNumber(amount);

                    if (isAmountValid)
                    {
                        string[] res = amount.Split('.');

                        if (res.Length > 1)
                        {
                            bool isPrecisionValid = res[1].Length == 2 ? true : false;

                            if (isPrecisionValid)
                            {
                                amt = Convert.ToDouble(amount);
                                status = amt > 0 ? ResponseStatus.Success : ResponseStatus.Failure;

                            }
                            else
                            {
                                status = ResponseStatus.Failure;
                                message = "Decimals are allowed up to 2 decimal places";
                            }
                        }
                        else
                        {
                            status = ResponseStatus.Failure;
                            message = "Amount must have precisions";
                            msg.Add(message);
                        }
                    }
                    else
                    {
                        status = ResponseStatus.Failure;
                        message = "Amount must be decimal and it should be greater than zero";
                        msg.Add(message);
                    }


                }
                else
                {
                    status = ResponseStatus.Failure;
                    message = "Amount should not be empty";
                    msg.Add(message);
                }
                if (!string.IsNullOrEmpty(transType))
                {
                    if (transType.ToUpper() == Constants.ACCOUNT_TYPE_W || transType.ToUpper() == Constants.ACCOUNT_TYPE_D)
                    {
                        status = ResponseStatus.Success;

                    }
                    else
                    {
                        status = ResponseStatus.Failure;
                        message = "Transaction type shoudd be either D (Deposit) or W (Withdrawl)";
                        msg.Add(message);
                    }
                }
                else
                {
                    status = ResponseStatus.Failure;
                    message = "Transaction type must not be empty";
                    msg.Add(message);
                }

                string[] s = msg.Select(jv => (string)jv).ToArray();

                message = string.Join("\n", s);

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to validate input format. Method: {nameof(CheckInputFormatForInputTransaction)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }
            return new BaseResponse<dynamic>()
            {
                Status = status,
                Data = null,
                Message = message
            };
            #endregion
        }
        #endregion

        #region Static Methods
        static bool IsAlphaNum(string inputToBeChecked)
        {
            if (string.IsNullOrEmpty(inputToBeChecked))
                return false;

            return (inputToBeChecked.ToCharArray().All(c => Char.IsLetter(c) || Char.IsNumber(c)));
        }

        static string[] SplitInputs(string Inputs, char splitBy)
        {
            string[] result = Inputs.Trim().Split(splitBy);

            return result;
        }

        static bool IsDoubleNumber(string valueToTest)
        {
            if (double.TryParse(valueToTest, out double d) && !Double.IsNaN(d) && !Double.IsInfinity(d) && Convert.ToDouble(valueToTest) > 0)
            {
                return true;
            }

            return false;
        }

        static bool CheckDate(string date)
        {
            DateTime dt; bool isValidFormat; bool isValid = true; ResponseStatus status = ResponseStatus.Success; string message = string.Empty;

            Regex regex = new Regex(@"([12]\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01]))$");

            isValidFormat = regex.IsMatch(date.Trim());

            isValid = isValidFormat == true ? true : false;

            return isValid;
        }
        static bool IsdateFutureDate(string date)
        {
            DateTime? dt = DateTime.ParseExact(date, @"yyyyMMdd", CultureInfo.InvariantCulture);

            bool isValid = dt != null && dt > DateTime.ParseExact(DateTime.Now.Date.ToString("yyyyMMdd"), @"yyyyMMdd", CultureInfo.InvariantCulture) ? false : true;

            return isValid;
        }
        static bool IsdatePastDate(string date)
        {
            DateTime? dt = DateTime.ParseExact(date, @"yyyyMMdd", CultureInfo.InvariantCulture);

            bool isValid = dt != null && dt < DateTime.ParseExact(DateTime.Now.Date.ToString("yyyyMMdd"), @"yyyyMMdd", CultureInfo.InvariantCulture) ? false : true;

            return isValid;
        }

        #endregion
    }
}
