using Microsoft.ML.Data;

namespace Algorithm.Common.Model {
    public class RTGFileDetails {
        [LoadColumn(0)]
        public required string SessionId { get; init; }
        [LoadColumn(1)]
        public required string FilePath { get; init; }
        public required DateTime Timestamp { get; init; }
    }
}
