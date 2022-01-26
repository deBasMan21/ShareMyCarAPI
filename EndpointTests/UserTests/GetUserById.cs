using Domain;
using DomainServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShareMyCarBackend.Controllers;
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
    public class GetUserById
    {
        [Fact]
        public void Get_Should_Return_User()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();

            var carRepo = new Mock<ICarRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user1);

            var sut = new UserController(userRepo.Object, carRepo.Object, null);
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            // ACT
            var response = sut.Get(user1.Id);
            var innerValue = response?.Result as OkObjectResult;
            var value = innerValue?.Value as SuccesResponse;
            var result = value?.Result as User;

            // ASSERT
            Assert.Equal(user1, result);
        }

        [Fact]
        public void Get_Should_Return_Not_Found()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();

            var carRepo = new Mock<ICarRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            userRepo.Setup(u => u.GetById(It.IsAny<int>()));

            var sut = new UserController(userRepo.Object, carRepo.Object, null);
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            // ACT
            var response = sut.Get(user1.Id);
            var innerValue = response?.Result as NotFoundObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(404, result?.ErrorCode);
            Assert.Equal("User not found", result?.Message);
        }
    }
}
