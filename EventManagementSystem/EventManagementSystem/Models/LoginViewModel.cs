using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле 'Ім'я' є обов'язковим")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Поле 'Пароль' є обов'язковим")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; } = false;
    }
}

