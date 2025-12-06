using Algorithm.Common.Model;
using Algorithms.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Mscc.GenerativeAI;

namespace Algorithm.E.WorkerService.Service {
    public class GeminiAgentService : IGeminiAgentService {
        private readonly string apiKey;
        private readonly string modelName;

        public GeminiAgentService(string apiKey, string modelName) {
            this.apiKey = apiKey;
            this.modelName = modelName;
        }

        public async Task<bool> FindAnomalies(IEnumerable<RTGFileDetails> input) {
            var convertedFile = ImageConverter.LoadBmpAsFloatArray(input.First().FilePath);
            var prompt = "Poszukaj anomalii w poniższych danych:\n" + convertedFile
                + "\n jeśli znajdziesz anomalię to zwróć 1 w przeciwnym razie zwróć 0. "
                + "Zwróć tylko jedną liczbę oraz współrzędne prostokąta w którym znajduje się anomalia"
                + "w formacie json";
            var googleAI = new GoogleAI(apiKey: apiKey);
            var model = googleAI.GenerativeModel(model: modelName);
            var response = await model.GenerateContent(prompt);
            var result = long.Parse(response.Text);

            return result == 1;
        }
    }
}
