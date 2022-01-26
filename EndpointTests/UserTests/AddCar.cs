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

namespace EndpointTests.UserTests
{
    public class AddCar
    {
        [Fact]
        public void Put_Should_Return_User()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();

            var carRepo = new Mock<ICarRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            Car car = new Car() { Id = 1, ShareCode = "test"};    

            ShareCarModel model = new ShareCarModel() { ShareCode = "test" };

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user1);

            carRepo.Setup(u => u.GetById(car.Id, user1)).Returns(car);

            userRepo.Setup(u => u.AddCar(user1, car)).Returns(Task.FromResult(user1));

            var sut = new UserController(userRepo.Object, carRepo.Object, null) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.AddCar(user1.Id, car.Id, model).Result;
            var innerValue = response?.Result as OkObjectResult;
            var value = innerValue?.Value as SuccesResponse;
            var result = value?.Result as User;

            // ASSERT
            Assert.Equal(user1, result);
        }

        [Fact]
        public void Put_Should_Return_Unauthorized()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();

            var carRepo = new Mock<ICarRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            User user2 = new User() { Id = 2 };

            Car car = new Car() { Id = 1, ShareCode = "test" };

            ShareCarModel model = new ShareCarModel() { ShareCode = "test" };

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user2);

            var sut = new UserController(userRepo.Object, carRepo.Object, null) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.AddCar(user1.Id, car.Id, model).Result;
            var innerValue = response?.Result as UnauthorizedObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(401, result?.ErrorCode);
            Assert.Equal("Not authorized to update this user", result?.Message);
        }

        [Fact]
        public void Put_Should_Return_Not_Found()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();

            var carRepo = new Mock<ICarRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            Car car = new Car() { Id = 1, ShareCode = "test" };

            ShareCarModel model = new ShareCarModel() { ShareCode = "test" };

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user1);

            carRepo.Setup(u => u.GetById(car.Id, user1));

            var sut = new UserController(userRepo.Object, carRepo.Object, null) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.AddCar(user1.Id, car.Id, model).Result;
            var innerValue = response?.Result as NotFoundObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(404, result?.ErrorCode);
            Assert.Equal("Car not found", result?.Message);
        }

        [Fact]
        public void Put_Should_Return_Bad_Request()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();

            var carRepo = new Mock<ICarRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            Car car = new Car() { Id = 1, ShareCode = "test" };

            ShareCarModel model = new ShareCarModel() { ShareCode = "blabla" };

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user1);

            carRepo.Setup(u => u.GetById(car.Id, user1)).Returns(car);

            var sut = new UserController(userRepo.Object, carRepo.Object, null) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.AddCar(user1.Id, car.Id, model).Result;
            var innerValue = response?.Result as BadRequestObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(400, result?.ErrorCode);
            Assert.Equal("Incorrect shareCode", result?.Message);
        }

        [Fact]
        public void Put_Should_Return_Bad_Request_undefined()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();

            var carRepo = new Mock<ICarRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            Car car = new Car() { Id = 1, ShareCode = "undefined" };

            ShareCarModel model = new ShareCarModel() { ShareCode = "blabla" };

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user1);

            carRepo.Setup(u => u.GetById(car.Id, user1)).Returns(car);

            var sut = new UserController(userRepo.Object, carRepo.Object, null) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.AddCar(user1.Id, car.Id, model).Result;
            var innerValue = response?.Result as BadRequestObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(400, result?.ErrorCode);
            Assert.Equal("Owner needs to share this car to generate a shareCode", result?.Message);
        }
    }
}
