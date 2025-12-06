using Common.RabbitMQ;
using Configuration.Common;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSSql.Infrastructure.Entities;
using MSSql.Infrastructure.Repositories.Abstractions;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UploadStreamToQuestDB.API.Hub;

public class BackgroundNotifier : BackgroundService {
    private readonly ILogger<BackgroundNotifier> _logger;
    private readonly IHubContext<AgentsStatusHub, IAgentsStatusHub> _broadcastLocalesStatus;
    private readonly IQueueConsumerService consumer;
    private readonly IAlgorithmSummaryRepository algorithmSummaryRepository;
    private readonly IRankingRepository rankingRepository;

    public BackgroundNotifier(ILogger<BackgroundNotifier> logger
        ,IHubContext<AgentsStatusHub, IAgentsStatusHub> broadcastLocalesStatus,
        IServiceScopeFactory factory) {
        _logger = logger;
        _broadcastLocalesStatus = broadcastLocalesStatus;
        using var scope = factory.CreateScope();
        this.consumer = scope.ServiceProvider.GetRequiredService<IQueueConsumerService>();
        algorithmSummaryRepository = scope.ServiceProvider.GetRequiredService<IAlgorithmSummaryRepository>();
        rankingRepository = scope.ServiceProvider.GetRequiredService<IRankingRepository>();

        AsyncEventHandler<BasicDeliverEventArgs> bo = async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var obj = JsonSerializer.Deserialize<AlgorithmResultEntry>(body);

            var summaryId = obj?.SummaryId;
            var summary = await algorithmSummaryRepository.GetByIdAsync(summaryId);
            bool? mainResult = null;

            var rankings = await rankingRepository.GetAllAsync(string.Empty, CancellationToken.None);
            await _broadcastLocalesStatus.Clients.All.OnChangeAsync(
                    obj.AlgorithmName,
                    obj.Result.Value.ToString(),
                    (int)rankings.FirstOrDefault(p=> p.AlgorithmId == obj.AlgorithmName).Score,
                    obj.SessionId);

            if (summary.Results.Count() == Constants.AlgorithmsCount) {
                // zrobic glosowanie
                var ranks = await DoVoting(summary);
                //zapisac wynik glosowania
                mainResult = ranks.Item2;
                // zaktualizowac ranking
                await UpdateRanking(summary, ranks.Item2, ranks.Item1);
                if (mainResult.HasValue) {
                    summary.VotedResult = mainResult.Value;
                    var updated = await algorithmSummaryRepository.UpdateAsync(summary, default);
                }
            }

            if (obj != null && obj is AlgorithmResultEntry && summary.Results.Count() == Constants.AlgorithmsCount) {
                if(mainResult.HasValue && mainResult.Value) {
                    // jesli anomalia wystepuje
                    var overallResult = new OverallResult {
                        SummaryId = summary.Id,
                        Result = mainResult.Value,
                        DetectedAt = DateTime.UtcNow,
                        Details = "Anomalia wykryta przez system.",
                    };
                    await _broadcastLocalesStatus.Clients.All.OnAnomalyDetectedAsync(overallResult);
                } else {
                    var overallResult = new OverallResult {
                        SummaryId = summary.Id,
                        Result = mainResult.Value,
                        DetectedAt = DateTime.UtcNow,
                        Details = "Anomalia niewykryta przez system.",
                    };
                    await _broadcastLocalesStatus.Clients.All.OnAnomalyNotDetectedAsync(overallResult);
                }
            }
        };

        this.consumer.StartAsync(bo, RabbitMQConfiguration.SolutionSupervisorRoute);
    }

    private async Task UpdateRanking(AlgorithmSummaryEntry summary, bool mainResult, RankingEntry[] ranks) {
        try {
            foreach (var result in summary.Results) {
                var rank = ranks.Where(p => p.AlgorithmId == result.AlgorithmName).FirstOrDefault();
                if (result.Result == mainResult) {
                    rank.Score += 1;
                } else {
                    rank.Score -= 1;
                    if (rank.Score <= 0) {
                        rank.Score = 1;
                    }
                }
                var updated = await rankingRepository.UpdateAsync(rank, CancellationToken.None);
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error updating ranking");
            throw;
        }
    }

    private async Task<Tuple<RankingEntry[], bool>> DoVoting(AlgorithmSummaryEntry summary) {
        try {
            var ranks = await rankingRepository.GetAllAsync(string.Empty, CancellationToken.None);

            var allResults = summary.Results
                .Where(p => p.Result.HasValue)
                .SelectMany(p => {
                    var n = (int)ranks.FirstOrDefault(pp => pp.AlgorithmId == p.AlgorithmName).Score;
                    return Enumerable.Repeat(p.Result.Value, n);
                })
                .ToList();
            var trues = allResults.Count(p => p == true);
            var falses = allResults.Count(p => p == false);
            var mainResult = trues > falses;

            return Tuple.Create(ranks, mainResult);
        } catch (Exception ex) {
            _logger.LogError(ex, "Error during voting process");
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            // intentionally left blank
            await Task.Delay(1000);
        }
    }
}
