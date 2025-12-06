using Algorithm.Common.ML;
using Algorithms.Common.QuestDB.Service;
using Common.RabbitMQ;
using Common.RabbitMQ.Model;
using Configuration.Common;
using MSSql.Infrastructure.Entities;
using MSSql.Infrastructure.Repositories.Abstractions;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Algorithm.A.WorkerService {
    public class Worker : BackgroundService {
        public const string AGENT_NAME = Constants.AGENT_NAME_A;
        private const string path = @"R:/SolutionA/Model1.zip";
        private const string path2 = @"R:/SolutionA/Model2.zip";

        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IAlgorithmResultRepository algorithmRepository;
        private readonly IQueueConsumerService queueConsumerService;
        private readonly IQueueService _queueService;
        private readonly ICustomMlContext mlService;

        public Worker(ILogger<Worker> logger,
            IServiceScopeFactory _scopeFactory) {
            _logger = logger;
            this._scopeFactory = _scopeFactory;
            using var scope = _scopeFactory.CreateScope();
            algorithmRepository = scope.ServiceProvider.GetRequiredService<IAlgorithmResultRepository>();
            queueConsumerService = scope.ServiceProvider.GetRequiredService<IQueueConsumerService>();
            _queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
            mlService = scope.ServiceProvider.GetRequiredService<ICustomMlContext>();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Hosted Service running.");

            AsyncEventHandler<BasicDeliverEventArgs> bo = async (model, ea) => {
                var body = ea.Body.ToArray();
                var obj = JsonSerializer.Deserialize<AlgorithmDetails>(body);

                if (obj != null) {
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

            await queueConsumerService.StartAsync(bo, RabbitMQConfiguration.SolutionARoute, cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            return queueConsumerService.StopAsync(cancellationToken);
        }

        private async Task<bool> ProcessData(string sessionId) {
            var allItems = await new DataExtractor().GetCount(sessionId);

            var totalItems = allItems;
            int totalPages = (int)Math.Ceiling(totalItems / (double)Constants.PageSize);

            var detections = new List<bool>();
            if (totalItems <= Constants.PageSize) {
                var dataModel = await new DataExtractor()
                    .GetDataAsList(sessionId, 0, (int)totalItems);

                var wynik = mlService.DetectAnomaliesBySpike(dataModel.ToList(), path);
                var wynik2 = mlService.DetectAnomaliesBySpike(dataModel.ToList(), path2);

                return wynik || wynik2;
            }
            for (int pageIndex = 0; pageIndex < totalPages; pageIndex++) {
                int skip = (pageIndex - 1) * Constants.PageSize;
                var dataModel = await new DataExtractor()
                    .GetDataAsList(sessionId, skip, Constants.PageSize);

                var wynik = mlService.DetectAnomaliesBySpike(dataModel.ToList(), path);
                var wynik2 = mlService.DetectAnomaliesBySpike(dataModel.ToList(), path2);

                detections.Add(wynik);
                detections.Add(wynik2);
                var isAnomalyPreDetected = detections.Any(x => x == true);
                if (isAnomalyPreDetected) return true;
            }
            var isAnomalyDetected = detections.Any(x => x == true);

            return isAnomalyDetected;
        }
    }
}
