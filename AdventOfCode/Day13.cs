using System.Text;

namespace AdventOfCode; 

public class Day13 : BaseDay {
    private readonly string _input;

    public Day13() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var (transparentPaper, foldInstructions) = ParseInput();
        
        transparentPaper.Fold(foldInstructions[0]);
        var dotCount = transparentPaper.DotCount;
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {dotCount}");
    }

    public override ValueTask<string> Solve_2() {
        var (transparentPaper, foldInstructions) = ParseInput();

        foreach (var foldInstruction in foldInstructions) {
            transparentPaper.Fold(foldInstruction);
        }

        var output = transparentPaper.ToTextBoard();
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2:\n{output}");
    }

    private (TransparentPaper, List<FoldInstruction>) ParseInput() {
        using var reader = new StringReader(_input);
        
        var transparentPaper = new TransparentPaper();
        for (var line = reader.ReadLine(); !string.IsNullOrWhiteSpace(line); line = reader.ReadLine()) {
            var split = line.Split(',');
            var x = int.Parse(split[0]);
            var y = int.Parse(split[1]);
            transparentPaper.AddDot(x, y);
        }
        
        var foldInstructions = new List<FoldInstruction>();
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var location = int.Parse(line[13..]);
            foldInstructions.Add(line[11] == 'x' ? new XFoldInstruction(location) : new YFoldInstruction(location));
        }

        return (transparentPaper, foldInstructions);
    }

    private class TransparentPaper {
        private readonly HashSet<Dot> _dots = new();
        
        public void AddDot(int x, int y) {
            _dots.Add(new Dot(x, y));
        }

        public void Fold(FoldInstruction foldInstruction) {
            List<Dot> dotsToRemove = new();
            List<Dot> dotsToAdd = new();
            
            foreach (var dot in _dots) {
                if (!foldInstruction.ShouldFold(dot)) continue;
                
                dotsToRemove.Add(dot);
                dotsToAdd.Add(foldInstruction.ToFoldedDot(dot));
            }
            
            _dots.ExceptWith(dotsToRemove);
            _dots.UnionWith(dotsToAdd);
        }
        
        public int DotCount => _dots.Count;

        public string ToTextBoard() {
            var maxX = _dots.Max(x => x.X);
            var maxY = _dots.Max(x => x.Y);
            
            var sb = new StringBuilder();
            for (var y = 0; y <= maxY; y++) {
                for (var x = 0; x <= maxX; x++) {
                    sb.Append(_dots.Contains(new Dot(x, y)) ? '#' : '.');
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
    
    private readonly struct Dot {
        public readonly int X;
        public readonly int Y;
            
        public Dot(int x, int y) {
            X = x;
            Y = y;
        }
    }
    
    private abstract class FoldInstruction {
        protected readonly int Location;

        protected FoldInstruction(int location) {
            Location = location;
        }
        
        public abstract bool ShouldFold(Dot dot);
        public abstract Dot ToFoldedDot(Dot dot);
    }
    
    private class XFoldInstruction : FoldInstruction {
        public XFoldInstruction(int location) : base(location) { }
        public override bool ShouldFold(Dot dot) => dot.X > Location;
        public override Dot ToFoldedDot(Dot dot) => new(Location - (dot.X - Location), dot.Y);
    }
    
    private class YFoldInstruction : FoldInstruction {
        public YFoldInstruction(int location) : base(location) { }
        public override bool ShouldFold(Dot dot) => dot.Y > Location;
        public override Dot ToFoldedDot(Dot dot) => new(dot.X, Location - (dot.Y - Location));
    }
}