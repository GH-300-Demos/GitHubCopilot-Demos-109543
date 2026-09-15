# TextAnalyzer API

`TextAnalyzer` provides simple text metrics for the Demo 15 GitHub agent task.

## Public methods

- `CountWords(string text)` counts whitespace-separated words.
- `CountCharacters(string text, bool includeWhitespace = true)` counts characters, optionally excluding whitespace.
- `CountSentences(string text)` counts non-empty sentence fragments split on `.`, `!`, and `?`.
- `AverageWordLength(string text)` returns the average number of letters per word rounded to two decimals, or `0` for empty text.
- `EstimateReadingTimeMinutes(string text)` estimates reading time at about 200 words per minute, rounded up with a minimum of one minute for non-empty text.
- `TopWords(string text, int n)` returns the `n` most frequent words case-insensitively, breaking ties alphabetically.

## Usage

```csharp
var analyzer = new TextAnalyzer();
var text = "GitHub Copilot helps developers. Copilot can summarize text!";

Console.WriteLine(analyzer.CountSentences(text));              // 2
Console.WriteLine(analyzer.AverageWordLength(text));           // 6.38
Console.WriteLine(analyzer.EstimateReadingTimeMinutes(text));  // 1

foreach (var (word, count) in analyzer.TopWords(text, 3))
{
    Console.WriteLine($"{word}: {count}");
}
```
