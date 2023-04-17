using HUST.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HUST.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController, Authorize]
    public class BaseApiController : ControllerBase
    {
        protected IHustServiceCollection ServiceCollection;

        public BaseApiController(IHustServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }
    }
}
