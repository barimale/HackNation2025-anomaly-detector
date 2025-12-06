using TypeGen.Core.TypeAnnotations;

namespace MSSql.Domain {
    [ExportTsInterface]
    public class Ranking {
        public string Id { get; set; }
        public string AlgorithmId { get; set; }
        public long Score { get; set; }
    }
}
