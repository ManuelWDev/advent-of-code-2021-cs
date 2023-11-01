namespace AdventOfCode; 

public class Day20 : BaseDay {
    private readonly string _input;

    public Day20() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var (enhancer, inputImage) = ReadInput(_input);
        var enhancedImage = enhancer.EnhanceImage(inputImage);
        enhancedImage = enhancer.EnhanceImage(enhancedImage);

        var litCount = enhancedImage.GetLitPixelsCount();
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {litCount}");
    }

    public override ValueTask<string> Solve_2() {
        var (enhancer, inputImage) = ReadInput(_input);
        for (int i = 0; i < 50; i++) {
            inputImage = enhancer.EnhanceImage(inputImage);
        }

        var litCount = inputImage.GetLitPixelsCount();
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {litCount}");
    }
    
    private static (ImageEnhancer enhancer, Image image) ReadInput(string input) {
        using var reader = new StringReader(input);

        var enhancer = new ImageEnhancer();
        for (var line = reader.ReadLine(); !string.IsNullOrEmpty(line); line = reader.ReadLine()) {
            enhancer.AddEnhancerLine(line.Select(c => c == '#' ? 1 : 0).ToArray());
        }
        
        var inputImage = new List<int[]>();
        for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            inputImage.Add(line.Select(c => c == '#' ? 1 : 0).ToArray());
        }

        return (enhancer, new Image(inputImage.ToArray()));
    }

    private class ImageEnhancer {
        private readonly List<int> _enhancer = new();

        public void AddEnhancerLine(IEnumerable<int> line) {
            _enhancer.AddRange(line);
        }

        public Image EnhanceImage(Image image) {
            var newImageSize = image.Size + 2;
            var newImage = new int[newImageSize][];
            for (int y = 0; y < newImageSize; y++) {
                var row = new int[newImageSize];
                newImage[y] = row;
                for (int x = 0; x < newImageSize; x++) {
                    var enhancerIndex = GetEnhancerIndex(image, x - 1, y - 1);
                    row[x] = _enhancer[enhancerIndex];
                }
            }

            var newOutsideValue = image.OuterValue == 0 ? _enhancer[0] : _enhancer[511];
            return new Image(newImage, newOutsideValue);
        }
        
        private static int GetEnhancerIndex(Image input, int x, int y) {
            var index = 0;
            index += input.Get(x - 1, y - 1) << 8;
            index += input.Get(x, y - 1) << 7;
            index += input.Get(x + 1, y - 1) << 6;
            index += input.Get(x - 1, y) << 5;
            index += input.Get(x, y) << 4;
            index += input.Get(x + 1, y) << 3;
            index += input.Get(x - 1, y + 1) << 2;
            index += input.Get(x, y + 1) << 1;
            index += input.Get(x + 1, y + 1);
            return index;
        }
    }

    private class Image {
        private readonly int[][] _data;
        public int OuterValue { get; }
        public int Size => _data.Length;
        
        public Image(int[][] data, int outerValue = 0) {
            _data = data;
            OuterValue = outerValue;
        }
        
        public int Get(int x, int y) {
            if (x < 0 || y < 0 || x >= _data.Length || y >= _data.Length)
                return OuterValue;
            return _data[y][x];
        }

        public int GetLitPixelsCount() {
            if (OuterValue == 1)
                throw new InvalidOperationException("Count would be infinite with OuterValue of 1");
            var count = 0;
            foreach (var row in _data) {
                foreach (var pixel in row) {
                    count += pixel;
                }
            }

            return count;
        }
    }
}