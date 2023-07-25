using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Requests
{
    public class AccountRequest
    {

        public string? AccountId { get; set; }

        public string? AccountNumber { get; set; }

        public string? AmountValue { get; set; }

        public string? Interest { get; set; }
    }
}