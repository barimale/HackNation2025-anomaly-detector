using System.ComponentModel.DataAnnotations.Schema;

namespace MSSql.Infrastructure.Entities {
    public class AlgorithmResultEntry {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string SessionId { get; set; }
        public string AlgorithmName { get; set; }
        public bool? Result { get; set; }

        public string? SummaryId { get; set; }
        public AlgorithmSummaryEntry Summary { get; set; }
    }
}
