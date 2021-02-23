using NUnit.Framework;
using Rebus.Tests.Contracts.Transports;

#pragma warning disable 1998

namespace Rebus.GooglePubSub.Tests.Transport
{
    [TestFixture]
    public class GooglePubSubTransportBasicSendReceive : BasicSendReceive<GooglePubSubTransportFactory>
    {
    }
}