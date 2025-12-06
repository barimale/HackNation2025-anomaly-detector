using Algorithm.Common.Model;
using Questdb.Net;
using System.Text.Json;
using UploadStreamToQuestDB.API.Model;
using UploadStreamToQuestDB.Infrastructure.Utilities;

namespace Algorithms.Common.QuestDB.Service {
    public class DataExtractor {
        private const string GetCountQuery = "SELECT StationId FROM ";
        public async Task<long> GetCount(string sessionId, string endpoint = "http://127.0.0.1") {
            var questDbClient = new QuestDBClient(endpoint);
            var queryApi = questDbClient.GetQueryApi();
            var count = queryApi.Query(GetCountQuery + "'" + sessionId + "'");

            return count.Count;
        }
        public async Task<string> GetData(string sessionId, int pageIndex, int pageSize, string endpoint = "http://127.0.0.1") {
            var questDbClient = new QuestDBClient(endpoint);

            var request = new PaginationRequest() {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var query = BuildQuery(request, sessionId);
            var queryApi = questDbClient.GetQueryApi();
            var dataModel = await queryApi.QueryEnumerableAsync<WeatherDataResult>(query);
            var requestJson = JsonSerializer.Serialize(dataModel);

            return requestJson;
        }

        public async Task<IEnumerable<WeatherDataResult>> GetDataAsList(string sessionId, int pageIndex, int pageSize, string endpoint = "http://127.0.0.1") {
            var questDbClient = new QuestDBClient(endpoint);

            var request = new PaginationRequest() {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var query = BuildQuery(request, sessionId);
            var queryApi = questDbClient.GetQueryApi();
            var dataModel = await queryApi.QueryEnumerableAsync<WeatherDataResult>(query);

            return dataModel;
        }

        private string BuildQuery(PaginationRequest request, string sessionId) {
            var query = new QueryBuilder()
                .WithSessionId(sessionId)
                .WithDateRange(request.StartDate, request.EndDate)
                .WithPageIndexAndSize(request.PageIndex, request.PageSize)
                .Build();

            return query;
        }
    }
}
