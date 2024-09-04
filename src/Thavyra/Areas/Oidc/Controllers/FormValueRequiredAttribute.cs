using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Thavyra.Oidc.Controllers;

public class FormValueRequiredAttribute : ActionMethodSelectorAttribute
{
    private readonly string _name;

    public FormValueRequiredAttribute(string name)
    {
        _name = name;
    }

    public override bool IsValidForRequest(RouteContext context, ActionDescriptor action)
    {
        if (string.Equals(context.HttpContext.Request.Method, HttpMethod.Get.Method, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(context.HttpContext.Request.Method, HttpMethod.Head.Method, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(context.HttpContext.Request.Method, HttpMethod.Delete.Method, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(context.HttpContext.Request.Method, HttpMethod.Trace.Method, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.IsNullOrEmpty(context.HttpContext.Request.ContentType))
        {
            return false;
        }

        if (!context.HttpContext.Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !string.IsNullOrEmpty(context.HttpContext.Request.Form[_name]);
    }
}