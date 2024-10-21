using DotnetGeminiSDK.Client;
using DotnetGeminiSDK.Client.Interfaces;
using DotnetGeminiSDK.Model.Request;
using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Services;
using GroqSharp;
using GroqSharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using E_Retalling_Portal.Models;
using E_Retalling_Portal.Models.Query;
using System.Web;
using Newtonsoft.Json;

namespace E_Retalling_Portal.Controllers
{
    public class AiAssistantController : Controller
    {
        private readonly GroqAiService _aiService;
        private readonly GeminiClient _geminiClient;

        public AiAssistantController(GroqAiService aiService, GeminiClient geminiClient)
        {
            _aiService = aiService;
            _geminiClient = geminiClient;
        }

        //[HttpPost]
        //public async Task<IActionResult> Ask(string question)
        //{
        //    String prompt = question;
        //    string response = await _aiService.AskQuestionAsync(prompt);
        //    return Ok(response);
        //}

        [HttpPost]
        public async Task<IActionResult> Ask(string question)
        {
            try
            {
                var context = new Context();
                var product = context.Products.GetProduct().ToList();
                string productSummary = string.Join("\n", product.Select(p =>
                    $"Product {p.id}: {p.name}, Category:{p.category.name}, Price: ${p.price}, We have {p.quantity} units available"));
                var category = context.Categories.GetCategories().ToList();
                string categorySummary = string.Join("\n", category.Select(c =>
                    $"Category {c.id}, CategoryName:{c.name}, ParentId:{c.parentCategoryId}"));

                var messages = new List<Content>
                {
                    new Content
                    {
                        Role = "User",
                        Parts = new List<Part>
                        {
                            new Part
                            {
                                Text = question
                            }
                        }
                    },
                    new Content
                    {
                        Role = "model",
                        Parts = new List<Part>
                        {
                            new Part
                            {
                                Text = "You are a helpful assistant. Here are some products and categories in our store. Use this information to answer the user's query."
                            },
                            new Part
                            {
                                Text = productSummary
                            },
                            new Part
                            {
                                Text = categorySummary 
                            }
                        }
                    }
                };
                
                
                var response = await _geminiClient.TextPrompt(messages);
                if (response?.Candidates != null && response.Candidates.Any())
                {
                    var texts = response.Candidates
                        .Select(c => string.Join(" ", c.Content.Parts.Select(p => p.Text)))
                        .ToList();
                    return Ok(texts.FirstOrDefault());
                }

                return NotFound("No response");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
