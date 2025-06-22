using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniAccounting.Data;
using MiniAccounting.Exceptions;
using MiniAccounting.Helpers;
using MiniAccounting.Models.Users;
using System.Reflection;

namespace MiniAccounting.Controllers
{
    [ModuleAuthorize("UserManagement", 1)]
    public class UserController : Controller
    {
        private readonly  ManageUsersRepository _repository;
        public UserController(ManageUsersRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]//index 

        public async Task<IActionResult> GetAllUsersDetails()
        {
            var data = await _repository.GetUserDetailsAsync();

            List<bool> has_Voucher = new List<bool>();

            foreach(var i in data)
            {
                bool result = await _repository.hasVoucherAsync(i.Id);
                has_Voucher.Add(result);
            }

            ViewBag.Has_Voucher = has_Voucher;

            return View(data);

        }


        [HttpGet] //create
        public IActionResult RegisterUser() => View();

       
        [HttpPost] //create
        public async Task<IActionResult> RegisterUser(RegisterUserModel model)
        {
            try
            {
                bool userExists = await _repository.UserExistsAsync(model.UserName);

                if (userExists)
                {
                    ModelState.AddModelError("UserName", "Username is already taken.");
                    
                }

                bool password_Length_Min8Chars = false, password_contains_LowerCase = false,
                    password_contains_UpperCase = false, password_contains_SpecialChar = false,
                    password_contains_Number = false;

                for (int i=0; i<model.Password.Length; i++)
                {
                    if (model.Password[i] >= 'a' && model.Password[i] <= 'z')
                        password_contains_LowerCase = true;
                    else if (model.Password[i] >= 'A' && model.Password[i] <= 'Z')
                        password_contains_UpperCase = true;
                    else if (model.Password[i] >= '0' && model.Password[i] <= '9')
                        password_contains_Number = true;
                    else
                        password_contains_SpecialChar = true;
                }

                if (model.Password.Length >= 8)
                    password_Length_Min8Chars = true;

                if(!password_Length_Min8Chars || !password_contains_LowerCase || !password_contains_UpperCase
                    || !password_contains_SpecialChar || !password_contains_Number)
                {
                    ModelState.AddModelError("Password", "Minimum 8 chars and should contain lowercase, uppercase, number and special characters.");

                }

                if (ModelState.IsValid && userExists==false)
                {


                    await _repository.CreateUserAsync(model.UserName, model.Email,
                        model.Password, model.PhoneNumber, model.RoleName);

                    TempData["Success"] = "User created successfully";

                    return RedirectToAction("GetAllUsersDetails");
                }

                return View(model);


            }

            catch (RepositoryException ex)
            {
                //add some message for showing
                return RedirectToAction("RegisterUser");
            }

        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string Id)
        {
            var data = await _repository.GetUserDataAsync(Id);

            if(data.RoleName=="Admin")
                ViewBag.RoleName = "Admin";

            return View(data);
        }

        
        [HttpPost]
        public async Task<IActionResult> EditUser(UserModel model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    await _repository.UpdateUserAsync(model.Id, model.Email, model.Password, model.PhoneNumber,
                        model.RoleName);

                    TempData["Success"] = "User updated successfully";


                    return RedirectToAction("GetAllUsersDetails");
                }

                return View(model);
            }

            catch (RepositoryException ex)
            {
                //add some message for showing
                return RedirectToAction("GetAllUsersDetails");
            }





        }

        
        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var data = await _repository.GetUserDataAsync(id);

            return View(data);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(UserModel model)
        {
            bool has_voucher = await _repository.hasVoucherAsync(model.Id);

            if (has_voucher)
                return RedirectToAction("GetAllUsersDetails");

            await _repository.DeleteUserAsync(model.Id);

            TempData["Success"] = "User deleted successfully";


            return RedirectToAction("GetAllUsersDetails");


        }




    }
}
