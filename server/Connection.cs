using System;

namespace backend_server
{
    public class Connection
    {
        public string Database { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }
    }

    public class RedisConnection
    {
        public string Host { get; set; }

        public string Port { get; set; }
    }
}
