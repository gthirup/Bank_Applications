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

        public string? AccountId { get; set; } = string.Empty;

        public string? AccountNumber { get; set; } = string.Empty;

        public string? Date { get; set; } = string.Empty;

        public string? OpenBalance { get; set; } = string.Empty;

        public string? CurrentBalance { get; set; } = string.Empty;

        public string? Interest { get; set; } = string.Empty;
    }
}
