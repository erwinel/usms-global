using System.Runtime.InteropServices;

namespace IPRangeHelper
{

    [StructLayout(LayoutKind.Explicit)]
    public readonly struct IPv4Address : IEquatable<IPv4Address>, IComparable<IPv4Address>
    {
        [FieldOffset(0)] private readonly int _hashcode;

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

        public IPv4Address(uint address) => _address = address;

        public IPv4Address(byte octet0, byte octet1, byte octet2, byte octet3)
        {
            _octet0 = octet0;
            _octet1 = octet1;
            _octet2 = octet2;
            _octet3 = octet3;
        }

        public int CompareTo(IPv4Address other)
        {
            int result = _octet3.CompareTo(other._octet3);
            return (result != 0 || (result = _octet2.CompareTo(other._octet2)) != 0 || (result = _octet1.CompareTo(other._octet1)) != 0) ? result : _octet0.CompareTo(other._octet0);
        }

        public IPv4Address Decrement(uint count = 1)
        {
            if (count == 0)
                return this;
            count -= BitConverter.ToUInt32(new byte[] { _octet3, _octet2, _octet1, _octet0 });
            var bytes = BitConverter.GetBytes(count);
            return new IPv4Address(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        public bool Equals(IPv4Address other) => _address.Equals(other._address);

        public override bool Equals(object? obj) => obj is IPv4Address other && _address.Equals(other._address);

        public override int GetHashCode() => _hashcode;

        public IPv4Address Increment(uint count = 1)
        {
            if (count == 0)
                return this;
            count += BitConverter.ToUInt32(new byte[] { _octet3, _octet2, _octet1, _octet0 });
            var bytes = BitConverter.GetBytes(count);
            return new IPv4Address(bytes[3], bytes[2], bytes[1], bytes[0]);
        }

        public override string ToString() => $"{_octet0}.{_octet1}.{_octet2}.{_octet3}";

        public bool TryDecrement(out IPv4Address result) => TryDecrement(1u, out result);

        public bool TryDecrement(uint count, out IPv4Address result)
        {
            if (count == 0)
                result = this;
            else
            {
                uint value = BitConverter.ToUInt32(new byte[] { _octet3, _octet2, _octet1, _octet0 });
                if (value > count)
                {
                    result = this;
                    return false;
                }
                var bytes = BitConverter.GetBytes(value - count);
                result = new IPv4Address(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            return true;
        }

        public bool TryIncrement(out IPv4Address result) => TryIncrement(1u, out result);

        public bool TryIncrement(uint count, out IPv4Address result)
        {
            if (count == 0)
                result = this;
            else
            {
                uint value = BitConverter.ToUInt32(new byte[] { _octet3, _octet2, _octet1, _octet0 });
                if ((uint.MaxValue - value) > count)
                {
                    result = this;
                    return false;
                }
                var bytes = BitConverter.GetBytes(value + count);
                result = new IPv4Address(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            return true;
        }

        public static bool TryParse(string value, out IPv4Address result)
        {
            if (!string.IsNullOrEmpty(value))
            {
                string[] octets = value.Split('.', 5);
                if (octets.Length == 4 && byte.TryParse(octets[0], out byte octet0) && byte.TryParse(octets[1], out byte octet1) && byte.TryParse(octets[2], out byte octet2) && byte.TryParse(octets[3], out byte octet3))
                {
                    result = new(octet0, octet1, octet2, octet3);
                    return true;
                }
            }
            result = default;
            return false;
        }

        public static IPv4Address Parse(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            string[] octets = value.Split('.', 5);
            if (octets.Length != 4)
                throw new FormatException($"The input string '{value}' was not in a correct format.");
            try { return new(byte.Parse(octets[0]), byte.Parse(octets[0]), byte.Parse(octets[0]), byte.Parse(octets[0])); }
            catch (FormatException exception) { throw new FormatException($"The input string '{value}' was not in a correct format.", exception); }
        }

        public static bool operator ==(IPv4Address left, IPv4Address right) => left.Equals(right);

        public static bool operator !=(IPv4Address left, IPv4Address right) => !left.Equals(right);

        public static bool operator <(IPv4Address left, IPv4Address right) => left.CompareTo(right) < 0;

        public static bool operator <=(IPv4Address left, IPv4Address right) => left.CompareTo(right) <= 0;

        public static bool operator >(IPv4Address left, IPv4Address right) => left.CompareTo(right) > 0;

        public static bool operator >=(IPv4Address left, IPv4Address right) => left.CompareTo(right) >= 0;

        public static IPv4Address operator |(IPv4Address left, IPv4Address right) => new(left.Address | right.Address);

        public static IPv4Address operator |(IPv4Address left, uint right) => new(left.Address | right);

        public static IPv4Address operator |(uint left, IPv4Address right) => new(left | right.Address);

        public static IPv4Address operator &(IPv4Address left, IPv4Address right) => new(left.Address & right.Address);

        public static IPv4Address operator &(IPv4Address left, uint right) => new(left.Address & right);

        public static IPv4Address operator &(uint left, IPv4Address right) => new(left & right.Address);

        public static IPv4Address operator <<(IPv4Address left, int bits) => new(left.Address << bits);

        public static IPv4Address operator >>(IPv4Address left, int bits) => new(left.Address << bits);
    }
}