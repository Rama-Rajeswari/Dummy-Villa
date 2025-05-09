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
    [Route("api/v{version:apiVersion}/FacilityAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class FacilityAPIController:ControllerBase
    {
        private readonly IFacilityRepository _facilityRepository;
        private readonly IMapper _mapper;
        private APIResponse _response;

        public FacilityAPIController(IFacilityRepository facilityRepository, IMapper mapper)
        {
            _facilityRepository = facilityRepository;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetFacilities()
        {
            try
            {
                var facilityList = await _facilityRepository.GetAllAsync();
                _response.Result = _mapper.Map<List<FacilityDTO>>(facilityList);
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

        [HttpGet("{id:int}", Name = "GetFacility")]
        public async Task<ActionResult<APIResponse>> GetFacility(int id)
        {
            if (id <= 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var facility = await _facilityRepository.GetAsync(u => u.Id == id);
            if (facility == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            _response.Result = _mapper.Map<FacilityDTO>(facility);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> CreateFacility([FromBody] FacilityCreateDTO createDTO)
        {
            if (createDTO == null)
            {
                return BadRequest("Invalid facility data.");
            }

            var facilityModel = _mapper.Map<Facility>(createDTO);
            await _facilityRepository.CreateAsync(facilityModel);

            _response.Result = _mapper.Map<FacilityDTO>(facilityModel);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetFacility", new { id = facilityModel.Id }, _response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> UpdateFacility(int id, [FromBody] FacilityUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest("Invalid request data.");
            }

            var existingFacility = await _facilityRepository.GetAsync(u => u.Id == id);
            if (existingFacility == null)
            {
                return NotFound("Facility not found.");
            }

            _mapper.Map(updateDTO, existingFacility);
            await _facilityRepository.UpdateAsync(existingFacility);

            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> DeleteFacility(int id)
        {
            var facility = await _facilityRepository.GetAsync(u => u.Id == id);
            if (facility == null)
            {
                return NotFound("Facility not found.");
            }

            await _facilityRepository.RemoveAsync(facility);
            _response.StatusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
        
    }
}