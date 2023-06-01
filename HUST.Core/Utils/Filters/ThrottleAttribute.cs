using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net;

namespace HUST.Core.Utils
{
    public class ThrottleAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }
        public int Seconds { get; set; }
        public string Message { get; set; }
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _context;

        public ThrottleAttribute(IMemoryCache cache, IHttpContextAccessor context, string name, int seconds, string message)
        {
            this.Name = name;
            this.Seconds = seconds;
            this.Message = message;
            this._cache = cache;
            this._context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var clientIp = _context.HttpContext.Connection.RemoteIpAddress.ToString();
            var key = $"{Name}-{clientIp}";
            var allowExecute = false;

            if (_cache.Get(key) == null)
            {
                _cache.Set(key,
                    true, 
                    DateTime.Now.AddSeconds(Seconds) 
                ); 

                allowExecute = true;
            }

            if (!allowExecute)
            {
                if (string.IsNullOrEmpty(Message))
                {
                    Message = "You may only perform this action every {n} seconds.";
                }

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Result = new ContentResult { Content = Message };
            }
        }

    }
}
