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
    public class PutCar
    {
        [Fact]
        public void Put_Should_Return_Car()
        {
            var carRepo = new Mock<ICarRepository>();

            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            Car car = new Car() { Id = 1, Name = "test", OwnerId = user1.Id };

            NewCarModel model = new NewCarModel() { Image = "", Name = "", Plate = "" };

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            carRepo.Setup(c => c.GetById(car.Id, user1)).Returns(car);

            carRepo.Setup(c => c.Update(It.IsAny<Car>(), user1)).Returns(Task.FromResult(car));

            var sut = new CarController(carRepo.Object, userRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.Put(car.Id, model).Result;
            var innerValue = response.Result as OkObjectResult;
            var value = innerValue?.Value as SuccesResponse;
            var result = value?.Result as Car;

            // ASSERT
            Assert.Equal(car, result);
        }

        [Fact]
        public void Put_Should_Return_Unauthorized()
        {
            var carRepo = new Mock<ICarRepository>();

            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            Car car = new Car() { Id = 1, Name = "test", OwnerId = 2 };

            NewCarModel model = new NewCarModel() { Image = "", Name = "", Plate = "" };

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            carRepo.Setup(c => c.GetById(car.Id, user1)).Returns(car);

            var sut = new CarController(carRepo.Object, userRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.Put(car.Id, model).Result;
            var innerValue = response.Result as UnauthorizedObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(401, result?.ErrorCode);
            Assert.Equal("Not authorized to update this car", result?.Message);
        }
    }
}
