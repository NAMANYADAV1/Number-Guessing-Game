using System.ComponentModel.DataAnnotations;

namespace NumberGuessingGame.Models;

public class GuessInputModel
{
    [Required(ErrorMessage = "Enter a number to make a guess.")]
    [Range(1, 100, ErrorMessage = "Your guess must be between 1 and 100.")]
    public int? Guess { get; set; }
}
