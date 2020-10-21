using System;
using System.ComponentModel.DataAnnotations;

namespace BankAccounts.Models
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string LoginEmail { get; set; }
        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Длина пароля не менее 8 символов")]
        public string LoginPassword { get; set; }
    }
}
