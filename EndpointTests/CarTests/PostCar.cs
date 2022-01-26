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
    public class PostCar
    {
        [Fact]
        public void Post_Should_Return_Car()
        {
            var carRepo = new Mock<ICarRepository>();

            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            Car car = new Car() { Id = 1, Name = "test" };

            NewCarModel model = new NewCarModel() { Image = "", Name = "", Plate = ""};  

            userRepo.Setup(u => u.GetById(user1.Id)).Returns(user1);

            carRepo.Setup(c => c.Create(It.IsAny<Car>(), user1)).Returns(Task.FromResult(car));

            var sut = new CarController(carRepo.Object, userRepo.Object) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.Post(model).Result;
            var innerValue = response.Result as OkObjectResult;
            var value = innerValue?.Value as SuccesResponse;
            var result = value?.Result as Car;

            // ASSERT
            Assert.Equal(car, result);
        }
    }
}
