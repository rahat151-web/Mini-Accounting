﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniAccounting.Data;
using MiniAccounting.Exceptions;
using MiniAccounting.Models.Users;

namespace MiniAccounting.Controllers
{
    public class UserController : Controller
    {
        private readonly  ManageUsersRepository _repository;
        public UserController(ManageUsersRepository repository)
        {
            _repository = repository;
        }

        public IActionResult RegisterUser() => View();

        // INSERT
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserModel model)
        {
            try
            {
                bool userExists = await _repository.UserExistsAsync(model.UserName);

                if (userExists)
                {
                    ModelState.AddModelError("UserName", "Username is already taken.");
                    
                }
                
                if (ModelState.IsValid && userExists==false)
                {
                    var user = new IdentityUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.UserName,
                        NormalizedUserName = model.UserName.ToUpper(),
                        Email = model.Email,
                        NormalizedEmail = model.Email.ToUpper(),
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        PhoneNumber = model.PhoneNumber,
                        PhoneNumberConfirmed = true,
                        TwoFactorEnabled = false,
                        LockoutEnd = null,
                        LockoutEnabled = true,
                        AccessFailedCount = 0
                    };

                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

                    await _repository.ManageUserAsync("INSERT", user, model.RoleName);

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

        // UPDATE
        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserModel model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var user = new IdentityUser
                    {
                        Id = model.Id,
                        Email = model.Email,
                        NormalizedEmail = model.Email.ToUpper(),
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        PhoneNumber = model.PhoneNumber,
                        PhoneNumberConfirmed = true,
                        TwoFactorEnabled = false,
                        LockoutEnd = null,
                        LockoutEnabled = true,
                        AccessFailedCount = 0
                    };


                    if (model.Password == null)
                    {
                        user.PasswordHash = null;
                    }

                    else
                    {
                        var passwordHasher = new PasswordHasher<IdentityUser>();
                        user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
                    }


                    await _repository.ManageUserAsync("UPDATE", user, model.RoleName);

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

        // DELETE
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = new IdentityUser
            {
                Id = id
            };

            await _repository.ManageUserAsync("DELETE", user, null);

            return Ok("User deleted successfully");
        }

        [HttpGet]

        public async Task<IActionResult> GetAllUsersDetails()
        {
            var data = await _repository.GetUserDetailsAsync();

            return View(data);

        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string Id)
        {
            var data = await _repository.GetUserDataAsync(Id);

            return View(data);
        }
    }
}
