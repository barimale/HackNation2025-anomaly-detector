using Algorithm.Common.Model;
using Algorithms.Common;
using Mscc.GenerativeAI;

namespace Algorithm.D.WorkerService.Service {
    public class GeminiAgentService : IGeminiAgentService {
        private readonly string apiKey;
        private readonly string modelName;
        public GeminiAgentService(string apiKey, string modelName) {
            this.apiKey = apiKey;
            this.modelName = modelName;
        }

        public async Task<bool> FindAnomalies(IEnumerable<RTGFileDetails> input) {
            var file = File.ReadAllText(input.First().FilePath); // WIP lub ponizsza linia zamiast tej 
            //var convertedFile = ImageConverter.LoadBmpAsFloatArray(input.First().FilePath);
            var prompt = "Poszukaj anomalii w poniższych danych:\n" + file
                + "\n jeśli znajdziesz anomalię to zwróć 1 w przeciwnym razie zwróć 0. "
                + "Zwróć tylko jedną liczbę oraz współrzędne prostokąta w którym znajduje się anomalia"
                + "w formacie json";
            var googleAI = new GoogleAI(apiKey: apiKey);
            var model = googleAI.GenerativeModel(model: modelName);
            var response = await model.GenerateContent(prompt);
            var result = long.Parse(response.Text); // WIP obsluzyc jsona 
            return result == 1;
        }
    }
}
