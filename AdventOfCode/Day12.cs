namespace AdventOfCode; 

public class Day12 : BaseDay {
    private readonly string _input;

    public Day12() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var caveSystem = CreateCaveSystem();
        var count = caveSystem.CountPaths();
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {count}");
    }

    public override ValueTask<string> Solve_2() {
        var caveSystem = CreateCaveSystem();
        var count = caveSystem.CountPathsOnceSmallDoubleVisit();
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {count}");
    }

    private CaveSystem CreateCaveSystem() {
        using var reader = new StringReader(_input);

        var caveSystem = new CaveSystem();
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var caves = line.Split('-');
            caveSystem.AddConnection(caves[0], caves[1]);
        }

        return caveSystem;
    }

    private class CaveSystem {
        private readonly Dictionary<string, Cave> _caves = new();
        private Cave _start, _end;
        
        public void AddConnection(string cave1, string cave2) {
            var cave1Instance = GetOrAddCave(cave1);
            var cave2Instance = GetOrAddCave(cave2);
            
            cave1Instance.AddNeighbour(cave2Instance);
            cave2Instance.AddNeighbour(cave1Instance);
        }

        private Cave GetOrAddCave(string name) {
            if (_caves.TryGetValue(name, out var cave)) {
                return cave;
            }
            
            return AddCave(name);
        }

        private Cave AddCave(string name) {
            var isBig = name.All(char.IsUpper);
            var cave = new Cave(isBig);
            _caves.Add(name, cave);
            
            if (name == "start") {
                _start = cave;
            } else if (name == "end") {
                _end = cave;
            }

            return cave;
        }

        public int CountPaths() {
            _start.IsVisited = true;
            return CountPathsRec(_start);
        }

        private int CountPathsRec(Cave from) {
            if (from == _end) {
                return 1;
            }
            
            var count = 0;
            foreach (var neighbour in from.Neighbours) {
                if (neighbour.IsVisited && !neighbour.IsBig) continue;
                
                neighbour.IsVisited = true;
                count += CountPathsRec(neighbour);
                neighbour.IsVisited = false;
            }

            return count;
        }

        public int CountPathsOnceSmallDoubleVisit() {
            _start.IsVisited = true;
            return CountPathsOnceSmallDoubleVisitRec(_start, false);
        }

        private int CountPathsOnceSmallDoubleVisitRec(Cave from, bool alreadyDoubleVisit) {
            if (from == _end) {
                return 1;
            }
            
            var count = 0;
            foreach (var neighbour in from.Neighbours) {
                if (neighbour.IsVisited && !neighbour.IsBig) {
                    if (alreadyDoubleVisit || neighbour == _start) continue;

                    count += CountPathsOnceSmallDoubleVisitRec(neighbour, true);
                    continue;
                }
                
                neighbour.IsVisited = true;
                count += CountPathsOnceSmallDoubleVisitRec(neighbour, alreadyDoubleVisit);
                neighbour.IsVisited = false;
            }

            return count;
        }
    }

    private class Cave {
        private readonly List<Cave> _neighbours = new();
        public IEnumerable<Cave> Neighbours => _neighbours;
        public bool IsBig { get; }
        public bool IsVisited { get; set; }

        public Cave(bool isBig) {
            IsBig = isBig;
        }

        public void AddNeighbour(Cave neighbour) {
            _neighbours.Add(neighbour);
        }
    }
}