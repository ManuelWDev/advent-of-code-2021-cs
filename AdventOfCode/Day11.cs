namespace AdventOfCode; 

public class Day11 : BaseDay {
    private readonly string _input;

    public Day11() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var rows = ReadInput();

        var totalCount = 0;
        const int stepCount = 100;
        for (int s = 0; s < stepCount; s++) {
            totalCount += GetCountOfSimulatedFlashStep(rows);
        }
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {totalCount}");
    }
    
    public override ValueTask<string> Solve_2() {
        var rows = ReadInput();

        var stepCount = 0;
        var lastCount = int.MinValue;
        var fieldCount = rows.Count * rows[0].Length;
        
        // check for all lights flashing at the same step
        while (lastCount != fieldCount) {
            lastCount = GetCountOfSimulatedFlashStep(rows);
            stepCount++;
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {stepCount}");
    }

    private List<int[]> ReadInput() {
        using var reader = new StringReader(_input);
        var rows = new List<int[]>();
        
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var row = new int[line.Length];
            for (var index = 0; index < line.Length; index++) {
                var character = line[index];
                row[index] = character - '0';
            }
            rows.Add(row);
        }

        return rows;
    }

    private static int GetCountOfSimulatedFlashStep(List<int[]> rows) {
        var count = 0;
        for (int i = 0; i < rows.Count; i++) {
            for (int j = 0; j < rows.Count; j++) {
                count += FlashAndCount(rows, i, j);
            }
        }
        ResetFlashes(rows);
        return count;
    }

    private static int FlashAndCount(IReadOnlyList<int[]> rows, int i, int j) {
        var count = 0;
        if (++rows[i][j] == 10) {
            // this light is flashing
            count++;
            
            var i2Max = Math.Min(i + 1, rows.Count - 1);
            var j2Max = Math.Min(j + 1, rows[i].Length - 1);
            
            // increase light of all neighbours
            for (var i2 = Math.Max(i - 1, 0); i2 <= i2Max; i2++) {
                for (var j2 = Math.Max(j - 1, 0); j2 <= j2Max; j2++) {
                    if (i == i2 && j == j2)
                        continue;
                    count += FlashAndCount(rows, i2, j2);
                }
            }
        }

        return count;
    }

    private static void ResetFlashes(List<int[]> rows) {
        foreach (var row in rows) {
            for (var i = 0; i < row.Length; i++) {
                if (row[i] >= 10) {
                    row[i] = 0;
                }
            }
        }
    }
}