using TypeGen.Core.TypeAnnotations;

namespace MSSql.Domain {
    [ExportTsInterface]
    public class AlgorithmResult {
        public string Id { get; set; }
        public string SessionId { get; set; }
        public string AlgorithmName { get; set; }
        public bool? Result { get; set; }
    }
}
