using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace MiniAccounting.Models
{
    public class Accounts
    {
        [Key]
        public int AccountId { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountCode { get; set; }

        [Required]
        [StringLength(100)]
        public string AccountName { get; set; }
        public int? ParentAccountId { get; set; }

        [ForeignKey("ParentAccountId")]
        public Accounts? ParentAccount { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountType { get; set; }

       
    }
}
