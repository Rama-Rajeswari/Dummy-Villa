 using NUnit.Framework;
using MagicVilla_VillaAPI.Controller; // Adjust this namespace to match your project
using AutoMapper;
using Moq;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository;

namespace MagicVilla_NUnit.APIControllerTests
{
    

  [TestFixture]
    public class VillaNumberAPIControllerTests
    {
        private VillaNumberAPIController _controller;

        [SetUp]
        public void Setup()
        {
            var mockVillaRepo = new Mock<IVillaRepository>();
            var mockVillaNumberRepo = new Mock<IVillaNumberRepository>();
            var mockMapper = new Mock<IMapper>();
            var response = new APIResponse();

            _controller = new VillaNumberAPIController(
                mockVillaNumberRepo.Object,
                mockMapper.Object,
                response,
                mockVillaRepo.Object
            );
        }

        [Test]
        public void TestMethod_ReturnsExpectedString()
        {
            // Act
            var result = _controller.TestMethod();

            // Assert
            Assert.That(result,Is.EqualTo("Hello, CI/CD!"));
        }
    }
}
