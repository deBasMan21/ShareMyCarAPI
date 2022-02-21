using Domain;
using DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShareMyCarBackend.Models;
using ShareMyCarBackend.Response;
using System.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShareMyCarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RideController : ControllerBase
    {
        private readonly IRideRepository _rideRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICarRepository _carRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly HttpClient _httpClient = new HttpClient();

        public RideController(IRideRepository rideRepository, IUserRepository userRepository, ICarRepository carRepository, ILocationRepository locationRepository)
        {
            _rideRepository = rideRepository;
            _userRepository = userRepository;
            _carRepository = carRepository;
            _locationRepository = locationRepository;
        }
        // GET: api/<RideController>
        [HttpGet]
        public ActionResult<IResponse> Get()
        {
            User user = GetUser();

            List<Ride> rides = _rideRepository.GetAll(user);

            if(rides.Count == 0) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Ride not found" }); }

            return Ok(new SuccesResponse() { Result = rides });
        }

        [HttpGet("requested")]
        public ActionResult<IResponse> GetRequestedRides()
        {
            User user = GetUser();

            List<Ride> rides = _rideRepository.GetRequested(user);

            if (rides.Count == 0) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Ride not found" }); }

            return Ok(new SuccesResponse() { Result = rides });
        }

        [HttpGet("denied")]
        public ActionResult<IResponse> GetDeniedRides()
        {
            User user = GetUser();

            List<Ride> rides = _rideRepository.GetDenied(user);

            if (rides.Count == 0) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Ride not found" }); }

            return Ok(new SuccesResponse() { Result = rides });
        }

        // GET api/<RideController>/5
        [HttpGet("{id}")]
        public ActionResult<IResponse> Get(int id)
        {
            Ride ride = _rideRepository.GetById(id);

            if(ride == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Ride not found" }); }

            return Ok(new SuccesResponse() { Result = ride });
        }

        // POST api/<RideController>
        [HttpPost]
        public async Task<ActionResult<IResponse>> Post([FromBody] NewRideModel model)
        {
            User user = GetUser();

            Car car = _carRepository.GetById(model.CarId, user);

            if(car == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Car not found" }); }

            Location location = _locationRepository.GetById(model.LocationId, user.Id);

            bool possible = RideIsPossible(car, model);

            if (!possible)
            {
                return BadRequest(new ErrorResponse() { ErrorCode = 400, Message = "Already a ride planned at this time"});
            }

            Ride ride = new Ride() { Name = model.Name, BeginDateTime = model.BeginDateTime, EndDateTime = model.EndDateTime, User = user, Car = car, Destination = location};

            ride = await _rideRepository.Create(ride);

            if (car.NeedsApproval && user.Id != car.OwnerId)
            {
                ride.Status = StatusType.REQUESTED;
                SendNotificationsForApproval(ride, user);
            } else
            {
                ride.Status = StatusType.APPROVED;
                SendNotifications(car, ride);
            }

            return Ok(new SuccesResponse() { Result = ride });
        }

        // PUT api/<RideController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<IResponse>> Put(int id, [FromBody] UpdateRideModel model)
        {
            User user = GetUser();

            Ride ride = _rideRepository.GetById(id);

            if(ride == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Ride not found" }); }

            if(ride.User.Id != user.Id) { return Unauthorized(new ErrorResponse() { ErrorCode = 401, Message = "Not authorized to update this ride" }); }

            StatusType status;

            if (model.Status == 1)
            {
                status = StatusType.DENIED;
            } else if (model.Status == 0)
            {
                status = StatusType.APPROVED;
            } else
            {
                status = StatusType.REQUESTED;
            }

            if (status != ride.Status)
            {
                SendNotificationsAfterApproval(ride);
            }

            Location location = _locationRepository.GetById(model.LocationId, user.Id);

            ride.Name = model.Name;
            ride.BeginDateTime = model.BeginDateTime;
            ride.EndDateTime = model.EndDateTime;
            ride.Destination = location;
            ride.Status = status;

            ride = await _rideRepository.Update(ride);

            return Ok(new SuccesResponse() { Result = ride });
        }

        // DELETE api/<RideController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<IResponse>> Delete(int id)
        {
            User user = GetUser();

            Ride ride = _rideRepository.GetById(id);

            if (ride == null) { return NotFound(new ErrorResponse() { ErrorCode = 404, Message = "Ride not found" }); }

            if (ride.User.Id != user.Id) { return Unauthorized(new ErrorResponse() { ErrorCode = 401, Message = "Not authorized to delete this ride" }); }

            await _rideRepository.Delete(ride);

            return Ok(new SuccesResponse() { Result = ride });
        }

        private User GetUser()
        {
            int id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            return _userRepository.GetById(id);
        }

        private bool RideIsPossible(Car car, NewRideModel model)
        {
            bool possible = car.Rides.Where(r => (model.BeginDateTime > r.BeginDateTime && model.BeginDateTime < r.EndDateTime) || (model.EndDateTime > r.BeginDateTime && model.EndDateTime < r.EndDateTime)).FirstOrDefault() == null;
            possible = possible && car.Rides.Where(r => (r.BeginDateTime > model.BeginDateTime && r.BeginDateTime < model.EndDateTime) || (r.EndDateTime > model.BeginDateTime && r.EndDateTime < model.EndDateTime)).FirstOrDefault() == null;
            return possible;
        }

        private void SendNotificationsForApproval(Ride ride, User user)
        {
            if (user.SendNotifications)
            {
                _ = SendNotificationForRequest(ride, user.FBToken);
            }
        }

        private void SendNotificationsAfterApproval(Ride ride)
        {
            if (ride.User.SendNotifications)
            {
                _ = SendNotificationForRequestApproval(ride, ride.User.FBToken);
            }
        }

        private void SendNotifications(Car car, Ride ride)
        {
            foreach(User user in car.Users)
            {
                if (user.SendNotifications)
                {
                    _ = SendNotificationToPerson(ride, user.FBToken);
                }
            }
        }

        private async Task SendNotificationToPerson(Ride ride, string token)
        {
            var json = new
            {
                to = token,
                notification = new
                {
                    title = $"{ride.User.Name} heeft een rit ingepland",
                    body = $"{ride.User.Name} heeft op {ride.BeginDateTime.ToLongDateString()} een rit ingepland met de auto: {ride.Car.Name}",
                    mutable_content = true,
                    sound = "Tri-tone"
                }
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + Environment.GetEnvironmentVariable("SMC_firebase_token"));
            await _httpClient.PostAsJsonAsync("https://fcm.googleapis.com/fcm/send", json);
        }

        private async Task SendNotificationForRequest(Ride ride, string token)
        {
            var json = new
            {
                to = token,
                notification = new
                {
                    title = $"{ride.User.Name} wil een rit inplannen",
                    body = $"{ride.User.Name} wil met de auto: {ride.Car.Name} een rit maken. Open de app om deze te accepteren of af te wijzen.",
                    mutable_content = true,
                    sound = "Tri-tone"
                }
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + Environment.GetEnvironmentVariable("SMC_firebase_token"));
            await _httpClient.PostAsJsonAsync("https://fcm.googleapis.com/fcm/send", json);
        }


        private async Task SendNotificationForRequestApproval(Ride ride, string token)
        {
            string approved = ride.Status == StatusType.APPROVED ? "goedgekeurd" : "Afgewezen";

            var json = new
            {
                to = token,
                notification = new
                {
                    title = $"Je rit is {approved}",
                    body = $"Open de app voor meer informatie.",
                    mutable_content = true,
                    sound = "Tri-tone"
                }
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + Environment.GetEnvironmentVariable("SMC_firebase_token"));
            await _httpClient.PostAsJsonAsync("https://fcm.googleapis.com/fcm/send", json);
        }
    }
}
