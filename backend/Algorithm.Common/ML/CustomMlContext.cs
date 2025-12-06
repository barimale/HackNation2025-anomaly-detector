using Algorithm.Common.Model;
using Microsoft.ML;

namespace Algorithm.Common.ML
{
    public class CustomMlContext : ICustomMlContext
    {
        public bool DetectAnomaliesBySpike(IList<WeatherDataResult> dataFromDatabase, string modelPath) {
            // Create MLContext to be shared across the model creation workflow objects.
            var mlcontext = new MLContext();

            // STEP 1: Load the data into IDataView.
            IDataView dataView = mlcontext.Data.LoadFromEnumerable<WeatherDataResult>(dataFromDatabase);

            ITransformer tansformedModel = mlcontext.Model.Load(modelPath, out var modelInputSchema);

            // Step 3: Apply data transformation to create predictions.
            IDataView transformedData = tansformedModel.Transform(dataView);
            var predictions = mlcontext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);

            return predictions.Any(p => p.Prediction[0] == 1);
        }

        public bool DetectAnomaliesBychangePoint(IList<WeatherDataResult> dataFromDatabase, string modelPath) {
            // Create MLContext to be shared across the model creation workflow objects.
            var mlcontext = new MLContext();

            // STEP 1: Load the data into IDataView.
            IDataView dataView = mlcontext.Data.LoadFromEnumerable<WeatherDataResult>(dataFromDatabase);

            ITransformer tansformedModel = mlcontext.Model.Load(modelPath, out var modelInputSchema);

            // Step 3: Apply data transformation to create predictions.
            IDataView transformedData = tansformedModel.Transform(dataView);
            var predictions = mlcontext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);

            return predictions.Any(p => p.Prediction[0] == 1);
        }
    }
}
