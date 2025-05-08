using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;
using X.PagedList.Extensions;

namespace MagicVilla_Web.Controllers
{
    [Route("[controller]")]
    public class DestinationController:Controller
    {
      
        private readonly IDestinationService _destinationService;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public DestinationController(IDestinationService destinationService, IMapper mapper,IVillaService villaService)
        {
            _destinationService = destinationService;
            _villaService=villaService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        [Route("IndexDestination")]
        public async Task<IActionResult> IndexDestination(int? page, int? pageSize)
        {
            List<DestinationDTO> list = new();
            var response = await _destinationService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<DestinationDTO>>(Convert.ToString(response.Result));
            }
            int pageNumber = page ?? 1;
            int size = pageSize ?? 5;       
            if (size < 1) size = 5; 
            IPagedList<DestinationDTO> pagedList = list.ToPagedList(pageNumber, size); 
            ViewBag.PageSize = size;
            ViewBag.TotalRecords = list.Count;
            ViewBag.StartRecord = ((pageNumber - 1) * size) + 1;
            ViewBag.EndRecord = Math.Min(pageNumber * size, list.Count);
            return View(pagedList);
            //return View(list);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("CreateDestination")]
        public IActionResult CreateDestination()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreateDestination")]
        public async Task<IActionResult> CreateDestination(DestinationCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _destinationService.CreateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Destination Created Successfully";
                    return RedirectToAction(nameof(IndexDestination));
                }
            }
            TempData["error"] = "Error encountered";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("UpdateDestination/{id}")]
       
        public async Task<IActionResult> UpdateDestination(int id)
        {
            var response = await _destinationService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                DestinationDTO model = JsonConvert.DeserializeObject<DestinationDTO>(Convert.ToString(response.Result));
                return View(_mapper.Map<DestinationUpdateDTO>(model));
            }
            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost("UpdateDestination/{id}")]
        public async Task<IActionResult> UpdateDestination(DestinationUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _destinationService.UpdateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Destination Updated Successfully";
                    return RedirectToAction(nameof(IndexDestination));
                }
            }
            TempData["error"] = "Error encountered";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("DeleteDestination")]
        public async Task<IActionResult> DeleteDestination(int id)
        {
            var response = await _destinationService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                DestinationDTO model = JsonConvert.DeserializeObject<DestinationDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteDestination")]
        public async Task<IActionResult> DeleteDestination(DestinationDTO model)
        {
            var response = await _destinationService.DeleteAsync<APIResponse>(model.Id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Destination Deleted Successfully";
                return RedirectToAction(nameof(IndexDestination));
            }
            TempData["error"] = "Error encountered";
            return View(model);
        }
        
       [HttpGet("AllDestinations")]
        public async Task<IActionResult> AllDestinations()
        {
            var response = await _destinationService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

           if (response != null && response.IsSuccess)
           {
                var destinations = JsonConvert.DeserializeObject<List<DestinationDTO>>(response.Result.ToString());
                return View("AllDestinations", destinations);
           }

               return View("AllDestinations", new List<DestinationDTO>()); 
        }
        

    }
}