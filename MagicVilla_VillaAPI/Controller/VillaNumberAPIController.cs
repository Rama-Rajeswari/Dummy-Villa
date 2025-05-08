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
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiController]  
    [ApiVersion("1.0")]
    
    public class VillaNumberAPIController:ControllerBase
    {
         
        protected APIResponse _response;
        private IVillaNumberRepository _villaNumberRepository;
        private readonly IVillaRepository _villarepository;
        private readonly IMapper _mapper;

        public VillaNumberAPIController(IVillaNumberRepository villaNumberRepository,IMapper mapper,APIResponse response,IVillaRepository villaRepository)
        {
         _villaNumberRepository=villaNumberRepository;
         _villarepository=villaRepository;
         _mapper=mapper;
        this._response=new();
           
        }
        [HttpGet]
        //[MapToApiVersion("1.0")]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
               IEnumerable<VillaNumber> villaNumberList=await _villaNumberRepository.GetAllAsync(includeProperties:"Villa");
             // var villaDTOList = _mapper.Map<IEnumerable<VillaDTO>>(villaList);
               _response.Result=_mapper.Map<List<VillaNumberDTO>>(villaNumberList);
               _response.StatusCode=HttpStatusCode.OK;
               return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess=false;
                _response.ErrorMessages=new List<string>(){ex.ToString()};
            }
            return _response;
        }

        [HttpGet("{id:int}",Name ="GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [ProducesResponseType(200)]
        // [ProducesResponseType(404)]
        // [ProducesResponseType(400)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            //try{

            if(id==0)
            {
               _response.StatusCode=HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            var villaNumber=await _villaNumberRepository.GetAsync(u=>u.VillaNo==id);
            if(villaNumber==null)
            {
                _response.StatusCode=HttpStatusCode.NotFound;
                return NotFound(_response);
            }
             _response.Result=_mapper.Map<VillaNumberDTO>(villaNumber);
             _response.StatusCode=HttpStatusCode.OK;
              return Ok(_response);
            //    }
            // catch(Exception ex)
            // {
            //     _response.IsSuccess=false;
            //     _response.ErrorMessages=new List<string>(){ex.ToString()};
            // }
            return _response;
            
        }
        [HttpPost]
        [Authorize(Roles ="admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>>CreateVillaNumber([FromBody]VillaNumberCreateDTO createDTO)
        {
            // if(!ModelState.IsValid)
            // {
            //     return BadRequest(ModelState);
            // }
            //try{
            if(await _villaNumberRepository.GetAsync(u=>u.VillaNo==createDTO.VillaNo)!=null)
            {
                ModelState.AddModelError("ErrorMessages","VillaNumber already exits!!");
                return BadRequest(ModelState);
            }
            if(await _villarepository.GetAsync(u=>u.Id==createDTO.VillaId)==null)
            {
                ModelState.AddModelError("ErrorMessages","Villa Id is Invalid!!");
                return BadRequest(ModelState);
            }
            if(createDTO==null)
            {
                return BadRequest(createDTO);
            }
            // if(villaDTO.Id>0)
            // {
            //     return StatusCode(StatusCodes.Status500InternalServerError);
            // }
           VillaNumber villaNumber=_mapper.Map<VillaNumber>(createDTO);
            // Villa model=new()
            // {
            //     Amenity=createDTO.Amenity,
            //     Details=createDTO.Details,              
            //     ImageUrl=createDTO.ImageUrl,
            //     Name=createDTO.Name,
            //     Occupancy=createDTO.Occupancy,
            //     Rate=createDTO.Rate,
            //     Sqft=createDTO.Sqft
            // };
           await _villaNumberRepository.CreateAsync(villaNumber);
           _response.Result=_mapper.Map<VillaNumberDTO>(villaNumber);
             _response.StatusCode=HttpStatusCode.Created;
              return Ok(_response);
           return CreatedAtRoute("GetVilla",new {id=villaNumber.VillaNo},_response);
            // }
            // catch(Exception ex)
            // {
            //     _response.IsSuccess=false;
            //     _response.ErrorMessages=new List<string>(){ex.ToString()};
            // }
            return _response;
        }
        [Authorize(Roles ="admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}",Name ="DeleteVillaNumber")]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
           // try{
            if(id==0)
            {
                _response.StatusCode=HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            
            var villaNumber=await _villaNumberRepository.GetAsync(u=>u.VillaNo==id);
            if(villaNumber==null)
            {
                _response.StatusCode=HttpStatusCode.NotFound;
                return NotFound(_response);
            }
           await _villaNumberRepository.RemoveAsync(villaNumber);
           _response.StatusCode=HttpStatusCode.NoContent;
           _response.IsSuccess=true;
            return Ok(_response);
            //  }
            // catch(Exception ex)
            // {
            //     _response.IsSuccess=false;
            //     _response.ErrorMessages=new List<string>(){ex.ToString()};
            // }
            return _response;
        }
        [Authorize(Roles ="admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id,[FromBody]VillaNumberUpdateDTO updateDTO)
        {
            //try{
            if(updateDTO==null || id!=updateDTO.VillaNo)
            {
                _response.StatusCode=HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            if(await _villarepository.GetAsync(u=>u.Id==updateDTO.VillaId)==null)
            {
                ModelState.AddModelError("ErrorMessages","Villa Id is Invalid!!");
                return BadRequest(ModelState);
            }
            // var villa=_context.Villas.FirstOrDefault(u=>u.Id==id);
            // villa.Name=villaDTO.Name;
            // villa.Sqft=villaDTO.Sqft;
            // villa.Occupancy=villaDTO.Occupancy;
           VillaNumber villaNumber=_mapper.Map<VillaNumber>(updateDTO);
            // Villa model=new()
            // {
            //    Amenity=updateDTO.Amenity,
            //     Details=updateDTO.Details,
            //     Id=updateDTO.Id,
            //     ImageUrl=updateDTO.ImageUrl,
            //     Name=updateDTO.Name,
            //     Occupancy=updateDTO.Occupancy,
            //     Rate=updateDTO.Rate,
            //     Sqft=updateDTO.Sqft 
            // };
           await _villaNumberRepository.UpdateAsync(villaNumber);
           _response.StatusCode=HttpStatusCode.NoContent;
           _response.IsSuccess=true;
            return Ok(_response);
            //  }
            // catch(Exception ex)
            // {
            //     _response.IsSuccess=false;
            //     _response.ErrorMessages=new List<string>(){ex.ToString()};
            // }
            return _response;
        }


        
          // This is a test comment to trigger the CI/CD pipeline
           public string TestMethod()
          {
               return "Hello, CI/CD!";
          }

 
    }
}