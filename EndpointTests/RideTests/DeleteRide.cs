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
    public class DeleteRide
    {

        [Fact]
        public void Post_Should_Return_Ride()
        {
            var rideRepo = new Mock<IRideRepository>();
            var locationRepo = new Mock<ILocationRepository>();
            var carRepo = new Mock<ICarRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            Ride ride = new Ride() { Id = 1, User = user1 };

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            rideRepo.Setup(u => u.GetById(ride.Id)).Returns(ride);

            rideRepo.Setup(u => u.Delete(ride)).Returns(Task.FromResult(ride));

            var sut = new RideController(rideRepo.Object, userRepo.Object, carRepo.Object, locationRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.Delete(ride.Id).Result;
            var innerValue = response.Result as OkObjectResult;
            var value = innerValue?.Value as SuccesResponse;
            var result = value?.Result as Ride;

            // ASSERT
            Assert.Equal(ride, result);
        }

        [Fact]
        public void Post_Should_Return_Not_Found()
        {
            var rideRepo = new Mock<IRideRepository>();
            var locationRepo = new Mock<ILocationRepository>();
            var carRepo = new Mock<ICarRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            Ride ride = new Ride() { Id = 1, User = user1 };

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            rideRepo.Setup(u => u.GetById(ride.Id));

            var sut = new RideController(rideRepo.Object, userRepo.Object, carRepo.Object, locationRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.Delete(ride.Id).Result;
            var innerValue = response.Result as NotFoundObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(404, result?.ErrorCode);
            Assert.Equal("Ride not found", result?.Message);
        }

        [Fact]
        public void Post_Should_Return_Unauthorized()
        {
            var rideRepo = new Mock<IRideRepository>();
            var locationRepo = new Mock<ILocationRepository>();
            var carRepo = new Mock<ICarRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            Ride ride = new Ride() { Id = 1, User = new User() { Id = 2 } };

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            rideRepo.Setup(u => u.GetById(ride.Id)).Returns(ride);

            var sut = new RideController(rideRepo.Object, userRepo.Object, carRepo.Object, locationRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.Delete(ride.Id).Result;
            var innerValue = response.Result as UnauthorizedObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(401, result?.ErrorCode);
            Assert.Equal("Not authorized to delete this ride", result?.Message);
        }

    }
}
