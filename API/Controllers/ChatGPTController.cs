using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class ChatGptController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private const string OpenAiApiKey = "";
    private const string OpenAiEndpoint = "";

    public ChatGptController(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenAiApiKey}");
    }

    [HttpPost]
    public async Task<IActionResult> GetChatGptResponse([FromBody] PromptRequest request)
    {
        var requestBody = new
        {
            model = "gpt-4o",
            messages = new[]
            {
                new { role = "user", content = request.Prompt }
            },
            temperature = 0.3
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(OpenAiEndpoint, content);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);
            var chatGptResponse = responseObject.choices[0].message.content.ToString();
            return Ok(chatGptResponse);
        }
        else
        {
            return StatusCode((int)response.StatusCode, "Error calling ChatGPT API");
        }
    }

    [HttpPost("Wizard")]
    public async Task<IActionResult> Wizard([FromBody] WizardRequest req)
    {
        Console.WriteLine($"UserNeeds: {req.UserNeeds}");
        Console.WriteLine($"Products count: {req.Products?.Count ?? 0}");

        if (req.Products != null)
        {
            foreach (var product in req.Products)
            {
                Console.WriteLine($"Product: {product.Name}, Price: {product.Price}, Description: {product.Description}");
            }
        }

        if (req.Products == null || req.Products.Count == 0)
        {
            return BadRequest("Няма налични продукти за анализ");
        }

        var maxRecommendations = Math.Min(5, req.Products.Count);

        var prompt = BuildRecommendationPrompt(req.UserNeeds, req.Products, maxRecommendations);

        var requestBody = new
        {
            model = "gpt-4o",
            messages = new[] {
                new {
                    role = "system",
                    content = "Ти си експерт консултант за продукти и услуги. Винаги отговаряш на български език и даваш структурирани, полезни препоръки."
                },
                new {
                    role = "user",
                    content = prompt
                }
            },
            temperature = 0.3,
            max_tokens = 3500
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(OpenAiEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"ChatGPT API Error: {errorContent}");
            return StatusCode((int)response.StatusCode, "ChatGPT грешка");
        }

        dynamic obj = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        string result = obj.choices[0].message.content;

        Console.WriteLine($"AI Response: {result}");

        return Ok(result);
    }

    private string BuildRecommendationPrompt(string userNeeds, List<ProductInfo> products, int maxRecommendations)
    {
        var prompt = new StringBuilder();

        prompt.AppendLine("ЗАДАЧА: Анализирай следните продукти/услуги и препоръчай най-подходящите според нуждите на потребителя.");
        prompt.AppendLine();

        prompt.AppendLine("НУЖДИ НА ПОТРЕБИТЕЛЯ:");
        prompt.AppendLine(userNeeds);
        prompt.AppendLine();

        prompt.AppendLine("НАЛИЧНИ ПРОДУКТИ/УСЛУГИ:");
        for (int i = 0; i < products.Count; i++)
        {
            var product = products[i];
            prompt.AppendLine($"{i + 1}. {product.Name}");
            prompt.AppendLine($"   Описание: {product.Description}");
            prompt.AppendLine($"   Цена: ${product.Price}");
            prompt.AppendLine();
        }

        prompt.AppendLine("ИНСТРУКЦИИ:");
        prompt.AppendLine($"1. Върни точно {maxRecommendations} препоръки (или по-малко ако няма достатъчно подходящи продукти)");
        prompt.AppendLine("2. Сортирай ги по най-подходящи за нуждите на потребителя");
        prompt.AppendLine("3. За всяка препоръка използвай ТОЧНО този формат:");
        prompt.AppendLine("   [НОМЕР]. **[ТОЧНОТО ИМЕ НА ПРОДУКТА]**");
        prompt.AppendLine("   **[Обяснение в 2-3 изречения защо този продукт е подходящ]**");
        prompt.AppendLine();
        prompt.AppendLine("4. Използвай ** за bold форматиране на името и обяснението");
        prompt.AppendLine("5. Разгледай внимателно цената, качеството и съответствието с нуждите");
        prompt.AppendLine("6. Ако има продукти или услуги с много различни цени, спомени съотношението цена-качество");
        prompt.AppendLine("7. Отговори само на български език");
        prompt.AppendLine("8. Ако въпростът не свързан с нито един от подадените продукти или услуги, просто върни че няма такъв продукт");

        return prompt.ToString();
    }

    public class WizardRequest
    {
        public string UserNeeds { get; set; }
        public List<ProductInfo> Products { get; set; }
    }

    public class ProductInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}

public class PromptRequest
{
    public string Prompt { get; set; }
}