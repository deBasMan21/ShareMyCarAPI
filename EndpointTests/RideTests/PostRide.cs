using Domain;
using DomainServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShareMyCarBackend.Controllers;
using ShareMyCarBackend.Models;
using ShareMyCarBackend.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EndpointTests.RideTests
{
    public class PostRide
    {
        [Fact]
        public void Post_Should_Return_Ride()
        {
            var rideRepo = new Mock<IRideRepository>();
            var locationRepo = new Mock<ILocationRepository>();
            var carRepo = new Mock<ICarRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            Car car = new Car() { Id = 1, Users = new List<User>(), Rides = new List<Ride>() { } };

            Ride ride = new Ride() { Id = 1, Car = car };

            User user1 = new User() { Id = 1 };

            Location location = new Location() { Id = 1 };

            NewRideModel model = new NewRideModel() { BeginDateTime = DateTime.Now, EndDateTime = DateTime.Now, CarId = 1, LocationId = 1, Name = "" };

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            carRepo.Setup(u => u.GetById(car.Id, user1)).Returns(car);

            locationRepo.Setup(l => l.GetById(location.Id, user1.Id)).Returns(location);

            rideRepo.Setup(u => u.Create(It.IsAny<Ride>())).Returns(Task.FromResult(ride));

            var sut = new RideController(rideRepo.Object, userRepo.Object, carRepo.Object, locationRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.Post(model).Result;
            var innerValue = response.Result as OkObjectResult;
            var value = innerValue?.Value as SuccesResponse;
            var result = value?.Result as Ride;

            // ASSERT
            Assert.Equal(ride, result);
        }

        [Fact]
        public void Post_Should_Return_Not_Found_Car()
        {
            var rideRepo = new Mock<IRideRepository>();
            var locationRepo = new Mock<ILocationRepository>();
            var carRepo = new Mock<ICarRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            Ride ride = new Ride() { Id = 1 };

            Car car = new Car() { Id = 1 };

            User user1 = new User() { Id = 1 };

            Location location = new Location() { Id = 1 };

            NewRideModel model = new NewRideModel() { BeginDateTime = DateTime.Now, EndDateTime = DateTime.Now, CarId = 1, LocationId = 1, Name = "" };

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            carRepo.Setup(u => u.GetById(car.Id, user1));

            var sut = new RideController(rideRepo.Object, userRepo.Object, carRepo.Object, locationRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.Post(model).Result;
            var innerValue = response.Result as NotFoundObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(404, result?.ErrorCode);
            Assert.Equal("Car not found", result?.Message);
        }

        [Fact]
        public void Post_Should_Return_Not_Found_Location()
        {
            var rideRepo = new Mock<IRideRepository>();
            var locationRepo = new Mock<ILocationRepository>();
            var carRepo = new Mock<ICarRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            Ride ride = new Ride() { Id = 1 };

            Car car = new Car() { Id = 1 };

            User user1 = new User() { Id = 1 };

            Location location = new Location() { Id = 1 };

            NewRideModel model = new NewRideModel() { BeginDateTime = DateTime.Now, EndDateTime = DateTime.Now, CarId = 1, LocationId = 1, Name = "" };

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            carRepo.Setup(u => u.GetById(car.Id, user1)).Returns(car);

            locationRepo.Setup(l => l.GetById(location.Id, user1.Id));

            var sut = new RideController(rideRepo.Object, userRepo.Object, carRepo.Object, locationRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.Post(model).Result;
            var innerValue = response.Result as NotFoundObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(404, result?.ErrorCode);
            Assert.Equal("Location not found", result?.Message);
        }
    }
}
