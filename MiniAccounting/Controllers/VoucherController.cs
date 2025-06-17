using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniAccounting.Data;
using MiniAccounting.Exceptions;
using MiniAccounting.Helpers;
using MiniAccounting.Models;
using System.Data;
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

        [HttpPost]
        public async Task<IActionResult> Create(VoucherModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            foreach(var data in model.VoucherDetails)
            {
                string acctCode =  data.AccountIdNum.ToString();
                int acctId = -2;

                do
                {
                   acctId = _accountRepository.GetAccountId(acctCode);

                } while (acctId <= 0);

                data.AccountIdNum = acctId;



            }

            DataTable dataTableData = DataTableHelper.CreateVoucherDetailsDataTable(model.VoucherDetails);

            string userId = await _userRepository.GetUserIdAsync(model.CreatedBy);

            try
            {
                _voucherRepository.SaveVoucher(model.VoucherType, model.VoucherDate,
                   model.ReferenceNo, userId, dataTableData);


            }
            catch (RepositoryException ex)
            {
                ModelState.AddModelError("", ex.Message);
                // optionally log ex.InnerException
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred.");
                // optionally log ex
            }

            
            return View(model);


        }
    }
}
