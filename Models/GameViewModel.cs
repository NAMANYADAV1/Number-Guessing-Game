namespace NumberGuessingGame.Models;

public class GameViewModel
{
    public GuessInputModel Input { get; set; } = new();

    public int Attempts { get; set; }

    public string Feedback { get; set; } = string.Empty;

    public bool HasWon { get; set; }
}
