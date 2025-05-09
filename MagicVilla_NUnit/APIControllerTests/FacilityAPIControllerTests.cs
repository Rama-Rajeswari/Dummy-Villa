using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_VillaAPI.Controller;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace MagicVilla_NUnit.APIControllerTests
{
    [TestFixture]
    public class FacilityAPIControllerTests
    {
        private Mock<IFacilityRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private FacilityAPIController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IFacilityRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new FacilityAPIController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetFacilities_ReturnsOkResult()
        {
            // Arrange
            var facilities = new List<Facility> { new Facility { Id = 1, Name = "Test Facility" } };
            _mockRepo.Setup(repo => repo.GetAllAsync(null, null, 0, 1)).ReturnsAsync(facilities);

            var facilityDTOs = new List<FacilityDTO> { new FacilityDTO { Id = 1, Name = "Test Facility" } };
            _mockMapper.Setup(mapper => mapper.Map<List<FacilityDTO>>(facilities)).Returns(facilityDTOs);

            // Act
            var result = await _controller.GetFacilities();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as APIResponse;
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(apiResponse.Result, Is.EqualTo(facilityDTOs));
        }

        [Test]
        public async Task GetFacility_ReturnsOkResult()
        {
            // Arrange
            var facility = new Facility { Id = 1, Name = "Test Facility" };
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Facility, bool>>>(), true, null))
                     .ReturnsAsync(facility);
            var facilityDTO = new FacilityDTO { Id = 1, Name = "Test Facility" };
            _mockMapper.Setup(mapper => mapper.Map<FacilityDTO>(facility)).Returns(facilityDTO);

            // Act
            var result = await _controller.GetFacility(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as APIResponse;
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(apiResponse.Result, Is.EqualTo(facilityDTO));
        }

        [Test]
        public async Task CreateFacility_ReturnsCreatedAtRoute()
        {
            // Arrange
            var createDTO = new FacilityCreateDTO { Name = "New Facility" };
            var facility = new Facility { Id = 1, Name = "New Facility" };
            _mockMapper.Setup(mapper => mapper.Map<Facility>(createDTO)).Returns(facility);
            _mockRepo.Setup(repo => repo.CreateAsync(facility)).Returns(Task.CompletedTask);
            var facilityDTO = new FacilityDTO { Id = 1, Name = "New Facility" };
            _mockMapper.Setup(mapper => mapper.Map<FacilityDTO>(facility)).Returns(facilityDTO);

            // Act
            var result = await _controller.CreateFacility(createDTO);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtRouteResult>());
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            var apiResponse = createdAtRouteResult.Value as APIResponse;
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async Task UpdateFacility_ReturnsOkResult()
        {
            // Arrange
            var updateDTO = new FacilityUpdateDTO { Id = 1, Name = "Updated Facility" };
            var facility = new Facility { Id = 1, Name = "Old Facility" };
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Facility, bool>>>(), true, null))
                     .ReturnsAsync(facility);
            
           _mockMapper.Setup(mapper => mapper.Map(updateDTO, facility)).Returns(facility);
           _mockRepo.Setup(repo => repo.UpdateAsync(facility)).Returns(Task.FromResult(facility));

           // Act
           var result = await _controller.UpdateFacility(1, updateDTO);

           // Assert
           Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
           var okResult = result.Result as OkObjectResult;
           var apiResponse = okResult.Value as APIResponse;
           Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
       }

       [Test]
       public async Task DeleteFacility_ReturnsOkResult()
       {
           // Arrange
           var facility = new Facility { Id = 1, Name = "Test Facility" };
           _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Facility, bool>>>(), true, null))
                    .ReturnsAsync(facility);
           _mockRepo.Setup(repo => repo.RemoveAsync(facility)).Returns(Task.CompletedTask);

           // Act
           var result = await _controller.DeleteFacility(1);

           // Assert
           Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
           var okResult = result.Result as OkObjectResult;
           var apiResponse = okResult.Value as APIResponse;
           Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
       }
   }
}
