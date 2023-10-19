namespace AdventOfCode; 

public class Day07 : BaseDay {
    
    private readonly int[] _positions;

    public Day07() {
        var input = File.ReadAllText(InputFilePath);
        _positions = input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        Array.Sort(_positions);
    }
    
    public override ValueTask<string> Solve_1() {
        var minDistance = CalculateLocationWithMinimumDistance(_positions, LinearCostFunction);
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {minDistance}");
        
        int LinearCostFunction(int i) => i;
    }

    public override ValueTask<string> Solve_2() {
        var minDistance = CalculateLocationWithMinimumDistance(_positions, CrabCostFunction);
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {minDistance}");
        
        int CrabCostFunction(int i) {
            var cost = 0;
            for (int j = 1; j <= i; j++) {
                cost += j;
            }

            return cost;
        }
    }
    
    private static int CalculateLocationWithMinimumDistance(int[] positions, Func<int, int> distanceCostFunction) {
        var min = positions[0];
        var max = positions[^1];
        var minDistance = GetDistance(positions, min, distanceCostFunction);

        for (int i = min; i <= max; i++) {
            var distance = GetDistance(positions, i, distanceCostFunction);
            if (distance < minDistance) {
                minDistance = distance;
            }
        }

        return minDistance;
    }
    
    private static int GetDistance(IEnumerable<int> positions, int mid, Func<int, int> costFunction) {
        return positions.Sum(position => costFunction(Math.Abs(position - mid)));
    }
}