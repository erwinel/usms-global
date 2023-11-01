using IPRangeHelper;

namespace IPRangeHelperTests;

public class IPv4AddressUnitTest
{
    // [SetUp]
    // public void Setup()
    // {
    // }

    [Test]
    public void AddressConstructorTest()
    {
        var target = new IPv4Address();
        var expectedAddress = 0u;
        byte expected0 = 0;
        Assert.Multiple(() =>
        {
            Assert.That(target.Address, Is.EqualTo(expectedAddress));
            Assert.That(target.Octet0, Is.EqualTo(expected0));
            Assert.That(target.Octet1, Is.EqualTo(expected0));
            Assert.That(target.Octet2, Is.EqualTo(expected0));
            Assert.That(target.Octet3, Is.EqualTo(expected0));
        });
        expected0 = 0x0a;
        byte expected1 = 0;
        byte expected2 = 0;
        byte expected3 = 0;
        expectedAddress = 0x0000000au;
        target = new(expectedAddress);
        Assert.Multiple(() =>
        {
            Assert.That(target.Address, Is.EqualTo(expectedAddress));
            Assert.That(target.Octet0, Is.EqualTo(expected0));
            Assert.That(target.Octet1, Is.EqualTo(expected1));
            Assert.That(target.Octet2, Is.EqualTo(expected2));
            Assert.That(target.Octet3, Is.EqualTo(expected3));
        });
        expectedAddress = 0x0501a8c0;
        expected0 = 0x05;
        expected1 = 0x01;
        expected2 = 0xa8;
        expected3 = 0xc0;
        target = new(expectedAddress);
        Assert.Multiple(() =>
        {
            Assert.That(target.Address, Is.EqualTo(expectedAddress));
            Assert.That(target.Octet0, Is.EqualTo(expected0));
            Assert.That(target.Octet1, Is.EqualTo(expected1));
            Assert.That(target.Octet2, Is.EqualTo(expected2));
            Assert.That(target.Octet3, Is.EqualTo(expected3));
        });
        expected0 = 0x0a;
        expected1 = expected2 = expected3 = 0;
        expectedAddress = 0x0000000au;
        target = new(expected0, expected1, expected2, expected3);
        Assert.Multiple(() =>
        {
            Assert.That(target.Address, Is.EqualTo(expectedAddress));
            Assert.That(target.Octet0, Is.EqualTo(expected0));
            Assert.That(target.Octet1, Is.EqualTo(expected1));
            Assert.That(target.Octet2, Is.EqualTo(expected2));
            Assert.That(target.Octet3, Is.EqualTo(expected3));
        });
        expectedAddress = 0x0501a8c0;
        expected0 = 0x05;
        expected1 = 0x01;
        expected2 = 0xa8;
        expected3 = 0xc0;
        target = new IPv4Address(expected0, expected1, expected2, expected3);
        Assert.Multiple(() =>
        {
            Assert.That(target.Address, Is.EqualTo(expectedAddress));
            Assert.That(target.Octet0, Is.EqualTo(expected0));
            Assert.That(target.Octet1, Is.EqualTo(expected1));
            Assert.That(target.Octet2, Is.EqualTo(expected2));
            Assert.That(target.Octet3, Is.EqualTo(expected3));
        });
    }
}
