using TypeGen.Core.TypeAnnotations;

namespace MSSql.Domain {
    [ExportTsInterface]
    public class AlgorithmSummary {
        public string Id { get; set; }
        public bool? VotedResult { get; set; }
        public IEnumerable<AlgorithmResult> Results { get; set; } = new List<AlgorithmResult>();
    }
}
