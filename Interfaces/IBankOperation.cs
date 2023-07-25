using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using BankApplication.Responses;

namespace BankApplication.Interfaces
{
    public interface IBankOperation
    {
        Task<BaseResponse<dynamic>> InvokeBankingOperations(bool printOption);
    }
}
