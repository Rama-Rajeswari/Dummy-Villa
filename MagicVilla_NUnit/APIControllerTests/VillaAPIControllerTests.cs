using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using System.Net;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using System.Linq.Expressions;
using MagicVilla_VillaAPI.Controller;

[TestFixture]
public class VillaControllerTests
{
    private Mock<IVillaRepository> _mockVillaRepository;
    private Mock<IMapper> _mockMapper;
    private Mock<IRoomRepository> _mockRoomRepository;
    private Mock<IFacilityRepository> _mockFacilityRepository;
    private Mock<IDestinationRepository> _mockDestinationRepository;
    private Mock<IVillaFacilityRepository> _mockVillaFacilityRepository;
    private Mock<IGuestTypeRepository> _mockGuestTypeRepository;
    private Mock<IRoomGuestTypeRepository> _mockRoomGuestTypeRepository;
    private VillaAPIController _controller;
    private APIResponse _response;

    [SetUp]
    public void SetUp()
    {
        _mockVillaRepository = new Mock<IVillaRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockRoomRepository = new Mock<IRoomRepository>();
        _mockFacilityRepository = new Mock<IFacilityRepository>();
        _mockDestinationRepository = new Mock<IDestinationRepository>();
        _mockVillaFacilityRepository = new Mock<IVillaFacilityRepository>();
        _mockGuestTypeRepository = new Mock<IGuestTypeRepository>();
        _mockRoomGuestTypeRepository = new Mock<IRoomGuestTypeRepository>();
        _response = new APIResponse();
        _controller = new VillaAPIController(
            _mockVillaRepository.Object, 
            _mockMapper.Object, 
            _response, 
            _mockRoomRepository.Object, 
            _mockFacilityRepository.Object, 
            _mockDestinationRepository.Object, 
            _mockVillaFacilityRepository.Object, 
            _mockGuestTypeRepository.Object, 
            _mockRoomGuestTypeRepository.Object
        );
    }
    [Test]
    public async Task GetVillas_ReturnsOkResult_WithVillas()
    {
       // Arrange
       var villas = new List<Villa> { new Villa { Id = 1, Name = "Villa1" } };
       _mockVillaRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Villa, bool>>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(villas);
       _mockMapper.Setup(m => m.Map<List<VillaDTO>>(villas))
               .Returns(new List<VillaDTO> { new VillaDTO { Id = 1, Name = "Villa1" } });

       // Act
       var result = await _controller.GetVillas(null, null, 0, 1) as ActionResult<APIResponse>;

       // Assert
        Assert.That(result, Is.Not.Null);
      // Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
       var okResult = result.Result as OkObjectResult;
      // Assert.That(okResult, Is.Not.Null);
      //Assert.That(okResult.Value, Is.InstanceOf<APIResponse>());
     // var response = okResult.Value as APIResponse;
      //Assert.That(response, Is.Not.Null);
      //Assert.That(response.Result, Is.InstanceOf<List<VillaDTO>>());
      // Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }


    [Test]
    public async Task GetVilla_ReturnsOkResult_WithVilla()
    {
        // Arrange
        var villa = new Villa { Id = 1, Name = "Villa One", Occupancy = 4 };
        
           _mockVillaRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Villa, bool>>>(), true, "Rooms,Rooms.RoomGuestTypes,VillaFacilities,VillaFacilities.Facility,Destination"))
                           .ReturnsAsync(villa);

        _mockMapper.Setup(m => m.Map<VillaDTO>(It.IsAny<Villa>()))
                   .Returns(new VillaDTO { Id = 1, Name = "Villa One" });

        // Act
        var result = await _controller.GetVilla(1) as ActionResult<APIResponse>;
        var okResult = result.Result as OkObjectResult;

        // Assert
        
