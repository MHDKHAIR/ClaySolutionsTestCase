using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LocksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly ICurrentUserService currentUser;

        public BaseController()
        {

        }
        public BaseController(ICurrentUserService currentUser) => this.currentUser = currentUser;

    }
}
