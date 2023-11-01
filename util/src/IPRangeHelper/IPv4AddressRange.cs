using System.Collections;

namespace IPRangeHelper
{
    public class IPv4AddressRange : IIPv4AddressRange
    {
        private readonly uint _firstValue;
        private readonly uint _lastValue;

        public IPv4Address First { get; }

        public IPv4Address Last { get; }

        public uint Count => _lastValue - _firstValue + 1;

        public static uint ToValue(IPv4Address address) => address.Address switch
        {
            0u or uint.MaxValue => address.Address,
            _ => BitConverter.ToUInt32(new byte[] { address.Octet3, address.Octet2, address.Octet1, address.Octet0 }),
        };

        public static IPv4Address FromValue(uint value)
        {
            switch (value)
            {
                case 0u:
                case uint.MaxValue:
                    return new(value);
                default:
                    byte[] bytes = BitConverter.GetBytes(value);
                    return new(bytes[3], bytes[2], bytes[1], bytes[0]);
            }
        }

        int IReadOnlyCollection<IPv4Address>.Count
        {
            get
            {
                uint count = Count;
                return (count > int.MaxValue) ? int.MaxValue : (int)count;
            }
        }

        private IPv4AddressRange(uint firstValue, IPv4Address first, uint lastValue, IPv4Address last)
        {
            _firstValue = firstValue;
            First = first;
            _lastValue = lastValue;
            Last = last;
        }

        public IPv4AddressRange(IPv4Address first, IPv4Address last)
        {
            if ((First = first) > (Last = last))
                throw new ArgumentException($"{nameof(first)} cannot be greater than {nameof(last)}.", nameof(first));
            _firstValue = ToValue(first);
            _lastValue = ToValue(last);
        }

        public IPv4AddressRange(IPv4Address first, uint count = 1)
        {
            switch (count)
            {
                case 0:
                    throw new ArgumentOutOfRangeException(nameof(count));
                case 1:
                    _lastValue = _firstValue = ToValue(First = Last = first);
                    break;
                default:
                    _firstValue = ToValue(First = first);
                    Last = FromValue(_lastValue = _firstValue + count - 1);
                    break;
            }
        }

        public IPv4AddressRange(uint count, IPv4Address last)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            _lastValue = ToValue(Last = last);
            First = FromValue(_firstValue = _lastValue - count + 1);
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
                    yield return FromValue(u);
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
            if (other is IPv4AddressRange range)
                return ReferenceEquals(this, range) || (_firstValue >= range._firstValue && _lastValue <= range._lastValue);
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
            if (other is IPv4AddressRange range)
                return ReferenceEquals(this, range) || (_firstValue <= range._firstValue && _lastValue >= range._lastValue);
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
            if (other is IPv4AddressRange range)
                return ReferenceEquals(this, range) || (diff = _firstValue.CompareTo(range._lastValue)) == 0 || (diff < 0 && _lastValue >= range._firstValue);
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

        public IPv4AddressRange? Remove(IPv4Address item, out IPv4AddressRange? after)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<IPv4Address> other)
        {
            if (other is null)
                return false;
            if (other is IPv4AddressRange range)
                return ReferenceEquals(this, other) || (_firstValue == range._firstValue && _lastValue == range._lastValue);
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

        public bool TryMergeWith(IPv4AddressRange other, out IPv4AddressRange merged)
        {
            if (other is null)
            {
                merged = this;
                return false;
            }
            if (ReferenceEquals(this, other))
                merged = new(_firstValue, First, _lastValue, Last);
            else
            {
                if (_lastValue < other._firstValue - 1 || other._lastValue < _firstValue - 1)
                {
                    merged = this;
                    return false;
                }
                if (_firstValue > other._firstValue)
                    merged = (_lastValue < other._lastValue) ? new(other._firstValue, other.First, other._lastValue, other.Last) : new(other._firstValue, other.First, _lastValue, Last);
                else
                    merged = (_lastValue < other._lastValue) ? new(_firstValue, First, other._lastValue, other.Last) : new(_firstValue, First, _lastValue, Last);
            }
            return true;
        }
    }
}