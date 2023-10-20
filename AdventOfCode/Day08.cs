namespace AdventOfCode; 

public class Day08 : BaseDay {
    
    private readonly string _input;

    public Day08() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        
        using var reader = new StringReader(_input);

        var totalRelevantCount = 0;
        
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var splitLine = line.Split('|');
            var digitStrings = splitLine[1].Split(' ', StringSplitOptions.TrimEntries);
            totalRelevantCount += digitStrings.Count(x => IsCountRelevant(x.Length));
        }

        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {totalRelevantCount}");
    }

    private static bool IsCountRelevant(int count) {
        return count switch {
            2 => true,
            3 => true,
            4 => true,
            7 => true,
            _ => false
        };
    }

    public override ValueTask<string> Solve_2() {
        using var reader = new StringReader(_input);
        var sevenSegmentDisplay = new SevenSegmentDisplay();
        var total = 0;
        
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            
            var parts = line.Split('|');
            var digitInfos = parts[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var puzzleDigits = parts[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            sevenSegmentDisplay.SetDigitInfos(digitInfos);
            total += sevenSegmentDisplay.DigitStringsToValue(puzzleDigits);
        }
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {total}");
    }

    private class SevenSegmentDisplay {
        private readonly List<char[]> _segmentInput = new();
        private readonly Dictionary<char[], int> _numberLookUp = new(new ArrayInnerEqualityComparer());

        private void Reset() {
            _segmentInput.Clear();
            _numberLookUp.Clear();
        }

        public void SetDigitInfos(IEnumerable<string> infos) {
            Reset();
            foreach (var info in infos) {
                _segmentInput.Add(info.ToCharArray());
            }
            Solve();
        }

        private int LookUp(char[] input) {
            Array.Sort(input);
            return _numberLookUp[input];
        }

        private void Solve() {
            var one = GetAnyObjectWithCount(2);
            var seven = GetAnyObjectWithCount(3);
            var eight = GetAnyObjectWithCount(7);
            var four = GetAnyObjectWithCount(4);

            var twoThreeFive = GetAllObjectsWithCount(5).ToArray();
            var sixNineZero = GetAllObjectsWithCount(6).ToArray();

            var nine = FirstArrayThatFullyContains(sixNineZero, four);
            var bottomLeft = eight.Except(nine).First();
            var sixZero = sixNineZero.Where(x => x != nine).ToArray();
            var zero = FirstArrayThatFullyContains(sixZero, one);
            var six = sixZero.First(x => x != zero);
            var middleNumber = eight.Except(zero).First();
            var topLeft = four.Except(one).First(x => x != middleNumber);
            var five = twoThreeFive.First(x => x.Contains(topLeft));
            var twoThree = twoThreeFive.Where(x => x != five).ToArray();
            var two = twoThree.First(x => x.Contains(bottomLeft));
            var three = twoThree.First(x => x != two);

            AddToNumberLookUp(zero, 0);
            AddToNumberLookUp(one, 1);
            AddToNumberLookUp(two, 2);
            AddToNumberLookUp(three, 3);
            AddToNumberLookUp(four, 4);
            AddToNumberLookUp(five, 5);
            AddToNumberLookUp(six, 6);
            AddToNumberLookUp(seven, 7);
            AddToNumberLookUp(eight, 8);
            AddToNumberLookUp(nine, 9);
        }

        private void AddToNumberLookUp(char[] array, int number) {
            Array.Sort(array);
            _numberLookUp.Add(array, number);
        }

        private char[] GetAnyObjectWithCount(int count) {
            return _segmentInput.Where(t => t.Length == count).FirstOrDefault(Array.Empty<char>());
        }
        
        private IEnumerable<char[]> GetAllObjectsWithCount(int count) {
            return _segmentInput.Where(t => t.Length == count);
        }

        private static char[] FirstArrayThatFullyContains(IEnumerable<char[]> numbers, char[] contain) {
            return numbers.First(x => contain.All(x.Contains));
        }

        public int DigitStringsToValue(IReadOnlyList<string> digitStrings) {
            var totalValue = 0;
            for (int i = 0; i < digitStrings.Count; i++) {
                var digit = digitStrings[i];
                var digitCharArray = digit.ToCharArray();
                var value = LookUp(digitCharArray);
                
                totalValue += value * (int)Math.Pow(10, digitStrings.Count - 1 - i);
            }

            return totalValue;
        }
        
        private class ArrayInnerEqualityComparer : IEqualityComparer<char[]> {
            public bool Equals(char[] x, char[] y) {
                if (ReferenceEquals(x, y)) {
                    return true;
                }
                if (x is null || y is null) {
                    return false;
                }
            
                if (x.Length != y.Length) {
                    return false;
                }

                return !x.Where((t, i) => t != y[i]).Any();
            }

            public int GetHashCode(char[] obj) {
                int result = 17;
                foreach (var t in obj) {
                    unchecked
                    {
                        result = result * 23 + t;
                    }
                }
                return result;
            }
        }
    }
}