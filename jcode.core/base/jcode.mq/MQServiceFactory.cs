using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace jcode.mq
{
    /// <summary>
    /// 消息队列服务
    /// </summary>
    public class MQServiceFactory
    {
        private static readonly IConnectionFactory connectionFactory = new ConnectionFactory()
        {
            HostName = "43.142.110.160",
            Password = "jcode.rabbitmq.com",
            UserName = "admin"
        };
        
        //admin jcode.rabbitmq.com

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns></returns>
        public static IConnection GetConnection()
        {
            // RabbitMQ.Client 7 只提供异步方法，这里同步等待以保持原有签名
            return  connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// 获取通道
        /// </summary>
        /// <returns></returns>
        public static IChannel GetChannel()
        {
            var connection = GetConnection();
            return  connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        public static void BasicPublish(string data,string routingKey,string exchange = "")
        {
            var connection = GetConnection();
            var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
            channel.BasicPublishAsync(
                // 使用默认交换器
                exchange: exchange,
                // 推送到哪个队列中
                routingKey: routingKey,
                // 没有匹配队列时是否退回
                mandatory: false,
                // 队列属性
                basicProperties: new BasicProperties(),
                // 要发送的消息需要先转换为 byte[]
                body: Encoding.UTF8.GetBytes(data)
            ).GetAwaiter().GetResult();
        }


        /// <summary>
        /// 监听消息
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="func"></param>
        public static void BasicConsume(string queue, Func<string, Task> func)
        {
            var channel = GetChannel();
            // 定义消费者
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.Span);
                await func(message);
                // Trace.WriteLine($" [x] Received {message}");
            };

            // 开始消费
            channel.BasicConsumeAsync(queue: queue,
                autoAck: true,
                consumerTag: string.Empty,
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer).GetAwaiter().GetResult();
        }

    }
}