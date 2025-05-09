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
    public class GuestTypeAPIControllerTests
    {
        private Mock<IGuestTypeRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private GuestTypeAPIController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IGuestTypeRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new GuestTypeAPIController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetGuestTypes_ReturnsOkResult()
        {
            // Arrange
            var guestTypes = new List<GuestType> { new GuestType { Id = 1, Name = "Test Guest Type" } };
            _mockRepo.Setup(repo => repo.GetAllAsync(null, null, 0, 1)).ReturnsAsync(guestTypes);
            var guestTypeDTOs = new List<GuestTypeDTO> { new GuestTypeDTO { Id = 1, Name = "Test Guest Type" } };
            _mockMapper.Setup(mapper => mapper.Map<List<GuestTypeDTO>>(guestTypes)).Returns(guestTypeDTOs);

            // Act
            var result = await _controller.GetGuestTypes();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as APIResponse;
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(apiResponse.Result, Is.EqualTo(guestTypeDTOs));
        }

        [Test]
        public async Task GetGuestType_ReturnsOkResult()
        {
            // Arrange
            var guestType = new GuestType { Id = 1, Name = "Test Guest Type" };
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<GuestType, bool>>>(), true, null))
                     .ReturnsAsync(guestType);
            var guestTypeDTO = new GuestTypeDTO { Id = 1, Name = "Test Guest Type" };
            _mockMapper.Setup(mapper => mapper.Map<GuestTypeDTO>(guestType)).Returns(guestTypeDTO);

            // Act
            var result = await _controller.GetGuestType(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as APIResponse;
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(apiResponse.Result, Is.EqualTo(guestTypeDTO));
        }

        [Test]
        public async Task CreateGuestType_ReturnsCreatedAtRoute()
        {
            // Arrange
            var createDTO = new GuestTypeCreateDTO { Name = "New Guest Type" };
            var guestType = new GuestType { Id = 1, Name = "New Guest Type" };
            _mockMapper.Setup(mapper => mapper.Map<GuestType>(createDTO)).Returns(guestType);
            _mockRepo.Setup(repo => repo.CreateAsync(guestType)).Returns(Task.CompletedTask);
            var guestTypeDTO = new GuestTypeDTO { Id = 1, Name = "New Guest Type" };
            _mockMapper.Setup(mapper => mapper.Map<GuestTypeDTO>(guestType)).Returns(guestTypeDTO);

            // Act
            var result = await _controller.CreateGuestType(createDTO);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtRouteResult>());
            var createdAtRouteResult = result.Result as CreatedAtRouteResult;
            var apiResponse = createdAtRouteResult.Value as APIResponse;
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        public async Task UpdateGuestType_ReturnsOkResult()
        {
           // Arrange
           var updateDTO = new GuestTypeUpdateDTO { Id = 1, Name = "Updated Guest Type" };
           var guestType = new GuestType { Id = 1, Name = "Old Guest Type" };
           _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<GuestType, bool>>>(), true, null))
                    .ReturnsAsync(guestType);
           _mockMapper.Setup(mapper => mapper.Map(updateDTO, guestType)).Returns(guestType);
           _mockRepo.Setup(repo => repo.UpdateAsync(guestType)).Returns(Task.FromResult(guestType));

           // Act
           var result = await _controller.UpdateGuestType(1, updateDTO);

           // Assert
           Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
           var okResult = result.Result as OkObjectResult;
           var apiResponse = okResult.Value as APIResponse;
           Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
       }

       [Test]
       public async Task DeleteGuestType_ReturnsOkResult()
       {
           // Arrange
           var guestType = new GuestType { Id = 1, Name = "Test Guest Type" };
           _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<GuestType, bool>>>(), true, null))
                    .ReturnsAsync(guestType);
           _mockRepo.Setup(repo => repo.RemoveAsync(guestType)).Returns(Task.CompletedTask);

           // Act
           var result = await _controller.DeleteGuestType(1);

           // Assert
           Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
           var okResult = result.Result as OkObjectResult;
           var apiResponse = okResult.Value as APIResponse;
           Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
       }
   }
}
