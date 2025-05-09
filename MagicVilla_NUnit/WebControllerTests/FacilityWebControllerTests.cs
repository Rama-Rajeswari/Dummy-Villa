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

namespace MagicVilla_NUnit.WebControllerTests
{
    [TestFixture]
    public class FacilityWebControllerTests
    {
        private Mock<IFacilityService> _facilityServiceMock;
        private Mock<IMapper> _mapperMock;
        private FacilityController _controller;
        private CustomSession _customSession;
        private Mock<ITempDataDictionary> _tempDataMock;

        [SetUp]
        public void Setup()
        {
            _facilityServiceMock = new Mock<IFacilityService>();
            _mapperMock = new Mock<IMapper>();
            _customSession = new CustomSession();
            _tempDataMock = new Mock<ITempDataDictionary>();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = _customSession;

            _controller = new FacilityController(_facilityServiceMock.Object, _mapperMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                },
                TempData = _tempDataMock.Object
            };

            // Set up custom session to return a test token
            _customSession.SetString("SessionToken", "test_token");
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task IndexFacility_ReturnsViewResult_WithPagedList()
        {
            // Arrange
            var facilities = new List<FacilityDTO>
            {
                new FacilityDTO { Id = 1, Name = "Facility1" },
                new FacilityDTO { Id = 2, Name = "Facility2" }
            };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(facilities) };
            _facilityServiceMock.Setup(s => s.GetAllAsync<APIResponse>(It.IsAny<string>())).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.IndexFacility(1, 5);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var model = viewResult.Model as IPagedList<FacilityDTO>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(2));
        }

        [Test]
        public void CreateFacility_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.CreateFacility();

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
        }

        [Test]
        public async Task CreateFacility_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var facilityCreateDTO = new FacilityCreateDTO { Name = "New Facility" };
            var apiResponse = new APIResponse { IsSuccess = true };
            _facilityServiceMock.Setup(s => s.CreateAsync<APIResponse>(It.IsAny<FacilityCreateDTO>(), It.IsAny<string>())).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.CreateFacility(facilityCreateDTO);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(redirectResult.ActionName, Is.EqualTo("IndexFacility"));
        }

        [Test]
        public async Task UpdateFacility_Get_ReturnsViewResult_WithFacilityUpdateDTO()
        {
            // Arrange
            var facilityDTO = new FacilityDTO { Id = 1, Name = "Facility1" };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(facilityDTO) };
            _facilityServiceMock.Setup(s => s.GetAsync<APIResponse>(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(apiResponse);
            _mapperMock.Setup(m => m.Map<FacilityUpdateDTO>(It.IsAny<FacilityDTO>())).Returns(new FacilityUpdateDTO { Id = 1, Name = "Facility1" });

            // Act
            var result = await _controller.UpdateFacility(1);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var model = viewResult.Model as FacilityUpdateDTO;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateFacility_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var facilityUpdateDTO = new FacilityUpdateDTO { Id = 1, Name = "Updated Facility" };
            var apiResponse = new APIResponse { IsSuccess = true };
            _facilityServiceMock.Setup(s => s.UpdateAsync<APIResponse>(It.IsAny<FacilityUpdateDTO>(), It.IsAny<string>())).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.UpdateFacility(1, facilityUpdateDTO);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(redirectResult.ActionName, Is.EqualTo("IndexFacility"));
        }

        [Test]
        public async Task DeleteFacility_Get_ReturnsViewResult_WithFacilityDTO()
        {
            // Arrange
            var facilityDTO = new FacilityDTO { Id = 1, Name = "Facility1" };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(facilityDTO) };
            _facilityServiceMock.Setup(s => s.GetAsync<APIResponse>(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.DeleteFacility(1);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var model = viewResult.Model as FacilityDTO;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteFacility_Post_ReturnsRedirectToActionResult_WhenDeletionIsSuccessful()
        {
            // Arrange
            var facilityDTO = new FacilityDTO { Id = 1, Name = "Facility1" };
            var apiResponse = new APIResponse { IsSuccess = true };
            _facilityServiceMock.Setup(s => s.GetAsync<APIResponse>(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(apiResponse);
            _facilityServiceMock.Setup(s => s.DeleteAsync<APIResponse>(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.DeleteFacility(1, facilityDTO);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(redirectResult.ActionName, Is.EqualTo("IndexFacility"));
        }
    }
}
