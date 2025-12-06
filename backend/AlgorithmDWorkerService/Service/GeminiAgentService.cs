using Mscc.GenerativeAI;

namespace Algorithm.D.WorkerService.Service {
    public class GeminiAgentService : IGeminiAgentService {
        private readonly string apiKey;
        private readonly string modelName;
        public GeminiAgentService(string apiKey, string modelName) {
            this.apiKey = apiKey;
            this.modelName = modelName; // Model.Gemini25FlashLite;
        }

        public async Task<bool> FindAnomalies(string data) {
            var prompt = "Poszukaj anomalii w poniższych danych:\n" + data
                + "\n jeśli znajdziesz anomalię to zwróć 1 bez wyjaśnienia w przeciwnym razie zwróć 0 bez wyjaśnienia"
                + "Zwróć tylko jedną liczbę.";
            var googleAI = new GoogleAI(apiKey: apiKey);
            var model = googleAI.GenerativeModel(model: modelName);
            var response = await model.GenerateContent(prompt);
            var result = long.Parse(response.Text);
            return result == 1;
        }
    }
}
