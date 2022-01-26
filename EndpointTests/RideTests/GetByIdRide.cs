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
    public class GetByIdRide
    {
        [Fact]
        public void Get_Should_Return_Ride()
        {
            var rideRepo = new Mock<IRideRepository>();
            var locationRepo = new Mock<ILocationRepository>();
            var carRepo = new Mock<ICarRepository>();
            var userRepo = new Mock<IUserRepository>();

            Ride ride =new Ride() { Id = 1 };

            rideRepo.Setup(u => u.GetById(ride.Id)).Returns(ride);

            var sut = new RideController(rideRepo.Object, userRepo.Object, carRepo.Object, locationRepo.Object);

            // ACT
            var response = sut.Get(ride.Id);
            var innerValue = response.Result as OkObjectResult;
            var value = innerValue?.Value as SuccesResponse;
            var result = value?.Result as Ride;

            // ASSERT
            Assert.Equal(ride, result);
        }

        [Fact]
        public void Get_Should_Return_Not_Found()
        {
            var rideRepo = new Mock<IRideRepository>();
            var locationRepo = new Mock<ILocationRepository>();
            var carRepo = new Mock<ICarRepository>();
            var userRepo = new Mock<IUserRepository>();

            Ride ride = new Ride() { Id = 1 };

            rideRepo.Setup(u => u.GetById(ride.Id));

            var sut = new RideController(rideRepo.Object, userRepo.Object, carRepo.Object, locationRepo.Object);

            // ACT
            var response = sut.Get(ride.Id);
            var innerValue = response.Result as NotFoundObjectResult;
            var result = innerValue?.Value as ErrorResponse;

            // ASSERT
            Assert.Equal(404, result?.ErrorCode);
            Assert.Equal("Ride not found", result?.Message);
        }
    }
}
