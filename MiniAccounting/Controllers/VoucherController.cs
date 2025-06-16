using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniAccounting.Data;
using MiniAccounting.Helpers;
using MiniAccounting.Models;
using System.Threading.Tasks;

namespace MiniAccounting.Controllers
{
    [ModuleAuthorize("VoucherEntry", 1)]
    public class VoucherController : Controller
    {
        private readonly VoucherRepository _voucherRepository;
        private readonly AccountsRepository _accountRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserRepository _userRepository;

        public VoucherController(VoucherRepository voucherRepository,
            AccountsRepository accountRepository, IHttpContextAccessor contextAccessor,
            UserRepository userRepository)
        {
            _voucherRepository = voucherRepository;
            _accountRepository = accountRepository;
            _contextAccessor = contextAccessor;
            _userRepository = userRepository;
        }
        

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            string userId, userName;

            do
            {
                userId = _contextAccessor.HttpContext.Session.GetString("UserId");
                userName = await _userRepository.GetUserNameAsync(userId);

            } while (userName == null);

            


            var model = new VoucherModel
            {
                VoucherDate = DateTime.Today,
                CreatedBy = userName,
                VoucherDetails = new List<VoucherDetailModel>
                {
                    new VoucherDetailModel() // Initialize one empty row for UI binding
                }
            };

            var accountList = _accountRepository.GetLeafAccounts();

            ViewBag.AccountList = accountList;

            return View(model);
        }
    }
}
