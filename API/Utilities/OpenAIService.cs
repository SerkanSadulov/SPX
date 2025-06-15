using OpenAI_API;
using OpenAI_API.Completions;

namespace API.Services
{
    public class OpenAIService
    {
        private readonly OpenAIAPI _api;

        public OpenAIService(IConfiguration configuration)
        {
            string apiKey = "";
            _api = new OpenAIAPI(apiKey);
        }

        public async Task<string> GetChatGPTResponseAsync(string prompt)
        {
            var completionRequest = new CompletionRequest
            {
                Prompt = prompt,
                Model = "gpt-3.5-turbo",
                MaxTokens = 150
            };

            var result = await _api.Completions.CreateCompletionAsync(completionRequest);

            if (result.Completions != null && result.Completions.Count > 0)
            {
                return result.Completions[0].Text.Trim();
            }

            return "No response generated.";
        }
    }
}