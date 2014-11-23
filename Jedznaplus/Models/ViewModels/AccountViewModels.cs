using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Jedznaplus.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class UserListItemViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        [Display(Name = "Nazwa użytkownika")]
        public string UserName { get; set; }
        [Display(Name = "Email potwierdzony")]
        public bool EmailConfirmed { get; set; }
    }

        public class UserDetailsViewModel : UserListItemViewModel
    {
        [Display(Name = "Avatar użytkownika")]
        public string AvatarUrl { get; set; }
        [Display(Name = "Role użytkownika")]
        public string[] UserRoles { get; set; }

        public List<SelectListItem> Roles {get; set;}
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Kod")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Zapamiętaj mnie")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required (ErrorMessage = "Pole Email jest wymagane.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Pole Login jest wymagane.")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Pole Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamiętaj mnie")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Pole Login jest wymagane.")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Pole Email jest wymagane.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pole Hasło jest wymagane.")]
        [StringLength(100, ErrorMessage = "{0} musi się składać minimum z {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Podane hasła nie są takie same.")]
        public string ConfirmPassword { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Pole Email jest wymagane.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pole Hasło jest wymagane.")]
        [StringLength(100, ErrorMessage = "{0} musi się składać minimum z {2} znaków..", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Podane hasła nie są takie same.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Pole Email jest wymagane.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
