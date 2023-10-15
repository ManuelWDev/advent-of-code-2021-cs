namespace AdventOfCode; 

public class Day03 : BaseDay {
    private const int BitLength = 12;
    private readonly string[] _lines;

    public Day03()
    {
        var input = File.ReadAllText(InputFilePath);
        _lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    }
    
    public override ValueTask<string> Solve_1() {
        var (gammaRate, epsilonRate) = CalculateMostCommonAndLeastCommonBits(_lines);
        
        var result = gammaRate * epsilonRate;
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {result}");
    }

    public override ValueTask<string> Solve_2() {
        var mostCommon = FilterStringMostCommonBits(_lines, GetMostCommonBitAtPosition);
        var leastCommon = FilterStringMostCommonBits(_lines, (strings, i) => GetMostCommonBitAtPosition(strings, i) ^ 1);

        var oxygenGeneratorRating = BitStringToInt(mostCommon);
        var co2ScrubbingRate = BitStringToInt(leastCommon);
        
        var result = oxygenGeneratorRating * co2ScrubbingRate;
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {result}");
    }

    private static string FilterStringMostCommonBits(IEnumerable<string> lines, Func<IReadOnlyCollection<string>, int, int> filterBitGetter) {
        var filterList = new List<string>(lines);

        for (int i = 0; i < BitLength; i++) {
            if (filterList.Count == 1)
                break;
            
            var targetBit = filterBitGetter(filterList, i);

            for (int j = filterList.Count - 1; j >= 0; j--) {
                var equals = filterList[j][i] == '1' ^ targetBit == 0;
                if (!equals) {
                    filterList.RemoveAt(j);
                    if (filterList.Count == 1)
                        break;
                }
            }
        }
        
        return filterList[0];
    }

    private static (int, int) CalculateMostCommonAndLeastCommonBits(IReadOnlyCollection<string> lines) {
        var mostCommon = 0;
        var leastCommon = 0;
        for (int i = 0; i < BitLength; i++) {
            var mostCommonBit = GetMostCommonBitAtPosition(lines, i);
            
            // BitLength - 1 - i to reverse the order of the bits
            if (mostCommonBit == 1) {
                mostCommon |= 1 << (BitLength - 1 - i);
            }
            else {
                leastCommon |= 1 << (BitLength - 1 - i);
            }
        }

        return (mostCommon, leastCommon);
    }
    
    private static int GetMostCommonBitAtPosition(IReadOnlyCollection<string> lines, int position) {
        var zeroCount = 0;
        foreach (var line in lines) {
            var character = line[position];
            if (character == '0') {
                zeroCount++;
            }
        }

        var zeroRate = (double)zeroCount / lines.Count;
        return zeroRate > 0.5 ? 0 : 1;
    }
    
    private static int BitStringToInt(string bitString) {
        var result = 0;
        for (int i = 0; i < bitString.Length; i++) {
            var bit = bitString[i];
            if (bit == '1') {
                result |= 1 << (bitString.Length - 1 - i);
            }
        }

        return result;
    }
}