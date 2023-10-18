namespace AdventOfCode; 

public class Day06 : BaseDay {
    
    private readonly string _input;

    public Day06() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        using var reader = new StringReader(_input);
        
        var fish = new List<int>();
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var fishStrings = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
            fish.AddRange(fishStrings.Select(int.Parse));
        }

        const int simulationDays = 80;

        var dayStartFishCount = fish.Count;
        for (int i = 0; i < simulationDays; i++) {
            for (int j = 0; j < dayStartFishCount; j++) {
                var fishDay = fish[j];
                if (fishDay == 0) {
                    fish[j] = 6;
                    fish.Add(8);
                }
                else {
                    fish[j] = fishDay - 1;
                }
            }

            dayStartFishCount = fish.Count;
        }
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {fish.Count}");
    }

    public override ValueTask<string> Solve_2() {
        const int differentBreedingDays = 9;
        using var reader = new StringReader(_input);
        
        var fishCounts = new long[differentBreedingDays];
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var fishStrings = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var fishString in fishStrings) {
                fishCounts[int.Parse(fishString)]++;
            }
        }

        const int simulationDays = 256;
        
        var tempFishCounts = new long[differentBreedingDays];
        for (int i = 0; i < simulationDays; i++) {
            // create new fish for zero day fish
            tempFishCounts[6] = fishCounts[0];
            tempFishCounts[8] = fishCounts[0];
            
            for (int j = 1; j < differentBreedingDays; j++) {
                tempFishCounts[j - 1] += fishCounts[j];
            }

            tempFishCounts.CopyTo(fishCounts, 0);
            Array.Fill(tempFishCounts, 0);
        }
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {fishCounts.Sum()}");
    }
}