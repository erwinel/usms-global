namespace IPRangeHelper
{
    public interface IIPv4AddressRange : IReadOnlySet<IPv4Address>
    {
        IPv4Address First { get; }

        IPv4Address Last { get; }
    }
}