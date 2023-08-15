﻿using Azure.Messaging.ServiceBus;
using Mango.Service.EmailAPI.Services.Email;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.DTO;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Service.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string registerUserQueue;
        private readonly string orderCreated_Topic;
        private readonly string orderCreated_Email_Subscription;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private ServiceBusProcessor _emailOrderPlacedProcessor;
        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _registerUserProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

            orderCreated_Topic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreated_Email_Subscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _registerUserProcessor = client.CreateProcessor(registerUserQueue);
            _emailOrderPlacedProcessor = client.CreateProcessor(orderCreated_Topic, orderCreated_Email_Subscription);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();

            _registerUserProcessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();

            _emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
            _emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailOrderPlacedProcessor.StartProcessingAsync();
        }


        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();

            await _emailOrderPlacedProcessor.StopProcessingAsync();
            await _emailOrderPlacedProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you received the message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDTO? cartDTO = JsonConvert.DeserializeObject<CartDTO>(body);

            try
            {
                //try to log email
                await _emailService.EmailCartAndLog(cartDTO);
                await args.CompleteMessageAsync(args.Message);
            }
            catch { throw; }
        }

        private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you received the message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string email = JsonConvert.DeserializeObject<string>(body);

            try
            {
                //try to log email
                await _emailService.RegisterUserEmailAndLog(email);
                await args.CompleteMessageAsync(args.Message);
            }
            catch { throw; }
        }

        private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you received the message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage? rewardsMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                //try to log email
                await _emailService.LogPlacedOrder(rewardsMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch { throw; }
        }
    }
}