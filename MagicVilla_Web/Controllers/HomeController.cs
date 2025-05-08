using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services;
using AutoMapper;
using MagicVilla_Web.Models.Dto;
using Newtonsoft.Json;
using MagicVilla_Utility;

namespace MagicVilla_Web.Controllers;

public class HomeController : Controller
{
    private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public HomeController(IVillaService villaService,IMapper mapper)
        {
           _villaService=villaService;
           _mapper=mapper;
        }
       

        public async Task<IActionResult> Index()
        {
            List<VillaDTO>list=new();
            var response=await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                string jsonData = JsonConvert.SerializeObject(response.Result);
        list = JsonConvert.DeserializeObject<List<VillaDTO>>(jsonData);
                
            }
            return View(list);
        }
        [HttpGet]
        public IActionResult SearchVilla()
        {
            return View(); 
        }
    
        public IActionResult Privacy()
        {
           return View();
        }

    
}
