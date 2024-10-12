using GroqSharp.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Retalling_Portal.Services
{
    public class GroqAiService
    {
        private string _ApiKey {  get; set; }

        public GroqAiService(string apiKey)
        {
            _ApiKey = apiKey;
        }
        
        public async Task<String> AskQuestionAsync(string question)
        {
            var apiModel = "gemma2-9b-it";

            var groqClient = new GroqClient(_ApiKey, apiModel)
                .SetMaxTokens(512)
                .SetStructuredRetryPolicy(10)
                .SetStop("NONE")
                .SetTopP(1)
                .SetDefaultSystemMessage(new Message { Content = "You are an helpful assistant for my website", Role = MessageRoleType.Assistant });

            var prompt = question;
            var message = new Message { Content = prompt, Role = MessageRoleType.Assistant };
            
            var response = await groqClient.CreateChatCompletionAsync(message);

            if (!string.IsNullOrEmpty(response))
            {
                return response;
            }
            else
            {
                return "No response";
            }
        }
    }
}
