using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MessageTest.Attributes
{
	public class ValidateModelAttribute : Attribute, IAuthorizationFilter
	{
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			if(!context.ModelState.IsValid)
			{
				var errors = string.Join(", ", context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
				context.Result = new BadRequestObjectResult(context.ModelState);
			}
		}
	}
}
