using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Responses
{
    public class PrintStatementResponse
    {
        public string? AccountId { get; set; }

        public string? AccountNumber { get; set; }

        public string? OpenBalance { get; set; }

        public string? CurrentBalance { get; set; }

        public string? TransactionDate { get; set; }

        public string? TxnId { get; set; }

        public string? Type { get; set; }

        public string? TransactionAmount { get; set; }
    }
}