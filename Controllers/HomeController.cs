using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NumberGuessingGame.Models;

namespace NumberGuessingGame.Controllers;

public class HomeController : Controller
{
    private const string TargetNumberKey = "TargetNumber";
    private const string AttemptsKey = "Attempts";
    private const string HasWonKey = "HasWon";
    private const string FeedbackKey = "Feedback";

    public IActionResult Index()
    {
        StartGameIfNeeded();
        return View(CreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Guess([Bind("Guess")] GuessInputModel input)
    {
        StartGameIfNeeded();

        if (HttpContext.Session.GetInt32(HasWonKey) == 1)
        {
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(nameof(Index), CreateViewModel(input));
        }

        var guess = input.Guess!.Value;
        var targetNumber = HttpContext.Session.GetInt32(TargetNumberKey)!.Value;
        var attempts = HttpContext.Session.GetInt32(AttemptsKey)!.Value + 1;
        HttpContext.Session.SetInt32(AttemptsKey, attempts);

        if (guess == targetNumber)
        {
            HttpContext.Session.SetInt32(HasWonKey, 1);
            HttpContext.Session.SetString(FeedbackKey, $"You got it! The number was {targetNumber}.");
        }
        else if (guess < targetNumber)
        {
            HttpContext.Session.SetString(FeedbackKey, "Too low — try a higher number.");
        }
        else
        {
            HttpContext.Session.SetString(FeedbackKey, "Too high — try a lower number.");
        }

        return View(nameof(Index), CreateViewModel(input));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult NewGame()
    {
        HttpContext.Session.Clear();
        StartGameIfNeeded();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private void StartGameIfNeeded()
    {
        if (HttpContext.Session.GetInt32(TargetNumberKey) is null)
        {
            HttpContext.Session.SetInt32(TargetNumberKey, Random.Shared.Next(1, 101));
            HttpContext.Session.SetInt32(AttemptsKey, 0);
            HttpContext.Session.SetInt32(HasWonKey, 0);
            HttpContext.Session.SetString(FeedbackKey, "Make your first guess to get started.");
        }
    }

    private GameViewModel CreateViewModel(GuessInputModel? input = null)
    {
        return new GameViewModel
        {
            Input = input ?? new GuessInputModel(),
            Attempts = HttpContext.Session.GetInt32(AttemptsKey) ?? 0,
            Feedback = HttpContext.Session.GetString(FeedbackKey) ?? string.Empty,
            HasWon = HttpContext.Session.GetInt32(HasWonKey) == 1
        };
    }
}
