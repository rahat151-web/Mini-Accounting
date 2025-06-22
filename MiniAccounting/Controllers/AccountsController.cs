using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniAccounting.Data;
using MiniAccounting.Exceptions;
using MiniAccounting.Helpers;
using MiniAccounting.Models;

namespace MiniAccounting.Controllers
{
    public class AccountsController : Controller
    {
        private readonly AccountsRepository _repository;
        private readonly IHttpContextAccessor _contextAccessor;


        public AccountsController(AccountsRepository repository, IHttpContextAccessor contextAccessor)
        {
            _repository = repository;
            _contextAccessor = contextAccessor;
        }

        [ModuleAuthorize("ChartOfAccounts", 0)]
        public IActionResult Index()
        {
            var parentAccounts = _repository.GetAccountsByParent(null);
            var roleName = _contextAccessor.HttpContext.Session.GetString("RoleName");
            ViewBag.RoleName = roleName;

            return View(parentAccounts);
        }


        [ModuleAuthorize("ChartOfAccounts", 0)]
        public IActionResult GetChildAccounts(int parentId)
        {
            var childAccounts = _repository.GetAccountsByParent(parentId);
            var roleName = _contextAccessor.HttpContext.Session.GetString("RoleName");
            ViewBag.RoleName = roleName;

            return PartialView("_ChildAccounts", childAccounts);
        }

        [ModuleAuthorize("ChartOfAccounts", 1)]
        [HttpGet]
        public IActionResult Create(int? id)
        
        {
            if (id != null)
            {
                ViewBag.parentAcct =  _repository.GetAccountDetails((int)id);
                

            }
            return View(new Accounts() { ParentAccountId = id});
        }

        [ModuleAuthorize("ChartOfAccounts", 1)]

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

                 TempData["Success"] = "Account created successfully";

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

        [ModuleAuthorize("ChartOfAccounts", 1)]
        [HttpGet]

        public IActionResult Edit(int id)
        {
            var acct = _repository.GetAccountDetails(id);

            if (acct == null)
            {
                return RedirectToAction("Index");
            }



            return View( acct );
        }


        [ModuleAuthorize("ChartOfAccounts", 1)]

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
        [ModuleAuthorize("ChartOfAccounts", 1)]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var acct = _repository.GetAccountDetails(id);

            if (acct == null)
            {
                return RedirectToAction("Index");
            }



            return View(acct);
        }

        [ModuleAuthorize("ChartOfAccounts", 1)]

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
