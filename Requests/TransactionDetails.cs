using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Requests
{
    public class TransactionDetails
    {
        public string? Id { get; set; } = string.Empty;

        public string? AccNumber { get; set; } = string.Empty;
        public string? Date { get; set; } = string.Empty;

        public string? TxnId { get; set; } = string.Empty;

        public string? Type { get; set; } = string.Empty;

        public string? Amount { get; set; } = string.Empty;

        public string? Balance { get; set; } = string.Empty;
    }
}
