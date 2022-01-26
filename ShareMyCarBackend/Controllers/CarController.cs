using Domain;
using DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShareMyCarBackend.Models;
using ShareMyCarBackend.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShareMyCarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarController : ControllerBase
    {
        private readonly ICarRepository _carRepository;
        private readonly IUserRepository _userRepository;
        private static Random random = new Random();

        public CarController(ICarRepository carRepository, IUserRepository userRepository)
        {
            _carRepository = carRepository;
            _userRepository = userRepository;
        }

        // GET api/<CarController>/5
        [HttpGet("{id}")]
        public ActionResult<IResponse> Get(int id)
        {
            User user = GetUser();

            Car car = _carRepository.GetById(id, user);

            if(car == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Car not found"}); }

            return Ok(new SuccesResponse() { Result = car });
        }

        // POST api/<CarController>
        [HttpPost]
        public async Task<ActionResult<IResponse>> Post([FromBody] NewCarModel model)
        {
            User user = GetUser();
            
            Car newCar = new Car() { Image = model.Image, Name = model.Name, Plate = model.Plate, OwnerId = user.Id, Users = new List<User>() { user }, ShareCode = "undefined" };

            newCar = await _carRepository.Create(newCar, user);

            return Ok(new SuccesResponse() { Result = newCar });
        }

        // PUT api/<CarController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<IResponse>> Put(int id, [FromBody] NewCarModel model)
        {
            User user = GetUser();

            Car car = _carRepository.GetById(id, user);

            if(car.OwnerId != user.Id) { return Unauthorized(new ErrorResponse() { ErrorCode = 401, Message = "Not authorized to update this car"}); }

            car.Name = model.Name;
            car.Plate = model.Plate;
            car.Image = model.Image;

            await _carRepository.Update(car, user);

            return Ok(new SuccesResponse() { Result = car });
        }

        // DELETE api/<CarController>/5
        [HttpDelete("{id}")]
        public ActionResult<IResponse> Delete(int id)
        {
            User user = GetUser();

            Car car = _carRepository.GetById(id, user);

            if (car == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Car not found" }); }

            if (car.OwnerId != user.Id) { return Unauthorized(new ErrorResponse() { ErrorCode = 401, Message = "Not authorized to delete this car" }); }

            _carRepository.Delete(car, user);

            return Ok(new SuccesResponse() { Result = car });
        }

        [HttpPut("{id}/share")]
        public async Task<ActionResult<IResponse>> Share(int id)
        {
            User user = GetUser();

            Car car = _carRepository.GetById(id, user);

            if(car == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Car not found"}); }

            if (car.OwnerId != user.Id) { return Unauthorized(new ErrorResponse() { ErrorCode = 401, Message = "Not authorized to update this car" }); }

            string shareCode = RandomString(4);
            car.ShareCode = shareCode;

            await _carRepository.Update(car, user);

            return Ok(new SuccesResponse() { Result = car.ShareCode });
        }

        [HttpPut("{id}/endShare")]
        public async Task<ActionResult<IResponse>> EndShare(int id)
        {
            User user = GetUser();

            Car car = _carRepository.GetById(id, user);

            if(car == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Car not found"}); }

            if (car.OwnerId != user.Id) { return Unauthorized(new ErrorResponse() { ErrorCode = 401, Message = "Not authorized to update this car" }); }

            car.ShareCode = "undefined";

            car = await _carRepository.Update(car, user);

            return Ok(new SuccesResponse() { Result = car });
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private User GetUser()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            return _userRepository.GetById(id);
        }
    }
}
