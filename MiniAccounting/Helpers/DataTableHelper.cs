using MiniAccounting.Models;
using System.Data;

namespace MiniAccounting.Helpers
{
    public static class DataTableHelper
    {
        public static DataTable CreateVoucherDetailsDataTable(List<VoucherDetailModel> details)
        {
            DataTable table = new DataTable();

            table.Columns.Add("AccountId", typeof(int));
            table.Columns.Add("Debit", typeof(decimal));
            table.Columns.Add("Credit", typeof(decimal));
            table.Columns.Add("Description", typeof(string));

            foreach (var detail in details)
            {
                table.Rows.Add(detail.AccountId, detail.Debit, detail.Credit, detail.Description);
            }

            return table;
        }

    }
}