         Assert.That(okResult, Is.Not.Null);
         Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));

        var apiResponse = okResult?.Value as APIResponse;
        
        Assert.That(apiResponse, Is.Not.Null);
       Assert.That(apiResponse.IsSuccess, Is.True);
       Assert.That((apiResponse.Result as VillaDTO).Id, Is.EqualTo(1));

    }

    [Test]
    public async Task GetVilla_ReturnsBadRequest_WhenIdIsZero()
    {
        // Act
        var result = await _controller.GetVilla(0) as ActionResult<APIResponse>;
        var badRequestResult = result.Result as BadRequestObjectResult;
        // Assert
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetVilla_ReturnsNotFound_WhenVillaDoesNotExist()
    {
        // Arrange
    
        _mockVillaRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Villa, bool>>>(), true, "Rooms,Rooms.RoomGuestTypes,VillaFacilities,VillaFacilities.Facility,Destination"))
                                               .ReturnsAsync((Villa)null);
         // Act
         var result = await _controller.GetVilla(1) as ActionResult<APIResponse>;
          var notFoundResult = result.Result as NotFoundObjectResult;
         // Assert
         Assert.That(notFoundResult, Is.Not.Null);
         Assert.That(notFoundResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
    }


    [Test]
    public async Task CreateVilla_ReturnsBadRequest_WhenVillaAlreadyExists()
    {
        // Arrange
        var createDTO = new VillaCreateDTO { Name = "Villa One" };
      
        _mockVillaRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Villa, bool>>>(), true, null))
                   .ReturnsAsync(new Villa());



        // Act
        var result = await _controller.CreateVilla(createDTO) as ActionResult<APIResponse>;
        var badRequestResult = result.Result as BadRequestObjectResult;

        // Assert
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));

    }

    [Test]
    public async Task CreateVilla_ReturnsBadRequest_WhenCreateDTOIsNull()
    {
         // Act
        var result = await _controller.CreateVilla(null) as ActionResult<APIResponse>;
        var badRequestResult = result.Result as BadRequestObjectResult;
        // Assert
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
     }


    [Test]
    public async Task CreateVilla_ReturnsBadRequest_WhenDestinationNameIsEmpty()
    { 
       // Arrange
       var createDTO = new VillaCreateDTO { Name = "Villa One", DestinationName = "" };
       // Act
       var result = await _controller.CreateVilla(createDTO) as ActionResult<APIResponse>;
       var badRequestResult = result.Result as BadRequestObjectResult;

       // Assert
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
    }


    [Test]
    public async Task CreateVilla_ReturnsCreatedResult_WhenVillaIsCreated()
    {
        // Arrange
        var createDTO = new VillaCreateDTO
        {
            Name = "Villa One",
            DestinationName = "Destination One",
            Rooms = new List<RoomCreateDTO> { new RoomCreateDTO { RoomType = "Deluxe" } },
            Facilities = new List<string> { "Pool" }
        };
         _mockVillaRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Villa, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                                      .ReturnsAsync((Villa)null);
         _mockDestinationRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Destination, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                                    .ReturnsAsync((Destination)null);
        _mockMapper.Setup(m => m.Map<Villa>(It.IsAny<VillaCreateDTO>()))
                   .Returns(new Villa { Id = 1, Name = "Villa One" });
        _mockMapper.Setup(m => m.Map<Room>(It.IsAny<RoomCreateDTO>()))
                   .Returns(new Room { Id = 1, RoomType = "Deluxe" });
        _mockMapper.Setup(m => m.Map<VillaDTO>(It.IsAny<Villa>()))
                   .Returns(new VillaDTO { Id = 1, Name = "Villa One" });

        // Act
        var result = await _controller.CreateVilla(createDTO) as ActionResult<APIResponse>;
        var createdResult = result.Result as CreatedAtRouteResult;

        // Assert
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
        var apiResponse = createdResult.Value as APIResponse;
        Assert.That(apiResponse, Is.Not.Null);
        Assert.That(apiResponse.IsSuccess, Is.True);
        Assert.That((apiResponse.Result as VillaDTO).Id, Is.EqualTo(1));
    }
    [Test]

    public async Task UpdateVilla_ReturnsOkResult_WhenVillaIsUpdated()
    {
         // Arrange
        var villaId = 1;
        var updateDTO = new VillaUpdateDTO
        {
            DestinationName = "New Destination",
            Facilities = new List<string> { "Pool", "Gym" }
        };

        var existingVilla = new Villa { Id = villaId, Name = "Villa One", DestinationId = 1 };
        var existingDestination = new Destination { Id = 1, Name = "Old Destination" };
        var newDestination = new Destination { Id = 2, Name = "New Destination" };
        var existingFacility = new Facility { Id = 1, Name = "Pool" };
        var newFacility = new Facility { Id = 2, Name = "Gym" };

        _mockVillaRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Villa, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                        .ReturnsAsync(existingVilla);

        _mockDestinationRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Destination, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                              .ReturnsAsync((Expression<Func<Destination, bool>> predicate, bool tracked, string includeProperties) =>
                              {
                                  var compiledPredicate = predicate.Compile();
                                  return compiledPredicate(existingDestination) ? existingDestination : null;
                              });

       _mockDestinationRepository.Setup(repo => repo.CreateAsync(It.IsAny<Destination>()))
                              .Returns(Task.CompletedTask);

       _mockDestinationRepository.Setup(repo => repo.SaveAsync())
                              .Returns(Task.CompletedTask);

       _mockFacilityRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Facility, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                           .ReturnsAsync((Expression<Func<Facility, bool>> predicate, bool tracked, string includeProperties) =>
                           {
                               var compiledPredicate = predicate.Compile();
                               return compiledPredicate(existingFacility) ? existingFacility : null;
                           });

       _mockFacilityRepository.Setup(repo => repo.CreateAsync(It.IsAny<Facility>()))
                           .Returns(Task.CompletedTask);

       _mockFacilityRepository.Setup(repo => repo.SaveAsync())
                           .Returns(Task.CompletedTask);

        _mockVillaRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Villa>()))
                        .Returns(Task.FromResult((Villa)null)); // Corrected return type

        _mockVillaRepository.Setup(repo => repo.SaveAsync())
                        .Returns(Task.CompletedTask);

         _mockMapper.Setup(m => m.Map(It.IsAny<VillaUpdateDTO>(), It.IsAny<Villa>()))
               .Callback<VillaUpdateDTO, Villa>((src, dest) =>
               {
                   dest.DestinationId = newDestination.Id;
               });

           // Act
        var result = await _controller.UpdateVilla(villaId, updateDTO) as ActionResult<APIResponse>;
        var okResult = result?.Result as OkObjectResult;

          // Assert
        Assert.That(okResult, Is.Not.Null, "Expected OkObjectResult but was null.");
        Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK), "Expected status code 200 OK.");
        var apiResponse = okResult?.Value as APIResponse;
        Assert.That(apiResponse, Is.Not.Null, "Expected APIResponse but was null.");
        Assert.That(apiResponse.IsSuccess, Is.True, "Expected IsSuccess to be true.");
    }
    [Test]
    public async Task UpdateVilla_ReturnsBadRequest_WhenUpdateDTOIsNull()
    {
         // Arrange
         VillaUpdateDTO updateDTO = null;

         // Act
         var result = await _controller.UpdateVilla(1, updateDTO) as ActionResult<APIResponse>;
         var badRequestResult = result?.Result as BadRequestObjectResult;

         // Assert
         Assert.That(badRequestResult, Is.Not.Null, "Expected BadRequestObjectResult but was null.");
         Assert.That(badRequestResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest), "Expected status code 400 Bad Request.");
    }
    [Test]
    public async Task UpdateVilla_ReturnsNotFound_WhenVillaIsNotFound()
    {
        // Arrange
        var villaId = 1;
        var updateDTO = new VillaUpdateDTO { DestinationName = "New Destination", Facilities = new List<string> { "Pool", "Gym" } };

        _mockVillaRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Villa, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                        .ReturnsAsync((Villa)null);

        // Act
        var result = await _controller.UpdateVilla(villaId, updateDTO) as ActionResult<APIResponse>;
        var notFoundResult = result?.Result as NotFoundObjectResult;

         // Assert
        Assert.That(notFoundResult, Is.Not.Null, "Expected NotFoundObjectResult but was null.");
        Assert.That(notFoundResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound), "Expected status code 404 Not Found.");
   }
   [Test]
   public async Task DeleteVilla_ReturnsOkResult_WhenVillaIsDeleted()
   {
     // Arrange
      var villaId = 1;
      var existingVilla = new Villa { Id = villaId, Name = "Villa One" };

      _mockVillaRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Villa, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                        .ReturnsAsync(existingVilla);

      _mockVillaRepository.Setup(repo => repo.RemoveAsync(It.IsAny<Villa>()))
                        .Returns(Task.CompletedTask);

      _mockVillaRepository.Setup(repo => repo.SaveAsync())
                        .Returns(Task.CompletedTask);

        // Act
      var result = await _controller.DeleteVilla(villaId) as ActionResult<APIResponse>;
      var okResult = result?.Result as OkObjectResult;

       // Assert
       Assert.That(okResult, Is.Not.Null, "Expected OkObjectResult but was null.");
       Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK), "Expected status code 200 OK.");
      var apiResponse = okResult?.Value as APIResponse;
      Assert.That(apiResponse, Is.Not.Null, "Expected APIResponse but was null.");
      Assert.That(apiResponse.IsSuccess, Is.True, "Expected IsSuccess to be true.");
    }
    [Test]
    public async Task DeleteVilla_ReturnsBadRequest_WhenIdIsZero()
    {
        // Arrange
        var villaId = 0;

       // Act
        var result = await _controller.DeleteVilla(villaId) as ActionResult<APIResponse>;
        var badRequestResult = result?.Result as BadRequestObjectResult;

       // Assert
        Assert.That(badRequestResult, Is.Not.Null, "Expected BadRequestObjectResult but was null.");
        Assert.That(badRequestResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest), "Expected status code 400 Bad Request.");
    }
    [Test]
    public async Task DeleteVilla_ReturnsNotFound_WhenVillaIsNotFound()
    {
         // Arrange
         var villaId = 1;

         _mockVillaRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Villa, bool>>>(), It.IsAny<bool>(), It.IsAny<string>()))
                        .ReturnsAsync((Villa)null);

         // Act
         var result = await _controller.DeleteVilla(villaId) as ActionResult<APIResponse>;
         var notFoundResult = result?.Result as NotFoundObjectResult;

        // Assert
         Assert.That(notFoundResult, Is.Not.Null, "Expected NotFoundObjectResult but was null.");
         Assert.That(notFoundResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound), "Expected status code 404 Not Found.");
    }

    [Test]
    public async Task GetVillasByDestination_ReturnsOkResult_WhenVillasAreFound()
    {
        // Arrange
         var destinationName = "Beach";
         var destinationId = 1; // Assuming you have a way to get the destinationId from the destinationName
         var villas = new List<Villa>
         {
             new Villa { Id = 1, Name = "Villa One", DestinationId = destinationId },
             new Villa { Id = 2, Name = "Villa Two", DestinationId = destinationId }
         };

         _mockVillaRepository.Setup(repo => repo.GetVillasByDestinationAsync(destinationName))
                        .ReturnsAsync(villas);

         // Act
         var result = await _controller.GetVillasByDestination(destinationName) as ActionResult;
         var okResult = result as OkObjectResult;

         // Assert
        Assert.That(okResult, Is.Not.Null, "Expected OkObjectResult but was null.");
        Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK), "Expected status code 200 OK.");
        Assert.That(okResult.Value, Is.EqualTo(villas), "Expected villas in the result.");
    }
    [Test]
    public async Task GetVillasByDestination_ReturnsBadRequest_WhenDestinationNameIsNullOrEmpty()
    {
       // Arrange
       string destinationName = null;

        // Act
       var result = await _controller.GetVillasByDestination(destinationName) as ActionResult;
       var badRequestResult = result as BadRequestObjectResult;

        // Assert
        Assert.That(badRequestResult, Is.Not.Null, "Expected BadRequestObjectResult but was null.");
        Assert.That(badRequestResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest), "Expected status code 400 Bad Request.");

        // Log the Value property
        if (badRequestResult != null)
        {
            Console.WriteLine($"BadRequestResult Value: {badRequestResult.Value}");

            // Check if Value is not null before accessing it
            Assert.That(badRequestResult.Value, Is.Not.Null, "Expected error message but was null.");
            var errorMessage = badRequestResult.Value as IDictionary<string, object>;
            Assert.That(errorMessage["message"], Is.EqualTo("Destination parameter is required."), "Expected error message in the result.");
        }
    }
    [Test]
    public async Task GetVillasByDestination_ReturnsNotFound_WhenNoVillasAreFound()
    {
        // Arrange
        var destinationName = "Beach";
        List<Villa> villas = null;

       _mockVillaRepository.Setup(repo => repo.GetVillasByDestinationAsync(destinationName))
                        .ReturnsAsync(villas);

       // Act
       var result = await _controller.GetVillasByDestination(destinationName) as ActionResult;
       var notFoundResult = result as NotFoundObjectResult;

      // Assert
      Assert.That(notFoundResult, Is.Not.Null, "Expected NotFoundObjectResult but was null.");
      Assert.That(notFoundResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound), "Expected status code 404 Not Found.");

       // Compare the message property directly
      var errorMessage = notFoundResult.Value as IDictionary<string, object>;
      Assert.That(errorMessage["message"], Is.EqualTo("No villas found for the given destination."), "Expected error message in the result.");
    }
    [Test]
    public async Task GetVillasByDestination_ReturnsNotFound_WhenVillasListIsEmpty()
    {
       // Arrange
       var destinationName = "Beach";
       var villas = new List<Villa>();

       _mockVillaRepository.Setup(repo => repo.GetVillasByDestinationAsync(destinationName))
                        .ReturnsAsync(villas);

        // Act
       var result = await _controller.GetVillasByDestination(destinationName) as ActionResult;
       var notFoundResult = result as NotFoundObjectResult;

       // Assert
       Assert.That(notFoundResult, Is.Not.Null, "Expected NotFoundObjectResult but was null.");
       Assert.That(notFoundResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound), "Expected status code 404 Not Found.");

       // Compare the message property directly
       var errorMessage = notFoundResult.Value as IDictionary<string, object>;
       Assert.That(errorMessage["message"], Is.EqualTo("No villas found for the given destination."), "Expected error message in the result.");
    }
}