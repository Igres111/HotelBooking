using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Web.Models.ViewModels
{
    public class SignUpViewModel
    {
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
