namespace Algorithm.E.WorkerService.Service {
    public interface IGeminiAgentService {
        Task<bool> FindAnomalies(string data);
    }
}
