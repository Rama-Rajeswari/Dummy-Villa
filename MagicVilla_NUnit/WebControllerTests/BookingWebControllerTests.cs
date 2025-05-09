using NUnit.Framework;
using Moq;
using MagicVilla_Web.Controllers;
using MagicVilla_Web.Services;
using MagicVilla_Web.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MagicVilla_Utility;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MagicVilla_Web.Models;

namespace MagicVilla_NUnit.WebControllerTests
{
    [TestFixture]
    public class BookingControllerTests
    {
        private Mock<IBookingService> _bookingServiceMock;
        private Mock<IVillaService> _villaServiceMock;
        private Mock<IRoomService> _roomServiceMock;
        private BookingController _controller;
        private DefaultHttpContext _httpContext;
        private TempDataDictionary _tempData;

        [SetUp]
        public void SetUp()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _villaServiceMock = new Mock<IVillaService>();
            _roomServiceMock = new Mock<IRoomService>();

            _httpContext = new DefaultHttpContext();
            _httpContext.Session = new Mock<ISession>().Object;

            _tempData = new TempDataDictionary(_httpContext, Mock.Of<ITempDataProvider>());

            _controller = new BookingController(_bookingServiceMock.Object, _villaServiceMock.Object, _roomServiceMock.Object)
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
        public async Task Index_ReturnsViewResult_WithListOfBookingDTO()
        {
            // Arrange
            var bookings = new List<BookingDTO> { new BookingDTO {  VillaName = "Villa1" } };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(bookings) };
            _bookingServiceMock.Setup(s => s.GetAllAsync<APIResponse>("")).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<List<BookingDTO>>());
            var model = result.Model as List<BookingDTO>;
            Assert.That(model.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Create_Get_ReturnsViewResult_WithBookingDTO()
        {
            // Arrange
            var villaDTO = new VillaDTO { Id = 1, Name = "Villa1" };
            var roomDTO = new RoomDTO { Id = 1, RoomType = "Room1", MaxGuests = 4 };
            var villaResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(villaDTO) };
            var roomResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(roomDTO) };
            var bookedDates = new List<string> { "2025-04-25" };

            _villaServiceMock.Setup(s => s.GetAsync<APIResponse>(1, "")).ReturnsAsync(villaResponse);
            _roomServiceMock.Setup(s => s.GetAsync<APIResponse>(1, "")).ReturnsAsync(roomResponse);
            _bookingServiceMock.Setup(s => s.GetBookedDatesAsync(1)).ReturnsAsync(bookedDates);

            // Act
            var result = await _controller.Create(1, 1) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<BookingDTO>());
            var model = result.Model as BookingDTO;
            Assert.That(model.VillaId, Is.EqualTo(1));
            Assert.That(model.RoomId, Is.EqualTo(1));
            Assert.That(model.VillaName, Is.EqualTo("Villa1"));
            Assert.That(model.RoomName, Is.EqualTo("Room1"));
            Assert.That(model.MaxGuests, Is.EqualTo(4));
        }

        [Test]
        public async Task Create_Post_ReturnsViewResult_WhenDatesAreBooked()
        {
            // Arrange
            var bookingDTO = new BookingDTO
            {
                RoomId = 1,
                CheckIn = new DateTime(2025, 4, 25),
                CheckOut = new DateTime(2025, 4, 26)
            };
            var bookedDates = new List<string> { "2025-04-25", "2025-04-26" };

            _bookingServiceMock.Setup(s => s.GetBookedDatesAsync(1)).ReturnsAsync(bookedDates);

            // Act
            var result = await _controller.Create(bookingDTO) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<BookingDTO>());
            Assert.That(_controller.ModelState["CheckIn"].Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Create_Post_ReturnsBookingConfirmationView_WhenBookingIsSuccessful()
        {
            // Arrange
            var bookingDTO = new BookingDTO
            {
                RoomId = 1,
                CheckIn = new DateTime(2025, 4, 25),
                CheckOut = new DateTime(2025, 4, 26)
            };
            var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(new BookingResponseDTO { }) };

            _bookingServiceMock.Setup(s => s.GetBookedDatesAsync(1)).ReturnsAsync(new List<string>());
            _bookingServiceMock.Setup(s => s.CreateAsync<APIResponse>(bookingDTO, "")).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Create(bookingDTO) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("BookingConfirmation"));
            Assert.That(result.Model, Is.InstanceOf<BookingResponseDTO>());
            var model = result.Model as BookingResponseDTO;
            //Assert.That(model.BookingId, Is.EqualTo(1));
        }
    }
}
