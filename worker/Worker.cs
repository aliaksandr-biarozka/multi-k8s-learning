using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IConnectionMultiplexer client;
        private ISubscriber subscriber;
        private RedisConnection settings;

        public Worker(ILogger<Worker> logger, IOptions<RedisConnection> settings)
        {
            _logger = logger;
            this.settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            client = await ConnectionMultiplexer.ConnectAsync($"{settings.Host}:{settings.Port}");

            subscriber = client.GetSubscriber();

            (await subscriber.SubscribeAsync(new RedisChannel("messages", RedisChannel.PatternMode.Auto))).OnMessage(async channel =>
            {

                try
                {
                    var result = Fibbonachi(Int32.Parse(channel.Message), stoppingToken);
                    await client.GetDatabase().HashSetAsync("values", channel.Message, result);
                }
                catch(OperationCanceledException) { }
            });
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await subscriber.UnsubscribeAsync(new RedisChannel("messages", RedisChannel.PatternMode.Auto));
            }
            finally
            {
                client?.Dispose();
            }
        }

        protected int Fibbonachi(int index, CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            if (index < 2) return 1;

            return Fibbonachi(index - 1, stoppingToken) + Fibbonachi(index - 2, stoppingToken);
        }
    }
}
