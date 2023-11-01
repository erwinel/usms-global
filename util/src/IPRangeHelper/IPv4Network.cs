using System.Collections;
using System.Collections.Immutable;

namespace IPRangeHelper
{
    public sealed class IPv4Network : IIPv4AddressRange
    {
        public const byte MAX_BLOCK_BIT_COUNT = 32;

        public static readonly ImmutableArray<uint> MaskValues = new uint[]
        {
            0b0000_0000_0000_0000_0000_0000_1000_0000, // 1
            0b0000_0000_0000_0000_0000_0000_1100_0000, // 2
            0b0000_0000_0000_0000_0000_0000_1110_0000, // 3
            0b0000_0000_0000_0000_0000_0000_1111_0000, // 4
            0b0000_0000_0000_0000_0000_0000_1111_1000, // 5
            0b0000_0000_0000_0000_0000_0000_1111_1100, // 6
            0b0000_0000_0000_0000_0000_0000_1111_1110, // 7
            0b0000_0000_0000_0000_0000_0000_1111_1111, // 8
            0b0000_0000_0000_0000_1000_0000_1111_1111, // 9
            0b0000_0000_0000_0000_1100_0000_1111_1111, // 10
            0b0000_0000_0000_0000_1110_0000_1111_1111, // 11
            0b0000_0000_0000_0000_1111_0000_1111_1111, // 12
            0b0000_0000_0000_0000_1111_1000_1111_1111, // 13
            0b0000_0000_0000_0000_1111_1100_1111_1111, // 14
            0b0000_0000_0000_0000_1111_1110_1111_1111, // 15
            0b0000_0000_0000_0000_1111_1111_1111_1111, // 16 
            0b0000_0000_1000_0000_1111_1111_1111_1111, // 17
            0b0000_0000_1100_0000_1111_1111_1111_1111, // 18
            0b0000_0000_1110_0000_1111_1111_1111_1111, // 19
            0b0000_0000_1111_0000_1111_1111_1111_1111, // 20
            0b0000_0000_1111_1000_1111_1111_1111_1111, // 21
            0b0000_0000_1111_1100_1111_1111_1111_1111, // 22
            0b0000_0000_1111_1110_1111_1111_1111_1111, // 23
            0b0000_0000_1111_1111_1111_1111_1111_1111, // 24
            0b1000_0000_1111_1111_1111_1111_1111_1111, // 25
            0b1100_0000_1111_1111_1111_1111_1111_1111, // 26
            0b1110_0000_1111_1111_1111_1111_1111_1111, // 27
            0b1111_0000_1111_1111_1111_1111_1111_1111, // 28
            0b1111_1000_1111_1111_1111_1111_1111_1111, // 29
            0b1111_1100_1111_1111_1111_1111_1111_1111, // 30
            0b1111_1110_1111_1111_1111_1111_1111_1111, // 31
            0b1111_1111_1111_1111_1111_1111_1111_1111  // 32
        }.ToImmutableArray();

        private readonly uint _firstValue;
        private readonly uint _lastValue;

        public IPv4Address First { get; }

        public IPv4Address OriginalAddress { get; }

        public IPv4Address Last { get; }

        public byte BlockBitCount { get; }

        public uint Count => _lastValue - _firstValue + 1;

        int IReadOnlyCollection<IPv4Address>.Count
        {
            get
            {
                uint count = Count;
                return (count > int.MaxValue) ? int.MaxValue : (int)count;
            }
        }

        public IPv4Network(IPv4Network other)
        {
            First = other.First;
            Last = other.Last;
            OriginalAddress = other.OriginalAddress;
            BlockBitCount = other.BlockBitCount;
        }

        public IPv4Network(IPv4Address address, byte blockBitCount)
        {
            switch (blockBitCount)
            {
                case 0:
                    throw new ArgumentOutOfRangeException(nameof(blockBitCount));
                case MAX_BLOCK_BIT_COUNT:
                    First = Last = address;
                    break;
                default:
                    if (blockBitCount > MAX_BLOCK_BIT_COUNT)
                        throw new ArgumentOutOfRangeException(nameof(blockBitCount));
                    First = new IPv4Address(address.Address & MaskValues[blockBitCount - 1]);
                    Last = new IPv4Address(First.Address | uint.MaxValue << blockBitCount);
                    break;
            }
            BlockBitCount = blockBitCount;
            OriginalAddress = address;
        }

        public IPv4Network(uint address, byte blockBitCount)
        {
            switch (blockBitCount)
            {
                case 0:
                    throw new ArgumentOutOfRangeException(nameof(blockBitCount));
                case MAX_BLOCK_BIT_COUNT:
                    First = Last = OriginalAddress = new IPv4Address(address);
                    break;
                default:
                    if (blockBitCount > MAX_BLOCK_BIT_COUNT)
                        throw new ArgumentOutOfRangeException(nameof(blockBitCount));
                    OriginalAddress = new IPv4Address(address);
                    First = new IPv4Address(address & MaskValues[blockBitCount - 1]);
                    Last = new IPv4Address(First.Address | uint.MaxValue << blockBitCount);
                    break;
            }
            BlockBitCount = blockBitCount;
        }

