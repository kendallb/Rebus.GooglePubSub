﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace Rebus.GooglePubSub
{
    internal class CloudQueueClientQueueFactory : ICloudQueueFactory
    {
        private readonly ConcurrentDictionary<string, Task<CloudQueue>> _queues = new ConcurrentDictionary<string, Task<CloudQueue>>();
        private readonly CloudQueueClient _cloudQueueClient;

        public CloudQueueClientQueueFactory(CloudQueueClient cloudQueueClient)
        {
            _cloudQueueClient = cloudQueueClient;
        }

        private Task<CloudQueue> InternalQueueFactory(string queueName)
        {
            return Task.FromResult(_cloudQueueClient.GetQueueReference(queueName));
        }

        public Task<CloudQueue> GetQueue(string queueName)
        {
            return _queues.GetOrAdd(queueName, InternalQueueFactory);
        }
    }
}
