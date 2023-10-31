namespace IPRangeHelper
{
    public interface IIPV4 : IEquatable<IIPV4>, IComparable<IIPV4>
    {
        uint GetAddress();
        byte Octet0 { get; }
        byte Octet1 { get; }
        byte Octet2 { get; }
        byte Octet3 { get; }
    }
}