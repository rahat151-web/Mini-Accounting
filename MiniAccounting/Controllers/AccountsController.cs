using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniAccounting.Data;
using MiniAccounting.Exceptions;
using MiniAccounting.Models;

namespace MiniAccounting.Controllers
{
    public class AccountsController : Controller
    {
        private readonly AccountsRepository _repository;
        
        public AccountsController(AccountsRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        
        
        {
            var parentAccounts = _repository.GetAccountsByParent(null);
            return View(parentAccounts);
        }

        

        public IActionResult GetChildAccounts(int parentId)
        {
            var childAccounts = _repository.GetAccountsByParent(parentId);
            return PartialView("_ChildAccounts", childAccounts);
        }

        public IActionResult Create()
        {
            return View(new Accounts());
        }

        [HttpPost]
        public IActionResult Create(Accounts account)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Check your data whether something missing or not.";
                return View(account);

            }

            
            try
              {
                 _repository.AddAccount(account);

                 TempData["Success"] = "Voucher created successfully";

                 return RedirectToAction("Create");

            }

            catch (RepositoryException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }



            return View(account);
            

            
        }

        public IActionResult Edit(int id)
        {
            var acct = _repository.GetAccountDetails(id);

            if (acct == null)
            {
                return RedirectToAction("Index");
            }



            return View( acct );
        }

        [HttpPost]
        public IActionResult Edit(Accounts account)
        {
            if (account.AccountName!=null)
            {
                _repository.UpdateAccount(account);
                return RedirectToAction("Index");
            }
            return View(account);
        }

        public IActionResult Delete(int id)
        {
            var acct = _repository.GetAccountDetails(id);

            if (acct == null)
            {
                return RedirectToAction("Index");
            }



            return View(acct);
        }


        [HttpPost]
        public IActionResult Delete(Accounts account)
        {
            if (account.AccountId > 0)
            {
                _repository.DeleteAccount(account);
                return RedirectToAction("Index");
            }
            return View(account);
        }
    }
}
