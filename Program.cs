using System;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using Microsoft.Extensions.Configuration;

namespace MyService
{
    public class Program
    {
        public static IConfiguration _configuration;
        public static void Main(string[] args)
        {
            // Set up configuration sources.
            string env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings/appsettings.json")
                .AddJsonFile($"appsettings/appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            Console.WriteLine("BusinessEntityManager "+ _configuration["ApplicationTitle"] +"..." + _configuration["RabbitMQServer"]);
            var factory = new ConnectionFactory();
            factory.HostName = _configuration["RabbitMQServer"];
            factory.UserName = _configuration["RabbitMQUser"];
            factory.Password = _configuration["RabbitMQPass"];

            using( var connection = factory.CreateConnection())
            {
                using( var channel = connection.CreateModel())
                {
                    var subscription = new Subscription(channel, "ProviderManager", false);
                    
                    while(true)
                    {
                        MessageHandler messageHandler = new MessageHandler(subscription);
                    }
                }
            }
        }
    }
}