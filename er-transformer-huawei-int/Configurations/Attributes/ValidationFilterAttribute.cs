using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using er.library.dto.Response.Errors;

namespace er_transformer_huawei_int.Configurations.Attributes
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = new List<ValidationError>();
                foreach (var action in context.ModelState)
                    foreach (var errorMessage in action.Value.Errors.Select(x => x.ErrorMessage))
                        errors.Add(new ValidationError(action.Key, errorMessage));

                context.Result = new UnprocessableEntityObjectResult(new Error<ValidationError>(errors));
            }
        }
        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
