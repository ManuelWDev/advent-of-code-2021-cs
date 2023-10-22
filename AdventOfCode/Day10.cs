namespace AdventOfCode; 

public class Day10 : BaseDay {
    private readonly string _input;

    public Day10() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        using var reader = new StringReader(_input);
        
        var bracketStack = new Stack<char>();
        var syntaxErrorScore = 0;
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            bracketStack.Clear();
            foreach (var character in line) {
                if (IsOpeningBracket(character)) {
                    bracketStack.Push(character);
                    continue;
                }

                if (bracketStack.TryPop(out var previousOpen)) {
                    if (GetClosingBracketFor(previousOpen) != character) {
                        syntaxErrorScore += GetCostsForIncorrectBracket(character);
                        break;
                    }
                }
            }
        }

        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {syntaxErrorScore}");
    }

    public override ValueTask<string> Solve_2() {
        using var reader = new StringReader(_input);
        
        var bracketStack = new Stack<char>();
        List<long> autocompleteCosts = new();
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            bracketStack.Clear();
            var syntaxError = false;
            foreach (var character in line) {
                if (IsOpeningBracket(character)) {
                    bracketStack.Push(character);
                    continue;
                }

                if (bracketStack.TryPop(out var previousOpen)) {
                    if (GetClosingBracketFor(previousOpen) != character) {
                        syntaxError = true;
                        break;
                    }
                }
            }
            if (syntaxError) continue;
            
            var score = 0L;
            while (bracketStack.TryPop(out var nextOpening)) {
                score *= 5;
                score += GetCostsForAutocompleteBracket(nextOpening);
            }
            autocompleteCosts.Add(score);
        }
        
        autocompleteCosts.Sort();
        var middleScore = autocompleteCosts[autocompleteCosts.Count / 2];
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {middleScore}");
    }

    private static char GetClosingBracketFor(char bracket) {
        return bracket switch {
            '(' => ')',
            '[' => ']',
            '{' => '}',
            '<' => '>',
            _ => throw new ArgumentOutOfRangeException(nameof(bracket), bracket, null)
        };
    }
    
    private static bool IsOpeningBracket(char bracket) {
        return bracket switch {
            '(' => true,
            '[' => true,
            '{' => true,
            '<' => true,
            _ => false
        };
    }

    private static int GetCostsForIncorrectBracket(char bracket) {
        return bracket switch {
            ')' => 3,
            ']' => 57,
            '}' => 1197,
            '>' => 25137,
            _ => throw new ArgumentOutOfRangeException(nameof(bracket), bracket, null)
        };
    }
    
    private static int GetCostsForAutocompleteBracket(char bracket) {
        return bracket switch {
            '(' => 1,
            '[' => 2,
            '{' => 3,
            '<' => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(bracket), bracket, null)
        };
    }
}