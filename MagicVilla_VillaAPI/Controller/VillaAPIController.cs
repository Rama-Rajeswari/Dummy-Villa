using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace MagicVilla_VillaAPI.Controller
{
    
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiController]  
     [ApiVersion("1.0")]
    public class VillaAPIController:ControllerBase
    { 
        protected APIResponse _response;
        private IVillaRepository _villaRepository;
        private IDestinationRepository _destinationRepository;
        private IRoomRepository _roomRepository;
        private IFacilityRepository _facilityRepository;
        private IVillaFacilityRepository _villaFacilityRepository;
        private IGuestTypeRepository _guestTypeRepository;
        private IRoomGuestTypeRepository _roomGuestTypeRepository;
        private readonly IMapper _mapper;
        

        public VillaAPIController(IVillaRepository villaRepository,IMapper mapper,APIResponse response,IRoomRepository roomRepository,IFacilityRepository facilityRepository,IDestinationRepository destinationRepository,IVillaFacilityRepository villaFacilityRepository,IGuestTypeRepository guestTypeRepository,IRoomGuestTypeRepository roomGuestTypeRepository)
        {
         _villaRepository=villaRepository;
         _roomRepository=roomRepository; 
         _facilityRepository=facilityRepository;
         _destinationRepository=destinationRepository;
         _villaFacilityRepository=villaFacilityRepository;
         _guestTypeRepository=guestTypeRepository;
         _roomGuestTypeRepository=roomGuestTypeRepository;
        
         _mapper=mapper;
        this._response=new();   
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name ="filterOccupancy")]int? occupancy,[FromQuery]string? search,int pageSize=0,int pageNumber=1)
        {
            try{
           IEnumerable<Villa> villaList;
           if(occupancy>0)
           {
            villaList=await _villaRepository.GetAllAsync(u=>u.Occupancy==occupancy,pageSize:pageSize,pageNumber:pageNumber,includeProperties: "Rooms,Rooms.RoomGuestTypes,VillaFacilities,VillaFacilities.Facility,Destination");
           }
           else{
            villaList=await _villaRepository.GetAllAsync(pageSize:pageSize,pageNumber:pageNumber,includeProperties: "Rooms,Rooms.RoomGuestTypes,VillaFacilities,VillaFacilities.Facility,Destination"); 
           }
           if(!string.IsNullOrEmpty(search))
           {
            villaList=villaList.Where(u=>u.Name.ToLower().Contains(search.ToLower()));
           }
           Console.WriteLine($"Fetched Villas Count: {villaList.Count()}"); // Debugging
        foreach (var villa in villaList)
        {
            Console.WriteLine($"Villa: {villa.Name}, Rooms: {villa.Rooms?.Count ?? 0}, Facilities: {villa.VillaFacilities?.Count ?? 0}");
        }
           Pagination pagination=new(){PageNumber=pageNumber,PageSize=pageSize};
           Response.Headers.Add("X-Pagination",JsonSerializer.Serialize(pagination));

          _response.Result=_mapper.Map<List<VillaDTO>>(villaList);
          _response.StatusCode=HttpStatusCode.OK;
            return Ok(_response);

            }
            catch(Exception ex)
            {
                _response.IsSuccess=false;
                _response.ErrorMessages=new List<string>(){ex.ToString()};
               return StatusCode(500, _response); // Ensure a proper status code is returned in case of an error
            }
            return _response;
        }
        
        [HttpGet("{id:int}",Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(200)]
        // [ProducesResponseType(404)]
        // [ProducesResponseType(400)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            if(id==0)
            {
               _response.StatusCode=HttpStatusCode.BadRequest;
               return BadRequest("Villa Id Can not be 0");
                
            }
            var villa=await _villaRepository.GetAsync(u=>u.Id==id, includeProperties: "Rooms,Rooms.RoomGuestTypes,VillaFacilities,VillaFacilities.Facility,Destination");
            Console.WriteLine($"Fetching villas with includeProperties: Rooms,Rooms.RoomGuestTypes,VillaFacilities,VillaFacilities.Facility,Destination");
            
            if(villa==null)
            {
                _response.StatusCode=HttpStatusCode.NotFound;
                return NotFound("Villa Not Found");
            }
             var villaDto = _mapper.Map<VillaDTO>(villa);
             _response.Result=villaDto;
             _response.StatusCode=HttpStatusCode.OK;
              return Ok(_response);
        }
        [HttpPost]
        [Authorize(Roles ="admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>>CreateVilla([FromBody]VillaCreateDTO createDTO)
        {
            if(await _villaRepository.GetAsync(u=>u.Name.ToLower()==createDTO.Name.ToLower())!=null)
            {
                ModelState.AddModelError("ErrorMessages","Villa already exits!!");
                return BadRequest(ModelState);
            }
            if(createDTO==null)
            {
                return BadRequest(createDTO);
            }
            if(string.IsNullOrWhiteSpace(createDTO.DestinationName))
            {
                return BadRequest("destination name is required.");
            }
           var destination = await _destinationRepository.GetAsync(u => u.Name.ToLower().Trim() == createDTO.DestinationName.ToLower().Trim());
           if (destination == null)
           {
            destination = new Destination
            {
                Name = createDTO.DestinationName.Trim(),
                ImageUrl = ""
            };
             await _destinationRepository.CreateAsync(destination);
            await _destinationRepository.SaveAsync(); 
           }
               Console.WriteLine($"Rooms Count: {createDTO.Rooms?.Count ?? 0}");
               Console.WriteLine($"Facilities Count: {createDTO.Facilities?.Count ?? 0}");

                 Villa model=_mapper.Map<Villa>(createDTO);
                 model.DestinationId=destination.Id;

                 await _villaRepository.CreateAsync(model);
                 await _villaRepository.SaveAsync();
                 int villaId=model.Id;

                 if (createDTO.Rooms != null && createDTO.Rooms.Count > 0)
                 {
                            List<Room> createdRooms = new List<Room>();

                         foreach (var roomDto in createDTO.Rooms)
                        {
        
                            var existingRoom = await _roomRepository.GetAsync(r => r.VillaId == villaId && r.RoomType == roomDto.RoomType);
        
                            if (existingRoom == null) 
                             {
                                var room = _mapper.Map<Room>(roomDto);
                                room.VillaId = villaId;
                                createdRooms.Add(room);
                            }
                         }
    
                         foreach (var room in createdRooms)
                         {
                             await _roomRepository.CreateAsync(room);
                             await _roomRepository.SaveAsync();
                         }

   
                         foreach (var room in createdRooms)
                         {
                            var roomDto = createDTO.Rooms.FirstOrDefault(r => r.RoomType == room.RoomType);
                            if (roomDto != null && roomDto.GuestTypeIds != null)
                            {
                                 foreach (var guestTypeId in roomDto.GuestTypeIds)
                                 {
                                      var guestType = await _guestTypeRepository.GetAsync(g => g.Id == guestTypeId);
                                      if (guestType != null)
                                      {
                                        await _roomGuestTypeRepository.CreateAsync(new RoomGuestType
                                        {
                                             RoomId = room.Id,
                                             GuestTypeId = guestTypeId
                                         });
                                        await _roomGuestTypeRepository.SaveAsync();
                                      }
                                 }
                             }
                        }
                    }

                    if ( createDTO.Facilities.Count>0)
                    {
                        foreach (var facilityName in createDTO.Facilities)
                        {
                              var existingFacility = await _facilityRepository.GetAsync(f => f.Name.ToLower() == facilityName.ToLower());
                              if (existingFacility == null)
                               {
                                  existingFacility = new Facility { Name = facilityName };
                                  await _facilityRepository.CreateAsync(existingFacility);
                                  await _facilityRepository.SaveAsync();                            
                               }
                               var existingVillaFacility = await _villaFacilityRepository.GetAsync(vf => vf.VillaId == model.Id && vf.FacilityId == existingFacility.Id);
                               if (existingVillaFacility == null)
                               {
                                  await _villaFacilityRepository.CreateAsync(new VillaFacility
                                  {
                                      VillaId = model.Id,
                                      FacilityId = existingFacility.Id
                                  });
                                  await _villaFacilityRepository.SaveAsync();
                                }    
                        }
                    }
          var result = await _villaRepository.GetAsync(
          u => u.Id == villaId,
          includeProperties: "Rooms.RoomGuestTypes.GuestType,VillaFacilities.Facility");
         _response.Result=_mapper.Map<VillaDTO>(result);
         _response.StatusCode=HttpStatusCode.Created;
         return CreatedAtRoute("GetVilla",new {id=model.Id},_response);
            
        }
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        [Authorize(Roles ="admin")]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
           
            if(id==0)
            {
                _response.StatusCode=HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            var villa=await _villaRepository.GetAsync(u=>u.Id==id);
            if(villa==null)
            {
                _response.StatusCode=HttpStatusCode.NotFound;
                return NotFound(_response);
            }
           await _villaRepository.RemoveAsync(villa);
           _response.StatusCode=HttpStatusCode.NoContent;
           _response.IsSuccess=true;
            return Ok(_response);
             
        }
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles ="admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id,[FromBody]VillaUpdateDTO updateDTO)
        {
            
            if(updateDTO==null )
            {
                _response.StatusCode=HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            var existingVilla = await _villaRepository.GetAsync(u => u.Id == id);
            if (existingVilla == null)
            {
              _response.StatusCode = HttpStatusCode.NotFound;
              return NotFound(_response);
            }

            var existingLocation = await _destinationRepository.GetAsync(u => u.Name.ToLower().Trim() == updateDTO.DestinationName.ToLower().Trim());
            if (existingLocation == null)
            {
                existingLocation = new Destination
                {
                     Name = updateDTO.DestinationName.Trim(),
                     ImageUrl = ""
                };
                await _destinationRepository.CreateAsync(existingLocation);
                await _destinationRepository.SaveAsync(); // Ensure ID is available
            }

           
            existingVilla.DestinationId = existingLocation.Id;

            _mapper.Map(updateDTO, existingVilla);
            var updatedVillaFacilities = new List<VillaFacility>();
            foreach (var facilityName in updateDTO.Facilities)
            {
                var existingFacility = await _facilityRepository.GetAsync(f => f.Name.ToLower() == facilityName.ToLower());
                if (existingFacility == null)
                {
                   existingFacility = new Facility { Name = facilityName };
                   await _facilityRepository.CreateAsync(existingFacility);
                   await _facilityRepository.SaveAsync();
                }
                 updatedVillaFacilities.Add(new VillaFacility
                 {
                    VillaId = existingVilla.Id,
                    FacilityId = existingFacility.Id
                 });
             }
            existingVilla.VillaFacilities = updatedVillaFacilities;
            await _villaRepository.UpdateAsync(existingVilla);
            await _villaRepository.SaveAsync();
           _response.StatusCode=HttpStatusCode.NoContent;
           _response.IsSuccess=true;
            return Ok(_response);
            
        }
        [HttpGet("GetVillasByDestination/{destinationName}")]
        public async Task<IActionResult> GetVillasByDestination(string destinationName)
        {
             if (string.IsNullOrEmpty(destinationName))
             {
                 return BadRequest(new { message = "Destination parameter is required." });
             }

             var villas = await _villaRepository.GetVillasByDestinationAsync(destinationName);

             if (villas == null || !villas.Any())
              {
                 return NotFound(new { message = "No villas found for the given destination." });
              }

         return Ok(villas );
    
        }
        [HttpGet("GetAllVillasWithDetails")]
        public async Task<IActionResult> GetAllVillasWithDetails()
        {
           var villas = await _villaRepository.GetAllVillasWithDetailsAsync();
    
            if (villas == null || villas.Count == 0)
            {
             return NotFound(new { message = "No villas found." });
            }

            return Ok(villas);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchVillas(string? query)
        {
            var result = await _villaRepository.SearchVillasByQueryAsync(query);
            return Ok(result);
        }
    }
}