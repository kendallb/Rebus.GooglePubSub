using NUnit.Framework;
using Rebus.Tests.Contracts.Transports;

namespace Rebus.GooglePubSub.Tests.Transport
{
    [TestFixture]
    public class GooglePubSubTransportMessageExpiration : MessageExpiration<GooglePubSubTransportFactory>
    {
    }
}