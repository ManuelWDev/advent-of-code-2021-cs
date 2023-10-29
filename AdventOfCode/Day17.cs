namespace AdventOfCode; 

public class Day17 : BaseDay {
    private readonly string _input;

    public Day17() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var targetArea = ParseInput(_input);

        var validXStepCombos = GetValidXStepCombos(targetArea);
        var startVelocities = GetPossibleStartVelocities(targetArea, validXStepCombos);
        var highestStartVelocityY = startVelocities.Max(s => s.Y);
        var highestPosition = 0;
        for (int i = highestStartVelocityY; i > 0; i--) {
            highestPosition += i;
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {highestPosition}");
    }

    public override ValueTask<string> Solve_2() {
        var targetArea = ParseInput(_input);

        var validXStepCombos = GetValidXStepCombos(targetArea);
        var startVelocities = GetPossibleStartVelocities(targetArea, validXStepCombos);

        var differentVelocities = startVelocities.Count;
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {differentVelocities}");
    }

    private static Area ParseInput(string input) {
        var parts = input.Split(',');
        var (x1, x2) = ParseRange(parts[0]);
        var (y1, y2) = ParseRange(parts[1]);
        return new Area(new Point(x1, y2), new Point(x2, y1));
    }

    private static (int, int) ParseRange(string equalsPart) {
        var parts = equalsPart.Split('=');
        var numbers = parts[1].Split("..");
        return (int.Parse(numbers[0]), int.Parse(numbers[1]));
    }

    private static HashSet<ValidXCombo> GetValidXStepCombos(Area targetArea) {
        var validStepCombos = new HashSet<ValidXCombo>();
        for (int vx = 0; vx <= targetArea.BottomRight.X; vx++) {
            var xLocation = 0;
            var currentSpeed = vx;
            for (int s = 0; xLocation <= targetArea.BottomRight.X && currentSpeed > 0; s++, xLocation += currentSpeed, currentSpeed--) {
                if (xLocation >= targetArea.TopLeft.X) {
                    validStepCombos.Add(new ValidXCombo(vx, s));
                }
            }
        }
        
        return validStepCombos;
    }

    private static HashSet<StartVelocity> GetPossibleStartVelocities(Area targetArea, HashSet<ValidXCombo> validStepCombos) {
        var startVelocities = new HashSet<StartVelocity>(); ;
        var maxVy = -targetArea.BottomRight.Y;
        foreach (var validStepCombo in validStepCombos) {
            for (int vy = -maxVy; vy <= maxVy; vy++) {
                var yLocation = 0;
                var currentSpeed = vy;
                
                for (var s = 0; yLocation >= targetArea.BottomRight.Y && (s <= validStepCombo.Steps || validStepCombo.CouldAddMoreSteps); s++, yLocation += currentSpeed, currentSpeed--) {
                    if (yLocation <= targetArea.TopLeft.Y && (s == validStepCombo.Steps || (s > validStepCombo.Steps && validStepCombo.CouldAddMoreSteps))) {
                        startVelocities.Add(new StartVelocity(validStepCombo.Velocity, vy));
                    }
                }
            }
        }
        
        return startVelocities;
    }
    
    private struct Area {
        public readonly Point TopLeft;
        public readonly Point BottomRight;

        public Area(Point topLeft, Point bottomRight) {
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }
    }

    private readonly struct Point {
        public readonly int X;
        public readonly int Y;
        
        public Point(int x, int y) {
            X = x;
            Y = y;
        }
    }
    
     private readonly struct ValidXCombo {
         public readonly int Velocity;
         public readonly int Steps;
         public bool CouldAddMoreSteps => Steps + 1 >= Velocity;
         
         public ValidXCombo(int velocity, int steps) {
             Velocity = velocity;
             Steps = steps;
         }
     }

     private readonly struct StartVelocity { 
         public readonly int X;
         public readonly int Y;

         public StartVelocity(int x, int y) {
             X = x;
             Y = y;
         }
     }
}