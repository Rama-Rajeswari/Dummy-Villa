using NUnit.Framework;
using Moq;
using MagicVilla_Web.Controllers;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MagicVilla_Web.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MagicVilla_NUnit.WebControllerTests;
using MagicVilla_Utility;

namespace MagicVilla_Web.Tests
{
    [TestFixture]
    public class VillaControllerTests : IDisposable
    {
        private Mock<IVillaService> _villaServiceMock;
        private Mock<IRoomService> _roomServiceMock;
        private Mock<IMapper> _mapperMock;
        private VillaController _controller;

        [SetUp]
        public void SetUp()
        {
            _villaServiceMock = new Mock<IVillaService>();
            _roomServiceMock = new Mock<IRoomService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new VillaController(_villaServiceMock.Object, _mapperMock.Object, _roomServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        public void Dispose()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task IndexVilla_ReturnsViewResult_WithPagedList()
        {
            // Arrange
            var villaList = new List<VillaDTO> { new VillaDTO(), new VillaDTO() };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(villaList) };
            _villaServiceMock.Setup(service => service.GetAllAsync<APIResponse>(It.IsAny<string>())).ReturnsAsync(apiResponse);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Session = new Mock<ISession>().Object;

            // Act
            var result = await _controller.IndexVilla(1, 5);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var model = viewResult.Model as IEnumerable<VillaDTO>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task CreateVilla_Get_ReturnsViewResult()
        {
            // Act
            var result = await _controller.CreateVilla();

           // Assert
           var viewResult = result as ViewResult;
           Assert.That(viewResult, Is.Not.Null);
         }


         [Test]
         public async Task CreateVilla_Post_RedirectsToIndexVilla_OnSuccess()
        {
            // Arrange
             var villaCreateDto = new VillaCreateDTO();
             var apiResponse = new APIResponse { IsSuccess = true };
             _villaServiceMock.Setup(service => service.CreateAsync<APIResponse>(villaCreateDto, It.IsAny<string>())).ReturnsAsync(apiResponse);

            var customSession = new CustomSession();
             customSession.SetString(SD.SessionToken, "mockSessionToken");

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Session = customSession;
            _controller.TempData = new Mock<ITempDataDictionary>().Object;

              // Act
             var result = await _controller.CreateVilla(villaCreateDto);

            // Assert
             var redirectResult = result as RedirectToActionResult;
             Assert.That(redirectResult, Is.Not.Null);
             Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(VillaController.IndexVilla)));
         }




                [Test]
        public async Task UpdateVilla_Get_ReturnsViewResult_WithVillaUpdateDTO()
        {
            // Arrange
            var villaDto = new VillaDTO();
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(villaDto) };
            _villaServiceMock.Setup(service => service.GetAsync<APIResponse>(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(apiResponse);
            _mapperMock.Setup(mapper => mapper.Map<VillaUpdateDTO>(It.IsAny<VillaDTO>())).Returns(new VillaUpdateDTO());
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Session = new Mock<ISession>().Object;

            // Act
            var result = await _controller.UpdateVilla(1);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var model = viewResult.Model as VillaUpdateDTO;
            Assert.That(model, Is.Not.Null);
        }
         [Test]
         public async Task UpdateVilla_Post_RedirectsToIndexVilla_OnSuccess()
         {
             // Arrange
            var villaUpdateDto = new VillaUpdateDTO();
            var apiResponse = new APIResponse { IsSuccess = true };
            _villaServiceMock.Setup(service => service.UpdateAsync<APIResponse>(villaUpdateDto, It.IsAny<string>())).ReturnsAsync(apiResponse);

            var customSession = new CustomSession();
            customSession.SetString(SD.SessionToken, "mockSessionToken");

            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
             _controller.ControllerContext.HttpContext.Session = customSession;
             _controller.TempData = new Mock<ITempDataDictionary>().Object;

            // Act
             var result = await _controller.UpdateVilla(villaUpdateDto);

             // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(VillaController.IndexVilla)));
        }


        [Test]
        public async Task DeleteVilla_Get_ReturnsViewResult_WithVillaDTO()
        {
            // Arrange
            var villaDto = new VillaDTO();
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(villaDto) };
            _villaServiceMock.Setup(service => service.GetAsync<APIResponse>(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(apiResponse);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Session = new Mock<ISession>().Object;

            // Act
            var result = await _controller.DeleteVilla(1);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var model = viewResult.Model as VillaDTO;
            Assert.That(model, Is.Not.Null);
        }
         [Test]
         public async Task DeleteVilla_Post_RedirectsToIndexVilla_OnSuccess()
         {
                // Arrange
                var villaDto = new VillaDTO { Id = 1 };
                var apiResponse = new APIResponse { IsSuccess = true };
                 _villaServiceMock.Setup(service => service.DeleteAsync<APIResponse>(villaDto.Id, It.IsAny<string>())).ReturnsAsync(apiResponse);

                var customSession = new CustomSession();
                 customSession.SetString(SD.SessionToken, "mockSessionToken");

                 _controller.ControllerContext.HttpContext = new DefaultHttpContext();
                  _controller.ControllerContext.HttpContext.Session = customSession;
                 _controller.TempData = new Mock<ITempDataDictionary>().Object;

                 // Act
                 var result = await _controller.DeleteVilla(villaDto);

                // Assert
                var redirectResult = result as RedirectToActionResult;
                 Assert.That(redirectResult, Is.Not.Null);
                 Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(VillaController.IndexVilla)));
        }

    }
}

