using System.Threading.Tasks;

namespace UploadStreamToQuestDB.API.Hub {
    public interface IAgentsStatusHub
    {
        Task OnChangeAsync(string agentName, string value, int number, string sessionId);
        Task OnAnomalyNotDetectedAsync(OverallResult id);
        Task OnAnomalyDetectedAsync(OverallResult id);
    }
}
