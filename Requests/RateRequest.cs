using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Requests
{
    public class RateRequest
    {
        public string? Date { get; set; } = string.Empty;

        public string? RuleId { get; set; } = string.Empty;

        public string? Rate { get; set; } = string.Empty;

    }
}