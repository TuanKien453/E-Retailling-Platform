using E_Retalling_Portal.Models.Enums;
using E_Retalling_Portal.Services;
using GroqSharp;
using GroqSharp.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Controllers
{
    public class AiAssistantController : Controller
    {
        private readonly GroqAiService _aiService;

        public AiAssistantController(GroqAiService aiService)
        {
            _aiService = aiService;
        }
        [HttpPost]
        public async Task<IActionResult> Ask(string question)
        {
            String prompt = question;
            string response = await _aiService.AskQuestionAsync(prompt);
            return Ok(response);
        }
    }
}
