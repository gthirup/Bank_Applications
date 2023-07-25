using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Requests
{
    public class AccountRequest
    {

        public string? AccountId { get; set; } = string.Empty;

        public string? AccountNumber { get; set; } = string.Empty;

        public string? AmountValue { get; set; } = string.Empty;

        public string? Interest { get; set; } = string.Empty;
    }
}