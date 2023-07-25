using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Responses
{
    public class PrintStatementResponse
    {
        public string? AccountId { get; set;} = string.Empty;

        public string? AccountNumber { get; set;} = string.Empty;

        public string? OpenBalance { get; set;} = string.Empty;

        public string? CurrentBalance { get; set;} = string.Empty;

        public string? TransactionDate { get; set;} = string.Empty;

        public string? TxnId { get; set;} = string.Empty;

        public string? Type { get; set;} = string.Empty;

        public string? TransactionAmount { get; set; } = string.Empty;
    }
}