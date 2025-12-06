using System.ComponentModel.DataAnnotations.Schema;

namespace MSSql.Infrastructure.Entities {
    public class RankingEntry {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string AlgorithmId { get; set; }
        public long Score { get; set; }
    }
}
