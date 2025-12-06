using Algorithm.Common.ML;
using Algorithm.Common.Model;
using Configuration.Common;
using Microsoft.ML;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Common.UTs {
    public class DataCreator {

        private static MLContext mlContext;

        [Fact]
        public void Create_folders() {
            //given
            string pathA = FolderStructure.SolutionAPath;
            string pathB = FolderStructure.SolutionBPath;
            string pathC = FolderStructure.SolutionCPath;
            string pathD = FolderStructure.SolutionDPath;
            string pathE = FolderStructure.SolutionEPath;

            //when
            Directory.CreateDirectory(pathA);
            Directory.CreateDirectory(pathB);
            Directory.CreateDirectory(pathC);
            Directory.CreateDirectory(pathD);
            Directory.CreateDirectory(pathE);

            //then
            bool existsA = Directory.Exists(pathA);
            bool existsB = Directory.Exists(pathB);
            bool existsC = Directory.Exists(pathC);
            bool existsD = Directory.Exists(pathD);
            bool existsE = Directory.Exists(pathE);

            Assert.True(existsA);
            Assert.True(existsB);
            Assert.True(existsC);
            Assert.True(existsD);
            Assert.True(existsE);
        }

        [Fact]
        public void Create_models() {
            //given
            mlContext = new MLContext();

            // Assign the Number of records in dataset file to constant variable.
            const int size = 36;

            // Load the data into IDataView.
            // This dataset is used for detecting spikes or changes not for training.
            IDataView dataView = mlContext.Data.LoadFromTextFile<WeatherDataResult>(
                path: GetAbsolutePath(FolderStructure.DatasetRelativePath),
                hasHeader: true,
                separatorChar: ',');

            // Detect temporary changes (spikes) in the pattern.
            ITransformer trainedSpikeModelRF_10 = DetectSpike(size, dataView, nameof(WeatherDataResult.RF_10));
            ITransformer trainedSpikeModelPP_10 = DetectSpike(size, dataView, nameof(WeatherDataResult.PP_10));

            // Detect persistent change in the pattern.
            //when
            ITransformer trainedChangePointModelRF_10 = DetectChangepoint(size, dataView, nameof(WeatherDataResult.RF_10));
            ITransformer trainedChangePointModelPP_10 = DetectChangepoint(size, dataView, nameof(WeatherDataResult.PP_10));

            SaveModel(mlContext, trainedSpikeModelRF_10, FolderStructure.SpikeModelPath1, dataView);
            SaveModel(mlContext, trainedSpikeModelPP_10, FolderStructure.SpikeModelPath2, dataView);

            SaveModel(mlContext, trainedChangePointModelRF_10, FolderStructure.ChangePointModelPath1, dataView);
            SaveModel(mlContext, trainedSpikeModelPP_10, FolderStructure.ChangePointModelPath2, dataView);

        }

        private ITransformer DetectSpike(int size, IDataView dataView, string inputColumnName) {
            Console.WriteLine("===============Detect temporary changes in pattern===============");

            // STEP 1: Create Estimator.
            var estimator = mlContext.Transforms.DetectIidSpike(outputColumnName: nameof(ProductSalesPrediction.Prediction), inputColumnName: inputColumnName, confidence: 95, pvalueHistoryLength: size / 4);

            // STEP 2:The Transformed Model.
            // In IID Spike detection, we don't need to do training, we just need to do transformation. 
            // As you are not training the model, there is no need to load IDataView with real data, you just need schema of data.
            // So create empty data view and pass to Fit() method. 
            ITransformer tansformedModel = estimator.Fit(CreateEmptyDataView());

            // STEP 3: Use/test model.
            // Apply data transformation to create predictions.
            IDataView transformedData = tansformedModel.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);

            return tansformedModel;
        }

        private static ITransformer DetectChangepoint(int size, IDataView dataView, string inputColumnName) {
            Console.WriteLine("===============Detect Persistent changes in pattern===============");

            // STEP 1: Setup transformations using DetectIidChangePoint.
            var estimator = mlContext.Transforms.DetectIidChangePoint(outputColumnName: nameof(ProductSalesPrediction.Prediction), inputColumnName: inputColumnName, confidence: 95, changeHistoryLength: size / 4);

            // STEP 2:The Transformed Model.
            // In IID Change point detection, we don't need need to do training, we just need to do transformation. 
            // As you are not training the model, there is no need to load IDataView with real data, you just need schema of data.
            // So create empty data view and pass to Fit() method.  
            ITransformer tansformedModel = estimator.Fit(CreateEmptyDataView());

            // STEP 3: Use/test model.
            // Apply data transformation to create predictions.
            IDataView transformedData = tansformedModel.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);

            return tansformedModel;
        }

        private static void SaveModel(MLContext mlcontext, ITransformer trainedModel, string modelPath, IDataView dataView) {
            Console.WriteLine("=============== Saving model ===============");
            mlcontext.Model.Save(trainedModel, dataView.Schema, modelPath);

            Console.WriteLine($"The model is saved to {modelPath}");
        }

        public static string GetAbsolutePath(string relativePath) {
            var _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        private static IDataView CreateEmptyDataView() {
            //Create empty DataView. We just need the schema to call fit()
            IEnumerable<WeatherDataResult> enumerableData = new List<WeatherDataResult>();
            var dv = mlContext.Data.LoadFromEnumerable(enumerableData);
            return dv;
        }
    }
}
