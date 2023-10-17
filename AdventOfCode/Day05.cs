namespace AdventOfCode; 

public class Day05 : BaseDay {
    
    private readonly string _input;

    public Day05() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var vents = ReadInputVents();

        var lineCounts = new Dictionary<Point, int>();
        foreach (var vent in vents.Where(vent => vent.IsVerticalOrHorizontal())) {
            DrawLineIntoDict(vent, lineCounts);
        }

        var atLeastTwoOverlapCount = lineCounts.Count(pair => pair.Value >= 2);
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {atLeastTwoOverlapCount}");
    }

    public override ValueTask<string> Solve_2() {
        var vents = ReadInputVents();

        var lineCounts = new Dictionary<Point, int>();
        foreach (var vent in vents) {
            DrawLineIntoDict(vent, lineCounts);
        }

        var atLeastTwoOverlapCount = lineCounts.Count(pair => pair.Value >= 2);
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {atLeastTwoOverlapCount}");
    }

    private List<Line> ReadInputVents() {
        using var reader = new StringReader(_input);

        var vents = new List<Line>();
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var pointStrings = line.Split("->", StringSplitOptions.TrimEntries);
            vents.Add(new Line(Point.FromString(pointStrings[0]), Point.FromString(pointStrings[1])));
        }

        return vents;
    }

    private static void DrawLineIntoDict(Line line, Dictionary<Point, int> dict) {
        var xDiff = line.End.X - line.Start.X;
        var yDiff = line.End.Y - line.Start.Y;
        var xSign = Math.Sign(xDiff);
        var ySign = Math.Sign(yDiff);
        var x = line.Start.X;
        var y = line.Start.Y;
        while (x != line.End.X + xSign || y != line.End.Y + ySign) {
            var point = new Point(x, y);
            if (dict.TryGetValue(point, out var value)) {
                dict[point] = value + 1;
            } else {
                dict.Add(point, 1);
            }
            x += xSign;
            y += ySign;
        }
    }
    
    private readonly struct Point {
        public readonly int X;
        public readonly int Y;

        public Point(int x, int y) {
            X = x;
            Y = y;
        }
        
        public static Point FromString(string pointString) {
            var coords = pointString.Split(',');
            return new Point(int.Parse(coords[0]), int.Parse(coords[1]));
        }
    }
    
    private class Line {
        public readonly Point Start;
        public readonly Point End;
        
        public Line(Point start, Point end) {
            Start = start;
            End = end;
        }
        
        public bool IsVerticalOrHorizontal() {
            return Start.X == End.X || Start.Y == End.Y;
        }
    }
}