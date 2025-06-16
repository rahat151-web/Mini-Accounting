namespace MiniAccounting.Models
{
    public class RolePermission
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int AccessValue { get; set; } // 0 = Read, 1 = Write
    }
}
