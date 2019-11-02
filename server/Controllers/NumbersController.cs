using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using StackExchange.Redis;

namespace backend_server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NumbersController : ControllerBase
    {
        private readonly ILogger<NumbersController> _logger;
        private readonly Connection setting;
        private readonly RedisConnection redisSetting;

        public NumbersController(ILogger<NumbersController> logger, IOptions<Connection> setting, IOptions<RedisConnection> redisSetting)
        {
            _logger = logger;
            this.setting = setting.Value;
            this.redisSetting = redisSetting.Value;
        }

        [HttpGet("all")]
        public async Task<IEnumerable<int>> Get()
        {
            using (var connection = new NpgsqlConnection($"Host={setting.Host};Username={setting.User};Password={setting.Password};Database={setting.Database}"))
            {
                connection.Open();
                await connection.ExecuteAsync("CREATE TABLE IF NOT EXISTS values (number INT)", commandType: System.Data.CommandType.Text);

                var result = await connection.QueryAsync<int>("SELECT number FROM values");

                return result;
            }
        }

        [HttpGet("current")]
        public async Task<IEnumerable<KeyValuePair<int, int?>>> GetCurrent(int value)
        {
            using (var client = await ConnectionMultiplexer.ConnectAsync($"{redisSetting.Host}:{redisSetting.Port}"))
            {
                var entities = await client.GetDatabase().HashGetAllAsync("values");

                return entities.Select(e => new KeyValuePair<int, int?>(Int32.Parse(e.Name), e.Value.HasValue && e.Value != RedisValue.Null ? Int32.Parse(e.Value) : (int?)null)).ToList();
            }
        }

        [HttpPost]
        public async Task Post([FromBody]int index)
        {
            using (var connection = new NpgsqlConnection($"Host={setting.Host};Username={setting.User};Password={setting.Password};Database={setting.Database}"))
            using(var client = await ConnectionMultiplexer.ConnectAsync($"{redisSetting.Host}:{redisSetting.Port}"))
            {
                connection.Open();
                var database = client.GetDatabase();

                await connection.ExecuteAsync($"INSERT INTO values(number) VALUES({index})", commandType: System.Data.CommandType.Text);

                await database.HashSetAsync("values", index, RedisValue.Null);

                var publisher = client.GetSubscriber();
                await publisher.PublishAsync(new RedisChannel("messages", RedisChannel.PatternMode.Auto), index);
            }
        }
    }
}
