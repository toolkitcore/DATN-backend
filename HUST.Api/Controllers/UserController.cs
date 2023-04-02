using HUST.Core.Models.DTO;
using HUST.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HUST.Api.Controllers
{
    public class UserController : BaseApiController
    {
        public readonly IHustServiceCollection ServiceCollection;
        private readonly ILogger<UserController> _logger;

        public UserController(IHustServiceCollection serviceCollection, ILogger<UserController> logger)
        {
            ServiceCollection = serviceCollection;
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into HomeController");
        }


        [HttpGet]
        public User GetUser()
        {
            _logger.LogInformation("Hello, this is the index!");
            return ServiceCollection.AuthUtil.GetCurrentUser();
        }
    }
}
