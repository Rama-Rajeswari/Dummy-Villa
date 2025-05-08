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
    [Route("api/v{version:apiVersion}/DestinationAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class DestinationAPIController:ControllerBase
    {
        private readonly IDestinationRepository _destinationRepository;
        private readonly IMapper _mapper;
        private APIResponse _response;

        public DestinationAPIController(IDestinationRepository destinationRepository, IMapper mapper)
        {
            _destinationRepository = destinationRepository;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetDestinations()
        {
            try
            {
                var destinationList = await _destinationRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<DestinationDTO>>(destinationList);
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

        [HttpGet("{id:int}", Name = "GetDestination")]
        public async Task<ActionResult<APIResponse>> GetDestination(int id)
        {
            if (id <= 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var destination = await _destinationRepository.GetAsync(u => u.Id == id);
            if (destination == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            _response.Result = _mapper.Map<DestinationDTO>(destination);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> CreateDestination([FromBody] DestinationCreateDTO createDTO)
        {
            if (createDTO == null)
            {
                return BadRequest("Invalid destination data.");
            }

            var destinationModel = _mapper.Map<Destination>(createDTO);
            await _destinationRepository.CreateAsync(destinationModel);

            _response.Result = _mapper.Map<DestinationDTO>(destinationModel);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetDestination", new { id = destinationModel.Id }, _response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        
        public async Task<ActionResult<APIResponse>> UpdateDestination(int id, [FromBody] DestinationUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest("Invalid request data.");
            }

            var existingDestination = await _destinationRepository.GetAsync(u => u.Id == id);
            if (existingDestination == null)
            {
                return NotFound("Destination not found.");
            }

            _mapper.Map(updateDTO, existingDestination);
            await _destinationRepository.UpdateAsync(existingDestination);

            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> DeleteDestination(int id)
        {
            var destination = await _destinationRepository.GetAsync(u => u.Id == id);
            if (destination == null)
            {
                return NotFound("Destination not found.");
            }

            await _destinationRepository.RemoveAsync(destination);
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
    }
}
    