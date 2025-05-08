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
    [Route("api/v{version:apiVersion}/RoomPricingAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class RoomPricingAPIController:ControllerBase
    {
        private readonly IRoomPricingRepository _roomPricingRepository;
        private readonly IMapper _mapper;
        private APIResponse _response;

        public RoomPricingAPIController(IRoomPricingRepository roomPricingRepository, IMapper mapper)
        {
            _roomPricingRepository = roomPricingRepository;
            _mapper = mapper;
            _response = new APIResponse();
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetRoomPricings()
        {
            try
            {
                var pricingList = await _roomPricingRepository.GetAllAsync(includeProperties: "Room");
                var pricingDTOList = _mapper.Map<List<RoomPricingDTO>>(pricingList);

                _response.Result = pricingDTOList;
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
        [HttpGet("{id:int}", Name = "GetRoomPricing")]
        public async Task<ActionResult<APIResponse>> GetRoomPricing(int id)
        {
            if (id <= 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var roomPricing = await _roomPricingRepository.GetAsync(u => u.Id == id);
            if (roomPricing == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            _response.Result = _mapper.Map<RoomPricingDTO>(roomPricing);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }      
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> CreateRoomPricing([FromBody] RoomPricingCreateDTO createDTO)
        {
            if (createDTO == null)
            {
                return BadRequest("Invalid pricing data.");
            }
            try
            {
                var pricingModel = _mapper.Map<RoomPricing>(createDTO);
                await _roomPricingRepository.CreateAsync(pricingModel);
                 return Ok(new
                 {
                      message = "Room pricing created successfully",
                      data = _mapper.Map<RoomPricingDTO>(pricingModel)
                 });

            }
            catch (Exception ex)
            {
                 return StatusCode(500, new
                 {
                        message = "An error occurred while saving the entity changes.",
                        error = ex.InnerException?.Message ?? ex.Message
                  });

             }
        }       
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> UpdateRoomPricing(int id, [FromBody] RoomPricingUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest("Invalid request data.");
            }

            var existingPricing = await _roomPricingRepository.GetAsync(u => u.Id == id);
            if (existingPricing == null)
            {
                return NotFound("Room pricing not found.");
            }

            _mapper.Map(updateDTO, existingPricing);
            await _roomPricingRepository.UpdateAsync(existingPricing);

            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }        
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> DeleteRoomPricing(int id)
        {
            var pricing = await _roomPricingRepository.GetAsync(u => u.Id == id);
            if (pricing == null)
            {
                return NotFound("Room pricing not found.");
            }

            await _roomPricingRepository.RemoveAsync(pricing);
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
    }
}