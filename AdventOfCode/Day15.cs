namespace AdventOfCode; 

public class Day15 : BaseDay {
    private readonly string _input;

    public Day15() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var map = new Map(_input);
        var shortest = map.GetLowestRiskPathLength();
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {shortest}");
    }

    public override ValueTask<string> Solve_2() {
        var map = new Map(_input);
        map.Part2Resize();
        var shortest = map.GetLowestRiskPathLength();
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {shortest}");
    }

    private class Map {
        private Node[][] _map;

        public Map(string input) {
            ParseFromInput(input);
        }

        private void ParseFromInput(string input) {
            using var reader = new StringReader(input);
            var y = 0;
            var map = new List<Node[]>();
            for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
                var row = new Node[line.Length];
                for (int x = 0; x < line.Length; x++) {
                    row[x] = new Node(x, y, line[x] - '0');
                }
                map.Add(row);
                y++;
            }

            _map = map.ToArray();
        }

        public void Part2Resize() {
            var oldWidth = _map[0].Length;
            var oldHeight = _map.Length;
            var newHeight = oldHeight * 5;
            var newWidth = oldWidth * 5;
            var newMap = new Node[newHeight][];
            for (int i = 0; i < oldHeight; i++) {
                for (int y = 0; y < 5; y++) {
                    newMap[y * oldHeight + i] = new Node[newWidth];
                    for (int j = 0; j < oldWidth; j++) {
                        var baseRiskLevel = _map[i][j].RiskLevel;
                        for (int x = 0; x < 5; x++) {
                            var riskLevel = baseRiskLevel + x + y;
                            if (riskLevel > 9)
                                riskLevel -= 9;
                            
                            var xPosition = x * oldWidth + j;
                            var yPosition = y * oldHeight + i;
                            newMap[yPosition][xPosition] = new Node(xPosition, yPosition, riskLevel);
                        }
                    }
                }
            }
            _map = newMap;
        }

        public int GetLowestRiskPathLength() {
            var start = _map[0][0];
            start.ShortestRiskLevelPath = 0;

            var queue = new PriorityQueue<Node, int>();
            queue.Enqueue(start, 0);
            while (queue.TryDequeue(out var node, out var priority)) {
                if (priority != node.ShortestRiskLevelPath) {
                    // hack, because updating the priority is not supported
                    continue;
                }
                foreach (var neighbour in GetNeighbours(node)) {
                    var distance = node.ShortestRiskLevelPath + neighbour.RiskLevel;
                    if (distance < neighbour.ShortestRiskLevelPath) {
                        neighbour.ShortestRiskLevelPath = distance;
                        if (neighbour == _map[^1][^1]) { 
                            return neighbour.ShortestRiskLevelPath;
                        }
                        queue.Enqueue(neighbour, distance);
                    }
                }
            }
            
            var end = _map[^1][^1];
            return end.ShortestRiskLevelPath;
        }
        
        private IEnumerable<Node> GetNeighbours(Node node) {
            if (node.X > 0) {
                yield return _map[node.Y][node.X - 1];
            }
            if (node.Y > 0) {
                yield return _map[node.Y - 1][node.X];
            }
            if (node.X < _map[0].Length - 1) {
                yield return _map[node.Y][node.X + 1];
            }
            if (node.Y < _map.Length - 1) {
                yield return _map[node.Y + 1][node.X];
            }
        }
    }

    private class Node {
        public readonly int RiskLevel;
        public int ShortestRiskLevelPath = int.MaxValue;
        public readonly int X;
        public readonly int Y;

        public Node(int x, int y, int riskLevel) {
            RiskLevel = riskLevel;
            X = x;
            Y = y;
        }
    }
}