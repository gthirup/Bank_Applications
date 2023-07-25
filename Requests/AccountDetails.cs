using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using BankApplication.Requests;

namespace BankApplication.Requests
{
    public class AccountDetails
    {

        public string? AccountId { get; set; }

        public string? AccountNumber { get; set; }

        public string? Date { get; set; }

        public string? OpenBalance { get; set; }

        public string? CurrentBalance { get; set; }

        public string? Interest { get; set; }
    }
}
