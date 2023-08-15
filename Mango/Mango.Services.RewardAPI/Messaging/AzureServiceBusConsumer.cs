using Azure.Messaging.ServiceBus;
using Mango.Service.RewardAPI.Services.Email;
using Mango.Services.RewardAPI.Message;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Service.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedRewardsSubscription;

        private readonly IConfiguration _configuration;
        private readonly RewardsService _rewardsService;

        private ServiceBusProcessor _rewardProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardsService rewardsService)
        {
            _configuration = configuration;
            _rewardsService = rewardsService;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreatedRewardsSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _rewardProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedRewardsSubscription);
        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _rewardProcessor.StopProcessingAsync();
            await _rewardProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you received the message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage? cartDTO = JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                //try to log email
                await _rewardsService.UpdateRewards(cartDTO);
                await args.CompleteMessageAsync(args.Message);
            }
            catch { throw; }
        }
    }
}
