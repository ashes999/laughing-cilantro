using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using LaughingCilantro.ObjectModel.Interfaces;
using LaughingCilantro.ObjectModel.Objects;

namespace LaughingCilantro.Controllers
{
    [Authorize]
    public class FinancialAccountController : Controller
    {
        private IUserRepository userRepository;
        private IFinancialAccountRepository accountRepository;
        private IOfxParser ofxParser;

        public FinancialAccountController(IUserRepository userRepository, IFinancialAccountRepository accountRepository, IOfxParser ofxParser) : base()
        {
            this.userRepository = userRepository;
            this.accountRepository = accountRepository;
            this.ofxParser = ofxParser;
        }

        public ActionResult Index()
        {
            var userId = this.GetUserId();
            var model = this.accountRepository.GetAllAccounts(userId);
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FinancialAccount account)
        {
            account.OwnerId = this.GetUserId();

            if (string.IsNullOrEmpty(account.Name) || string.IsNullOrEmpty(account.OwnerId) || !Enum.IsDefined(typeof(AccountType), account.AccountType))
            {
                ViewBag.Flash = "Please fill out all the required fields.";
                return View(account);
            }
            else
            {
                ViewBag.Flash = "Account created!";
                this.accountRepository.CreateAccount(account);
                return this.RedirectToAction("Index");
            }
        }

        public ActionResult Details(Guid id)
        {
            var account = this.accountRepository.GetAllAccounts(this.GetUserId()).SingleOrDefault(a => a.Id == id);
            if (account == null)
            {
                TempData["Flash"] = "That account doesn't seem to exist.";
                return this.RedirectToAction("Index");
            }

            return View(account);
        }

        public ActionResult MonthlyDetails(Guid id)
        {
            var account = this.accountRepository.GetAllAccounts(this.GetUserId()).SingleOrDefault(a => a.Id == id);
            if (account == null)
            {
                TempData["Flash"] = "That account doesn't seem to exist.";
                return this.RedirectToAction("Index");
            }

            return View(account);
        }

        public ActionResult UploadTransactions(Guid accountId)
        {
            string pathToSave = Server.MapPath(string.Format("~/Temp/{0}", GetUserId()));
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            var failureFiles = new List<string>();

            foreach (string upload in Request.Files)
            {
                if (Request.Files[upload].ContentLength == 0)
                {
                    continue;
                }

                // Use a GUID as the filename. Avoids collisions, attacks, etc.
                string fullPath = Path.Combine(pathToSave, Guid.NewGuid().ToString());
                Request.Files[upload].SaveAs(fullPath);
                var succeeded = ProcessAndDelete(fullPath, accountId);
                if (!succeeded)
                {
                    failureFiles.Add(Request.Files[upload].FileName);
                }
            }

            if (!Directory.EnumerateFiles(pathToSave).Any())
            {
                Directory.Delete(pathToSave);
            }

            if (failureFiles.Any())
            {
                TempData["Flash"] = string.Format("Import failed for the following files: {0}", string.Join(",", failureFiles));
            } else
            {
                TempData["Flash"] = "Transactions imported.";
            }

            return this.RedirectToAction("Details", new { id = accountId });
        }

        public ActionResult Stats()
        {
            return View();
        }

        // Return true if successful, false otherwise.
        private bool ProcessAndDelete(string fullPath, Guid accountId)
        {
            var contents = System.IO.File.ReadAllText(fullPath);
            var transactions = this.ofxParser.GetBankTransactionList(contents);
            this.accountRepository.SaveTransactions(accountId, transactions);
            System.IO.File.Delete(fullPath);
            return transactions.Any();
        }

        // TODO: move into a base controller
        private string GetUserId()
        {
            return this.userRepository.GetUserId(User.Identity.Name);   
        }
    }
}