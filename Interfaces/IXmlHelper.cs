using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankApplication.Requests;
using BankApplication.Responses;

namespace BankApplication.Interfaces
{
    public interface IXmlHelper
    {
        Task<BaseResponses<dynamic>> UpsertAccountDetailsInXML(AccountDetails accounts);

        Task<BaseResponses<dynamic>> UpsertTransactionDetailsInXML(TransactionDetails transactionDetails);

        Task<BaseResponses<dynamic>> UpsertRatesInXML(RateRequest rate);

        Task<List<RateRequest>> GetAllRateDetails();

        Task<List<AccountDetails>> GetAllAccountDetials();

        Task<BaseResponses<dynamic>> UpdateAccounts(string accId, string amount);

        Task<List<TransactionDetails>> GetAllTransactionDetials();

        Task<BaseResponses<dynamic>> UpdateRules(RateRequest rate);

        Task<BaseResponses<dynamic>> UpsertInterestLogInXML(string? date, string isInterestedApplied);

        Task<List<InterestLog>> GetAllInterestLogDetails();

        Task<BaseResponses<dynamic>> UpdateInterestInAccount(AccountRequest? accountRequest, bool isMonthEnd);

        Task<BaseResponses<dynamic>> UpdateLogDetails(string? date);
    }
}
