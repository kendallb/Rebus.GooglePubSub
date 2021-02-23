using System;
using System.Collections.Concurrent;
using Rebus.Config;
using Rebus.GooglePubSub.Transport;
using Rebus.Logging;
using Rebus.Tests.Contracts.Transports;
using Rebus.Time;
using Rebus.Transport;

namespace Rebus.GooglePubSub.Tests.Transport
{
    public class GooglePubSubTransportFactory : ITransportFactory
    {
        readonly ConcurrentDictionary<string, GooglePubSubTransport> _transports = new ConcurrentDictionary<string, GooglePubSubTransport>(StringComparer.OrdinalIgnoreCase);

        public ITransport CreateOneWayClient()
        {
            return Create(null);
        }

        public ITransport Create(string inputQueueAddress)
        {
            if (inputQueueAddress == null)
            {
                var transport = new GooglePubSubTransport(AzureConfig.StorageAccount, null, new ConsoleLoggerFactory(false), new GooglePubSubTransportOptions(), new DefaultRebusTime());

                transport.Initialize();

                return transport;
            }

            return _transports.GetOrAdd(inputQueueAddress, address =>
            {
                var transport = new GooglePubSubTransport(AzureConfig.StorageAccount, inputQueueAddress, new ConsoleLoggerFactory(false), new GooglePubSubTransportOptions(), new DefaultRebusTime());

                transport.PurgeInputQueue();

                transport.Initialize();

                return transport;
            });
        }

        public void CleanUp()
        {
        }
    }
}
