namespace MiniAccounting.Models
{
    public class VoucherModel
    {
        public string VoucherType { get; set; }
        public DateTime VoucherDate { get; set; }
        public string ReferenceNo { get; set; }
        public string CreatedBy { get; set; }
        public List<VoucherDetailModel> VoucherDetails { get; set; }
    }
}
