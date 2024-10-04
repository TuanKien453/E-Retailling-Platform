namespace E_Retalling_Portal.Services.ExtendService
{
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System;
    using System.Linq;

    public static class ModelStateExtensions
    {
        public static void ReadErrors(this ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                var errors = modelState.Values.SelectMany(v => v.Errors);

                foreach (var error in errors)
                {
                    var errorMessage = error.ErrorMessage;
                    Console.WriteLine($"model error: {errorMessage}");
                }
            }
        }
    }
}
