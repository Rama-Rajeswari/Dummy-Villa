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
    public class RoomPricingAPIControllerTests
    {
        private Mock<IRoomPricingRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private RoomPricingAPIController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IRoomPricingRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new RoomPricingAPIController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetRoomPricings_ReturnsOkResult()
        {
            // Arrange
            var roomPricings = new List<RoomPricing> { new RoomPricing { Id = 1, Price = 100, RoomId = 1 } };
            _mockRepo.Setup(repo => repo.GetAllAsync(null, "Room", 0, 1)).ReturnsAsync(roomPricings);
            var roomPricingDTOs = new List<RoomPricingDTO> { new RoomPricingDTO { Id = 1, Price = 100, RoomId = 1 } };
            _mockMapper.Setup(mapper => mapper.Map<List<RoomPricingDTO>>(roomPricings)).Returns(roomPricingDTOs);

            // Act
            var result = await _controller.GetRoomPricings();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as APIResponse;
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(apiResponse.Result, Is.EqualTo(roomPricingDTOs));
        }

        [Test]
        public async Task GetRoomPricing_ReturnsOkResult()
        {
            // Arrange
            var roomPricing = new RoomPricing { Id = 1, Price = 100, RoomId = 1 };
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<RoomPricing, bool>>>(), true, null))
                     .ReturnsAsync(roomPricing);
            var roomPricingDTO = new RoomPricingDTO { Id = 1, Price = 100, RoomId = 1 };
            _mockMapper.Setup(mapper => mapper.Map<RoomPricingDTO>(roomPricing)).Returns(roomPricingDTO);

            // Act
            var result = await _controller.GetRoomPricing(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as APIResponse;
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(apiResponse.Result, Is.EqualTo(roomPricingDTO));
        }
        [Test]
        public async Task CreateRoomPricing_ReturnsOkResult()
        {
            // Arrange
            var createDTO = new RoomPricingCreateDTO { Price = 100, RoomId = 1 };
            var roomPricing = new RoomPricing { Id = 1, Price = 100, RoomId = 1 };
            _mockMapper.Setup(mapper => mapper.Map<RoomPricing>(createDTO)).Returns(roomPricing);
            _mockRepo.Setup(repo => repo.CreateAsync(roomPricing)).Returns(Task.CompletedTask);
            var roomPricingDTO = new RoomPricingDTO { Id = 1, Price = 100, RoomId = 1 };
            _mockMapper.Setup(mapper => mapper.Map<RoomPricingDTO>(roomPricing)).Returns(roomPricingDTO);

            // Act
             var result = await _controller.CreateRoomPricing(createDTO);

            // Assert
              Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
              var okResult = result.Result as OkObjectResult;
              Assert.That(okResult, Is.Not.Null);
              Assert.That(okResult.Value, Is.Not.Null);
        }



        [Test]
        public async Task UpdateRoomPricing_ReturnsOkResult()
        {
            // Arrange
            var updateDTO = new RoomPricingUpdateDTO { Id = 1, Price = 150, RoomId = 1 };
            var roomPricing = new RoomPricing { Id = 1, Price = 100, RoomId = 1 };
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<RoomPricing, bool>>>(), true, null))
                     .ReturnsAsync(roomPricing);
            _mockMapper.Setup(mapper => mapper.Map(updateDTO, roomPricing)).Returns(roomPricing);
            _mockRepo.Setup(repo => repo.UpdateAsync(roomPricing)).Returns(Task.FromResult(roomPricing));

            // Act
            var result = await _controller.UpdateRoomPricing(1, updateDTO);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as APIResponse;
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task DeleteRoomPricing_ReturnsOkResult()
        {
            // Arrange
            var roomPricing = new RoomPricing { Id = 1, Price = 100, RoomId = 1 };
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<RoomPricing, bool>>>(), true, null))
                     .ReturnsAsync(roomPricing);
            _mockRepo.Setup(repo => repo.RemoveAsync(roomPricing)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteRoomPricing(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as APIResponse;
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
