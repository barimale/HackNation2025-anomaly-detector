using QuestDB;
using UploadStreamToQuestDB.Domain;
using UploadStreamToQuestDB.Domain.Utilities;

namespace UploadStreamToQuestDB.Infrastructure.Services {
    /// <summary>
    /// Service for ingesting queries into QuestDB.
    /// </summary>
    public class QueryIngestionerService : IQueryIngestionerService {
        private readonly string address;
        private readonly string port;
        private readonly string settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryIngestionerService"/> class.
        /// </summary>
        /// <param name="address">The address of the QuestDB server.</param>
        /// <param name="port">The port of the QuestDB server.</param>
        /// <param name="settings">The settings for the QuestDB connection.</param>
        public QueryIngestionerService(string address, string port, string settings) {
            this.address = address;
            this.port = port;
            this.settings = settings;
        }

        /// <summary>
        /// Executes the ingestion of a CSV file into QuestDB.
        /// </summary>
        /// <param name="file">The CSV file containing weather data.</param>
        /// <param name="sessionId">The session ID for the name of the table.</param>
        public void Execute(string filePath, string sessionId) {
            using var sender = Sender.New($"http::addr={this.address}:{this.port};{this.settings}");
            sender.Transaction(sessionId);
            try {
                var parsedDate = DateTimeUtility.yyyyMMddHHmmToDate(DateTime.Now);

                sender
                 .Column("SessionId", sessionId)
                 .Column("FilePath", filePath)
                 .At(parsedDate);

                sender.Commit();
            } catch (Exception) {
                sender.Rollback();
                throw;
            }
        }
    }
}
