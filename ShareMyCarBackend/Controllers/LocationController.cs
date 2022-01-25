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
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepo;
        private readonly IUserRepository _userRepository;

        public LocationController(ILocationRepository locationRepository, IUserRepository userRepository)
        {
            _locationRepo = locationRepository;
            _userRepository = userRepository;
        }

        // GET: api/<LocationController>
        [HttpGet]
        public ActionResult<IResponse> Get()
        {
            User user = GetUser();

            List<Location> locations = _locationRepo.GetAll(user.Id);

            if(locations.Count == 0) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Location not found" }); }

            return new SuccesResponse() { Result = locations };
        }

        // GET api/<LocationController>/5
        [HttpGet("{id}")]
        public ActionResult<IResponse> Get(int id)
        {
            User user = GetUser();

            Location location = _locationRepo.GetById(id, user.Id);

            if(location == null) { return NotFound(new ErrorResponse() { Message = "Location not found", ErrorCode = 404 }); }

            return Ok(new SuccesResponse() { Result = location});
        }

        // POST api/<LocationController>
        [HttpPost]
        public async Task<ActionResult<IResponse>> Post([FromBody] NewLocationModel value)
        {
            Location location = new Location() { Address = value.Address, City = value.City, Name = value.Name, ZipCode = value.ZipCode };

            location = await _locationRepo.CreateLocation(location);

            return Ok(new SuccesResponse() { Result = location });
        }

        // PUT api/<LocationController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<IResponse>> Put(int id, [FromBody] NewLocationModel value)
        {
            User user = GetUser();

            Location loc = _locationRepo.GetById(id, user.Id);

            if(loc == null) { return Unauthorized(new ErrorResponse() { ErrorCode = 404, Message = "This account is not authorized to update this location"}); }

            Location location = new Location() { Address = value.Address, City = value.City, Name = value.Name, ZipCode = value.ZipCode };

            location = await _locationRepo.UpdateLocation(location);

            if(location == null) { return BadRequest(new ErrorResponse() { ErrorCode = 400, Message = "Update failed" }); }

            return Ok(new SuccesResponse() { Result = location});
        }

        // DELETE api/<LocationController>/5
        [HttpDelete("{id}")]
        public ActionResult<IResponse> Delete(int id)
        {
            User user = GetUser();

            Location location = _locationRepo.GetById(id, user.Id);

            if(location == null) { return NotFound(new ErrorResponse() { Message = "Location not found", ErrorCode = 404 }); }

            location = _locationRepo.DeleteLocation(location);

            return Ok(new SuccesResponse { Result = location });
        }

        private User GetUser()
        {
            int id = int.Parse(this.User.Claims.First(i => i.Type == "UserId").Value);
            return _userRepository.GetById(id);
        }
    }
}
