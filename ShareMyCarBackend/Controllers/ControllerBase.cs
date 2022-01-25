using Domain;
using DomainServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShareMyCarBackend.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly IUserRepository _userRepository;

        public BaseController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected static User GetUserId()
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "UserId").Value);
            return _userRepository.GetById(id);
        }
    }
}
