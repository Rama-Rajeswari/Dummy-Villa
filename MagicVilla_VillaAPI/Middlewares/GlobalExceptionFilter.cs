using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MagicVilla_VillaAPI.Middlewares
{
    public class GlobalExceptionFilter:IExceptionFilter
    {
        public void OnException(ExceptionContext context)
    {
        var errorResponse = new
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = "An error occurred while processing your request.",
            Details = context.Exception.Message
        };

        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        context.ExceptionHandled = true; 
    }
    }
}

 
