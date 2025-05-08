using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using X.PagedList;
using X.PagedList.Extensions;

namespace MagicVilla_Web.Controllers
{
    [Route("[controller]")]
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        
        public VillaController(IVillaService villaService,IMapper mapper,IRoomService roomService)
        {
           _villaService=villaService;
           _mapper=mapper;
           _roomService=roomService;
          
        }
        [HttpGet]
        [Route("")]
        [Route("IndexVilla")]
        public async Task<IActionResult> IndexVilla(int? page, int? pageSize)
        {
            List<VillaDTO>list=new();
            var response=await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                list=JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
            }
            int pageNumber = page ?? 1;
            int size = pageSize ?? 5;       
            if (size < 1) size = 5; 
            IPagedList<VillaDTO> pagedList = list.ToPagedList(pageNumber, size); 
            ViewBag.PageSize = size;
            ViewBag.TotalRecords = list.Count;
            ViewBag.StartRecord = ((pageNumber - 1) * size) + 1;
            ViewBag.EndRecord = Math.Min(pageNumber * size, list.Count);
            return View(pagedList);
            //return View(list);
        }
        [Authorize(Roles ="admin")]
        [HttpGet]
        [Route("CreateVilla")]
        public async Task<IActionResult> CreateVilla()
        {
            return View();
        }
        [Authorize(Roles ="admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreateVilla")]
        public async Task<IActionResult> CreateVilla(VillaCreateDTO model)
        {
           if(ModelState.IsValid)
           {
            var response=await _villaService.CreateAsync<APIResponse>(model,HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                TempData["success"]="Villa Created Successfully";
                return RedirectToAction(nameof(IndexVilla));
            }
           }
            TempData["error"]="Error encounter";
            return View(model);
        }
        [Authorize(Roles ="admin")]
        [HttpGet]
        [Route("UpdateVilla")]
        public async Task<IActionResult> UpdateVilla(int villaId)
        {
            var response=await _villaService.GetAsync<APIResponse>(villaId,HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                
                VillaDTO model=JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
                return View(_mapper.Map<VillaUpdateDTO>(model));
            }
           
            return NotFound();
        }
        [Authorize(Roles ="admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UpdateVilla")]
        public async Task<IActionResult> UpdateVilla(VillaUpdateDTO model)
        {
           if(ModelState.IsValid)
           {
            var response=await _villaService.UpdateAsync<APIResponse>(model,HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                TempData["success"]="Villa Updated successfully";
                return RedirectToAction(nameof(IndexVilla));
            }
           }
            TempData["error"]="Error encounter";
            return View(model);
        }
        [Authorize(Roles ="admin")]
        [HttpGet]
        [Route("DeleteVilla")]
        public async Task<IActionResult> DeleteVilla(int villaId)
        {
            var response=await _villaService.GetAsync<APIResponse>(villaId,HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                VillaDTO model=JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteVilla")]
        public async Task<IActionResult> DeleteVilla(VillaDTO model)
        {
            var response=await _villaService.DeleteAsync<APIResponse>(model.Id,HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                TempData["success"]="Villa deleted Successfully";
                return RedirectToAction(nameof(IndexVilla));
            }
            TempData["error"]="Error encounter";
             return View(model);
        }
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
             var response = await _roomService.GetRoomsByVillaAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));

             if (response != null && response.IsSuccess)
             {
                  try
                  {  
                      VillaDTO villa = response.Result as VillaDTO;
                      if (villa == null)
                      {
                          string villaJson = JsonConvert.SerializeObject(response.Result);
                          Console.WriteLine($"Villa JSON: {villaJson}");
                          villa = JsonConvert.DeserializeObject<VillaDTO>(villaJson);
                      }

                      if (villa != null)
                      {
                          var viewModel = new VillaDetailsViewModel
                          {
                              Villa = villa,
                              Rooms = villa.Rooms ?? new List<RoomDTO>(),  
                              Facilities = villa.Facilities ?? new List<FacilityDTO>() 
                          };

                          return View(viewModel);
                      }
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
                   }
            }

             return NotFound();
         }
         [HttpGet("GetVillasByDestination/{destinationName}")]
         public async Task<IActionResult> GetVillasByDestination(string destinationName)
         {
             Console.WriteLine($"Received destination: {destinationName}"); 
             var response = await _villaService.GetVillasByDestinationAsync<APIResponse>(destinationName, "");

             if (response != null && response.IsSuccess)
             {
                var villas = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
                ViewBag.DestinationName = destinationName; 
                return View(villas);
             }

              TempData["error"] = "No villas found for this destination.";
              return RedirectToAction("AllDestinations", "Destination");
         }
         [HttpGet]
         [Route("Search")]
         public async Task<IActionResult> Search(string query)
         {
              List<VillaDTO> villaList = new();

            var response = await _villaService.SearchVillasAsync<APIResponse>(query, HttpContext.Session.GetString(SD.SessionToken));

           if (response != null && response.IsSuccess)
           {
           villaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
           } 

          return View(villaList);
        }

        
    }
}
