//===============================================================================
// Microsoft FastTrack for Azure
// Azure Service Bus Samples
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace CreateSubscriptions
{
    class Program
    {
        private static NamespaceManager _nameSpaceManager = null;
        private static readonly string _connectionString = "{your Service Bus connection string here}";
        private static readonly string _topicName = "DemoTopic1";
        private static readonly string _externalSubscriptionName = "ExternalSubscriber";
        private static readonly string _functionSubscriptionName = "FunctionSubscriber";

        static void Main(string[] args)
        {
            // Connect to the Service Bus namespace using a SAS Url with the Manage permission
            _nameSpaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);

            // Create the topic if it does not already exist
            if (!_nameSpaceManager.TopicExists(_topicName)) _nameSpaceManager.CreateTopic(_topicName);

            // Create a new subscription with a message filter to only accept
            // messages from the ServiceBusSender - to be used by the ServiceBusSubscriber web application
            SqlFilter externalSubscriptionFilter = new SqlFilter("From = 'ServiceBusSender'");
            if (!_nameSpaceManager.SubscriptionExists(_topicName, _externalSubscriptionName)) _nameSpaceManager.CreateSubscription(_topicName, _externalSubscriptionName, externalSubscriptionFilter);

            // Create a new subscription with a message filter to only accept
            // response messages from the ServiceBusSubscriber - to be used by the Azure Function subscriber
            SqlFilter functionSubscriptionFilter = new SqlFilter("From = 'ServiceBusSubscriber'");
            if (!_nameSpaceManager.SubscriptionExists(_topicName, _functionSubscriptionName)) _nameSpaceManager.CreateSubscription(_topicName, _functionSubscriptionName, functionSubscriptionFilter);
        }
    }
}
