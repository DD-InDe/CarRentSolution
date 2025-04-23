using System.ComponentModel.DataAnnotations;

namespace CarRentSolution.Entity;

public class LogInModel
{
    [Required(ErrorMessage = "Это поле обязательно для заполонения")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Это поле обязательно для заполонения")]
    public string Password { get; set; } = string.Empty;
}