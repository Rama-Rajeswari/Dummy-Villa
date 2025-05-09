using NUnit.Framework;
using Moq;
using AutoMapper;
using MagicVilla_Web.Controllers;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MagicVilla_Web.Models;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MagicVilla_Utility;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Session;

namespace MagicVilla_NUnit.WebControllerTests
{
    [TestFixture]
    public class DestinationControllerTests : IDisposable
    {
        private Mock<IDestinationService> _destinationServiceMock;
        private Mock<IVillaService> _villaServiceMock;
        private Mock<IMapper> _mapperMock;
        private DestinationController _controller;
        private CustomSession _customSession;
        private Mock<ITempDataDictionary> _tempDataMock;

        [SetUp]
        public void SetUp()
        {
            _destinationServiceMock = new Mock<IDestinationService>();
            _villaServiceMock = new Mock<IVillaService>();
            _mapperMock = new Mock<IMapper>();
            _customSession = new CustomSession();
            _tempDataMock = new Mock<ITempDataDictionary>();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = _customSession;

            _controller = new DestinationController(_destinationServiceMock.Object, _mapperMock.Object, _villaServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                },
                TempData = _tempDataMock.Object
            };

            // Set up custom session to return a test token
            _customSession.SetString(SD.SessionToken, "test_token");
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        public void Dispose()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task IndexDestination_ReturnsViewResult_WithPagedList()
        {
            // Arrange
            var destinations = new List<DestinationDTO>
            {
                new DestinationDTO { Id = 1, Name = "Destination 1" },
                new DestinationDTO { Id = 2, Name = "Destination 2" }
            };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(destinations) };
            _destinationServiceMock.Setup(s => s.GetAllAsync<APIResponse>(It.IsAny<string>())).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.IndexDestination(1, 5) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<IPagedList<DestinationDTO>>());
        }

        [Test]
        public void CreateDestination_ReturnsViewResult()
        {
            // Act
            var result = _controller.CreateDestination() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task CreateDestination_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var model = new DestinationCreateDTO { Name = "New Destination" };
            var apiResponse = new APIResponse { IsSuccess = true };
            _destinationServiceMock.Setup(s => s.CreateAsync<APIResponse>(model, It.IsAny<string>())).ReturnsAsync(apiResponse);

            var customSession = new CustomSession();
            customSession.SetString(SD.SessionToken, "mockSessionToken");

            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = customSession });
            _controller.ControllerContext.HttpContext = httpContext;
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Act
            _controller.ModelState.Clear(); // Ensure model state is valid
            var result = await _controller.CreateDestination(model);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult, Is.Not.Null, "Expected a RedirectToActionResult, but got null. Check if the session token and API response are correctly set.");
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(DestinationController.IndexDestination)));
        }

        [Test]
        public async Task UpdateDestination_Get_ReturnsViewResult_WithDestinationUpdateDTO()
        {
             // Arrange
             var destinationDTO = new DestinationDTO { Id = 1, Name = "Destination 1" };
             var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(destinationDTO) };
             _destinationServiceMock.Setup(s => s.GetAsync<APIResponse>(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(apiResponse);
             _mapperMock.Setup(m => m.Map<DestinationUpdateDTO>(It.IsAny<DestinationDTO>())).Returns(new DestinationUpdateDTO { Id = 1, Name = "Updated Destination" });

             var customSession = new CustomSession();
             customSession.SetString(SD.SessionToken, "mockSessionToken");

             var httpContext = new DefaultHttpContext();
             httpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = customSession });
             _controller.ControllerContext.HttpContext = httpContext;

              // Act
             var result = await _controller.UpdateDestination(1);

             // Assert
             var viewResult = result as ViewResult;
             Assert.That(viewResult, Is.Not.Null);
             var model = viewResult.Model as DestinationUpdateDTO;
             Assert.That(model, Is.Not.Null);
             Assert.That(model.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateDestination_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var destinationUpdateDTO = new DestinationUpdateDTO { Id = 1, Name = "Updated Destination" };
            var apiResponse = new APIResponse { IsSuccess = true };
            _destinationServiceMock.Setup(s => s.UpdateAsync<APIResponse>(destinationUpdateDTO, It.IsAny<string>())).ReturnsAsync(apiResponse);

            var customSession = new CustomSession();
            customSession.SetString(SD.SessionToken, "mockSessionToken");

            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = customSession });
            _controller.ControllerContext.HttpContext = httpContext;
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Act
            _controller.ModelState.Clear(); // Ensure model state is valid
            var result = await _controller.UpdateDestination(destinationUpdateDTO);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult, Is.Not.Null, "Expected a RedirectToActionResult, but got null. Check if the session token and API response are correctly set.");
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(DestinationController.IndexDestination)));
        }

        [Test]
        public async Task DeleteDestination_Get_ReturnsViewResult_WithModel()
        {
            // Arrange
            var destination = new DestinationDTO { Id = 1, Name = "Destination 1" };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(destination) };
            _destinationServiceMock.Setup(s => s.GetAsync<APIResponse>(1, It.IsAny<string>())).ReturnsAsync(apiResponse);

            var customSession = new CustomSession();
            customSession.SetString(SD.SessionToken, "mockSessionToken");

            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = customSession });
            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = await _controller.DeleteDestination(1) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<DestinationDTO>());
        }

        [Test]
        public async Task DeleteDestination_Post_ReturnsRedirectToActionResult_WhenDeletionIsSuccessful()
        {
            // Arrange
            var destination = new DestinationDTO { Id = 1, Name = "Destination to Delete" };
            var apiResponseGet = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(destination) };
            var apiResponseDelete = new APIResponse { IsSuccess = true };
            _destinationServiceMock.Setup(s => s.GetAsync<APIResponse>(1, It.IsAny<string>())).ReturnsAsync(apiResponseGet);
            _destinationServiceMock.Setup(s => s.DeleteAsync<APIResponse>(1, It.IsAny<string>())).ReturnsAsync(apiResponseDelete);

            var customSession = new CustomSession();
            customSession.SetString(SD.SessionToken, "mockSessionToken");

            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = customSession });
            _controller.ControllerContext.HttpContext = httpContext;
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Act
            var resultGetView = await _controller.DeleteDestination(1) as ViewResult;

            // Assert Get View Result
            Assert.That(resultGetView, Is.Not.Null);
            Assert.That(((DestinationDTO)resultGetView.Model).Id, Is.EqualTo(destination.Id));

            // Act Post Delete Action
            var resultPostDeleteAction = await _controller.DeleteDestination(destination) as RedirectToActionResult;

            // Assert Post Delete Action Result
            Assert.That(resultPostDeleteAction, Is.Not.Null);
            Assert.That(resultPostDeleteAction.ActionName, Is.EqualTo(nameof(DestinationController.IndexDestination)));
        }
    }
}
