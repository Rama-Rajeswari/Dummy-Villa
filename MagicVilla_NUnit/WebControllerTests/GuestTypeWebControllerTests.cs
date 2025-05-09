using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_Web.Controllers;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using X.PagedList;

namespace MagicVilla_NUnit.WebControllerTests
{
    public class GuestTypeWebControllerTests
    {
        private Mock<IGuestTypeService> _guestTypeServiceMock;
        private Mock<IMapper> _mapperMock;
        private GuestTypeController _controller;

        [SetUp]
        public void SetUp()
        {
            _guestTypeServiceMock = new Mock<IGuestTypeService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new GuestTypeController(_guestTypeServiceMock.Object, _mapperMock.Object);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Session = new Mock<ISession>().Object;

            var tempData = new Mock<ITempDataDictionary>();
            _controller.TempData = tempData.Object;
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task IndexGuestType_ReturnsViewResult_WithPagedList()
        {
            // Arrange
            var guestTypes = new List<GuestTypeDTO>
            {
                new GuestTypeDTO { Id = 1, Name = "Type1" },
                new GuestTypeDTO { Id = 2, Name = "Type2" }
            };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(guestTypes) };
            _guestTypeServiceMock.Setup(s => s.GetAllAsync<APIResponse>(It.IsAny<string>())).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.IndexGuestType(1, 5) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var model = result.Model as IPagedList<GuestTypeDTO>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(2));
        }

        [Test]
        public void CreateGuestType_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.CreateGuestType() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task CreateGuestType_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var model = new GuestTypeCreateDTO { Name = "Type1" };
            var apiResponse = new APIResponse { IsSuccess = true };
            _guestTypeServiceMock.Setup(s => s.CreateAsync<APIResponse>(model, It.IsAny<string>())).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.CreateGuestType(model) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("IndexGuestType"));
        }

        [Test]
        public async Task CreateGuestType_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");
            var model = new GuestTypeCreateDTO { Name = "" };

            // Act
            var result = await _controller.CreateGuestType(model) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(result.Model, Is.EqualTo(model));
        }

       [Test]
       public async Task UpdateGuestType_Get_ReturnsViewResult_WithGuestTypeUpdateDTO()
       {
             // Arrange
            var guestTypeDTO = new GuestTypeDTO { Id = 1, Name = "GuestType1" };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(guestTypeDTO) };
            _guestTypeServiceMock.Setup(s => s.GetAsync<APIResponse>(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(apiResponse);
            _mapperMock.Setup(m => m.Map<GuestTypeUpdateDTO>(It.IsAny<GuestTypeDTO>())).Returns(new GuestTypeUpdateDTO { Id = 1, Name = "GuestType1" });

              // Act
            var result = await _controller.UpdateGuestType(1);

             // Assert
             var viewResult = result as ViewResult;
             Assert.That(viewResult, Is.Not.Null);
             var model = viewResult.Model as GuestTypeUpdateDTO;
             Assert.That(model, Is.Not.Null);
             Assert.That(model.Id, Is.EqualTo(1));
             Assert.That(model.Name, Is.EqualTo("GuestType1"));
        }


        [Test]
        public async Task UpdateGuestType_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");
            var model = new GuestTypeUpdateDTO { Id = 1, Name = "" };

            // Act
            var result = await _controller.UpdateGuestType(1, model) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(result.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task DeleteGuestType_Get_ReturnsViewResult_WithGuestTypeDTO()
        {
            // Arrange
            var guestType = new GuestTypeDTO { Id = 1, Name = "Type1" };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(guestType) };
            _guestTypeServiceMock.Setup(s => s.GetAsync<APIResponse>(1, It.IsAny<string>())).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.DeleteGuestType(1) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var model = result.Model as GuestTypeDTO;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteGuestType_Post_ReturnsRedirectToActionResult_WhenDeletionIsSuccessful()
        {
            // Arrange
            var model = new GuestTypeDTO { Id = 1, Name = "Type1" };
            var apiResponse = new APIResponse { IsSuccess = true };
            _guestTypeServiceMock.Setup(s => s.DeleteAsync<APIResponse>(model.Id, It.IsAny<string>())).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.DeleteGuestType(1, model) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("IndexGuestType"));
        }

        [Test]
        public async Task DeleteGuestType_Post_ReturnsViewResult_WhenDeletionFails()
        {
            // Arrange
            var model = new GuestTypeDTO { Id = 1, Name = "Type1" };
            var apiResponse = new APIResponse { IsSuccess = false };
            _guestTypeServiceMock.Setup(s => s.DeleteAsync<APIResponse>(model.Id, It.IsAny<string>())).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.DeleteGuestType(1, model) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(result.Model, Is.EqualTo(model));
        }
    }
}
