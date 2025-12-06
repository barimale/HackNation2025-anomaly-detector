using System;
using TypeGen.Core.TypeAnnotations;
[ExportTsInterface]
public class OverallResult {
    public string SummaryId { get; set; }
    public DateTime DetectedAt { get; set; }
    public string Details { get; set; }
    public bool Result { get; internal set; }
}
