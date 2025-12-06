namespace Common.RabbitMQ.Model {
    public abstract class AlgorithmDetailsBase {

        public string Id { get; set; }
        public string SessionId { get; set; }
        public string SummaryId { get; set; }
    }

    public class AlgorithmDetails: AlgorithmDetailsBase {
        //intentionally left blank
    }
}
