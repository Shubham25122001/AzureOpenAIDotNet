using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DotNetConf.Models;
using Azure.AI.OpenAI;
using Azure;

namespace DotNetConf.Controllers;

public class ChatController : Controller
{
    string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
    string key = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");
    string model = Environment.GetEnvironmentVariable("AZURE_OPENAI_MODEL");

    private readonly ILogger<HomeController> _logger;

    public ChatController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetResponse(string userMessage)
    {
        OpenAIClient client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));
        var chatCompletionOptions = new ChatCompletionsOptions(){
            Messages ={
                new ChatMessage(ChatRole.System, "You are a helpful AI assistant"),
                new ChatMessage(ChatRole.User, "Does Azure support GPT-4 ?"),
                new ChatMessage(ChatRole.Assistant, "Yes it does"),
                new ChatMessage(ChatRole.User, userMessage)
            },
            MaxTokens = 400
        };

        Response<ChatCompletions> response = await client.GetChatCompletionsAsync(deploymentOrModelName: model, chatCompletionOptions);
        var botRespones = response.Value.Choices.First().Message.Content;
        return Json(new { response = botRespones });   
    }

}
