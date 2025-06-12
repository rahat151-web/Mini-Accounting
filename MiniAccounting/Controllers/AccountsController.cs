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

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Accounts account)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repository.AddAccount(account);
                    return RedirectToAction("Index");
                }
                return View(account);
            }

            catch (RepositoryException ex)
            {
                return RedirectToAction("Create");
            }
        }

        //public IActionResult Edit(int id)
        //{
        //    var student = _repository.GetStudentById(id);
        //    return View(student);
        //}

        //[HttpPost]
        //public IActionResult Edit(Student student)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _repository.UpdateStudent(student);
        //        return RedirectToAction("Index");
        //    }
        //    return View(student);
        //}

        //public IActionResult Delete(int id)
        //{
        //    var student = _repository.GetStudentById(id);
        //    return View(student);
        //}

        //[HttpPost]
        //public IActionResult DeleteConfirmed(int id)
        //{
        //    _repository.DeleteStudent(id);
        //    return RedirectToAction("Index");
        //}
    }
}
