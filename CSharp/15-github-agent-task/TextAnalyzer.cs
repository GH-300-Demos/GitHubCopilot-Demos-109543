using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Demo 15 — Run an Agent Task on GitHub.com
/// =========================================
/// This file is the starting point for delegating work to the GitHub Copilot
/// <b>coding agent</b> directly from the browser at https://github.com/copilot/agents
/// (the "Agents" page) — no local editor required.
///
/// Unlike Demo 14 (which starts from a GitHub Issue), here you describe a task in
/// plain language, hand it to the agent on GitHub.com, and the agent works
/// asynchronously in a cloud session: it creates a branch, edits files, runs the
/// build/tests, and opens a Pull Request for you to review.
///
/// The class below intentionally implements only the basics. The feature gaps
/// listed under "TODO (agent task)" are what you ask the agent to build.
/// See TASK.md in this folder for the exact prompt to paste into GitHub.com.
///
/// Demo flow (see the language README for the full script):
///   1. Push this repository to GitHub
///   2. Open https://github.com/copilot/agents and select this repository
///   3. Paste the prompt from TASK.md and start the task
///   4. Watch the agent session run, then review the Pull Request it opens
///   5. Iterate with review comments, then merge
/// </summary>
public class TextAnalyzer
{
    /// <summary>Counts the words in <paramref name="text"/> (whitespace-separated).</summary>
    public int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        return text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    /// <summary>Counts the characters in <paramref name="text"/>.</summary>
    public int CountCharacters(string text, bool includeWhitespace = true)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        return includeWhitespace
            ? text.Length
            : text.Count(c => !char.IsWhiteSpace(c));
    }

    /// <summary>Counts sentences in <paramref name="text"/> by splitting on '.', '!', and '?'.</summary>
    public int CountSentences(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        return text
            .Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .Count(fragment => !string.IsNullOrWhiteSpace(fragment));
    }

    /// <summary>Calculates the average number of letters per word in <paramref name="text"/>.</summary>
    public double AverageWordLength(string text)
    {
        var words = ExtractWords(text).ToList();
        if (words.Count == 0)
        {
            return 0;
        }

        return Math.Round(words.Average(word => word.Length), 2);
    }

    /// <summary>Estimates reading time in whole minutes at about 200 words per minute.</summary>
    public int EstimateReadingTimeMinutes(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        var wordCount = ExtractWords(text).Count();
        return Math.Max(1, (int)Math.Ceiling(wordCount / 200.0));
    }

    /// <summary>Returns the most frequent words in <paramref name="text"/>, case-insensitively.</summary>
    public IReadOnlyList<(string Word, int Count)> TopWords(string text, int n)
    {
        if (n <= 0)
        {
            return Array.Empty<(string Word, int Count)>();
        }

        return ExtractWords(text)
            .Select(word => word.ToLowerInvariant())
            .GroupBy(word => word)
            .Select(group => (Word: group.Key, Count: group.Count()))
            .OrderByDescending(entry => entry.Count)
            .ThenBy(entry => entry.Word)
            .Take(n)
            .ToList();
    }

    private static IEnumerable<string> ExtractWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Enumerable.Empty<string>();
        }

        return Regex.Matches(text, @"\p{L}+").Select(match => match.Value);
    }
}

public static class Program
{
    public static void Main()
    {
        const string sample =
            "GitHub Copilot can run an agent task directly on GitHub.com. " +
            "You describe the work and the agent opens a Pull Request!";

        var analyzer = new TextAnalyzer();

        Console.WriteLine("=== Text Analyzer (Demo 15) ===");
        Console.WriteLine($"Words:               {analyzer.CountWords(sample)}");
        Console.WriteLine($"Characters:          {analyzer.CountCharacters(sample)}");
        Console.WriteLine($"Characters (no ws):  {analyzer.CountCharacters(sample, includeWhitespace: false)}");
        Console.WriteLine($"Sentences:           {analyzer.CountSentences(sample)}");
        Console.WriteLine($"Avg word length:     {analyzer.AverageWordLength(sample)}");
        Console.WriteLine($"Reading time:        {analyzer.EstimateReadingTimeMinutes(sample)} minute(s)");
        Console.WriteLine("Top words:");
        foreach (var (word, count) in analyzer.TopWords(sample, 5))
        {
            Console.WriteLine($"  {word}: {count}");
        }
    }
}
