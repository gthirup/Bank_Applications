using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Requests
{
    public class RateRequest
    {
        public string? Date { get; set; }

        public string? RuleId { get; set; }

        public string? Rate { get; set; }

    }
}