using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Responses
{
    public class BaseResponses<T> : AdditionalInfo
    {
        public IEnumerable<T>? Data { get; set; }

    }
}