using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MagicVilla_VillaAPI.Middlewares
{
    public class CustomStatusCodeFilter: IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
       {
        
       }
        public void OnActionExecuted(ActionExecutedContext context)
       {
          if (context.Result is ObjectResult objectResult)
         {
            int statusCode = objectResult.StatusCode ?? (int)HttpStatusCode.OK;
            string message = GetStatusMessage(statusCode);

             
            if (objectResult.Value is APIResponse apiResponse)
            {
               
                return; 
            }

            var response = new
            {
                StatusCode = statusCode,
                IsSuccess = statusCode >= 200 && statusCode < 300, 
                Message = GetStatusMessage(statusCode),
                Result = objectResult.Value
            };

            context.Result = new JsonResult(response)
            {
                StatusCode = statusCode
            };
        }
        else if (context.Result is StatusCodeResult statusCodeResult)
        {
            int statusCode = statusCodeResult.StatusCode;

            var response = new
            {
                StatusCode = statusCode,
                IsSuccess = statusCode >= 200 && statusCode < 300,
                Message = GetStatusMessage(statusCode),
                Result = (object)null
            };

            context.Result = new JsonResult(response)
            {
                StatusCode = statusCode
            };
        }
    }

       private string GetStatusMessage(int statusCode)
       {
        return statusCode switch
        {
            200 => "Request successful.",
            201 => "Resource created successfully.",
            400 => "Bad request. Please check the input.",
            401 => "Unauthorized access.",
            403 => "Forbidden request.",
            404 => "Resource not found.",
            500 => "Internal server error.",
            _ => "An unexpected status occurred."
        };
    }
}
}
  