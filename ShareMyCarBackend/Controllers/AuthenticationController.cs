using Domain;
using DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShareMyCarBackend.Models;
using ShareMyCarBackend.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShareMyCarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly SignInManager<IdentityUser> _signInMgr;
        private readonly IUserRepository _userRepository;

        public AuthenticationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IUserRepository userRepository)
        {
            _userMgr = userManager;
            _signInMgr = signInManager;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<ActionResult<IResponse>> Login([FromBody] LoginModel credentials)
        {
            var user = await _userMgr.FindByNameAsync(credentials.Email);
            if (user != null)
            {
                if ((await _signInMgr.PasswordSignInAsync(user,
                    credentials.Password, false, false)).Succeeded)
                {
                    User loggedInUser = _userRepository.GetByEmail(credentials.Email);
                    loggedInUser.FBToken = credentials.FBToken;
                    await _userRepository.Update(loggedInUser);

                    var securityTokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = (await _signInMgr.CreateUserPrincipalAsync(user)).Identities.First(),
                        Expires = DateTime.Now.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_Secret"))), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var handler = new JwtSecurityTokenHandler();
                    var securityToken = new JwtSecurityTokenHandler().CreateToken(securityTokenDescriptor);

                    return Ok(new SuccesResponse() { Result = new { Token = handler.WriteToken(securityToken), ExpireDate = DateTime.Now.AddDays(1), User = loggedInUser } });
                }
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<IResponse>> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userMgr.FindByNameAsync(model.Email);
            if (userExists != null) return BadRequest(new ErrorResponse { ErrorCode = 400, Message = "Email already used" });

            IdentityUser user = new IdentityUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };

            var result = await _userMgr.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(new ErrorResponse { ErrorCode = 400, Message = result.Errors });

            User newUser = new User() { Email = model.Email, Cars = new List<Car>(), FBToken = model.FBToken, Name = model.Name, PhoneNumber = model.PhoneNumber };
            newUser = await _userRepository.Create(newUser);

            await _userMgr.AddClaimAsync(user, new Claim("UserId", $"{newUser.Id}"));

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = (await _signInMgr.CreateUserPrincipalAsync(user)).Identities.First(),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_Secret"))), SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();
            var securityToken = new JwtSecurityTokenHandler().CreateToken(securityTokenDescriptor);

            return Ok(new SuccesResponse() { Result = new { Token = handler.WriteToken(securityToken), ExpireDate = DateTime.Now.AddDays(1), User = newUser } });
        }

        [Authorize]
        [HttpDelete("logout")]
        public async Task<ActionResult<IResponse>> LogOut()
        {
            User user = GetUser();

            if (user == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "User not found" }); }

            user.FBToken = "";

            user = await _userRepository.Update(user);

            if (user == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "User not found" }); }

            return Ok(new SuccesResponse() { Result = user });
        }

        private User GetUser()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            return _userRepository.GetById(id);
        }
    }
}
