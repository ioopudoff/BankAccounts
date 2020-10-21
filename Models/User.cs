using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccounts.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [Display(Name = "Фамилия")]
        [MinLength(3, ErrorMessage = "Фамилия не может быть короче 3 символов")]
        public string SurName { get; set; }
        [Required]
        [Display(Name = " Имя")]
        [MinLength(2, ErrorMessage = "Имя не может быть короче 2 символов")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Отчество")]
        [MinLength(3, ErrorMessage = "Отчество не может быть короче 5 символов")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name ="Пароль")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Длина пароля не менее 8 символов")]
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль")]
        public string Confirm { get; set; }
    }
}
