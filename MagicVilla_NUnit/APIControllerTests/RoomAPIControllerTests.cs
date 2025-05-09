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

namespace MagicVilla_NUnit
{
    [TestFixture]
    public class RoomAPIControllerTests
    {
        private Mock<IRoomRepository> _mockRoomRepository;
        private Mock<IVillaRepository> _mockVillaRepository;
        private Mock<IMapper> _mockMapper;
        private RoomAPIController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRoomRepository = new Mock<IRoomRepository>();
            _mockVillaRepository = new Mock<IVillaRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new RoomAPIController(_mockRoomRepository.Object, _mockMapper.Object, _mockVillaRepository.Object);
        }

        [Test]
        public async Task GetRooms_ReturnsOkResult_WithListOfRooms()
        {
            // Arrange
            var rooms = new List<Room>
            {
                new Room { Id = 1, RoomType = "Deluxe" },
                new Room { Id = 2, RoomType = "Standard" }
            };
            Expression<Func<MagicVilla_VillaAPI.Models.Room, bool>> filterExpression = room => true;

       _mockRoomRepository.Setup(repo => repo.GetAllAsync(filterExpression, null, 0, 1)).ReturnsAsync(rooms);
            _mockMapper.Setup(m => m.Map<List<RoomDTO>>(It.IsAny<IEnumerable<Room>>())).Returns(new List<RoomDTO>
            {
                new RoomDTO { Id = 1, RoomType = "Deluxe" },
                new RoomDTO { Id = 2, RoomType = "Standard" }
            });

            // Act
            var result = await _controller.GetRooms() as ActionResult<APIResponse>;
            var okResult = result?.Result as OkObjectResult;

            // Assert
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var apiResponse = okResult?.Value as APIResponse;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.IsSuccess, Is.True);
            Assert.That((apiResponse.Result as List<RoomDTO>).Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetRoom_ReturnsBadRequest_ForInvalidId()
        {
            // Act
            var result = await _controller.GetRoom(0) as ActionResult<APIResponse>;
            var badRequestResult = result?.Result as BadRequestObjectResult;

            // Assert
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task GetRoom_ReturnsNotFound_ForNonExistentRoom()
        {
           Expression<Func<Room, bool>> filterExpression = room => true;



            // Arrange
         _mockRoomRepository.Setup(repo => repo.GetAsync(filterExpression, true, null)).ReturnsAsync((Room)null);

            // Act
            var result = await _controller.GetRoom(1) as ActionResult<APIResponse>;
            var notFoundResult = result?.Result as NotFoundObjectResult;

            // Assert
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }
         [Test]
        public async Task GetRoom_ReturnsOkResult_WithRoom()
        {
           // Arrange
           var room = new Room { Id = 1, RoomType = "Deluxe" };
           _mockRoomRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Room, bool>>>(), true, null)).ReturnsAsync(room);
          _mockMapper.Setup(m => m.Map<RoomDTO>(It.IsAny<Room>())).Returns(new RoomDTO { Id = 1, RoomType = "Deluxe" });

          // Act
          var result = await _controller.GetRoom(1) as ActionResult<APIResponse>;
          var okResult = result?.Result as OkObjectResult;

         // Assert
          Assert.That(result, Is.Not.Null); // Check if result is not null
          Assert.That(okResult, Is.Not.Null); // Check if okResult is not null
          Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
          var apiResponse = okResult?.Value as APIResponse;
          Assert.That(apiResponse, Is.Not.Null);
          Assert.That(apiResponse.IsSuccess, Is.True);
          var roomDTO = apiResponse.Result as RoomDTO;
          Assert.That(roomDTO, Is.Not.Null);
          Assert.That(roomDTO.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task CreateRoom_ReturnsCreatedResult_WithRoom()
        {
             // Arrange
             var createDTO = new RoomCreateDTO { RoomType = "Deluxe", VillaId = 1, GuestTypeIds = new List<int> { 1, 2 } };
             var room = new Room { Id = 1, RoomType = "Deluxe", VillaId = 1, RoomGuestTypes = new List<RoomGuestType> { new RoomGuestType { GuestTypeId = 1 }, new RoomGuestType { GuestTypeId = 2 } } };
            var apiResponse = new APIResponse
             {
                 IsSuccess = true,
                 Result = new
                 {
                      room.Id,
                      room.RoomType,
                      room.VillaId,
                     GuestTypeIds = room.RoomGuestTypes.Select(gt => gt.GuestTypeId).ToList()
                }
            };

            _mockMapper.Setup(m => m.Map<Room>(It.IsAny<RoomCreateDTO>())).Returns(room);
            _mockRoomRepository.Setup(repo => repo.CreateAsync(It.IsAny<Room>())).Returns(Task.CompletedTask);

             // Act
            var result = await _controller.CreateRoom(createDTO) as ActionResult<APIResponse>;
            var createdResult = result?.Result as CreatedAtRouteResult;

            // Assert
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
            var response = createdResult?.Value as APIResponse;
            Assert.That(response, Is.Not.Null);
           Assert.That(response.IsSuccess, Is.True);

         // Access the anonymous type properties
          var roomDTOResult = response.Result;
          Assert.That(roomDTOResult, Is.Not.Null);
          Assert.That(roomDTOResult.GetType().GetProperty("Id").GetValue(roomDTOResult, null), Is.EqualTo(1));
          Assert.That(roomDTOResult.GetType().GetProperty("RoomType").GetValue(roomDTOResult, null), Is.EqualTo("Deluxe"));
          Assert.That(roomDTOResult.GetType().GetProperty("VillaId").GetValue(roomDTOResult, null), Is.EqualTo(1));
          Assert.That(roomDTOResult.GetType().GetProperty("GuestTypeIds").GetValue(roomDTOResult, null), Is.EqualTo(new List<int> { 1, 2 }));
        }



       [Test]
       public async Task UpdateRoom_ReturnsOkResult_WithUpdatedRoom()
       {
          // Arrange
          var updateDTO = new RoomUpdateDTO { Id = 1, RoomType = "Deluxe", VillaId = 1, GuestTypeIds = new List<int> { 1, 2 } };
          var room = new Room { Id = 1, RoomType = "Standard", VillaId = 1, RoomGuestTypes = new List<RoomGuestType> { new RoomGuestType { GuestTypeId = 1 }, new RoomGuestType { GuestTypeId = 2 } } };
          _mockRoomRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Room, bool>>>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(room);
          _mockVillaRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Villa, bool>>>(), It.IsAny<bool>(), It.IsAny<string>())).ReturnsAsync(new Villa { Id = 1 });

          _mockMapper.Setup(m => m.Map(It.IsAny<RoomUpdateDTO>(), It.IsAny<Room>())).Callback<RoomUpdateDTO, Room>((dto, r) => r.RoomType = dto.RoomType);

          // Act
           var result = await _controller.UpdateRoom(1, updateDTO) as ActionResult<APIResponse>;
           var okResult = result?.Result as OkObjectResult;

         // Assert
         Assert.That(okResult, Is.Not.Null);
         Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
         var apiResponse = okResult?.Value as APIResponse;
         Assert.That(apiResponse, Is.Not.Null);
         Assert.That(apiResponse.IsSuccess, Is.True);

         // Access the anonymous type properties
         var roomDTOResult = apiResponse.Result;
         Assert.That(roomDTOResult, Is.Not.Null);
         Assert.That(roomDTOResult.GetType().GetProperty("RoomType").GetValue(roomDTOResult, null), Is.EqualTo("Deluxe"));
        }

