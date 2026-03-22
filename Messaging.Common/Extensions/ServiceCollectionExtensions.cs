using Messaging.Common.Connection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Messaging.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(
            this IServiceCollection services,
            string hostName,
            string userName,
            string password,
            string vhost)
        {
            var connectionManager = new ConnectionManager(hostName, userName, password, vhost);
            var connection = connectionManager.GetConnection();
            var channel = connection.CreateModel();

            services.AddSingleton(connectionManager);
            services.AddSingleton(connection);
            services.AddSingleton(channel);

            return services;
        }
    }
}
