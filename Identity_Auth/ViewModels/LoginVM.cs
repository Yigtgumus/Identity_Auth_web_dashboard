using System.ComponentModel.DataAnnotations;

namespace Identity_Auth.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage ="Username is Required.")]
        public string Username  { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is Required.")]
        public string Password { get; set; }
        [Display (Name ="Remember me ")]
        public bool RememberMe { get; set; }
    }
}
