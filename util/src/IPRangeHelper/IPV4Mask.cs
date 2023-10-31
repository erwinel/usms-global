using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace IPRangeHelper
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct IPV4Mask : IEquatable<IPV4Mask>, IComparable<IPV4Mask>, IIPV4
    {
        public const uint CLASS_A = 0xff00_0000u;
        
        public const uint CLASS_B = 0xffff_0000u;
        
        public const uint CLASS_C = 0xffff_ff00u;
        
        [FieldOffset(0)] private readonly uint _address;
        public uint Address => _address;
        
        [FieldOffset(0)] private readonly byte _octet0;
        public byte Octet0 => _octet0;

        [FieldOffset(1)] private readonly byte _octet1;
        public byte Octet1 => _octet1;

        [FieldOffset(2)] private readonly byte _octet2;
        public byte Octet2 => _octet2;

        [FieldOffset(3)] private readonly byte _octet3;
        public byte Octet3 => _octet3;

        public IPV4Mask(uint address)
        {
            _octet0 = _octet1 = _octet2 = _octet3 = 0;
            _address = address;
        }

        public IPV4Mask(byte octet0, byte octet1, byte octet2, byte octet3)
        {
            _address = 0u;
            _octet0 = octet0;
            _octet1 = octet1;
            _octet2 = octet2;
            _octet3 = octet3;
        }

        public int CompareTo(IPV4Mask other)
        {
            int result = _octet0.CompareTo(other._octet0);
            return (result != 0 || (result = _octet1.CompareTo(other._octet1)) != 0 || (result = _octet2.CompareTo(other._octet2)) != 0) ? result : _octet3.CompareTo(other._octet3);
        }

        public int CompareTo(IIPV4? other)
        {
            if (other is null)
                return 1;
            if (other is IPV4Mask m)
                return CompareTo(m);
            int result = _octet0.CompareTo(other.Octet0);
            return (result != 0 || (result = _octet1.CompareTo(other.Octet1)) != 0 || (result = _octet2.CompareTo(other.Octet2)) != 0) ? result : _octet3.CompareTo(other.Octet3);
        }

        public bool Equals(IPV4Mask other) => _address == other._address;

        public bool Equals([NotNullWhen(true)] IIPV4? other) => other is not null && ((other is IPV4Mask m) ? _address == m._address : _octet0 == other.Octet0 && _octet1 == other.Octet1 && _octet2 == other.Octet2 && _octet3 == other.Octet3);

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is not null && ((obj is IPV4Mask m) ? _address == m._address : obj is IIPV4 other && _octet0 == other.Octet0 && _octet1 == other.Octet1 && _octet2 == other.Octet2 && _octet3 == other.Octet3);

        uint IIPV4.GetAddress() => Address;

        public override int GetHashCode() => BitConverter.ToInt32(new byte[] { _octet3, _octet2, _octet1, _octet0 });

        public override string ToString() => $"{_octet0}.{_octet1}.{_octet2}.{_octet3}";

        public static bool operator ==(IPV4Mask left, IPV4Mask right) => left.Equals(right);

        public static bool operator !=(IPV4Mask left, IPV4Mask right) => !(left == right);

        public static bool operator <(IPV4Mask left, IPV4Mask right) => left.CompareTo(right) < 0;

        public static bool operator <=(IPV4Mask left, IPV4Mask right) => left.CompareTo(right) <= 0;

        public static bool operator >(IPV4Mask left, IPV4Mask right) => left.CompareTo(right) > 0;

        public static bool operator >=(IPV4Mask left, IPV4Mask right) => left.CompareTo(right) >= 0;
    }
}