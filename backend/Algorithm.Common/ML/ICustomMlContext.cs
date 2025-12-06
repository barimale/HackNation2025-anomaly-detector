
using Algorithm.Common.Model;

namespace Algorithm.Common.ML {
    public interface ICustomMlContext {
        bool DetectAnomaliesBychangePoint(IList<WeatherDataResult> dataFromDatabase, string modelPath);
        bool DetectAnomaliesBySpike(IList<WeatherDataResult> dataFromDatabase, string modelPath);
    }
}
