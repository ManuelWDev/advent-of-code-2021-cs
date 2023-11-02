namespace AdventOfCode; 

public class Day21 : BaseDay {
    private readonly string _input;

    public Day21() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var dice = new Deterministic100Dice();
        var (player1, player2) = ParseInput(_input);
        const int minWinScore = 1000;
        
        Player loosingPlayer;
        while (true) {
            Move(ref player1, dice);
            if (player1.Score >= minWinScore) {
                loosingPlayer = player2;
                break;
            }
            Move(ref player2, dice);
            if (player2.Score >= minWinScore) {
                loosingPlayer = player1;
                break;
            }
        }

        var result = dice.RollCount * loosingPlayer.Score;
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {result}");
    }

    public override ValueTask<string> Solve_2() {
        var (player1, player2) = ParseInput(_input);
        
        GetDiracDiceWins(player1, player2, out var player1Wins, out var player2Wins);
        var mostWins = player1Wins > player2Wins ? player1Wins : player2Wins;
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {mostWins}");
    }
    
    private static (Player, Player) ParseInput(string input) {
        using var reader = new StringReader(input);
        
        var player1 = ParsePlayer();
        var player2 = ParsePlayer();
        return (player1, player2);

        Player ParsePlayer() {
            var startPosition = int.Parse(reader.ReadLine().Split(":", StringSplitOptions.TrimEntries)[1]);
            return new Player(startPosition);
        }
    }

    private static void Move(ref Player player, Deterministic100Dice dice) {
        var movement = dice.Roll() + dice.Roll() + dice.Roll();
        MoveWithDiceRoll(ref player, movement);
    }

    private static void MoveWithDiceRoll(ref Player player, int roll) {
        const int boardSize = 10;
        var position = player.PositionIndex;
        var newPosition = (position + roll) % boardSize;
        
        player.ApplyMovement(newPosition, newPosition + 1);
    }

    private static readonly (int, ulong)[] RollFrequencies = { (3, 1), (4, 3), (5, 6), (6, 7), (7, 6), (8, 3), (9, 1) };
    
    // adapted version of https://gist.github.com/joshbduncan/5d7c64821111be5c7456b6f2cfc262a9
    private static void GetDiracDiceWins(Player player1, Player player2, out ulong player1Wins, out ulong player2Wins) {
        const int minDiracDiceWinScore = 21;
        player1Wins = 0;
        player2Wins = 0;
        
        foreach (var (roll, frequency) in RollFrequencies) {
            var player1Copy = player1;
            MoveWithDiceRoll(ref player1Copy, roll);
            if (player1Copy.Score >= minDiracDiceWinScore) {
                player1Wins += frequency;
            }
            else {
                GetDiracDiceWins(player2, player1Copy, out var p2Wins, out var p1Wins);
                player1Wins += p1Wins * frequency;
                player2Wins += p2Wins * frequency;
            }
        }
    }
    
    private class Deterministic100Dice {
        private int _index = 1;
        public int RollCount { get; private set; }
        
        public int Roll() {
            var result = _index;
            RollCount++;
            _index++;
            if (_index > 100)
                _index = 1;
            return result;
        }
    }

    private struct Player {
        public int Score { get; private set; }
        public int PositionIndex { get; private set; }

        public Player(int position) {
            PositionIndex = position - 1;
        }

        public void ApplyMovement(int newPosition, int score) {
            PositionIndex = newPosition;
            Score += score;
        }
    }
}