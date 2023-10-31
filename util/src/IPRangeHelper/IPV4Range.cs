using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace IPRangeHelper
{
    public class IPV4Range : IEquatable<IPV4Range>, IReadOnlyCollection<IPV4>
    {
        public const byte MAX_BLOCK_BIT_COUNT = 32;

        public IPV4 OriginalAddress { get; }

        public IPV4 First { get; }

        public IPV4 Last { get; }

        public byte BlockBitCount { get; }

        public IPV4Mask Mask { get; }

        public uint Count => IPV4.GetCount(this);

        int IReadOnlyCollection<IPV4>.Count
        {
            get
            {
                uint count = Count;
                return (count < int.MaxValue) ? (int)count : int.MaxValue;
            }
        }

        public IPV4Range()
        {
            Mask = new(IPV4.MAX_VALUE);
            First = Last = OriginalAddress = IPV4.Min;
            BlockBitCount = MAX_BLOCK_BIT_COUNT;
        }

        public IPV4Range(IPV4Range other)
        {
            Mask = other.Mask;
            First = other.First;
            Last = other.Last;
            OriginalAddress = other.OriginalAddress;
            BlockBitCount = other.BlockBitCount;
        }

        public IPV4Range(IPV4 address, byte blockBitCount)
        {
            switch (blockBitCount)
            {
                case 0:
                    throw new ArgumentOutOfRangeException("blockBitCount");
                case MAX_BLOCK_BIT_COUNT:
                    Mask = new(IPV4.MAX_VALUE);
                    First = Last = address;
                    break;
                default:
                    if (blockBitCount > MAX_BLOCK_BIT_COUNT)
                        throw new ArgumentOutOfRangeException("blockBitCount");
                    Mask = new(IPV4.MAX_VALUE >> (32 - blockBitCount));
                    First = new IPV4(address.GetAddress() & Mask.Address);
                    Last = new IPV4(First.GetAddress() | IPV4.MAX_VALUE << blockBitCount);
                    break;
            }
            BlockBitCount = blockBitCount;
            OriginalAddress = address;
        }

        public IPV4Range(uint address, byte blockBitCount)
        {
            switch (blockBitCount)
            {
                case 0:
                    throw new ArgumentOutOfRangeException("blockBitCount");
                case MAX_BLOCK_BIT_COUNT:
                    Mask = new(IPV4.MAX_VALUE);
                    First = Last = OriginalAddress = new IPV4(address);
                    break;
                default:
                    if (blockBitCount > MAX_BLOCK_BIT_COUNT)
                        throw new ArgumentOutOfRangeException("blockBitCount");
                    OriginalAddress = new IPV4(address);
                    Mask = new(IPV4.MAX_VALUE >> (32 - blockBitCount));
                    First = new IPV4(address & Mask.Address);
                    Last = new IPV4(First.GetAddress() | IPV4.MAX_VALUE << blockBitCount);
                    break;
            }
            BlockBitCount = blockBitCount;
        }

        public IPV4Range(byte octet0, byte octet1, byte octet2, byte octet3, byte blockBitCount)
        {
            switch (blockBitCount)
            {
                case 0:
                    throw new ArgumentOutOfRangeException("blockBitCount");
                case MAX_BLOCK_BIT_COUNT:
                    Mask = new(IPV4.MAX_VALUE);
                    First = Last = OriginalAddress = new IPV4(octet0, octet1, octet2, octet3);
                    break;
                default:
                    if (blockBitCount > MAX_BLOCK_BIT_COUNT)
                        throw new ArgumentOutOfRangeException("blockBitCount");
                    OriginalAddress = new IPV4(octet0, octet1, octet2, octet3);
                    Mask = new(IPV4.MAX_VALUE >> (32 - blockBitCount));
                    First = new IPV4(OriginalAddress.GetAddress() & Mask.Address);
                    Last = new IPV4(First.GetAddress() | IPV4.MAX_VALUE << blockBitCount);
                    break;
            }
            BlockBitCount = blockBitCount;
        }

        public bool Contains(IPV4 address)
        {
            int d = address.CompareTo(First);
            return d == 0 || (d > 0 && address.CompareTo(Last) <= 0);
        }

        public bool Contains(IPV4Range other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (BlockBitCount == other.BlockBitCount)
                return First.Equals(other.First);
            int d = First.CompareTo(other.First);
            if (d == 0)
                return BlockBitCount < other.BlockBitCount;
            return d > 0 && Last.CompareTo(other.Last) <= 0;
        }

        public bool Equals(IPV4Range? other) => other != null && (ReferenceEquals(this, other) || (BlockBitCount == other.BlockBitCount && First.Equals(other.First)));

        public override bool Equals(object? obj) => obj is IPV4Range range && Equals(range);

        public IEnumerator<IPV4> GetEnumerator() => IPV4.GetRange(First, Last).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)IPV4.GetRange(First, Last)).GetEnumerator();
        
        public override int GetHashCode()
        {
            unchecked
            {
                return (21 + First.GetHashCode()) * 7 + BlockBitCount;
            }
        }

        public bool Overlaps(IPV4Range other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            int d = First.CompareTo(other.First);
            return d == 0 || ((d > 0) ? First.CompareTo(other.Last) < 0 :  First.CompareTo(other.Last) <= 0);
        }

        public override string ToString() { return First.ToString() + "/" + BlockBitCount.ToString(); }

        public static bool TryParse(string cidrNotation, [NotNullWhen(true)] out IPV4Range? result)
        {
            if (!string.IsNullOrEmpty(cidrNotation))
            {
                int index = cidrNotation.IndexOf('/');
                if (index > 6 && index < cidrNotation.Length - 1 && IPV4.TryParse(cidrNotation[..index], out IPV4 address) && byte.TryParse(cidrNotation[(index + 1)..], out byte c) && c > 0 && c <= MAX_BLOCK_BIT_COUNT)
                {
                    result = new IPV4Range(address, c);
                    return true;
                }            }

            result = null;
            return false;
        }
    }
}