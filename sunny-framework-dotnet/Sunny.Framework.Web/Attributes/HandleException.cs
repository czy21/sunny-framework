using Microsoft.Extensions.Logging;
using Sunny.Framework.Core.Exceptions;
using Sunny.Framework.Core.Model;

namespace Sunny.Framework.Web.Attributes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

public class HandleExceptionAttribute : TypeFilterAttribute
{
    public HandleExceptionAttribute() : base(typeof(GlobalExceptionFilterImpl))
    {
    }

    private class GlobalExceptionFilterImpl : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilterImpl> _logger;

        public GlobalExceptionFilterImpl(ILogger<GlobalExceptionFilterImpl> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception.GetType() == typeof(CommonException))
            {
                var ce = (CommonException)context.Exception;
                context.Result = new JsonResult(CommonResult<string>.Error(ce.Code, ce.Message));
            }
            else
            {
                _logger.LogError(context.Exception, context.Exception.Message);
                context.Result = new JsonResult(CommonResult<string>.Error());
            }

            context.ExceptionHandled = true;
        }
    }
}