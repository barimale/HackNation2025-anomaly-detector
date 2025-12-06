using Microsoft.ML.Data;

public class ImageData {
    [LoadColumn(0)]
    public string ImagePath { get; set; }

    [ColumnName("Label")]
    public string Label { get; set; }
}

public class ImagePrediction {
    [ColumnName("PredictedLabel")]
    public string PredictedLabel { get; set; }
    public float[] Score { get; set; }
}
