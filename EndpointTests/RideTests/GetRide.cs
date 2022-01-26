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
    public class GetRide
    {
        [Fact]
        public void Get_Should_Return_Ridelist()
        {
            var rideRepo = new Mock<IRideRepository>();
            var locationRepo = new Mock<ILocationRepository>();
            var carRepo = new Mock<ICarRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            List<Ride> rides = new List<Ride>() { new Ride() { Id = 1}, new Ride() { Id = 2 } };

            User user1 = new User() { Id = 1 };

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            rideRepo.Setup(u => u.GetAll(user1)).Returns(rides);

            var sut = new RideController(rideRepo.Object, userRepo.Object, carRepo.Object, locationRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.Get();
            var innerValue = response.Result as OkObjectResult;
            var value = innerValue?.Value as SuccesResponse;
            var result = value?.Result as List<Ride>;

            // ASSERT
            Assert.Equal(rides, result);
        }

        [Fact]
        public void Get_Should_Return_Not_Found()
        {
            var rideRepo = new Mock<IRideRepository>();
            var locationRepo = new Mock<ILocationRepository>();
            var carRepo = new Mock<ICarRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            List<Ride> rides = new List<Ride>();

            User user1 = new User() { Id = 1 };

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            rideRepo.Setup(u => u.GetAll(user1)).Returns(rides);

            var sut = new RideController(rideRepo.Object, userRepo.Object, carRepo.Object, locationRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.Get();
            var innerValue = response.Result as NotFoundObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(404, result?.ErrorCode);
            Assert.Equal("Ride not found", result?.Message);
        }
    }
}
