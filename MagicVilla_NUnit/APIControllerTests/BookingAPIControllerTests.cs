using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class BookingAPIControllerTests
    {
        private Mock<IBookingRepository> _mockBookingRepository;
        private BookingAPIController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();
            _controller = new BookingAPIController(_mockBookingRepository.Object);
        }

        [Test]
        public async Task GetAllBookings_ReturnsOkResult()
        {
            // Arrange
            var bookings = new List<Booking> { new Booking { Id = 1, RoomId = 1, VillaId = 1 } };
            _mockBookingRepository.Setup(repo => repo.GetAllBookings()).ReturnsAsync(bookings);

            // Act
            var result = await _controller.GetAllBookings();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(bookings));
        }

        [Test]
        public async Task GetBookingById_ReturnsOkResult_WhenBookingExists()
        {
            // Arrange
            var booking = new Booking { Id = 1, RoomId = 1, VillaId = 1 };
            _mockBookingRepository.Setup(repo => repo.GetBookingById(1)).ReturnsAsync(booking);

            // Act
            var result = await _controller.GetBookingById(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(booking));
        }

        [Test]
        public async Task GetBookingById_ReturnsNotFound_WhenBookingDoesNotExist()
        {
            // Arrange
            _mockBookingRepository.Setup(repo => repo.GetBookingById(1)).ReturnsAsync((Booking)null);

            // Act
            var result = await _controller.GetBookingById(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("Booking not found"));
        }

        [Test]
        public async Task CreateBooking_ReturnsOkResult()
        {
            // Arrange
            var bookingDTO = new BookingDTO { RoomId = 1, VillaId = 1 };
            var response = new BookingResponseDTO { Message = "Booking created successfully" };
            _mockBookingRepository.Setup(repo => repo.BookVillaOrRoom(bookingDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateBooking(bookingDTO);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task UpdateBooking_ReturnsOkResult()
        {
            // Arrange
            var bookingDTO = new BookingDTO { RoomId = 1, VillaId = 1 };
            var response = "Booking updated successfully";
            _mockBookingRepository.Setup(repo => repo.UpdateBooking(1, bookingDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateBooking(1, bookingDTO);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task CancelBooking_ReturnsOkResult()
        {
            // Arrange
            var response = "Booking cancelled successfully";
            _mockBookingRepository.Setup(repo => repo.CancelBooking(1)).ReturnsAsync(response);

            // Act
            var result = await _controller.CancelBooking(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task GetBookedDates_ReturnsOkResult_WhenDatesExist()
        {
            // Arrange
            var bookedDates = new List<DateTime> { DateTime.UtcNow };
            _mockBookingRepository.Setup(repo => repo.GetBookedDatesByRoomIdAsync(1)).ReturnsAsync(bookedDates);

            // Act
            var result = await _controller.GetBookedDates(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var formattedDates = bookedDates.Select(d => d.ToString("yyyy-MM-dd")).ToList();
            Assert.That(okResult.Value, Is.EqualTo(formattedDates));
        }

        [Test]
        public async Task GetBookedDates_ReturnsOkResult_WhenNoDatesExist()
        {
            // Arrange
            _mockBookingRepository.Setup(repo => repo.GetBookedDatesByRoomIdAsync(1)).ReturnsAsync(new List<DateTime>());

            // Act
            var result = await _controller.GetBookedDates(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(new List<string>()));
        }

        [Test]
        public async Task CheckAvailability_ReturnsOkResult_WhenRoomIsAvailable()
        {
            // Arrange
            var checkIn = DateTime.UtcNow;
            var checkOut = DateTime.UtcNow.AddDays(1);
            _mockBookingRepository.Setup(repo => repo.IsRoomAvailable(1, 1, checkIn, checkOut)).ReturnsAsync(true);

            // Act
            var result = await _controller.CheckAvailability(1, 1, checkIn, checkOut);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var response = okResult.Value as AvailabilityResponseDTO;
            Assert.That(response.IsAvailable, Is.True);
            Assert.That(response.CheckIn, Is.EqualTo(checkIn.ToString("yyyy-MM-dd")));
            Assert.That(response.CheckOut, Is.EqualTo(checkOut.ToString("yyyy-MM-dd")));
        }

        [Test]
        public async Task CheckAvailability_ReturnsOkResult_WhenRoomIsNotAvailable()
        {
            // Arrange
            var checkIn = DateTime.UtcNow;
            var checkOut = DateTime.UtcNow.AddDays(1);
            _mockBookingRepository.Setup(repo => repo.IsRoomAvailable(1, 1, checkIn, checkOut)).ReturnsAsync(false);

            // Act
            var result = await _controller.CheckAvailability(1, 1, checkIn, checkOut);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var response = okResult.Value as AvailabilityResponseDTO;
            Assert.That(response.IsAvailable, Is.False);
            Assert.That(response.CheckIn, Is.EqualTo(checkIn.ToString("yyyy-MM-dd")));
            Assert.That(response.CheckOut, Is.EqualTo(checkOut.ToString("yyyy-MM-dd")));
        }
    }
}
