using Microsoft.ML.Data;

namespace Algorithm.Common.Model {
    public class WeatherDataResult {
        [LoadColumn(0)]
        public required string StationId { get; init; }
        [LoadColumn(1)]
        public required float QN { get; init; }
        [LoadColumn(2)]
        public required float PP_10 { get; init; }
        [LoadColumn(3)]
        public required float TT_10 { get; init; }
        [LoadColumn(4)]
        public required float TM5_10 { get; init; }
        [LoadColumn(5)]
        public required float RF_10 { get; init; }
        [LoadColumn(6)]
        public required float TD_10 { get; init; }
        [LoadColumn(7)]
        public required DateTime Timestamp { get; init; }
    }
}
