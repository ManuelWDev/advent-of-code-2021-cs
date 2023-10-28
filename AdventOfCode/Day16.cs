using System.Collections;
using System.Globalization;

namespace AdventOfCode; 

public class Day16 : BaseDay {
    private readonly string _input;

    public Day16() {
        _input = File.ReadAllText(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() {
        var basePacket = PacketParser.Parse(_input);
        var versionNumberSum = basePacket.GetVersionSum();
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {versionNumberSum}");
    }

    public override ValueTask<string> Solve_2() {
        var basePacket = PacketParser.Parse(_input);
        var result = basePacket.CalculateValue();
        return new ValueTask<string>($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {result}");
    }

    private static class PacketParser {
        private const int LiteralTypeId = 4;
        
        public static Packet Parse(string hexPacketList) {
            var bitArray = ConvertHexToBitArray(hexPacketList);
            return ParsePacket(bitArray, 0);
        }

        private static Packet ParsePacket(BitArray bitArray, int startOffset) {
            var totalLength = 0;
            var version = ParseLength(3);
            var typeId = ParseLength(3);

            if (typeId == LiteralTypeId) {
                // parse remaining bits as data
                var (data, length) = ParseLiteral(bitArray, startOffset + totalLength);
                totalLength += length;
                return new ValuePacket(version, typeId, totalLength, data);
            }

            // collection packet
            var packets = new List<Packet>();
            var lengthTypeId = ParseBit();
            if (lengthTypeId) {
                var subPacketCount = ParseLength(11);
                for (int i = 0; i < subPacketCount; i++) {
                    var packet = ParsePacket(bitArray, startOffset + totalLength);
                    packets.Add(packet);
                    totalLength += packet.BitLength;
                }
            }
            else {
                var subPacketLength = ParseLength(15);
                var endOffset = startOffset + totalLength + subPacketLength;
                while (startOffset + totalLength < endOffset) {
                    var packet = ParsePacket(bitArray, startOffset + totalLength);
                    packets.Add(packet);
                    totalLength += packet.BitLength;
                }
            }
            return new CollectionPacket(version, typeId, totalLength, packets);

            int ParseLength(int length) {
                var data = ParseData(bitArray, startOffset + totalLength, length);
                totalLength += length;
                return data;
            }

            bool ParseBit() {
                var data = bitArray[startOffset + totalLength];
                totalLength += 1;
                return data;
            }
        }

        private static int ParseData(BitArray bitArray, int startOffset, int length) {
            var result = 0;
            for (int i = 0; i < length; i++) {
                var offset = startOffset + i;
                if (bitArray[offset]) {
                    result += 1 << length - 1 - i;
                }
            }
            return result;
        }

        private static (long Value, int Length) ParseLiteral(BitArray bitArray, int startOffset) {
            bool isLast;
            var result = 0L;
            var length = 0;
            do {
                length += 5;
                result <<= 4;
                isLast = !bitArray[startOffset++];
                for (int i = 0; i < 4; i++) {
                    result += bitArray[startOffset++] ? 1 << 3 - i : 0;
                }
            } while (!isLast);
            return (result, length);
        }
        
        // https://stackoverflow.com/questions/4269737/function-convert-hex-string-to-bitarray-c-sharp
        private static BitArray ConvertHexToBitArray(string hexData) {
            var ba = new BitArray(4 * hexData.Length);
            for (int i = 0; i < hexData.Length; i++) {
                var b = byte.Parse(hexData[i].ToString(), NumberStyles.HexNumber);
                for (int j = 0; j < 4; j++) {
                    ba.Set(i * 4 + j, (b & (1 << (3 - j))) != 0);
                }
            }
            return ba;
        }
    }
    
    private abstract class Packet {
        protected readonly int Version;
        protected readonly int TypeId;
        public readonly int BitLength;
        
        protected Packet(int version, int typeId, int bitLength) {
            Version = version;
            TypeId = typeId;
            BitLength = bitLength;
        }

        public virtual int GetVersionSum() => Version;

        public abstract long CalculateValue();
    }

    private class ValuePacket : Packet {
        private readonly long _value;

        public ValuePacket(int version, int typeId, int bitLength, long value) : base(version, typeId, bitLength) {
            _value = value;
        }

        public override long CalculateValue() {
            return _value;
        }
    }

    private class CollectionPacket : Packet {
        private readonly Packet[] _subPackets;
        
        public CollectionPacket(int version, int typeId, int bitLength, IReadOnlyCollection<Packet> subPackets) :
            base(version, typeId, bitLength) {
            
            _subPackets = subPackets.ToArray();
        }
        
        public override int GetVersionSum() => Version + _subPackets.Sum(x => x.GetVersionSum());
        
        public override long CalculateValue() {
            Func<long, long, long> operation = TypeId switch {
                0 => (a, b) => a + b,
                1 => (a, b) => a * b,
                2 => Math.Min,
                3 => Math.Max,
                5 => (a, b) => a > b ? 1 : 0,
                6 => (a, b) => a < b ? 1 : 0,
                7 => (a, b) => a == b ? 1 : 0,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            if (IsComparison)
                return operation(_subPackets[0].CalculateValue(), _subPackets[1].CalculateValue());

            var identity = TypeId switch {
                1 => 1,
                2 => long.MaxValue,
                3 => long.MinValue,
                _ => 0
            };
            
            return _subPackets.Aggregate(identity, (current, packet) => operation(current, packet.CalculateValue()));
        }
        
        private bool IsComparison => TypeId is >= 5 and <= 7;
    }
}