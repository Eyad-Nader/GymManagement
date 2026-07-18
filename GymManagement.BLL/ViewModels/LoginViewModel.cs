
using System.ComponentModel.DataAnnotations;
namespace GymManagement.BLL.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}