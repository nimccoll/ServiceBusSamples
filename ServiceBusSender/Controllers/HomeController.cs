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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using ServiceBusSender.Models;
using System;
using System.Diagnostics;
using System.Text;

namespace ServiceBusSender.Controllers
{
    public class HomeController : Controller
    {
        private static TopicClient _topicClient = null;
        private readonly string _topicName = "DemoTopic1";

        public HomeController(IConfiguration configuration)
        {
            // For performance reasons we are using a static instance of the TopicClient per best practices
            if (_topicClient == null)
            {
                string connectionString = configuration.GetValue<string>("ServiceBus");
                _topicClient = new TopicClient(connectionString, _topicName);
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            // Generate a random number and send it as a message to the topic
            Random random = new Random();
            string messageText = random.Next(1, 10000).ToString();
            Message message = new Message(Encoding.UTF8.GetBytes(messageText));
            // Set the "From" value on the user properties to allow the subscriber to filter messages
            message.UserProperties["From"] = "ServiceBusSender";
            _topicClient.SendAsync(message).Wait();
            ViewBag.Message = $"Message {messageText} sent!";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
