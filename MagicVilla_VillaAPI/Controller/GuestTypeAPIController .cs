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
    [Route("api/v{version:apiVersion}/GuestTypeAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class GuestTypeAPIController :ControllerBase
    {
         private readonly IGuestTypeRepository _guestTypeRepository;
        private readonly IMapper _mapper;
        private APIResponse _response;

        public GuestTypeAPIController(IGuestTypeRepository guestTypeRepository, IMapper mapper)
        {
            _guestTypeRepository = guestTypeRepository;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetGuestTypes()
        {
            try
            {
                var guestTypeList = await _guestTypeRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<GuestTypeDTO>>(guestTypeList);
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

        [HttpGet("{id:int}", Name = "GetGuestType")]
        public async Task<ActionResult<APIResponse>> GetGuestType(int id)
        {
            if (id <= 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var guestType = await _guestTypeRepository.GetAsync(u => u.Id == id);
            if (guestType == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            _response.Result = _mapper.Map<GuestTypeDTO>(guestType);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> CreateGuestType([FromBody] GuestTypeCreateDTO createDTO)
        {
            if (createDTO == null)
            {
                return BadRequest("Invalid guest type data.");
            }

            var guestTypeModel = _mapper.Map<GuestType>(createDTO);
            await _guestTypeRepository.CreateAsync(guestTypeModel);

            _response.Result = _mapper.Map<GuestTypeDTO>(guestTypeModel);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetGuestType", new { id = guestTypeModel.Id }, _response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> UpdateGuestType(int id, [FromBody] GuestTypeUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest("Invalid request data.");
            }

            var existingGuestType = await _guestTypeRepository.GetAsync(u => u.Id == id);
            if (existingGuestType == null)
            {
                return NotFound("Guest type not found.");
            }

            _mapper.Map(updateDTO, existingGuestType);
            await _guestTypeRepository.UpdateAsync(existingGuestType);

            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> DeleteGuestType(int id)
        {
            var guestType = await _guestTypeRepository.GetAsync(u => u.Id == id);
            if (guestType == null)
            {
                return NotFound("Guest type not found.");
            }

            await _guestTypeRepository.RemoveAsync(guestType);
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
    }
}