        public IPv4Network(byte octet0, byte octet1, byte octet2, byte octet3, byte blockBitCount)
        {
            switch (blockBitCount)
            {
                case 0:
                    throw new ArgumentOutOfRangeException(nameof(blockBitCount));
                case MAX_BLOCK_BIT_COUNT:
                    First = Last = OriginalAddress = new IPv4Address(octet0, octet1, octet2, octet3);
                    break;
                default:
                    if (blockBitCount > MAX_BLOCK_BIT_COUNT)
                        throw new ArgumentOutOfRangeException(nameof(blockBitCount));
                    OriginalAddress = new IPv4Address(octet0, octet1, octet2, octet3);
                    First = new IPv4Address(OriginalAddress.Address & MaskValues[blockBitCount - 1]);
                    Last = new IPv4Address(First.Address | uint.MaxValue << blockBitCount);
                    break;
            }
            BlockBitCount = blockBitCount;
        }

        public bool Contains(IPv4Address item)
        {
            int result = item.CompareTo(First);
            if (result == 0)
                return true;
            return result > 0 && item <= Last;
        }

        private IEnumerable<IPv4Address> GetValues()
        {
            yield return First;
            if (_lastValue > _firstValue)
            {
                for (var u = _firstValue + 1u; u < _lastValue; u++)
                    yield return IPv4AddressRange.FromValue(u);
                yield return Last;
            }
        }

        public IEnumerator<IPv4Address> GetEnumerator() => GetValues().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)GetValues()).GetEnumerator();

        bool IReadOnlySet<IPv4Address>.IsProperSubsetOf(IEnumerable<IPv4Address> other) => IsSubsetOf(other);

        bool IReadOnlySet<IPv4Address>.IsProperSupersetOf(IEnumerable<IPv4Address> other) => IsSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<IPv4Address> other)
        {
            if (other is null)
                return false;
            if (other is IPv4Network network)
                return ReferenceEquals(this, network) || (_firstValue >= network._firstValue && _lastValue <= network._lastValue);
            using var enumerator = other.GetEnumerator();
            if (!enumerator.MoveNext())
                return false;
            var first = enumerator.Current;
            if (First < first || first > Last)
                return false;
            if (enumerator.MoveNext())
            {
                var last = enumerator.Current;
                while (enumerator.MoveNext())
                    last = enumerator.Current;
                return Last <= last;
            }
            return true;
        }

        public bool IsSupersetOf(IEnumerable<IPv4Address> other)
        {
            if (other is null)
                return false;
            if (other is IPv4Network network)
                return ReferenceEquals(this, network) || (_firstValue <= network._firstValue && _lastValue >= network._lastValue);
            using var enumerator = other.GetEnumerator();
            if (!enumerator.MoveNext())
                return false;
            var first = enumerator.Current;
            if (first < First || Last > first)
                return false;
            if (enumerator.MoveNext())
            {
                var last = enumerator.Current;
                while (enumerator.MoveNext())
                    last = enumerator.Current;
                return last <= Last;
            }
            return true;
        }

        public bool Overlaps(IEnumerable<IPv4Address> other)
        {
            if (other is null)
                return false;
            int diff;
            if (other is IPv4Network network)
                return ReferenceEquals(this, network) || (diff = _firstValue.CompareTo(network._lastValue)) == 0 || (diff < 0 && _lastValue >= network._firstValue);
            using var enumerator = other.GetEnumerator();
            if (!enumerator.MoveNext())
                return false;
            var first = enumerator.Current;
            if (enumerator.MoveNext())
            {
                var last = enumerator.Current;
                while (enumerator.MoveNext())
                    last = enumerator.Current;
                return (diff = First.CompareTo(last)) == 0 || (diff < 0 && Last >= first);
            }
            return (diff = First.CompareTo(first)) == 0 || (diff > 0 && first <= Last);
        }

        public bool SetEquals(IEnumerable<IPv4Address> other)
        {
            if (other is null)
                return false;
            if (other is IPv4Network network)
                return ReferenceEquals(this, other) || (_firstValue == network._firstValue && _lastValue == network._lastValue);
            using var enumerator = other.GetEnumerator();
            if (!enumerator.MoveNext() || !First.Equals(enumerator.Current))
                return false;
            if (enumerator.MoveNext())
            {
                var last = enumerator.Current;
                while (enumerator.MoveNext())
                    last = enumerator.Current;
                return Last.Equals(last);
            }
            return _firstValue == _lastValue;
        }
    }
}