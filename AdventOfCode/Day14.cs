namespace AdventOfCode; 

public class Day14 : BaseDay {
    private readonly string _input;

    public Day14() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var count = GetSolutionForStepCount(10);
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {count}");
    }

    public override ValueTask<string> Solve_2() {
        var count = GetSolutionForStepCount(40);
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {count}");
    }

    private ulong GetSolutionForStepCount(int stepCount) {
        var polymerization = ReadInput();
        
        for (int i = 0; i < stepCount; i++) {
            polymerization.SimulateStep();
        }

        return polymerization.MostCommonMinusLeastCommonCount();
    }

    private Polymerization ReadInput() {
        using var reader = new StringReader(_input);

        var polymerization = new Polymerization();
        
        polymerization.SetTemplate(reader.ReadLine().ToCharArray());
        // skip empty line
        reader.ReadLine();
        
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var elements = line.Split("->", StringSplitOptions.TrimEntries);
            polymerization.AddRule(new Pair(elements[0][0], elements[0][1]), elements[1][0]);
        }

        return polymerization;
    }

    private class Polymerization {
        private readonly Dictionary<Pair, char> _rules = new();
        private Dictionary<Pair, ulong> _pairCount = new();
        private char _first;
        private char _last;
        
        public void SetTemplate(char[] template) {
            for (int i = 0; i < template.Length - 1; i++) {
                var pair = new Pair(template[i], template[i + 1]);
                if (_pairCount.TryGetValue(pair, out var count)) {
                    _pairCount[pair] = count + 1;
                } else {
                    _pairCount.Add(pair, 1);
                }
            }

            _first = template[0];
            _last = template[^1];
        }

        public void AddRule(Pair from, char to) {
            _rules.Add(from, to);
        }

        public void SimulateStep() {
            var tempPairCount = new Dictionary<Pair, ulong>();
            foreach (var (oldPair, count) in _pairCount) {
                if (_rules.TryGetValue(oldPair, out var target)) {
                    AddToTempPairCount(new Pair(oldPair.First, target), count);
                    AddToTempPairCount(new Pair(target, oldPair.Second), count);
                }
                else {
                    AddToTempPairCount(oldPair, count);
                }
            }

            _pairCount = tempPairCount;
            return;

            void AddToTempPairCount(Pair pair, ulong count) {
                if (tempPairCount.TryGetValue(pair, out var beginCount)) {
                    tempPairCount[pair] = beginCount + count;
                } else {
                    tempPairCount.Add(pair, count);
                }
            }
        }
        
        public ulong MostCommonMinusLeastCommonCount() {
            var characterCounts = new Dictionary<char, ulong>();
            foreach (var (pair, count) in _pairCount) {
                AddToElementCounts(pair.First, count);
                AddToElementCounts(pair.Second, count);
            }
            
            // first and last are not double counted
            AddToElementCounts(_first, 1);
            AddToElementCounts(_last, 1);
            
            var mostCommonCount = ulong.MinValue;
            var leastCommonCount = ulong.MaxValue;
            foreach (var elementCount in characterCounts) {
                if (elementCount.Value > mostCommonCount) {
                    mostCommonCount = elementCount.Value;
                }
                if (elementCount.Value < leastCommonCount) {
                    leastCommonCount = elementCount.Value;
                }
            }

            return (mostCommonCount - leastCommonCount) / 2;
            
            void AddToElementCounts(char element, ulong count) {
                if (characterCounts.TryGetValue(element, out var currentCount)) {
                    characterCounts[element] = currentCount + count;
                } else {
                    characterCounts.Add(element, count);
                }
            }
        }
    }

    private readonly struct Pair {
        public readonly char First;
        public readonly char Second;

        public Pair(char first, char second) {
            First = first;
            Second = second;
        }
    }
}