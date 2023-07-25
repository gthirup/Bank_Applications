using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Requests
{
    public class TransactionDetails
    {
        public string? Id { get; set; }

        public string? AccNumber { get; set; }
        public string? Date { get; set; }

        public string? TxnId { get; set; }

        public string? Type { get; set; }

        public string? Amount { get; set; }

        public string? Balance { get; set; }
    }
}
