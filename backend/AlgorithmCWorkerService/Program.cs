using Algorithm.C.WorkerService.Service;
using Common.RabbitMQ;
using MSSql.Infrastructure;
using Questdb.Net;

namespace Algorithm.C.WorkerService {
    public class Program {
        public static void Main(string[] args) {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddRabbitMQServices();
            builder.Services.AddAlgorithmCommonServices();
            builder.Services.AddMSSQLServices();
            builder.Services.AddScoped<IML, MLNetExecutorA>();
            builder.Services.AddScoped<IQuestDBClient>(opt => new QuestDBClient());

            var host = builder.Build();
            host.Run();
        }
    }
}
