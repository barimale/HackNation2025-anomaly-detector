
using Algorithm.Common.Model;

namespace Algorithm.Common.ML {
    public interface ICustomMlContext {
        bool DetectAnomaliesBychangePoint(IList<RTGFileDetails> dataFromDatabase, string modelPath);
        bool DetectAnomaliesBySpike(IList<RTGFileDetails> dataFromDatabase, string modelPath);
    }
}
