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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MagicVilla_Web.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MagicVilla_NUnit.WebControllerTests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _authServiceMock;
        private AuthController _controller;
        private DefaultHttpContext _httpContext;
        private TempDataDictionary _tempData;
        private Mock<ISession> _mockSession;
       private ActionContext _actionContext;
       private Mock<IAuthenticationService> _mockAuthenticationService;
       [SetUp]
       public void SetUp()
       {
            _authServiceMock = new Mock<IAuthService>();
            _httpContext = new DefaultHttpContext();
            _httpContext.Session = new Mock<ISession>().Object;
            _mockSession = new Mock<ISession>();
            _httpContext.Session = _mockSession.Object;
            _tempData = new TempDataDictionary(_httpContext, Mock.Of<ITempDataProvider>());
            _mockAuthenticationService = new Mock<IAuthenticationService>();
             var serviceProvider = new ServiceCollection()
                .AddSingleton(_mockAuthenticationService.Object)
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddSingleton<IUrlHelperFactory, UrlHelperFactory>()
                .BuildServiceProvider();
            _httpContext.RequestServices = serviceProvider;

            // Initialize ActionContext
            _actionContext = new ActionContext(
                _httpContext,
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            );
            var urlHelperFactory = serviceProvider.GetRequiredService<IUrlHelperFactory>();
             var urlHelper = urlHelperFactory.GetUrlHelper(_actionContext);

            

             _controller = new AuthController(_authServiceMock.Object)
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
        public void Login_Get_ReturnsViewResult_WithLoginRequestDTO()
        {
            // Act
            var result = _controller.Login() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<LoginRequestDTO>());
        }

     [Test]
     public async Task Login_Post_ReturnsRedirectToActionResult_WhenLoginIsSuccessful()
     {
        // Arrange
        var loginRequestDTO = new LoginRequestDTO { UserName = "testuser", Password = "password" };
        var loginResponseDTO = new LoginResponseDTO { Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InRlc3R1c2VyIiwicm9sZSI6IkFkbWluIiwiaWF0IjoxNjE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c" };
        var apiResponse = new APIResponse { IsSuccess = true, Result = JsonConvert.SerializeObject(loginResponseDTO) };

        _authServiceMock.Setup(s => s.LoginAsync<APIResponse>(loginRequestDTO)).ReturnsAsync(apiResponse);

        var authServiceMock = new Mock<IAuthenticationService>();
        var urlHelperFactoryMock = new Mock<IUrlHelperFactory>();
        var actionContextAccessorMock = new Mock<IActionContextAccessor>();

         _httpContext.RequestServices = new ServiceCollection()
        .AddSingleton(authServiceMock.Object)
        .AddSingleton(urlHelperFactoryMock.Object)
        .AddSingleton(actionContextAccessorMock.Object)
        .BuildServiceProvider();

        // Act
           var result = await _controller.Login(loginRequestDTO) as RedirectToActionResult;

       // Assert
           Assert.That(result, Is.Not.Null);
           Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
     }


        [Test]
        public async Task Login_Post_ReturnsViewResult_WhenLoginFails()
        {
            // Arrange
            var loginRequestDTO = new LoginRequestDTO { UserName = "testuser", Password = "password" };
            var apiResponse = new APIResponse { IsSuccess = false, ErrorMessages = new List<string> { "Invalid credentials" } };

            _authServiceMock.Setup(s => s.LoginAsync<APIResponse>(loginRequestDTO)).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Login(loginRequestDTO) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<LoginRequestDTO>());
            Assert.That(_controller.ModelState["CustomError"].Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void Profile_Get_ReturnsRedirectToActionResult_WhenSessionIsEmpty()
        {
            // Act
            var result = _controller.Profile() as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Login"));
            Assert.That(result.ControllerName, Is.EqualTo("Auth"));
        }

        [Test]
        public void Profile_Get_ReturnsViewResult_WithRegisterationRequestDTO()
        {
            // Arrange
           var session = new Mock<ISession>();
           var keyUserRole = "UserRole";
           var keyName = "Name";
           var valueUserRole = "Admin";
           var valueName = "Test User";

            session.Setup(s => s.TryGetValue(keyUserRole, out It.Ref<byte[]>.IsAny)).Returns((string k, out byte[] v) => { v = System.Text.Encoding.UTF8.GetBytes(valueUserRole); return true; });
            session.Setup(s => s.TryGetValue(keyName, out It.Ref<byte[]>.IsAny)).Returns((string k, out byte[] v) => { v = System.Text.Encoding.UTF8.GetBytes(valueName); return true; });

             _httpContext.Session = session.Object;

             var urlHelperFactoryMock = new Mock<IUrlHelperFactory>();
             var actionContextAccessorMock = new Mock<IActionContextAccessor>();

              _httpContext.RequestServices = new ServiceCollection()
                 .AddSingleton(urlHelperFactoryMock.Object)
                 .AddSingleton(actionContextAccessorMock.Object)
                 .BuildServiceProvider();

             // Act
              var result = _controller.Profile() as ViewResult;

             // Assert
             Assert.That(result, Is.Not.Null);
             Assert.That(result.Model, Is.InstanceOf<RegisterationRequestDTO>());
             var model = result.Model as RegisterationRequestDTO;
             Assert.That(model.Role, Is.EqualTo("Admin"));
             Assert.That(model.Name, Is.EqualTo("Test User"));
        }


        [Test]
        public async Task Register_Post_ReturnsRedirectToActionResult_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerationRequestDTO = new RegisterationRequestDTO { UserName = "testuser", Password = "password" };
            var apiResponse = new APIResponse { IsSuccess = true };

            _authServiceMock.Setup(s => s.RegisterAsync<APIResponse>(registerationRequestDTO)).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Register(registerationRequestDTO) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Login"));
        }
        [Test]
        public async Task Register_Post_ReturnsViewResult_WhenRegistrationFails()
        {
              // Arrange
             var registerationRequestDTO = new RegisterationRequestDTO { UserName = "testuser", Password = "password" };
             var apiResponse = new APIResponse { IsSuccess = false };

             _authServiceMock.Setup(s => s.RegisterAsync<APIResponse>(registerationRequestDTO)).ReturnsAsync(apiResponse);

              // Act
             var result = await _controller.Register(registerationRequestDTO) as ViewResult;

             // Assert
              Assert.That(result, Is.Not.Null);
             Assert.That(result.Model, Is.InstanceOf<RegisterationRequestDTO>());
             Assert.That(result.Model, Is.EqualTo(registerationRequestDTO));
        }
        [Test]
        public async Task Logout_ShouldSignOutAndClearSessionToken()
        {
            // Arrange
            _mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()));

            // Act
            var result = await _controller.Logout();

            // Assert
            _mockAuthenticationService.Verify(a => a.SignOutAsync(
                It.IsAny<HttpContext>(), 
                It.IsAny<string>(), 
                It.IsAny<AuthenticationProperties>()), 
                Times.Once);
            _mockSession.Verify(s => s.Set(SD.SessionToken, It.Is<byte[]>(b => b.Length == 0)), Times.Once);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Home"));
        }


        [Test]
        public void AccessDenied_ReturnsViewResult()
        {
            // Act
            var result = _controller.AccessDenied() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void ForgotPassword_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.ForgotPassword() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task ForgotPassword_Post_ReturnsRedirectToActionResult_WhenPasswordResetIsSuccessful()
        {
            // Arrange
            var forgotPasswordDTO = new ForgotPasswordDTO { UserName = "test@example.com" };
            var apiResponse = new APIResponse { IsSuccess = true };

            _authServiceMock.Setup(s => s.ForgotPasswordAsync<APIResponse>(forgotPasswordDTO)).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.ForgotPassword(forgotPasswordDTO) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Login"));
        }

        [Test]
        public async Task ForgotPassword_Post_ReturnsViewResult_WhenPasswordResetFails()
        {
            // Arrange
            var forgotPasswordDTO = new ForgotPasswordDTO { UserName = "test@example.com" };
            var apiResponse = new APIResponse { IsSuccess = false };

            _authServiceMock.Setup(s => s.ForgotPasswordAsync<APIResponse>(forgotPasswordDTO)).ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.ForgotPassword(forgotPasswordDTO) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<ForgotPasswordDTO>());
            Assert.That(_controller.ModelState[""].Errors.Count, Is.EqualTo(1));
        }
    }
}
