using LaughingCilantro.ObjectModel.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaughingCilantro.ObjectModel.Interfaces
{
    public interface IOfxParser
    {
        List<Transaction> GetBankTransactionList(string contents);
    }
}
