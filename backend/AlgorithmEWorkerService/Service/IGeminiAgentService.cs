using Algorithm.Common.Model;

namespace Algorithm.E.WorkerService.Service {
    public interface IGeminiAgentService {
        Task<bool> FindAnomalies(IEnumerable<WeatherDataResult> input);
    }
}
