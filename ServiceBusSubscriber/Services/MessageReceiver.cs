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
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusSubscriber.Services
{
    public class MessageReceiver
    {
        private ITopicClient _topicClient = null;
        private ISubscriptionClient _subscriptionClient = null;
        private readonly string _topicName = "DemoTopic1";
        private readonly string _subscriptionName = "ExternalSubscriber";

        public MessageReceiver(IConfiguration configuration)
        {
            // Create a Service Bus TopicClient and SubscriptionClient
            string connectionString = configuration.GetValue<string>("ServiceBus");
            _topicClient = new TopicClient(connectionString, _topicName);
            _subscriptionClient = new SubscriptionClient(connectionString, _topicName, _subscriptionName);

            // Register a message handler function to begin receiving messages
            _subscriptionClient.RegisterMessageHandler(ReceiveMessageAsync,
                new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 1, AutoComplete = false });
        }

        private async Task ReceiveMessageAsync(Message message, CancellationToken cancellationToken)
        {
            // Retrieve the message from ServiceBusSender and remove the message from the subscription
            string output = Encoding.UTF8.GetString(message.Body);
            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

            // Reply to the message
            string response = $"I have received message {output}.";
            Message responseMessage = new Message(Encoding.UTF8.GetBytes(response));
            // Set the "From" value on the user properties to allow the subscriber to filter messages
            responseMessage.UserProperties["From"] = "ServiceBusSubscriber";
            await _topicClient.SendAsync(responseMessage);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            // Handle any exceptions here
            return Task.CompletedTask;
        }
    }
}
