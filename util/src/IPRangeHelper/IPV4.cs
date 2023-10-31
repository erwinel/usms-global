using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace IPRangeHelper
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct IPV4 : IEquatable<IPV4>, IComparable<IPV4>, IIPV4
    {
        public static readonly IPV4 Min = new(0u);
        public const uint MAX_VALUE = 0xffffffffu;
        public static readonly IPV4 Max = new(MAX_VALUE);
        [FieldOffset(0)] private readonly uint _value;
        
        [FieldOffset(0)] private readonly byte _octet3;
        public byte Octet3 => _octet3;

        [FieldOffset(1)] private readonly byte _octet2;
        public byte Octet2 => _octet2;

        [FieldOffset(2)] private readonly byte _octet1;
        public byte Octet1 => _octet1;

        [FieldOffset(3)] private readonly byte _octet0;
        public byte Octet0 => _octet0;

        public IPV4(uint address)
        {
            _value = 0u;
            byte[] bytes = BitConverter.GetBytes(address);
            _octet0 = bytes[0];
            _octet1 = bytes[1];
            _octet2 = bytes[2];
            _octet3 = bytes[3];
        }
        public IPV4(byte octet0, byte octet1, byte octet2, byte octet3)
        {
            _value = 0u;
            _octet0 = octet0;
            _octet1 = octet1;
            _octet2 = octet2;
            _octet3 = octet3;
        }

        private IPV4(byte[] bytes) : this()
        {
            _value = 0u;
            _octet0 = bytes[3];
            _octet1 = bytes[2];
            _octet2 = bytes[1];
            _octet3 = bytes[0];
        }
        public int CompareTo(IPV4 other) => _value.CompareTo(other._value);

        public int CompareTo(IIPV4? other)
        {
            if (other is null)
                return 1;
            if (other is IPV4 v)
                return _value.CompareTo(v._value);
            int result = _octet0.CompareTo(other.Octet0);
            return (result != 0 || (result = _octet1.CompareTo(other.Octet1)) != 0 || (result = _octet2.CompareTo(other.Octet2)) != 0) ? result : _octet3.CompareTo(other.Octet3);
        }

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is not null && ((obj is IPV4 v) ? _value == v._value : obj is IIPV4 i && i.Octet0 == _octet0 && i.Octet1 == _octet1 && i.Octet2 == _octet2 && i.Octet3 == _octet3);

        public bool Equals(IPV4 other) => _value == other._value;

        public bool Equals([NotNullWhen(true)] IIPV4? other) => other is not null && ((other is IPV4 v) ? _value == v._value : other.Octet0 == _octet0 && other.Octet1 == _octet1 && other.Octet2 == _octet2 && other.Octet3 == _octet3);

        public uint GetAddress() => BitConverter.ToUInt32(new byte[] { _octet0, _octet1, _octet2, _octet3 });

        public static uint GetCount(IPV4Range range) { return range.Last._value - range.First._value + 1; }

        public override int GetHashCode() => BitConverter.ToInt32(new byte[] { _octet3, _octet2, _octet1, _octet0 });

        public static IEnumerable<IPV4> GetRange(IPV4 first, IPV4 last)
        {
            uint e = last._value;
            uint v = first._value;
            if (e == v)
                yield return first;
            else if (e > v)
            {
                yield return first;
                while ((++v) < e)
                    yield return new IPV4(v);
                yield return last;
            }
        }

        public override string ToString() => $"{_octet0}.{_octet1}.{_octet2}.{_octet3}";

        public static bool TryParse(string value, out IPV4 result)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string[] octets = value.Split('.');
                if (octets.Length == 4 && byte.TryParse(octets[0], out byte o0) && byte.TryParse(octets[1], out byte o1) && byte.TryParse(octets[2], out byte o2) && byte.TryParse(octets[3], out byte o3))
                {
                    result = new IPV4(o0, o1, o2, o3);
                    return true;
                }
            }

            result = Min;
            return false;
        }

        public bool TryIncrement(uint count, out IPV4 following)
        {
            if (count == 0u)
                following = this;
            else
            {
                var r = MAX_VALUE - _value;
                if (r < count)
                {
                    following = this;
                    return false;
                }
                var bytes = BitConverter.GetBytes(count + _value);
                following = new IPV4(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            return true;
        }

        public bool TryIncrement(out IPV4 following) => TryIncrement(1u, out following);

        public bool TryDecrement(uint count, out IPV4 preceding)
        {
            if (count == 0u)
                preceding = this;
            else
            {
                if (_value < count)
                {
                    preceding = this;
                    return false;
                }
                var bytes = BitConverter.GetBytes(count - _value);
                preceding = new IPV4(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            return true;
        }

        public bool TryDecrement(out IPV4 preceding) => TryDecrement(1u, out preceding);

        public static bool operator ==(IPV4 left, IPV4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IPV4 left, IPV4 right)
        {
            return !(left == right);
        }

        public static bool operator <(IPV4 left, IPV4 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(IPV4 left, IPV4 right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(IPV4 left, IPV4 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(IPV4 left, IPV4 right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}