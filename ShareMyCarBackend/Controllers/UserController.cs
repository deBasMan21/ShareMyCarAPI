using Domain;
using DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShareMyCarBackend.Models;
using ShareMyCarBackend.Response;

namespace ShareMyCarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ICarRepository _carRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(IUserRepository userRepository, ICarRepository carRepository, UserManager<IdentityUser> userManager)
        {
            _userRepository = userRepository;
            _carRepository = carRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult<IResponse> Get()
        {
            User currentUser = GetUser();

            return Ok(new SuccesResponse() { Result = currentUser });
        }

        [HttpGet("{id}")]
        public ActionResult<IResponse> Get(int id)
        {
            User user = _userRepository.GetById(id);

            if(user == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "User not found" }); }

            return Ok(new SuccesResponse() { Result = user});
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IResponse>> Put(int id, [FromBody] UpdateUserModel model)
        {
            User user = GetUser();

            if(user.Id != id) { return Unauthorized(new ErrorResponse() { ErrorCode = 401, Message = "Not authorized to update this user" }); }

            user.Name = model.Name;
            user.PhoneNumber = model.PhoneNumber;

            await _userRepository.Update(user);

            return Ok(new SuccesResponse() { Result = user });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<IResponse>> Delete(int id)
        {
            User user = GetUser();
            
            if(user.Id!=id) { return Unauthorized(new ErrorResponse() { ErrorCode = 401, Message = "Not authorized to delete this user" }); }

            _userRepository.Delete(user);

            IdentityUser loginUser = await _userManager.FindByEmailAsync(user.Email);

            await _userManager.DeleteAsync(loginUser);

            return Ok(new SuccesResponse() { Result = user});
        }

        [HttpPut("{id}/car/{carId}")]
        public async Task<ActionResult<IResponse>> AddCar(int id, int carId, [FromBody] ShareCarModel model)
        {
            User user = GetUser();

            if (user.Id != id) { return Unauthorized(new ErrorResponse() { ErrorCode = 401, Message = "Not authorized to update this user" }); }
            
            Car car = _carRepository.GetById(carId, user);

            if(car == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Car not found" });}

            if(car.ShareCode == "undefined") { return BadRequest(new ErrorResponse() { ErrorCode = 400, Message = "Owner needs to share this car to generate a shareCode"}); }
            
            if(car.ShareCode != model.ShareCode) { return BadRequest(new ErrorResponse() { ErrorCode = 400, Message = "Incorrect shareCode"}); }

            user = await _userRepository.AddCar(user, car);

            return Ok(new SuccesResponse() { Result = user });
        }

        [HttpDelete("{id}/car/{carId}")]
        public async Task<ActionResult<IResponse>> RemoveCar(int id, int carId)
        {
            User user = GetUser();

            if (user.Id != id) { return Unauthorized(new ErrorResponse() { ErrorCode = 401, Message = "Not authorized to update this user" }); }

            Car car = _carRepository.GetById(carId, user);

            if (car == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Car not found" }); }

            user = await _userRepository.RemoveCar(user, car);

            return Ok(new SuccesResponse() { Result = user });
        }
        private User GetUser()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            return _userRepository.GetById(id);
        }
    }
}
