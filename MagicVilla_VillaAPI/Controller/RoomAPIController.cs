using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controller
{
    [Route("api/v{version:apiVersion}/RoomAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class RoomAPIController:ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;
        private APIResponse _response;
        

        public RoomAPIController(IRoomRepository roomRepository, IMapper mapper,IVillaRepository villaRepository)
        {
            _roomRepository = roomRepository;
            _villaRepository=villaRepository;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetRooms()
        {
            try
            {
                 var roomList = await _roomRepository.GetAllAsync(includeProperties: "Villa,RoomGuestTypes.GuestType");
                 var roomDTOList = _mapper.Map<List<RoomDTO>>(roomList);
                  _response.Result = roomDTOList;
                  _response.StatusCode = HttpStatusCode.OK;
                   return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(500, _response);
            }
        }
    
        [HttpGet("{id:int}", Name = "GetRoom")]
        public async Task<ActionResult<APIResponse>> GetRoom(int id)
        {
            if (id <= 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var room = await _roomRepository.GetAsync(u => u.Id == id);
            if (room == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            

            _response.Result = _mapper.Map<RoomDTO>(room);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> CreateRoom([FromBody] RoomCreateDTO createDTO)
        {
            if (createDTO == null)
            {
                return BadRequest("Invalid room data.");
            }
            var roomModel = _mapper.Map<Room>(createDTO); 
            if (createDTO.GuestTypeIds != null && createDTO.GuestTypeIds.Any())
            {
                roomModel.RoomGuestTypes = createDTO.GuestTypeIds.Select(id => new RoomGuestType { GuestTypeId = id, Room = roomModel }).ToList();
            }
            await _roomRepository.CreateAsync(roomModel);
             _response.Result = new
             {
                roomModel.Id,
                roomModel.RoomType,
                roomModel.VillaId,
                GuestTypeIds = roomModel.RoomGuestTypes.Select(gt => gt.GuestTypeId).ToList()
             };
              _response.StatusCode = HttpStatusCode.Created;
              return CreatedAtRoute("GetRoom", new { id = roomModel.Id }, _response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> UpdateRoom(int id, [FromBody] RoomUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
               return BadRequest("Invalid request data.");
            }
            var existingRoom = await _roomRepository.GetAsync(
            u => u.Id == id, 
            includeProperties: "RoomGuestTypes"
             );

            if (existingRoom == null)
            {
               return NotFound("Room not found.");
            }
            var villaExists = await _villaRepository.GetAsync(v => v.Id == updateDTO.VillaId);
            if (villaExists == null)
            {
                return BadRequest("The specified VillaId does not exist.");
            }
            _mapper.Map(updateDTO, existingRoom);
            existingRoom.VillaId = updateDTO.VillaId;
            if (updateDTO.GuestTypeIds != null)
            {
                existingRoom.RoomGuestTypes.Clear();
                foreach (var guestTypeId in updateDTO.GuestTypeIds)
                {
                    existingRoom.RoomGuestTypes.Add(new RoomGuestType
                    {
                        RoomId = existingRoom.Id,
                        GuestTypeId = guestTypeId
                    });
                }
            }
             await _roomRepository.UpdateAsync(existingRoom);
            _response.Result = new
            {
               existingRoom.Id,
               existingRoom.RoomType,
               existingRoom.VillaId,
               GuestTypeIds = existingRoom.RoomGuestTypes.Select(gt => gt.GuestTypeId).ToList()
            };
             _response.StatusCode = HttpStatusCode.OK;
             return Ok(_response);
            
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> DeleteRoom(int id)
        {
            var room = await _roomRepository.GetAsync(u => u.Id == id);
            if (room == null)
            {
                return NotFound("Room not found.");
            }

            await _roomRepository.RemoveAsync(room);
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
        [HttpGet("GetRoomsByVilla/{villaId}")]
        public async Task<ActionResult<VillaDTO>> GetRoomsByVilla(int villaId)
        {
           var villa = await _roomRepository.GetRoomsByVilla(villaId);
           if (villa == null)
            return NotFound(new { message = "Villa not found or has no rooms" });

            return Ok(villa);
        }
    }
}