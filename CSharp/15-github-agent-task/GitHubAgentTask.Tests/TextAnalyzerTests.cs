namespace GitHubAgentTask.Tests;

public class TextAnalyzerTests
{
    private readonly TextAnalyzer analyzer = new();

    [Theory]
    [InlineData("", 0)]
    [InlineData("   ", 0)]
    [InlineData("One sentence.", 1)]
    [InlineData("Hello! How are you? Fine.", 3)]
    [InlineData("Hello...Really?", 2)]
    public void CountSentences_IgnoresEmptyFragments(string text, int expected)
    {
        Assert.Equal(expected, analyzer.CountSentences(text));
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("word", 4)]
    [InlineData("Hi, world!", 3.5)]
    [InlineData("One two three.", 3.67)]
    public void AverageWordLength_ReturnsAverageLettersRoundedToTwoDecimals(string text, double expected)
    {
        Assert.Equal(expected, analyzer.AverageWordLength(text));
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("single", 1)]
    [InlineData("!!!", 1)]
    public void EstimateReadingTimeMinutes_ReturnsMinimumForNonEmptyText(string text, int expected)
    {
        Assert.Equal(expected, analyzer.EstimateReadingTimeMinutes(text));
    }

    [Fact]
    public void EstimateReadingTimeMinutes_RoundsUpAtTwoHundredWordsPerMinute()
    {
        var text = string.Join(" ", Enumerable.Repeat("word", 201));

        Assert.Equal(2, analyzer.EstimateReadingTimeMinutes(text));
    }

    [Fact]
    public void TopWords_ReturnsMostFrequentWordsCaseInsensitiveWithAlphabeticalTies()
    {
        var results = analyzer.TopWords("Banana apple banana! Cherry apple cherry.", 2);

        Assert.Equal(new[] { ("apple", 2), ("banana", 2) }, results);
    }

    [Fact]
    public void TopWords_ReturnsEmptyForEmptyTextOrNonPositiveLimit()
    {
        Assert.Empty(analyzer.TopWords("", 3));
        Assert.Empty(analyzer.TopWords("one two", 0));
    }
}
