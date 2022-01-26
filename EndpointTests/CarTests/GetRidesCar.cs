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

namespace EndpointTests.CarTests
{
    public class GetRidesCar
    {
        [Fact]
        public void Get_Should_Return_Ridelist()
        {
            var carRepo = new Mock<ICarRepository>();

            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            Car car = new Car() { Id = 1, Name = "test", Rides = new List<Ride>() { new Ride() { Id = 1 } } };

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            carRepo.Setup(c => c.GetById(car.Id, user1)).Returns(car);

            var sut = new CarController(carRepo.Object, userRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.GetRides(car.Id);
            var innerValue = response.Result as OkObjectResult;
            var value = innerValue?.Value as SuccesResponse;
            var result = value?.Result as List<Ride>;

            // ASSERT
            Assert.Equal(car.Rides, result);
        }

        [Fact]
        public void Get_Should_Return_Not_Found()
        {
            var carRepo = new Mock<ICarRepository>();

            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            Car car = new Car() { Id = 1, Name = "test", Rides = new List<Ride>() { new Ride() { Id = 1 } } };

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            carRepo.Setup(c => c.GetById(car.Id, user1));

            var sut = new CarController(carRepo.Object, userRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.GetRides(car.Id);
            var innerValue = response.Result as NotFoundObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(404, result?.ErrorCode);
            Assert.Equal("Car not found", result?.Message);
        }
    }
}
