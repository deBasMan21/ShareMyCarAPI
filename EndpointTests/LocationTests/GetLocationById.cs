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
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EndpointTests.LocationTests
{
    public class GetLocationById
    {
        [Fact]
        public void Get_Should_Return_One_Location()
        {
            // ARRANGE
            var locationRepo = new Mock<ILocationRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };
            Location loc = new Location() { Id = 1, Name = "test" } ;

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user1);

            locationRepo.Setup(u => u.GetById(loc.Id, user1.Id)).Returns(loc);

            var sut = new LocationController(locationRepo.Object, userRepo.Object);
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            // ACT
            var response = sut.Get(loc.Id).Result as OkObjectResult;
            var value = response?.Value as SuccesResponse;
            var result = value?.Result as Location;

            // ASSERT
            Assert.Equal(loc, result);
        }

        [Fact]
        public void Get_Should_Return_Zero_Location()
        {
            // ARRANGE
            var locationRepo = new Mock<ILocationRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("UserId", "1") }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };
            Location loc = new Location() { Id = 1, Name = "test" };

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user1);

            locationRepo.Setup(u => u.GetById(loc.Id, user1.Id));

            var sut = new LocationController(locationRepo.Object, userRepo.Object);
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            // ACT
            var response = sut.Get(loc.Id).Result as NotFoundObjectResult;
            var result = response?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(404, result?.ErrorCode);
            Assert.Equal("Location not found", result?.Message);
        }
    }
}
