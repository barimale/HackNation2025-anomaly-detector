using Algorithm.D.WorkerService.Service;
using Algorithms.Common.QuestDB.Service;
using Common.RabbitMQ;
using Common.RabbitMQ.Model;
using Configuration.Common;
using MSSql.Infrastructure.Entities;
using MSSql.Infrastructure.Repositories.Abstractions;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Algorithm.D.WorkerService {
    public class Worker : BackgroundService {
        public const string AGENT_NAME = Constants.AGENT_NAME_D;

        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IGeminiAgentService geminiAgentService;
        private readonly IAlgorithmResultRepository algorithmRepository;
        private readonly IQueueConsumerService _queueConsumerService;
        private readonly IQueueService _queueService;

        public Worker(ILogger<Worker> logger,
            IServiceScopeFactory _scopeFactory,
            IConfiguration configuration) {
            _logger = logger;
            this._scopeFactory = _scopeFactory;
            geminiAgentService = new GeminiAgentService(configuration["GoogleApiKey"], AGENT_NAME);
            using var scope = _scopeFactory.CreateScope();
            algorithmRepository = scope.ServiceProvider.GetRequiredService<IAlgorithmResultRepository>();
            _queueConsumerService = scope.ServiceProvider.GetRequiredService<IQueueConsumerService>();
            _queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
            AsyncEventHandler<BasicDeliverEventArgs> bo = async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var obj = JsonSerializer.Deserialize<AlgorithmDetails>(body);


                if (obj != null && obj is AlgorithmDetails)    {
                    try {
                        var wynik = await ProcessData(obj.SessionId);

                        var result = await algorithmRepository.AddAsync(new AlgorithmResultEntry() {
                            Id = Guid.NewGuid().ToString(),
                            SessionId = obj.SessionId,
                            AlgorithmName = AGENT_NAME,
                            Result = wynik,
                            SummaryId = obj.SummaryId
                        }, cancellationToken);

                        var msg = JsonSerializer.Serialize(result);

                        await _queueService.Publish(msg, RabbitMQConfiguration.SolutionSupervisorRoute);
                    } catch (Exception ex) {
                        _logger.LogError(ex.Message);
                        await _queueService.Publish(string.Empty, RabbitMQConfiguration.SolutionSupervisorRoute);
                    }
                }
            };

            await _queueConsumerService.StartAsync(bo, RabbitMQConfiguration.SolutionDRoute,cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            return _queueConsumerService.StopAsync(cancellationToken);
        }

        private async Task<bool> ProcessData(string sessionId) {
            var dataModel = await new DataExtractor()
                    .GetData(sessionId, 0, 1);

            var wynik = await geminiAgentService.FindAnomalies(dataModel);

            return wynik;
        }
    }
}
