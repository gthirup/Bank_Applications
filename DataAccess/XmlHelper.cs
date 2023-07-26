using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using BankApplication.Interfaces;
using BankApplication.Requests;
using BankApplication.Responses;
using Grpc.Core;
//using BankApplication.Extensions;
using Microsoft.Extensions.Configuration;
using BankApplication.Extension;
using System.Globalization;

namespace BankApplication.DataAccess
{
 
    public class XmlHelper : IXmlHelper
    {
        #region Initilization       
        private readonly ILogger _logger;
        public XmlHelper(ILogger logger)
        {             
            _logger = logger;
        }
        #endregion
        #region Public Mthods
        public Task<BaseResponses<dynamic>> UpsertAccountDetailsInXML(AccountDetails accounts)
        {
            #region Object Initialization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; dynamic? data = null;
            #endregion

            #region Insert Account Details
            try
            {


                string path = string.Concat(GetFolderPath(true), "AccountDetails.xml");
                if (!File.Exists(path))
                {
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.Indent = true;
                    xmlWriterSettings.NewLineOnAttributes = true;
                    using (XmlWriter xmlWriter = XmlWriter.Create(path, xmlWriterSettings))
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("accountDetails");

                        xmlWriter.WriteStartElement("head");
                        xmlWriter.WriteElementString("id", accounts.AccountId);
                        xmlWriter.WriteElementString("account", accounts.AccountNumber);
                        xmlWriter.WriteElementString("date", accounts.Date);
                        xmlWriter.WriteElementString("open_balance", accounts.OpenBalance);
                        xmlWriter.WriteElementString("current_balance", accounts.CurrentBalance);
                        xmlWriter.WriteElementString("interest", accounts.Interest);
                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndDocument();
                        xmlWriter.Flush();
                        xmlWriter.Close();
                        status = ResponseStatus.Success;
                    }
                }
                else
                {
                    XDocument xDocument = XDocument.Load(path);
                    XElement root = xDocument.Element("accountDetails");
                    IEnumerable<XElement> rows = root.Descendants("head");
                    XElement firstRow = rows.First();
                    firstRow.AddBeforeSelf(
                       new XElement("head",
                       new XElement("id", accounts.AccountId),
                       new XElement("account", accounts.AccountNumber),
                       new XElement("date", accounts.Date),
                       new XElement("open_balance", accounts.OpenBalance),
                       new XElement("current_balance", accounts.CurrentBalance),
                        new XElement("interest", accounts.Interest)));
                    xDocument.Save(path);
                    status = ResponseStatus.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to insert details in account table. Method: {nameof(UpsertAccountDetailsInXML)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }

            return Task.FromResult(new BaseResponses<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            });
            #endregion

        }

