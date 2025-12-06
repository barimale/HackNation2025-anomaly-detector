
using Algorithm.Common.Model;

namespace Algorithm.D.WorkerService.Service {
    public interface IGeminiAgentService {
        Task<bool> FindAnomalies(IEnumerable<WeatherDataResult> input);
    }
}
