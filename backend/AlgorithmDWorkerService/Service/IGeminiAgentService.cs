
namespace Algorithm.D.WorkerService.Service {
    public interface IGeminiAgentService {
        Task<bool> FindAnomalies(string data);
    }
}
