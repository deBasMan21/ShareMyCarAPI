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
    public class PutUser
    {
        [Fact]
        public void Put_Should_Return_User()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };

            User user2 = new User() { Id = 1, Name = "New", PhoneNumber = "blabla" };

            UpdateUserModel model = new UpdateUserModel() { Name = "New", PhoneNumber = "blabla" };

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user1);

            userRepo.Setup(u => u.Update(It.IsAny<User>())).Returns(Task.FromResult(user2));

            var sut = new UserController(userRepo.Object, null) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };

            // ACT
            var response = sut.Put(user1.Id, model).Result;
            var innerValue = response?.Result as OkObjectResult;
            var value = innerValue?.Value as SuccesResponse;
            var result = value?.Result as User;

            // ASSERT
            Assert.Equal(user2.Name, result?.Name);
            Assert.Equal(user2.PhoneNumber, result?.PhoneNumber);
            Assert.Equal(user2.Id, result?.Id);
        }

        [Fact]
        public void Put_Should_Return_Unauthorized()
        {
            // ARRANGE
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 2 };

            User user2 = new User() { Id = 1, Name = "New", PhoneNumber = "blabla" };

            UpdateUserModel model = new UpdateUserModel() { Name = "New", PhoneNumber = "blabla" };

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user1);

            var sut = new UserController(userRepo.Object, null) { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } } };


            // ACT
            var response = sut.Put(user2.Id, model).Result;
            var innerValue = response?.Result as UnauthorizedObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(401, result?.ErrorCode);
            Assert.Equal("Not authorized to update this user", result?.Message);
        }
    }
}
