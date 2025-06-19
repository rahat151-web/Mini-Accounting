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

        private VoucherModel PopulateViewData()
        {
            string userId, userName;

            do
            {
                userId = _contextAccessor.HttpContext.Session.GetString("UserId");
                userName =  _userRepository.GetUserName(userId);

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

            return model;
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        { 

            return View(PopulateViewData());
        }

        [HttpPost]
        public async Task<IActionResult> Create(VoucherModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Check your data whether something missing or not.";
                return View(PopulateViewData());

            }

            

            decimal total_debit = 0;
            decimal total_credit = 0;

            foreach(var data in model.VoucherDetails)
            {
                total_debit += data.Debit;

                total_credit += data.Credit;
            }

            if (total_credit != total_debit)
            {
                ViewBag.ErrorMessage = "Total debit must be equals to total credit";
                return View(PopulateViewData());
            }


            foreach (var data in model.VoucherDetails)
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

                TempData["Success"] = "Voucher created successfully";





            }
            catch (RepositoryException ex)
            {
                ViewBag.ErrorMessage =  ex.Message;
                // optionally log ex.InnerException
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage =  ex.Message;
                // optionally log ex
            }

            
            return View(PopulateViewData());


        }
    }
}
