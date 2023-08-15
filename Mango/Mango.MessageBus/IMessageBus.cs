namespace Mango.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessage(object message, string topicQueueName);
    }
}
