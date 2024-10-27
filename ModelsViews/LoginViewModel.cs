using System.ComponentModel.DataAnnotations;

namespace BTL.ModelsViews
{
    public class LoginViewModel
    {
        [MaxLength(100)]
        [Required(ErrorMessage = "Vui lòng nhập Email!")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email: ")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [MinLength(6,ErrorMessage ="Bạn cần nhập tối thiểu 6 ký tự")]
        public string Password { get; set; }
    }
}
