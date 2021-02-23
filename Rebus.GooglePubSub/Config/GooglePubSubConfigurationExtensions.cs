using System;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Rebus.GooglePubSub;
using Rebus.GooglePubSub.Transport;
using Rebus.Logging;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;
using Rebus.Time;
using Rebus.Timeouts;
using Rebus.Transport;
// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable UnusedMember.Global

namespace Rebus.Config
{
    /// <summary>
    /// Configuration extensions for the Google Pub/Sub transport
    /// </summary>
    public static class GooglePubSubConfigurationExtensions
    {
        const string GoogleTimeoutManagerText = @"A disabled timeout manager was installed as part of the Google Pub/Sub configuration, becuase the transport has native support for deferred messages.

If you don't want to use Google Pub/Sub's native support for deferred messages, please pass GooglePubSubTransportOptions with UseNativeDeferredMessages = false when
configuring the transport, e.g. like so:

Configure.With(...)
    .Transport(t => {
        var options = new GooglePubSubTransportOptions { UseNativeDeferredMessages = false };

        t.UseGooglePubSub(storageAccount, ""my-queue"", options: options);
    })
    .(...)
    .Start();";

        /// <summary>
        /// Configures Rebus to use Google Pub/Sub to transport messages as a one-way client (i.e. will not be able to receive any messages)
        /// </summary>
        public static void UseGooglePubSubAsOneWayClient(this StandardConfigurer<ITransport> configurer, string storageAccountConnectionString, GooglePubSubTransportOptions options = null)
        {
            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);

            Register(configurer, null, storageAccount, options);

            OneWayClientBackdoor.ConfigureOneWayClient(configurer);
        }

        /// <summary>
        /// Configures Rebus to use Google Pub/Sub to transport messages
        /// </summary>
        public static void UseGooglePubSub(this StandardConfigurer<ITransport> configurer, string storageAccountConnectionString, string inputQueueAddress, GooglePubSubTransportOptions options = null)
        {
            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);

            Register(configurer, inputQueueAddress, storageAccount, options);
        }

        /// <summary>
        /// Configures Rebus to use Google Pub/Sub to transport messages as a one-way client (i.e. will not be able to receive any messages)
        /// </summary>
        public static void UseGooglePubSubAsOneWayClient(this StandardConfigurer<ITransport> configurer, CloudStorageAccount storageAccount, GooglePubSubTransportOptions options = null)
        {
            Register(configurer, null, storageAccount, options);

            OneWayClientBackdoor.ConfigureOneWayClient(configurer);
        }

        /// <summary>
        /// Configures Rebus to use Google Pub/Sub to transport messages
        /// </summary>
        public static void UseGooglePubSub(this StandardConfigurer<ITransport> configurer, ICloudQueueFactory queueFactory, string inputQueueAddress, GooglePubSubTransportOptions options = null)
        {
            Register(configurer, inputQueueAddress, queueFactory, options);
        }

        /// <summary>
        /// Configures Rebus to use Google Pub/Sub to transport messages as a one-way client (i.e. will not be able to receive any messages)
        /// </summary>
        public static void UseGooglePubSubAsOneWayClient(this StandardConfigurer<ITransport> configurer, ICloudQueueFactory queueFactory, GooglePubSubTransportOptions options = null)
        {
            Register(configurer, null, queueFactory, options);

            OneWayClientBackdoor.ConfigureOneWayClient(configurer);
        }

        /// <summary>
        /// Configures Rebus to use Google Pub/Sub to transport messages
        /// </summary>
        public static void UseGooglePubSub(this StandardConfigurer<ITransport> configurer, CloudStorageAccount storageAccount, string inputQueueAddress, GooglePubSubTransportOptions options = null)
        {
            Register(configurer, inputQueueAddress, storageAccount, options);
        }


        static void Register(StandardConfigurer<ITransport> configurer, string inputQueueAddress,
            CloudStorageAccount cloudStorageAccount, GooglePubSubTransportOptions optionsOrNull)
        {
            var queueFactory = new CloudQueueClientQueueFactory(cloudStorageAccount.CreateCloudQueueClient());
            Register(configurer, inputQueueAddress, queueFactory, optionsOrNull);
        }

        static void Register(StandardConfigurer<ITransport> configurer, string inputQueueAddress, ICloudQueueFactory queueFactory, GooglePubSubTransportOptions optionsOrNull)
        {
            if (configurer == null) throw new ArgumentNullException(nameof(configurer));
            if (queueFactory == null) throw new ArgumentNullException(nameof(queueFactory));

            var options = optionsOrNull ?? new GooglePubSubTransportOptions();

            configurer.Register(c =>
            {
                var rebusLoggerFactory = c.Get<IRebusLoggerFactory>();
                var rebusTime = c.Get<IRebusTime>();
                return new GooglePubSubTransport(queueFactory, inputQueueAddress, rebusLoggerFactory, options, rebusTime);
            });

            if (options.UseNativeDeferredMessages)
            {
                configurer.OtherService<ITimeoutManager>().Register(c => new DisabledTimeoutManager(), description: GoogleTimeoutManagerText);

                configurer.OtherService<IPipeline>().Decorate(c =>
                {
                    var pipeline = c.Get<IPipeline>();

                    return new PipelineStepRemover(pipeline)
                        .RemoveIncomingStep(s => s.GetType() == typeof(HandleDeferredMessagesStep));
                });
            }
        }
    }
}