using System;
using System.Threading.Tasks;

namespace UploadStreamToQuestDB.API.Hub {
    public class AgentsStatusHub : Microsoft.AspNetCore.SignalR.Hub<IAgentsStatusHub>
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public Task OnChangeAsync(string agentName, string value, int number, string sessionId) {
            return Clients?.All?.OnChangeAsync(agentName, value, number, sessionId);
        }

        public Task OnAnomalyNotDetectedAsync(OverallResult data)
        {
            return Clients?.All?.OnAnomalyNotDetectedAsync(data);
        }

        public Task OnAnomalyDetectedAsync(OverallResult data)
        {
            return Clients?.All?.OnAnomalyDetectedAsync(data);
        }
    }
}
