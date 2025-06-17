namespace MiniAccounting.Models
{
    public class VoucherDetailModel
    {
        public int AccountIdNum { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Description { get; set; }
    }
}
