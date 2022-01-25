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
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EndpointTests.LocationTests
{
    public class PostLocation
    {
        [Fact]
        public void Post_Should_Return_New_Location()
        {
            // ARRANGE
            var locationRepo = new Mock<ILocationRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };
            Location loc = new Location() { Id = 1, Address = "test", City = "test", Name = "test", ZipCode = "test", CreatorId = 1 };
            NewLocationModel model = new NewLocationModel() { Address = "test", City = "test", Name = "test", ZipCode = "test" };

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user1);

            locationRepo.Setup(u => u.CreateLocation(It.IsAny<Location>())).Returns(Task.FromResult(loc));

            var sut = new LocationController(locationRepo.Object, userRepo.Object);
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            // ACT
            var response = sut.Post(model).Result;
            var innerValue = response?.Result as OkObjectResult;
            var value = innerValue?.Value as SuccesResponse;
            var result = value?.Result as Location;

            // ASSERT
            Assert.Equal(loc, result);
        }
    }
}
