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

        public UserController(IHustServiceCollection serviceCollection) : base (serviceCollection)
        {
        }


        [HttpGet]
        public User GetUser()
        {
            return ServiceCollection.AuthUtil.GetCurrentUser();
        }
    }
}
