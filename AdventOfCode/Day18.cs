namespace AdventOfCode; 

public class Day18 : BaseDay {
    private readonly string _input;

    public Day18() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var fishNumbers = ParseInput(_input);

        var resultFishNumber = fishNumbers[0];
        for (int i = 1; i < fishNumbers.Count; i++) {
            resultFishNumber += fishNumbers[i];
            resultFishNumber.Reduce();
        }

        var magnitude = resultFishNumber.CalculateMagnitude();
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {magnitude}");
    }

    public override ValueTask<string> Solve_2() {
        var fishNumbers = ParseInput(_input);
        
        var biggestMagnitude = 0;
        for (int i = 0; i < fishNumbers.Count; i++) {
            for (int j = 0; j < fishNumbers.Count; j++) {
                if (i == j)
                    continue;
                var resultNumber = fishNumbers[i] + fishNumbers[j];
                resultNumber.Reduce();
                var magnitude = resultNumber.CalculateMagnitude();
                if (magnitude > biggestMagnitude) {
                    biggestMagnitude = magnitude;
                }
            }
        }
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {biggestMagnitude}");
    }

    private static List<FishNumber> ParseInput(string input) {
        using var reader = new StringReader(input);

        var fishNumbers = new List<FishNumber>();
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            fishNumbers.Add(ParseFishNumber(line));
        }
        return fishNumbers;
    }

    private static FishNumber ParseFishNumber(string line) {
        return ParseFishNumberRec(line.AsSpan(0, line.Length));
    }

    private static FishNumber ParseFishNumberRec(ReadOnlySpan<char> remainder) {
        var c = remainder[0];
        if (c == '[') {
            SliceOuterMostBracket(remainder, out var left, out var right);
            return new PairFishNumber(ParseFishNumberRec(left), ParseFishNumberRec(right));
        }
            
        return new TerminalFishNumber(int.Parse(remainder));
    }
    
    private static void SliceOuterMostBracket(ReadOnlySpan<char> span, out ReadOnlySpan<char> first, out ReadOnlySpan<char> second) {
        var bracketCount = 0;
        var endIndex = 0;
        for (; endIndex < span.Length; endIndex++) {
            var c = span[endIndex];
            if (c == '[') {
                bracketCount++;
            }
            else if (c == ']') {
                bracketCount--;
            }
            else if (c == ',' && bracketCount == 1) {
                break;
            }
        }

        first = span[1..endIndex];
        second = span[(endIndex + 1)..^1];
    }

    private abstract class FishNumber {
        public static FishNumber operator +(FishNumber left, FishNumber right) {
            return new PairFishNumber(left.Clone(), right.Clone());
        }

        public void Reduce() {
            while (ExplodeFourNest(0, out _, out _) || SplitNumber()) { }
        }

        public abstract bool ExplodeFourNest(int depth, out int leftAdd, out int rightAdd);
        public abstract bool DeepAddLeft(int value);
        public abstract bool DeepAddRight(int value);
        public abstract bool SplitNumber();
        public abstract int CalculateMagnitude();
        public abstract FishNumber Clone();
    }
    
    private class TerminalFishNumber : FishNumber {
        public int Value { get; private set; }

        public TerminalFishNumber(int value) => Value = value;

        public override bool ExplodeFourNest(int depth, out int leftAdd, out int rightAdd) {
            leftAdd = 0;
            rightAdd = 0;
            return false;
        }

        public override bool DeepAddLeft(int value) {
            Value += value;
            return true;
        }

        public override bool DeepAddRight(int value) {
            Value += value;
            return true;
        }

        public override bool SplitNumber() => false;

        public override int CalculateMagnitude() => Value;

        public override FishNumber Clone() => new TerminalFishNumber(Value);
    }
    
    private class PairFishNumber : FishNumber {
        private FishNumber Left { get; set; }
        private FishNumber Right { get; set; }

        public PairFishNumber(FishNumber left, FishNumber right) {
            Left = left;
            Right = right;
        }

        public override bool ExplodeFourNest(int depth, out int leftAdd, out int rightAdd) {
            if (depth == 3) {
                leftAdd = 0;
                rightAdd = 0;
                if (HandleChildExplosion(Left, ref leftAdd, ref rightAdd)) {
                    Left = new TerminalFishNumber(0);
                    LeftExplosionReaction(ref rightAdd);
                    return true;
                }
                if (HandleChildExplosion(Right, ref leftAdd, ref rightAdd)) {
                    Right = new TerminalFishNumber(0);
                    RightExplosionReaction(ref leftAdd);
                    return true;
                }
            }
            else {
                if (Left.ExplodeFourNest(depth + 1, out leftAdd, out rightAdd)) {
                    LeftExplosionReaction(ref rightAdd);
                    return true;
                }
                if (Right.ExplodeFourNest(depth + 1, out leftAdd, out rightAdd)) {
                    RightExplosionReaction(ref leftAdd);
                    return true;
                }
            }
            return false;

            bool HandleChildExplosion(FishNumber child, ref int left, ref int right) {
                if (child is not PairFishNumber pair) return false;
                
                left += GetValueFromFishNumber(pair.Left);
                right += GetValueFromFishNumber(pair.Right);
                
                return true;
            }
            
            int GetValueFromFishNumber(FishNumber fishNumber) {
                if (fishNumber is TerminalFishNumber terminal) {
                    return terminal.Value;
                }
                throw new Exception("Cannot get value from non-terminal fish number");
            }

            void LeftExplosionReaction(ref int rightAdd) {
                if (rightAdd == 0) return;
                if (Right.DeepAddLeft(rightAdd)) {
                    rightAdd = 0;
                }
            }
            
            void RightExplosionReaction(ref int leftAdd) {
                if (leftAdd == 0) return;
                if (Left.DeepAddRight(leftAdd)) {
                    leftAdd = 0;
                }
            }
        }

        public override bool DeepAddLeft(int value) => Left.DeepAddLeft(value);

        public override bool DeepAddRight(int value) => Right.DeepAddRight(value);

        public override bool SplitNumber() {
            if (Left is TerminalFishNumber left && SplitIntoPairFishNumber(left.Value, out var leftPair)) {
                Left = leftPair;
                return true;
            }

            if (Left.SplitNumber()) {
                return true;
            }
            
            if (Right is TerminalFishNumber right && SplitIntoPairFishNumber(right.Value, out var rightPair)) {
                Right = rightPair;
                return true;
            }
            
            return Right.SplitNumber();
            
            bool SplitIntoPairFishNumber(int value, out PairFishNumber result) {
                result = null;
                if (value < 10) return false;
                
                var newLeftValue = new TerminalFishNumber((int)Math.Floor(value / 2.0));
                var newRightValue = new TerminalFishNumber((int)Math.Ceiling(value / 2.0));
                result = new PairFishNumber(newLeftValue, newRightValue);
                return true;
            }
        }

        public override int CalculateMagnitude() {
            return Left.CalculateMagnitude() * 3 + Right.CalculateMagnitude() * 2;
        }

        public override FishNumber Clone() => new PairFishNumber(Left.Clone(), Right.Clone());
    }
}