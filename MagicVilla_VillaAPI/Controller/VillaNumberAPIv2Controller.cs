using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controller
{
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiController]  
    
    [ApiVersion("2.0")]
    public class VillaNumberAPIv2Controller:ControllerBase
    {
         
        protected APIResponse _response;
        private IVillaNumberRepository _villaNumberRepository;
        private readonly IVillaRepository _villarepository;
        private readonly IMapper _mapper;
        
        public VillaNumberAPIv2Controller(IVillaNumberRepository villaNumberRepository,IMapper mapper,APIResponse response,IVillaRepository villaRepository)
        {
         _villaNumberRepository=villaNumberRepository;
         _villarepository=villaRepository;
         _mapper=mapper;
        this._response=new();
           
        }
       
        //[MapToApiVersion("2.0")]
        [HttpGet]
       public IEnumerable<string>Get()
       {
        return new string[] {"value1","value2"};
       }
        


       
    }
}
   