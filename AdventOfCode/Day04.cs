namespace AdventOfCode; 

public class Day04 : BaseDay {
    
    private readonly string _input;
    private const int BoardSize = 5;

    public Day04() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var (randomNumbers, bingoBoards) = ParseInput();
        var score = 0;
        foreach (var number in randomNumbers) {
            foreach (var bingoBoard in bingoBoards) {
                if (bingoBoard.TryWin(number, out score)) {
                    goto end;
                }
            }
        }
        end:
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {score}");
    }

    public override ValueTask<string> Solve_2() {
        var (randomNumbers, bingoBoards) = ParseInput();
        var score = 0;
        foreach (var randomNumber in randomNumbers) {
            for (int i = bingoBoards.Count - 1; i >= 0; i--) {
                if (bingoBoards[i].TryWin(randomNumber, out score)) {
                    if (bingoBoards.Count == 1)
                        goto end;
                    bingoBoards.RemoveAt(i);
                }
            }
        }
        end:
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {score}");
    }

    private (int[], List<BingoBoard>) ParseInput() {
        var bingoBoards = new List<BingoBoard>();
        using var reader = new StringReader(_input);

        
        var randomNumberLine = reader.ReadLine();
        
        var currentBoard = new BingoBoard();
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            if (string.IsNullOrEmpty(line)) {
                currentBoard = new BingoBoard();
                bingoBoards.Add(currentBoard);
                continue;
            }
            var numberStrings = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var row = new BingoNumber[BoardSize];

            for (int i = 0; i < BoardSize; i++) {
                row[i] = new BingoNumber(int.Parse(numberStrings[i]));
            }
            currentBoard.AddRow(row);
        }

        var randomNumbers = randomNumberLine.Split(',').Select(int.Parse).ToArray();
        return (randomNumbers, bingoBoards);
    }

    private class BingoBoard {
        private readonly Dictionary<int, (int, int)> _locationCache = new();
        private readonly BingoNumber[][] _board = new BingoNumber[5][];
        private int _addedRowIndex;
        private bool _isBingo;

        public void AddRow(BingoNumber[] row) {
            if (row.Length != BoardSize)
                throw new ArgumentException("Incorrect row size", nameof(row));
            if (_addedRowIndex >= BoardSize)
                throw new InvalidOperationException("Board is full");
            
            _board[_addedRowIndex] = row;
            UpdateLocationCache(row, _addedRowIndex);
            _addedRowIndex++;
        }

        private void UpdateLocationCache(BingoNumber[] row, int rowIndex) {
            for (int i = 0; i < row.Length; i++) {
                _locationCache[row[i].Number] = (rowIndex, i);
            }
        }

        public bool TryWin(int value, out int score) {
            score = 0;
            if (_isBingo)
                throw new Exception("Board is already won!");

            if (!_locationCache.TryGetValue(value, out var location)) {
                return false;
            }
            
            var (i, j) = location;
            _board[i][j].IsMarked = true;

            _isBingo = IsRowBingo(i) || IsColumnBingo(j);
            if (_isBingo) {
                score = CalculateScore(value);
            }

            return _isBingo;
        }

        private bool IsRowBingo(int rowIndex) {
            var row = _board[rowIndex];
            for (int i = 0; i < BoardSize; i++) {
                if (!row[i].IsMarked)
                    return false;
            }

            return true;
        }

        private bool IsColumnBingo(int column) {
            for (int i = 0; i < BoardSize; i++) {
                if (!_board[i][column].IsMarked)
                    return false;
            }

            return true;
        }

        private int CalculateScore(int winningValue) {
            return SumUnmarkedNumbers() * winningValue;
        }

        private int SumUnmarkedNumbers() {
            var sum = 0;
            for (int i = 0; i < BoardSize; i++) {
                for (int j = 0; j < BoardSize; j++) {
                    if (!_board[i][j].IsMarked)
                        sum += _board[i][j].Number;
                }
            }

            return sum;
        }
    }
    
    private struct BingoNumber {
        public readonly int Number;
        public bool IsMarked;

        public BingoNumber(int number) {
            Number = number;
        }
    }
}