//Copyright (c) Service Stack LLC. All Rights Reserved.
//License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;

namespace ServiceStack.Messaging
{
    public class InMemoryMessageQueueClient
        : IMessageQueueClient, IOneWayClient
    {
        private readonly MessageQueueClientFactory factory;

        public InMemoryMessageQueueClient(MessageQueueClientFactory factory)
        {
            this.factory = factory;
        }

        public void Publish<T>(T messageBody)
        {
            var message = messageBody as IMessage;
            if (message != null)
            {
                Publish(message.ToInQueueName(), message);
            }
            else
            {
                Publish(new Message<T>(messageBody));
            }
        }

        public void Publish<T>(IMessage<T> message)
        {
            factory.PublishMessage(QueueNames<T>.In, message);
        }

        public void Publish(string queueName, IMessage message)
        {
            var messageBytes = message.ToBytes();
            factory.PublishMessage(queueName, messageBytes);
        }

        public void SendOneWay(object requestDto)
        {
            Publish(MessageFactory.Create(requestDto));
        }

        public void SendOneWay(string queueName, object requestDto)
        {
            Publish(queueName, MessageFactory.Create(requestDto));
        }

        public void Notify(string queueName, IMessage message)
        {
            var messageBytes = message.ToBytes();
            factory.PublishMessage(queueName, messageBytes);
        }

        public IMessage<T> Get<T>(string queueName, TimeSpan? timeOut = null)
        {
            return GetAsync<T>(queueName);
        }

        public IMessage<T> GetAsync<T>(string queueName)
        {
            return factory.GetMessageAsync(queueName)
                .ToMessage<T>();
        }

        public void Ack(IMessage message)
        {
        }

        public void Nak(IMessage message, bool requeue, Exception exception = null)
        {
            var queueName = requeue
                ? message.ToInQueueName()
                : message.ToDlqQueueName();

            Publish(queueName, message);
        }

        public IMessage<T> CreateMessage<T>(object mqResponse)
        {
            return (IMessage<T>) mqResponse;
        }

        public void Dispose()
        {
        }
    }
}