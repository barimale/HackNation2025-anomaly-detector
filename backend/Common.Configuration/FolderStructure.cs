namespace Configuration.Common {
    public static class FolderStructure {
        public static string SolutionAPath = @"R:\SolutionA";
        public static string SolutionBPath = @"R:\SolutionB";
        public static string SolutionCPath = @"R:\SolutionC";
        public static string SolutionDPath = @"R:\SolutionD";
        public static string SolutionEPath = @"R:\SolutionE";

        public static string BaseDatasetsRelativePath = @"..//..//..//..";
        public static string DatasetRelativePath = $"{BaseDatasetsRelativePath}//big-data.txt";

        public static string ModelRelativePath1 = $"R:/SolutionA/Model1.zip";
        public static string ModelRelativePath2 = $"R:/SolutionB/Model1.zip";

        public static string SpikeModelPath1 = ModelRelativePath1;
        public static string SpikeModelPath2 = $"R:/SolutionA/Model2.zip";

        public static string ChangePointModelPath1 = ModelRelativePath2;
        public static string ChangePointModelPath2 = $"R:/SolutionB/Model2.zip";
    }
}
