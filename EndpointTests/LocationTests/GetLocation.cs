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
    public class GetLocation
    {
        [Fact]
        public void Get_Should_Return_One_Location()
        {
            // ARRANGE
            var locationRepo = new Mock<ILocationRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("UserId", "1")
                                   }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };
            List<Location> locs = new List<Location> { new Location { Id = 1, Name = "test" } };

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user1);

            locationRepo.Setup(u => u.GetAll(user1.Id)).Returns(locs);

            var sut = new LocationController(locationRepo.Object, userRepo.Object);
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            // ACT
            var response = sut.Get().Result as OkObjectResult;
            var value = response?.Value as SuccesResponse;
            var result = value?.Result as List<Location>;

            // ASSERT
            Assert.Equal(locs, result);
        }

        [Fact]
        public void Get_Should_Return_Zero_Location()
        {
            // ARRANGE
            var locationRepo = new Mock<ILocationRepository>();
            var userRepo = new Mock<IUserRepository>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim("UserId", "1")
                                   }, "TestAuthentication"));

            User user1 = new User() { Id = 1 };
            List<Location> locs = new List<Location> { };

            userRepo.Setup(u => u.GetById(It.IsAny<int>())).Returns(user1);

            locationRepo.Setup(u => u.GetAll(user1.Id)).Returns(locs);

            var sut = new LocationController(locationRepo.Object, userRepo.Object);
            sut.ControllerContext = new ControllerContext();
            sut.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            // ACT
            var response = sut.Get().Result  as NotFoundObjectResult;
            var result = response?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(404, result?.ErrorCode);
            Assert.Equal("Location not found", result?.Message);
        }
    }
}
