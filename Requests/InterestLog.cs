using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Requests
{
    public class InterestLog
    {
        public DateTime? Date { get; set; }

        public string? IsAppliedInterest { get; set; }
    }
}