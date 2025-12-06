using Algorithm.Common.ML;
using Common.RabbitMQ;
using MSSql.Infrastructure;
using Questdb.Net;

namespace Algorithm.B.WorkerService {
    public class Program {
        public static void Main(string[] args) {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddRabbitMQServices();
            builder.Services.AddAlgorithmCommonServices();
            builder.Services.AddMSSQLServices();
            builder.Services.AddScoped<ICustomMlContext, CustomMlContext>();
            builder.Services.AddScoped<IQuestDBClient>(opt => new QuestDBClient());

            var host = builder.Build();
            host.Run();
        }
    }
}