        public async Task<BaseResponses<dynamic>> UpsertTransactionDetailsInXML(TransactionDetails transactionDetails)
        {
            #region Object Initialization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; dynamic? data = null;
            #endregion

            #region Insert Transaction Details
            try
            {

                string path = string.Concat(GetFolderPath(true), "TransactionDetails.xml");
                if (!File.Exists(path))
                {
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.Indent = true;
                    xmlWriterSettings.NewLineOnAttributes = true;
                    using (XmlWriter xmlWriter = XmlWriter.Create(path, xmlWriterSettings))
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("Statements");

                        xmlWriter.WriteStartElement("head");
                        xmlWriter.WriteElementString("ac_id", transactionDetails.Id);
                        xmlWriter.WriteElementString("acc_no", transactionDetails.AccNumber);
                        xmlWriter.WriteElementString("date", transactionDetails.Date);
                        xmlWriter.WriteElementString("type", transactionDetails.Type);
                        xmlWriter.WriteElementString("amount", transactionDetails.Amount);
                        xmlWriter.WriteElementString("txn_id", transactionDetails.TxnId);
                        xmlWriter.WriteElementString("balance", transactionDetails.Balance);
                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndDocument();
                        xmlWriter.Flush();
                        xmlWriter.Close();
                        status = ResponseStatus.Success;
                    }
                }
                else
                {
                    XDocument xDocument = XDocument.Load(path);
                    XElement root = xDocument.Element("Statements");
                    IEnumerable<XElement> rows = root.Descendants("head");
                    XElement firstRow = rows.First();
                    firstRow.AddBeforeSelf(
                       new XElement("head",
                       new XElement("ac_id", transactionDetails.Id),
                       new XElement("acc_no", transactionDetails.AccNumber),
                       new XElement("date", transactionDetails.Date),
                       new XElement("type", transactionDetails.Type),
                       new XElement("amount", transactionDetails.Amount),
                        new XElement("txn_id", transactionDetails.TxnId),
                        new XElement("balance", transactionDetails.Balance)));
                    xDocument.Save(path);
                    status = ResponseStatus.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to insert the details in transaction table. Method: {nameof(UpsertTransactionDetailsInXML)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }

            return new BaseResponses<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion
        }

        public async Task<BaseResponses<dynamic>> UpsertRatesInXML(RateRequest rate)
        {
            #region Object Initialization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; dynamic? data = null;
            #endregion
            #region Insert Interest Rates
            try
            {

                string path = string.Concat(GetFolderPath(true), "DefineRateRule.xml");
                if (!File.Exists(path))
                {
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.Indent = true;
                    xmlWriterSettings.NewLineOnAttributes = true;
                    using (XmlWriter xmlWriter = XmlWriter.Create(path, xmlWriterSettings))
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("DefineRateRule");

                        xmlWriter.WriteStartElement("rate");
                        xmlWriter.WriteElementString("date", rate.Date);
                        xmlWriter.WriteElementString("rule_id", rate.RuleId);
                        xmlWriter.WriteElementString("rate_percent", rate.Rate);
                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndDocument();
                        xmlWriter.Flush();
                        xmlWriter.Close();
                        status = ResponseStatus.Success;
                    }
                }
                else
                {
                    XDocument xDocument = XDocument.Load(path);
                    XElement root = xDocument.Element("DefineRateRule");
                    IEnumerable<XElement> rows = root.Descendants("rate");
                    XElement firstRow = rows.First();
                    firstRow.AddBeforeSelf(
                       new XElement("rate",
                       new XElement("date", rate.Date),
                       new XElement("rule_id", rate.RuleId),
                       new XElement("rate_percent", rate.Rate)));
                    xDocument.Save(path);
                    status = ResponseStatus.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to insert the details in interest rate table. Method: {nameof(UpsertRatesInXML)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }

            return new BaseResponses<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion

        }

        public async Task<BaseResponses<dynamic>> UpsertInterestLogInXML(string? date, string isInterestedApplied)
        {
            #region Object Initialization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; dynamic? data = null;
            #endregion

            #region Instert detail in Interest log
            try
            {

                string path = string.Concat(GetFolderPath(true), "IntersetLog.xml");
                if (!File.Exists(path))
                {
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.Indent = true;
                    xmlWriterSettings.NewLineOnAttributes = true;
                    using (XmlWriter xmlWriter = XmlWriter.Create(path, xmlWriterSettings))
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("log");

                        xmlWriter.WriteStartElement("intersetLog");
                        xmlWriter.WriteElementString("date", date);
                        xmlWriter.WriteElementString("is_interest_applied", isInterestedApplied);
                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndDocument();
                        xmlWriter.Flush();
                        xmlWriter.Close();
                        status = ResponseStatus.Success;
                    }
                }
                else
                {

                    XDocument xDocument = XDocument.Load(path);
                    XElement root = xDocument.Element("log");
                    IEnumerable<XElement> rows = root.Descendants("intersetLog");
                    XElement firstRow = rows.First();
                    firstRow.AddBeforeSelf(
                       new XElement("intersetLog",
                       new XElement("date", date),
                       new XElement("is_interest_applied", isInterestedApplied.ToString())));
                    xDocument.Save(path);
                    status = ResponseStatus.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to insert the details in interst log table. Method: {nameof(UpsertInterestLogInXML)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
                status = ResponseStatus.Failure; message = ex.Message;
            }

            return new BaseResponses<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            };

            #endregion
        }

        public async Task<List<RateRequest>> GetAllRateDetails()
        {
            #region Object Initialization
            List<RateRequest> rateDetailsList = new List<RateRequest>(); RateRequest request = new RateRequest();
            #endregion
            #region Get all rate details
            try
            {
                string accXmlpath = Path.Combine(GetFolderPath(true), "DefineRateRule.xml");

                if (!File.Exists(accXmlpath))
                {
                    var date = Convert.ToDateTime(DateTime.Now.Date).ToString("yyyyMMdd");
                    request.Date = date;
                    request.Rate = "1.90";
                    request.RuleId = "RULE01";
                    await UpsertRatesInXML(request);

                }

                using (var xmlReader = new StreamReader(accXmlpath))
                {
                    XDocument doc = XDocument.Load(xmlReader);
                    var rateList = doc.Descendants("DefineRateRule").Elements("rate").
                       Select(x => new RateRequest()
                       {
                           Rate = x?.Element("rate_percent")?.Value,
                           Date = x?.Element("date")?.Value,
                           RuleId = x?.Element("rule_id")?.Value
                       }).ToList();

                    rateDetailsList = rateList;

                }

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to get all details from rate table. Method: {nameof(GetAllRateDetails)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");

            }
            return rateDetailsList;
            #endregion
        }

        public async Task<List<InterestLog>> GetAllInterestLogDetails()
        {
            #region Object Initialization
            List<InterestLog> logDetails = new List<InterestLog>();
            #endregion
            #region Get all interest log details
            try
            {
                string accXmlpath = Path.Combine(GetFolderPath(true), "IntersetLog.xml");
                if (!File.Exists(accXmlpath))
                {
                    await UpsertInterestLogInXML(Convert.ToDateTime(DateTime.Now).AddDays(-1).ToString("dd-MM-yyyy"), "1");
                }
                using (var xmlReader = new StreamReader(accXmlpath))
                {
                    XDocument doc = XDocument.Load(xmlReader);
                    var logs = doc.Descendants("log").Elements("intersetLog").
                       Select(x => new InterestLog()
                       {
                           Date = DateTime.ParseExact(x?.Element("date")?.Value, @"dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture),
                           IsAppliedInterest = x?.Element("is_interest_applied")?.Value.ToString()
                       }).ToList();

                    logDetails = logs;

                }


            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to get all details from interest log table. Method: {nameof(GetAllInterestLogDetails)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
            }
            return logDetails;
            #endregion
        }

        public async Task<List<AccountDetails>> GetAllAccountDetials()
        {
            #region Object Initialization
            List<AccountDetails> accountDetailsList = new List<AccountDetails>();
            #endregion

            #region Get All account details
            try
            {

                string accXmlpath = string.Concat(GetFolderPath(true), "AccountDetails.xml");

                using (var xmlReader = new StreamReader(accXmlpath))
                {
                    XDocument doc = XDocument.Load(xmlReader);
                    if (doc is not null)
                    {
                        var accList = doc.Descendants("accountDetails").Elements("head").
                        Select(x => new AccountDetails()
                        {
                            AccountId = x?.Element("id")?.Value,
                            Date = x?.Element("date")?.Value,
                            AccountNumber = x?.Element("account")?.Value,
                            OpenBalance = x?.Element("open_balance")?.Value,
                            CurrentBalance = x?.Element("current_balance")?.Value
                        }).ToList();

                        accountDetailsList = accList;
                    }


                }


            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to get all details from account table. Method: {nameof(GetAllAccountDetials)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
            }
            return accountDetailsList;
            #endregion
        }

        public async Task<List<TransactionDetails>> GetAllTransactionDetials()
        {
            #region Object Initialization
            List<TransactionDetails> transactionDetails = new List<TransactionDetails>();
            #endregion

            #region Get All transaction details
            try
            {
                string accXmlpath = string.Concat(GetFolderPath(true), "TransactionDetails.xml");

                using (var xmlReader = new StreamReader(accXmlpath))
                {
                    XDocument doc = XDocument.Load(xmlReader);
                    var accList = doc.Descendants("Statements").Elements("head").
                       Select(x => new TransactionDetails()
                       {
                           Id = x?.Element("ac_id")?.Value,
                           AccNumber = x?.Element("acc_no")?.Value,
                           Date = x?.Element("date")?.Value,
                           Type = x?.Element("type")?.Value,
                           TxnId = x?.Element("txn_id")?.Value,
                           Amount = x?.Element("amount")?.Value,
                           Balance = x?.Element("balance")?.Value
                       }).ToList();

                    transactionDetails = accList;

                }

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to get all details from transaction table. Method: {nameof(GetAllTransactionDetials)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
            }
            return transactionDetails;
            #endregion
        }

        public async Task<BaseResponses<dynamic>> UpdateAccounts(string accId, string amount)
        {
            #region Object Initialization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; dynamic? data = null;
            #endregion

            #region update account details
            try
            {

                string path = string.Concat(GetFolderPath(true), "AccountDetails.xml");

                XDocument xmlDoc = XDocument.Load(path);

                //Update Element value

                var accountDetails = from accDtl in xmlDoc.Descendants("accountDetails").Elements("head")
                                     where accDtl.Element("id").Value == accId
                                     select accDtl;

                foreach (XElement itemElement in accountDetails)
                {
                    itemElement.Element("current_balance").Value = amount;
                }
                xmlDoc.Save(path);

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to update account detial. Method: {nameof(UpdateAccounts)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
            }
            return new BaseResponses<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion
        }


        public async Task<BaseResponses<dynamic>> UpdateInterestInAccount(AccountRequest? accountRequest, bool isMonthEnd)
        {
            #region Object Initialization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; dynamic? data = null; TransactionDetails transactionDetails = new TransactionDetails();
            #endregion

            #region UpdateInterest for account

            try
            {

                string path = string.Concat(GetFolderPath(true), "AccountDetails.xml");

                XDocument xmlDoc = XDocument.Load(path);

                //Update Element value
                if (!isMonthEnd)
                {
                    var accountDetails = from accDtl in xmlDoc.Descendants("accountDetails").Elements("head")
                                         where accDtl?.Element("id")?.Value == accountRequest?.AccountId
                                         select accDtl;


                    foreach (XElement itemElement in accountDetails)
                    {
                        itemElement.Element("interest").Value = accountRequest.Interest;

                    }
                }
                else
                {
                    var accountDetails = from accDtl in xmlDoc.Descendants("accountDetails").Elements("head")
                                         select accDtl;
                    if (accountDetails is not null)
                    {

                        foreach (XElement itemElement in accountDetails)
                        {

                            decimal interestrate = Math.Round(Convert.ToDecimal(itemElement?.Element("interest")?.Value) / 365,2);
                        
                            itemElement.Element("current_balance").Value = (Convert.ToDecimal(itemElement?.Element("current_balance")?.Value) + interestrate).ToString();

                            transactionDetails.AccNumber = itemElement?.Element("account")?.Value;

                            transactionDetails.Amount = itemElement?.Element("interest")?.Value;

                            transactionDetails.Date = DateTime.Now.ToString("yyyyMMdd");

                            transactionDetails.Id = itemElement?.Element("id")?.Value;

                            var transactionDtls = await GetAllTransactionDetials();

                            transactionDtls = transactionDtls.Where(x => x?.AccNumber?.ToUpper() == transactionDetails?.AccNumber?.ToUpper()).ToList();

                            transactionDetails.TxnId = ExtensionLogic.BuildTxnId(transactionDtls, transactionDetails.Date);

                            transactionDetails.Type = Constants.INTEREST_TYPE;

                            transactionDetails.Balance = itemElement?.Element("current_balance")?.Value;

                            await UpsertTransactionDetailsInXML(transactionDetails);

                            itemElement.Element("interest").Value = "0.00";
                        }
                    }

                }


                xmlDoc.Save(path);

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to update interest detail. Method: {nameof(UpdateInterestInAccount)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
            }
            return new BaseResponses<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion

        }

        public async Task<BaseResponses<dynamic>> UpdateRules(RateRequest? rate)
        {
            #region Object Initialization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; dynamic? data = null;
            #endregion
            #region update rules
            try
            {

                string path = string.Concat(GetFolderPath(true), "DefineRateRule.xml");

                XDocument xmlDoc = XDocument.Load(path);

                //Update Element value

                var rateDetails = from rateDtl in xmlDoc.Descendants("DefineRateRule").Elements("rate")
                                  where rateDtl?.Element("date")?.Value == rate.Date
                                  select rateDtl;
                if (rateDetails is not null)
                {
                    foreach (XElement itemElement in rateDetails)
                    {
                        itemElement.Element("rule_id").Value = rate.RuleId;
                        itemElement.Element("date").Value = rate.Date.ToString();
                        itemElement.Element("rate_percent").Value = rate.Rate;
                    }
                }

                xmlDoc.Save(path);

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to update rules. Method: {nameof(UpdateRules)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
            }
            return new BaseResponses<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion
        }

        public async Task<BaseResponses<dynamic>> UpdateLogDetails(string? date)
        {
            #region Object Initialization
            ResponseStatus status = ResponseStatus.Success; string? message = string.Empty; dynamic? data = null;
            #endregion
            #region update log details
            try
            {

                string path = string.Concat(GetFolderPath(true), "IntersetLog.xml");

                XDocument xmlDoc = XDocument.Load(path);

                //Update Element value

                var logDetails = from logDtl in xmlDoc.Descendants("log").Elements("intersetLog")
                                 select logDtl;

                foreach (XElement itemElement in logDetails)
                {
                    itemElement.Element("is_interest_applied").Value = "1";
                    itemElement.Element("date").Value = date;

                }
                xmlDoc.Save(path);

            }
            catch (Exception ex)
            {
                _logger.writeLog($"Something went wrong when trying to update the log details. Method: {nameof(UpdateLogDetails)},Error: {ex?.Message}, StackTrace: {ex?.StackTrace}");
            }
            return new BaseResponses<dynamic>()
            {
                Status = status,
                Data = data,
                Message = message
            };
            #endregion
        }

        #endregion

        #region Static Methods
        static string? GetFolderPath(bool isXmlPath)
        {

            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            string SourcePath = isXmlPath is true ? "XmlFilePath" : "LogFilePath";

            string? filePath = builder.Build().GetSection("DataSource").GetSection(SourcePath).Value;

            string? startupPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName);

            string? path = startupPath + filePath;

            return path;
        }

        #endregion
    }
}
