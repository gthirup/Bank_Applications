using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Responses
{
    public class BaseResponse<T> : AdditionalInfo
    {
        public T? Data { get; set; }
    }
}
