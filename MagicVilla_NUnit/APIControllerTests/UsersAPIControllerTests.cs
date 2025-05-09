using System.Collections.Generic;
using System.Net;
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
    public class UsersControllerTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private UsersController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _controller = new UsersController(_mockUserRepository.Object);
        }

        [Test]
        public async Task Login_ReturnsOkResult_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO { UserName = "testuser", Password = "password123" };
            var loginResponse = new LoginResponseDTO { User = new UserDTO(), Token = "validToken" };
            _mockUserRepository.Setup(repo => repo.Login(loginRequest)).ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<APIResponse>());
            var response = okResult.Value as APIResponse;
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.Result, Is.EqualTo(loginResponse));
        }

        [Test]
        public async Task Login_ReturnsBadRequest_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO { UserName = "testuser", Password = "wrongpassword" };
            var loginResponse = new LoginResponseDTO { User = null, Token = null };
            _mockUserRepository.Setup(repo => repo.Login(loginRequest)).ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.InstanceOf<APIResponse>());
            var response = badRequestResult.Value as APIResponse;
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorMessages, Contains.Item("UserName or Password Incorrect"));
        }

        [Test]
        public async Task Register_ReturnsOkResult_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registrationRequest = new RegisterationRequestDTO { UserName = "newuser", Password = "password123" };
            _mockUserRepository.Setup(repo => repo.IsUniqueUser(registrationRequest.UserName)).Returns(true);
            _mockUserRepository.Setup(repo => repo.Register(registrationRequest)).ReturnsAsync(new UserDTO());

            // Act
            var result = await _controller.Register(registrationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<APIResponse>());
            var response = okResult.Value as APIResponse;
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.IsSuccess, Is.True);
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenUserNameIsNotUnique()
        {
            // Arrange
            var registrationRequest = new RegisterationRequestDTO { UserName = "existinguser", Password = "password123" };
            _mockUserRepository.Setup(repo => repo.IsUniqueUser(registrationRequest.UserName)).Returns(false);

            // Act
            var result = await _controller.Register(registrationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.InstanceOf<APIResponse>());
            var response = badRequestResult.Value as APIResponse;
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorMessages, Contains.Item("UserName already exists"));
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var registrationRequest = new RegisterationRequestDTO { UserName = "newuser", Password = "password123" };
            _mockUserRepository.Setup(repo => repo.IsUniqueUser(registrationRequest.UserName)).Returns(true);
            _mockUserRepository.Setup(repo => repo.Register(registrationRequest)).ReturnsAsync((UserDTO)null);

            // Act
            var result = await _controller.Register(registrationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.InstanceOf<APIResponse>());
            var response = badRequestResult.Value as APIResponse;
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.ErrorMessages, Contains.Item("Error while registering"));
        }
         [Test]
         public async Task ResetPassword_ReturnsOkResult_WhenPasswordResetIsSuccessful()
        {
           // Arrange
           var forgotPasswordDTO = new ForgotPasswordDTO { UserName = "testuser", NewPassword = "newpassword123" };
          _mockUserRepository.Setup(repo => repo.ResetPasswordAsync(forgotPasswordDTO)).ReturnsAsync(true);

             // Act
           var result = await _controller.ResetPassword(forgotPasswordDTO);

           // Assert
          Assert.That(result, Is.InstanceOf<OkObjectResult>());
          var okResult = result as OkObjectResult;
          Assert.That(okResult.Value, Is.InstanceOf<object>());

          // Use reflection to access the message property
          var response = okResult.Value;
          var messageProperty = response.GetType().GetProperty("message");
          Assert.That(messageProperty, Is.Not.Null);
          var messageValue = messageProperty.GetValue(response) as string;
          Assert.That(messageValue, Is.EqualTo("Password reset successfully."));
        }
         [Test]
        public async Task ResetPassword_ReturnsBadRequest_WhenPasswordResetFails()
        {
           // Arrange
            var forgotPasswordDTO = new ForgotPasswordDTO { UserName = "testuser", NewPassword = "newpassword123" };
             _mockUserRepository.Setup(repo => repo.ResetPasswordAsync(forgotPasswordDTO)).ReturnsAsync(false);

          // Act
              var result = await _controller.ResetPassword(forgotPasswordDTO);

         // Assert
         Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
         var badRequestResult = result as BadRequestObjectResult;
         Assert.That(badRequestResult.Value, Is.InstanceOf<object>());

         // Use reflection to access the message property
         var response = badRequestResult.Value;
         var messageProperty = response.GetType().GetProperty("message");
         Assert.That(messageProperty, Is.Not.Null);
         var messageValue = messageProperty.GetValue(response) as string;
         Assert.That(messageValue, Is.EqualTo("Invalid username or failed to reset password."));
        }

    }
}
