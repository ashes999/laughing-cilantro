using LaughingCilantro.ObjectModel.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LaughingCilantro.Controllers
{
    public class TransactionController : Controller
    {
        private IFinancialAccountRepository accountRepository;

        public TransactionController(IFinancialAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        public ActionResult GetAll(Guid accountId)
        {
            var results = this.accountRepository.GetAccountTransactions(accountId);
            var json = JsonConvert.SerializeObject(results);
            return new ContentResult() {
                Content = json,
                ContentType = "application/json"
            };
        }
    }
}