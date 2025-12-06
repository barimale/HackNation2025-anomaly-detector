using CsvHelper;
using CsvHelper.Configuration;
using UploadStreamToQuestDB.Infrastructure.Services;
using System.Globalization;
using System.Text;
using UploadStreamToQuestDB.Application.Handlers.Abstraction;
using UploadStreamToQuestDB.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UploadStreamToQuestDB.Application.Handlers {
    public class DataIngestionerHandler : AbstractHandler, IDataIngestionerHandler {
        private readonly IConfiguration configuration;
        private readonly IQueryIngestionerService _queryIngestionerService;
        private readonly ILogger<DataIngestionerHandler> _logger;
        public DataIngestionerHandler(
            IConfiguration configuration,
            IQueryIngestionerService queryIngestionerService,
            ILogger<DataIngestionerHandler> logger) {
            this.configuration = configuration;
            this._queryIngestionerService = queryIngestionerService;
            this._logger = logger;
        }
        public override async Task<object> Handle(FileModelsInput files) {
            bool isStepActive = bool.Parse(configuration["AntivirusActive"]);

            Parallel.ForEach(files.ToDataIngestionHandler(isStepActive), (file) => {
                    Execute(files, file);
                });

            return base.Handle(files);
        }

        private void Execute(FileModelsInput files, FileModel file) {
            try {
                _queryIngestionerService.Execute(file.FilePath, files.SessionId);

                file.State.Add(FileModelState.INGESTION_READY);
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
                file.State.Add(FileModelState.INGESTION_FAILED);
            }
        }
    }
}
