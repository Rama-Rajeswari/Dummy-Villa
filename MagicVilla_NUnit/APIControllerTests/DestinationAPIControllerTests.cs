using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_VillaAPI.Controller; // Corrected namespace
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace MagicVilla_NUnit.APIControllerTests
{
    [TestFixture]
    public class DestinationAPIControllerTests
    {
        private Mock<IDestinationRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private DestinationAPIController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IDestinationRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new DestinationAPIController(_mockRepo.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetDestinations_ReturnsOkResult_WithListOfDestinations()
        {
            // Arrange
            var destinations = new List<Destination> { new Destination { Id = 1, Name = "Test Destination" } };
            var destinationDTOs = new List<DestinationDTO> { new DestinationDTO { Id = 1, Name = "Test Destination" } };
            _mockRepo.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Destination, bool>>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                     .ReturnsAsync(destinations);
            _mockMapper.Setup(mapper => mapper.Map<List<DestinationDTO>>(destinations)).Returns(destinationDTOs);

            // Act
            var result = await _controller.GetDestinations();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(((APIResponse)okResult.Value).Result, Is.EqualTo(destinationDTOs));
        }

        [Test]
        public async Task GetDestination_ReturnsOkResult_WithDestination()
        {
            // Arrange
            var destination = new Destination { Id = 1, Name = "Test Destination" };
            var destinationDTO = new DestinationDTO { Id = 1, Name = "Test Destination" };
          _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Destination, bool>>>(), true, null))
         .ReturnsAsync(destination);





            _mockMapper.Setup(mapper => mapper.Map<DestinationDTO>(destination)).Returns(destinationDTO);

            // Act
            var result = await _controller.GetDestination(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.That(((APIResponse)okResult.Value).Result, Is.EqualTo(destinationDTO));
        }

        [Test]
        public async Task CreateDestination_ReturnsCreatedAtRouteResult_WithDestination()
        {
            // Arrange
            var createDTO = new DestinationCreateDTO { Name = "New Destination" };
            var destination = new Destination { Id = 1, Name = "New Destination" };
            var destinationDTO = new DestinationDTO { Id = 1, Name = "New Destination" };
            _mockMapper.Setup(mapper => mapper.Map<Destination>(createDTO)).Returns(destination);
            _mockRepo.Setup(repo => repo.CreateAsync(destination)).Returns(Task.CompletedTask);
            _mockMapper.Setup(mapper => mapper.Map<DestinationDTO>(destination)).Returns(destinationDTO);

            // Act
            var result = await _controller.CreateDestination(createDTO);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtRouteResult>());
            var createdResult = result.Result as CreatedAtRouteResult;
            Assert.That(createdResult.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
            Assert.That(((APIResponse)createdResult.Value).Result, Is.EqualTo(destinationDTO));
        }

        [Test]
        public async Task UpdateDestination_ReturnsOkResult()
        {
            // Arrange
            var updateDTO = new DestinationUpdateDTO { Id = 1, Name = "Updated Destination" };
            var destination = new Destination { Id = 1, Name = "Old Destination" };
             _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Destination, bool>>>(), true, null))
              .ReturnsAsync(destination);

           _mockMapper.Setup(mapper => mapper.Map(updateDTO, destination)).Returns(destination);
           _mockRepo.Setup(repo => repo.UpdateAsync(destination)).Returns(Task.FromResult(destination));

           // Act
           var result = await _controller.UpdateDestination(1, updateDTO);

          // Assert
           Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
           var okResult = result.Result as OkObjectResult;
            var apiResponse = okResult.Value as APIResponse;
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
         [Test]
         public async Task DeleteDestination_ReturnsOkResult()
         {
            // Arrange
            var destination = new Destination { Id = 1, Name = "Test Destination" };
            _mockRepo.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Destination, bool>>>(), true, null))
             .ReturnsAsync(destination);

             _mockRepo.Setup(repo => repo.RemoveAsync(destination)).Returns(Task.CompletedTask);

            // Act
             var result = await _controller.DeleteDestination(1);

             // Assert
             Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
             var okResult = result.Result as OkObjectResult;
             var apiResponse = okResult.Value as APIResponse;
             Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

    }
}
