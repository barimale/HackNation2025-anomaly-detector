using System.ComponentModel.DataAnnotations.Schema;

namespace MSSql.Infrastructure.Entities {
    public class AlgorithmSummaryEntry {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string SessionId { get; set; }
        public bool? VotedResult { get; set; }
        public IEnumerable<AlgorithmResultEntry> Results { get; set; } = new List<AlgorithmResultEntry>();
    }
}
