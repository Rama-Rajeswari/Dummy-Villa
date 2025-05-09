using NUnit.Framework;
using Moq;
using MagicVilla_Web.Controllers;
using MagicVilla_Web.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_Web.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MagicVilla_Utility;
using Newtonsoft.Json;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MagicVilla_Web.Models;

namespace MagicVilla_NUnit.WebControllerTests
{
    [TestFixture]
    public class RoomWebControllerTests
    {
        private Mock<IRoomService> _roomServiceMock;
        private Mock<IVillaService> _villaServiceMock;
        private Mock<IGuestTypeService> _guestTypeServiceMock;
        private Mock<IMapper> _mapperMock;
        private RoomController _controller;
        private DefaultHttpContext _httpContext;
        private TempDataDictionary _tempData;

        [SetUp]
        public void SetUp()
        {
            _roomServiceMock = new Mock<IRoomService>();
            _villaServiceMock = new Mock<IVillaService>();
            _guestTypeServiceMock = new Mock<IGuestTypeService>();
            _mapperMock = new Mock<IMapper>();

            _httpContext = new DefaultHttpContext();
            _httpContext.Session = new Mock<ISession>().Object;

            _tempData = new TempDataDictionary(_httpContext, Mock.Of<ITempDataProvider>());

            _controller = new RoomController(_roomServiceMock.Object, _mapperMock.Object, _villaServiceMock.Object, _guestTypeServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                },
                TempData = _tempData
            };
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task IndexRoom_Get_ReturnsViewResult_WithPagedList()
        {
            // Arrange
            var rooms = new List<RoomDTO> { new RoomDTO { Id = 1, RoomType = "Room1" }, new RoomDTO { Id = 2, RoomType = "Room2" } };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(rooms) };
            _roomServiceMock.Setup(s => s.GetAllAsync<APIResponse>(It.IsAny<string>())).ReturnsAsync(apiResponse);
            _httpContext.Session.SetString(SD.SessionToken, "dummyToken");

            // Act
            var result = await _controller.IndexRoom(1, 5) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<IPagedList<RoomDTO>>());
            var pagedList = result.Model as IPagedList<RoomDTO>;
            Assert.That(pagedList.Count, Is.EqualTo(2));
        }

        [Test]
        public void CreateRoom_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.CreateRoom() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.Null); // Default view name
        }

        [Test]
        public async Task CreateRoom_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var roomCreateDto = new RoomCreateDTO { RoomType = "New Room" };
            var apiResponse = new APIResponse { IsSuccess = true };
            _roomServiceMock.Setup(s => s.CreateAsync<APIResponse>(roomCreateDto, It.IsAny<string>())).ReturnsAsync(apiResponse);
            _httpContext.Session.SetString(SD.SessionToken, "dummyToken");

            // Act
            var result = await _controller.CreateRoom(roomCreateDto) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("IndexRoom"));
        }

        [Test]
        public async Task UpdateRoom_Get_ReturnsViewResult_WithRoomUpdateDTO()
        {
                // Arrange
             var roomDTO = new RoomDTO { Id = 1, RoomType = "Room1" };
             var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(roomDTO) };
             _roomServiceMock.Setup(s => s.GetAsync<APIResponse>(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(apiResponse);
            _mapperMock.Setup(m => m.Map<RoomUpdateDTO>(It.IsAny<RoomDTO>())).Returns(new RoomUpdateDTO { Id = 1, RoomType = "Room1" });

            // Act
             var result = await _controller.UpdateRoom(1);

            // Assert
             var viewResult = result as ViewResult;
             Assert.That(viewResult, Is.Not.Null);
             var model = viewResult.Model as RoomUpdateDTO;
             Assert.That(model, Is.Not.Null);
             Assert.That(model.Id, Is.EqualTo(1));
             Assert.That(model.RoomType, Is.EqualTo("Room1"));
        }

        [Test]
        public async Task UpdateRoom_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var roomUpdateDto = new RoomUpdateDTO { Id = 1, RoomType = "Updated Room" };
            var apiResponse = new APIResponse { IsSuccess = true };
            _roomServiceMock.Setup(s => s.UpdateAsync<APIResponse>(roomUpdateDto, It.IsAny<string>())).ReturnsAsync(apiResponse);
            _httpContext.Session.SetString(SD.SessionToken, "dummyToken");

            // Act
            var result = await _controller.UpdateRoom(roomUpdateDto) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("IndexRoom"));
        }

        [Test]
        public async Task DeleteRoom_Get_ReturnsViewResult_WithRoomDTO()
        {
            // Arrange
            var roomDto = new RoomDTO { Id = 1, RoomType = "Room1" };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(roomDto) };
            _roomServiceMock.Setup(s => s.GetAsync<APIResponse>(1, It.IsAny<string>())).ReturnsAsync(apiResponse);
            _httpContext.Session.SetString(SD.SessionToken, "dummyToken");

            // Act
            var result = await _controller.DeleteRoom(1) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<RoomDTO>());
            var roomDtoResult = result.Model as RoomDTO;
            Assert.That(roomDtoResult.Id, Is.EqualTo(1));
            Assert.That(roomDtoResult.RoomType, Is.EqualTo("Room1"));
        }

        [Test]
        public async Task DeleteRoom_Post_ReturnsRedirectToActionResult_WhenRoomIsDeleted()
        {
            // Arrange
            var roomDto = new RoomDTO { Id = 1, RoomType = "Room1" };
            var apiResponse = new APIResponse { IsSuccess = true };
            _roomServiceMock.Setup(s => s.DeleteAsync<APIResponse>(1, It.IsAny<string>())).ReturnsAsync(apiResponse);
            _httpContext.Session.SetString(SD.SessionToken, "dummyToken");

            // Act
            var result = await _controller.DeleteRoom(roomDto) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("IndexRoom"));
        }
    }
}
