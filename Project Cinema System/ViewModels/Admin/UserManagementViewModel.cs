
namespace Project_Cinema_System.ViewModels.Admin
{
    public class UserManagementViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsLocked { get; set; }
    }
}
