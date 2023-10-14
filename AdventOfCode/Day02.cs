namespace AdventOfCode; 

public class Day02 : BaseDay {
    
    private readonly string _input;

    public Day02()
    {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        int horizontal = 0, depth = 0;
        using var reader = new StringReader(_input);

        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var parts = line.Split(' ');
            var amount = int.Parse(parts[1]);
            switch (parts[0]) {
                case "forward":
                    horizontal += amount;
                    break;
                case "down":
                    depth += amount;
                    break;
                case "up":
                    depth -= amount;
                    break;
            }
        }

        var result = horizontal * depth;

        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {result}");
    }

    public override ValueTask<string> Solve_2() {
        int horizontal = 0, depth = 0, aim = 0;
        using var reader = new StringReader(_input);

        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var parts = line.Split(' ');
            var amount = int.Parse(parts[1]);
            switch (parts[0]) {
                case "forward":
                    horizontal += amount;
                    depth += aim * amount;
                    break;
                case "down":
                    aim += amount;
                    break;
                case "up":
                    aim -= amount;
                    break;
            }
        }

        var result = horizontal * depth;
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {result}");
    }
}