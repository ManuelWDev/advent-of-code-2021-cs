namespace AdventOfCode; 

public class Day09 : BaseDay {
    
    private readonly int[][] _input;

    public Day09() {
        var input = File.ReadAllText(InputFilePath);
        _input = ParseInput(input);
    }

    public override ValueTask<string> Solve_1() {
        var lowPointLocations = GetLowPoints(_input);
        var score = lowPointLocations.Sum(index => _input[index.Row][index.Column] + 1);
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {score}");
    }

    private static int[][] ParseInput(string input) {
        using var reader = new StringReader(input);
        var rows = new List<int[]>();

        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var numberLine = new int[line.Length];
            for (int i = 0; i < line.Length; i++) {
                numberLine[i] = line[i] - '0';
            }
            rows.Add(numberLine);
        }

        return rows.ToArray();
    }

    private static IEnumerable<Position> GetLowPoints(IReadOnlyList<int[]> rows) {
        for (int i = 0; i < rows.Count; i++) {
            var line = rows[i];
            for (int j = 0; j < line.Length; j++) {
                var value = line[j];
                if ((j + 1 == line.Length || value < line[j + 1])
                    && (j - 1 < 0 || value < line[j - 1])
                    && (i + 1 == rows.Count || value < rows[i + 1][j])
                    && (i - 1 < 0 || value < rows[i - 1][j])) {

                    yield return new Position(i, j);
                }
            }
        }
    }
    
    private readonly struct Position {
        public readonly int Row;
        public readonly int Column;

        public Position(int row, int column) {
            Row = row;
            Column = column;
        }
    }

    public override ValueTask<string> Solve_2() {
        var lowPoints = GetLowPoints(_input);
        var score = CalculateBasinSizes(lowPoints, _input).OrderDescending().Take(3).Aggregate((a, x) => a * x);
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {score}");
    }
    
    private static IEnumerable<int> CalculateBasinSizes(IEnumerable<Position> lowPointLocations, IReadOnlyList<int[]> rows) {
        var visited = GetArrayOfSameSize<bool, int>(rows);
        var queue = new Queue<Position>();
        
        foreach (var lowPoint in lowPointLocations) {
            var basinSize = 0;
            queue.Enqueue(lowPoint);
            while (queue.TryDequeue(out var position)) {
                if (visited[position.Row][position.Column]) {
                    continue;
                }

                visited[position.Row][position.Column] = true;
                basinSize++;
                foreach (var neighbour in GetNeighbours(position, rows)) {
                    if (visited[neighbour.Row][neighbour.Column]) {
                        continue;
                    }

                    var value = rows[neighbour.Row][neighbour.Column];
                    if (value != 9 && value > rows[position.Row][position.Column]) {
                        queue.Enqueue(neighbour);
                    }
                }
            }

            yield return basinSize;
        }
    }

    private static IEnumerable<Position> GetNeighbours(Position position, IReadOnlyList<int[]> rows) {
        if (position.Row > 0) {
            yield return new Position(position.Row - 1, position.Column);
        }
        if (position.Row < rows.Count - 1) {
            yield return new Position(position.Row + 1, position.Column);
        }
        if (position.Column > 0) {
            yield return new Position(position.Row, position.Column - 1);
        }
        if (position.Column < rows[position.Row].Length - 1) {
            yield return new Position(position.Row, position.Column + 1);
        }
    }

    private static T[][] GetArrayOfSameSize<T, TIn>(IReadOnlyList<TIn[]> rows) {
        var array = new T[rows.Count][];
        for (int i = 0; i < rows.Count; i++) {
            array[i] = new T[rows[i].Length];
        }

        return array;
    }
}