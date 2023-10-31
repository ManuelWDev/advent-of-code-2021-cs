namespace AdventOfCode; 

public class Day19 : BaseDay {
    private readonly string _input;

    public Day19() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var scanners = ReadInput(_input);
        
        FixScannerCoordinateSystem(scanners);
        
        var allPoints = new HashSet<Vector>();
        foreach (var scanner in scanners) {
            foreach (var beacon in scanner.GetOffsetBeacons()) {
                allPoints.Add(beacon);
            }
        }
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {allPoints.Count}");
    }

    public override ValueTask<string> Solve_2() {
        var scanners = ReadInput(_input);
        
        FixScannerCoordinateSystem(scanners);
        
        var distance = CalculateMaxManhattanDistance(scanners);
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {distance}");
    }

    private static List<Scanner> ReadInput(string input) {
        using var reader = new StringReader(input);
        
        var scanners = new List<Scanner>();
        for (var header = reader.ReadLine(); header != null; header = reader.ReadLine()) {
            var scanner = new Scanner();
            for (var line = reader.ReadLine(); !string.IsNullOrEmpty(line); line = reader.ReadLine()) {
                scanner.AddBeacon(Vector.FromString(line));
            }
            scanners.Add(scanner);
        }

        return scanners;
    }
    
    private static void FixScannerCoordinateSystem(IReadOnlyList<Scanner> scanners) {
        var solvedScanners = new List<Scanner>();
        var unsolvedScanners = new List<Scanner>();
        unsolvedScanners.AddRange(scanners);
        
        var firstScanner = scanners[0];
        firstScanner.ApplyOffset(new Offset(Vector.Zero, Vector.Zero));
        solvedScanners.Add(firstScanner);
        unsolvedScanners.Remove(firstScanner);

        for (int i = 0; i < solvedScanners.Count && unsolvedScanners.Count > 0; i++) {
            var solvedScanner = solvedScanners[i];
            for (int j = unsolvedScanners.Count - 1; j >= 0; j--) {
                var other = unsolvedScanners[j];
                if (solvedScanner.TryFixOtherCoordinateSystem(other)) {
                    solvedScanners.Add(other);
                    unsolvedScanners.RemoveAt(j);
                }
            }
        }
    }

    private static long CalculateMaxManhattanDistance(List<Scanner> scanners) {
        var maxManhattanDistance = 0L;
        foreach (var scanner1 in scanners) {
            foreach (var scanner2 in scanners) {
                if (scanner1 == scanner2)
                    continue;
                var manhattanDistance = CalculateManhattanDistance(scanner1, scanner2);
                if (manhattanDistance > maxManhattanDistance)
                    maxManhattanDistance = manhattanDistance;
            }
        }
        return maxManhattanDistance;
    }

    private static long CalculateManhattanDistance(Scanner scanner1, Scanner scanner2) {
        var scanner1Position = scanner1.ScannerPosition;
        var scanner2Position = scanner2.ScannerPosition;
        return Math.Max(scanner1Position.X, scanner2Position.X) - Math.Min(scanner1Position.X, scanner2Position.X) +
               Math.Max(scanner1Position.Y, scanner2Position.Y) - Math.Min(scanner1Position.Y, scanner2Position.Y) +
               Math.Max(scanner1Position.Z, scanner2Position.Z) - Math.Min(scanner1Position.Z, scanner2Position.Z);
    }

    private readonly struct Vector : IEquatable<Vector> {
        public readonly long X;
        public readonly long Y;
        public readonly long Z;

        public Vector(long x, long y, long z) {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector Zero => new(0, 0, 0);
        public static Vector operator +(Vector a, Vector b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector operator -(Vector a, Vector b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static bool operator ==(Vector lhs, Vector rhs) => lhs.Equals(rhs);
        public static bool operator !=(Vector lhs, Vector rhs) => !(lhs == rhs);

        public Vector ApplyRotation(Vector rotation) {
            var xRotated = RotateAroundX(this, rotation.X);
            var yRotated = RotateAroundY(xRotated, rotation.Y);
            return RotateAroundZ(yRotated, rotation.Z);
        }

        private static Vector RotateAroundX(Vector vector, long rotation) {
            if (rotation == 0)
                return vector;
            var radians = rotation * Math.PI / 180;
            var cos = (int)Math.Cos(radians);
            var sin = (int)Math.Sin(radians);
            var y = vector.Y * cos - vector.Z * sin;
            var z = vector.Y * sin + vector.Z * cos;
            return new Vector(vector.X, y, z);
        }

        private static Vector RotateAroundY(Vector vector, long rotation) {
            if (rotation == 0)
                return vector;
            var radians = rotation * Math.PI / 180;
            var cos = (int)Math.Cos(radians);
            var sin = (int)Math.Sin(radians);
            var x = vector.X * cos + vector.Z * sin;
            var z = -vector.X * sin + vector.Z * cos;
            return new Vector(x, vector.Y, z);
        }

        private static Vector RotateAroundZ(Vector vector, long rotation) {
            if (rotation == 0)
                return vector;
            var radians = rotation * Math.PI / 180;
            var cos = (int)Math.Cos(radians);
            var sin = (int)Math.Sin(radians);
            var x = vector.X * cos - vector.Y * sin;
            var y = vector.X * sin + vector.Y * cos;
            return new Vector(x, y, vector.Z);
        }

        public static ulong SquaredDistance(Vector a, Vector b) {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            var dz = a.Z - b.Z;
            return (ulong)(dx * dx) + (ulong)(dy * dy) + (ulong)(dz * dz);
        }
        
        public static Vector FromString(string s) {
            var parts = s.Split(',');
            return new Vector(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }
        
        public override string ToString() {
            return $"({X}, {Y}, {Z})";
        }

        public bool Equals(Vector other) {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj) {
            return obj is Vector other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(X, Y, Z);
        }
    }
    
    private readonly struct Pair<T> where T : struct {
        public readonly T Value1;
        public readonly T Value2;

        public Pair(T value1, T value2) {
            Value1 = value1;
            Value2 = value2;
        }
        
        public override string ToString() {
            return $"[{Value1}; {Value2}]";
        }
    }
    
    private readonly struct Offset {
        public readonly Vector Translation;
        public readonly Vector Rotation;
        
        public Offset(Vector translation, Vector rotation) {
            Translation = translation;
            Rotation = rotation;
        }
        
        public static Offset operator +(Offset offset, Offset other) {
            return new Offset(offset.Translation.ApplyRotation(offset.Rotation) + other.Translation, other.Rotation);
        }
    }

    private class Scanner {
        private List<Vector> _beacons = new();
        private readonly Dictionary<ulong, Pair<int>> _distanceToVectorIndexes = new();
        public Vector ScannerPosition { get; private set; } = Vector.Zero;
        private bool AlreadyOffset { get; set; }
        
        private static readonly Vector[] PossibleRotations = {
            new(0, 0, 0), new(90, 0, 0), new(180, 0, 0), new(270, 0, 0),
            new(0, 90, 0), new(90, 90, 0), new(180, 90, 0), new(270, 90, 0),
            new(0, 180, 0), new(90, 180, 0), new(180, 180, 0), new(270, 180, 0),
            new(0, 270, 0), new(90, 270, 0), new(180, 270, 0), new(270, 270, 0),
            new(0, 0, 90), new(90, 0, 90), new(180, 0, 90), new(270, 0, 90),
            new(0, 0, 270), new(90, 0, 270), new(180, 0, 270), new(270, 0, 270)
        };
        
        public void AddBeacon(Vector position) {
            UpdateDistances(position, _beacons.Count);
            _beacons.Add(position);
        }

        private void UpdateDistances(Vector forPosition, int newBeaconIndex) {
            // the code assumes that distances are unique in a scanner
            for (var index2 = 0; index2 < _beacons.Count; index2++) {
                var beacon = _beacons[index2];
                var distance = Vector.SquaredDistance(beacon, forPosition);
                _distanceToVectorIndexes[distance] = new Pair<int>(index2, newBeaconIndex);
            }
        }

        public void ApplyOffset(Offset offset) {
            if (offset.Rotation == Vector.Zero && offset.Translation == Vector.Zero) {
                AlreadyOffset = true;
                return;
            }
            var newBeacons = new List<Vector>();
            foreach (var beacon in _beacons) {
                newBeacons.Add(beacon.ApplyRotation(offset.Rotation) + offset.Translation);
            }

            ScannerPosition = ScannerPosition.ApplyRotation(offset.Rotation) + offset.Translation;

            _beacons = newBeacons;
            AlreadyOffset = true;
        }

        public IEnumerable<Vector> GetOffsetBeacons() {
            if (!AlreadyOffset)
                throw new InvalidOperationException("Offset has not been calculated yet");
            return _beacons;
        }

        public bool TryFixOtherCoordinateSystem(Scanner other) {
            if (other.AlreadyOffset)
                return true;
            
            if (!AlreadyOffset)
                throw new InvalidOperationException("Offset has not been calculated yet");
            
            var correspondingPairs = new List<Pair<Pair<Vector>>>();

            foreach (var (localDistance, localIndex) in _distanceToVectorIndexes) {
                foreach (var (remoteDistance, remoteIndex) in other._distanceToVectorIndexes) {
                    if (localDistance == remoteDistance) {
                        var localBeaconPair = new Pair<Vector>(_beacons[localIndex.Value1], _beacons[localIndex.Value2]);
                        var remoteBeaconPair = new Pair<Vector>(other._beacons[remoteIndex.Value1], other._beacons[remoteIndex.Value2]);
                        correspondingPairs.Add(new Pair<Pair<Vector>>(localBeaconPair, remoteBeaconPair));
                    }
                }
            }
            
            // instructions says that we want 12 corresponding beacons (11 + 10 + 9 + ... = 66)
            if (correspondingPairs.Count < 66) {
                return false;
            }

            var offset = CalculateOffsetFromBeacons(correspondingPairs);
            other.ApplyOffset(offset);
            
            return true;
        }
        
        private static Offset CalculateOffsetFromBeacons(IReadOnlyList<Pair<Pair<Vector>>> correspondingPairs) {
            foreach (var rotation in PossibleRotations) {
                var pair1 = correspondingPairs[0];
                var basePosition = pair1.Value1.Value1;
                var baseTarget = pair1.Value1.Value2;
                
                var rotated1 = pair1.Value2.Value1.ApplyRotation(rotation);
                var rotated2 = pair1.Value2.Value2.ApplyRotation(rotation);

                var baseVector = baseTarget - basePosition;

                var rotatedVector1 = rotated2 - rotated1;
                var rotatedVector2 = rotated1 - rotated2;

                if (rotatedVector1 == baseVector) {
                    var translation = basePosition - rotated1;
                    return new Offset(translation, rotation);
                    
                }
                else if (rotatedVector2 == baseVector) {
                    var translation = basePosition - rotated2;
                    return new Offset(translation, rotation);
                }
            }
            return new Offset(Vector.Zero, Vector.Zero);
        }
    }
}