        [Test]
        public async Task DeleteRoom_ReturnsOkResult_WithNoContentStatus()
        {
            // Arrange
            var room = new Room { Id = 1, RoomType = "Deluxe" };
            _mockRoomRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Room, bool>>>(), true, null)).ReturnsAsync(room);
            _mockRoomRepository.Setup(repo => repo.RemoveAsync(It.IsAny<Room>())).Returns(Task.CompletedTask);

             // Act
            var result = await _controller.DeleteRoom(1) as ActionResult<APIResponse>;
             var okResult = result?.Result as OkObjectResult;

             // Assert
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK)); // Check for 200 OK status code
            var apiResponse = okResult?.Value as APIResponse;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent)); // Check if APIResponse status code is No Content
        }
         [Test]
        public async Task GetRoomsByVilla_ReturnsOkResult_WithVilla()
        {
            // Arrange
            var villaDTO = new VillaDTO { Id = 1, Name = "Villa One", Rooms = new List<RoomDTO> { new RoomDTO { Id = 1, RoomType = "Deluxe" } } };
            _mockRoomRepository.Setup(repo => repo.GetRoomsByVilla(It.IsAny<int>())).ReturnsAsync(villaDTO);

              // Act
            var result = await _controller.GetRoomsByVilla(1) as ActionResult<VillaDTO>;
            var okResult = result?.Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null); // Check if result is not null
            Assert.That(okResult, Is.Not.Null); // Check if okResult is not null
            Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            var returnedVillaDTO = okResult?.Value as VillaDTO;
            Assert.That(returnedVillaDTO, Is.Not.Null);
            Assert.That(returnedVillaDTO.Id, Is.EqualTo(1));
        }

    }
}
