namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly string _input;

    public Day01()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1() 
    {
        using var reader = new StringReader(_input);

        var increaseCount = 0;
        var lastValue = int.MaxValue;
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var currentValue = int.Parse(line);
            if (currentValue > lastValue) {
                increaseCount++;
            }

            lastValue = currentValue;
        }
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {increaseCount}");
    }

    public override ValueTask<string> Solve_2() 
    {
        using var reader = new StringReader(_input);

        var initialValues = new int[3];
        for (int i = 0; i < initialValues.Length; i++) {
            initialValues[i] = int.Parse(reader.ReadLine());
        }

        var slidingWindow = new SlidingWindow(initialValues);

        var increaseCount = 0;
        var lastSum = slidingWindow.Sum;
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            var currentValue = int.Parse(line);
            slidingWindow.SlideValueIn(currentValue);
            var currentSum = slidingWindow.Sum;
            
            if (currentSum > lastSum) {
                increaseCount++;
            }

            lastSum = currentSum;
        }
        
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {increaseCount}");
    }

    private class SlidingWindow 
    {
        private readonly int _size;
        private int _cursor;
        private readonly int[] _values;
        public int Sum { get; private set; }

        public SlidingWindow(int[] initialValues) 
        {
            _size = initialValues.Length;
            _cursor = _size - 1;
            _values = initialValues;
            Sum = initialValues.Sum();
        }

        public void SlideValueIn(int value) 
        {
            MoveCursor();
            Sum -= _values[_cursor];
            _values[_cursor] = value;
            Sum += value;
        }

        private void MoveCursor() 
        {
            _cursor = (_cursor + 1) % _size;
        }
    }
}
