using BankApplication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Interfaces
{
    public interface IValidateInput
    {
        Task<BaseResponse<string>> ValidateProcess(string input, string option);
    }
}
