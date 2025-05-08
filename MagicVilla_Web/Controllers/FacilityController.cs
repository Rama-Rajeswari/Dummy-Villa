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
using X.PagedList.Mvc.Core;


namespace MagicVilla_Web.Controllers
{
    [Route("[controller]")]
    public class FacilityController:Controller
    {
        private readonly IFacilityService _facilityService;
        private readonly IMapper _mapper;

        public FacilityController(IFacilityService facilityService, IMapper mapper)
        {
            _facilityService = facilityService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        [Route("IndexFacility")]
        public async Task<IActionResult> IndexFacility(int? page, int? pageSize )
        {
            List<FacilityDTO> list = new();
            var response = await _facilityService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<FacilityDTO>>(Convert.ToString(response.Result));
            } 
            int pageNumber = page ?? 1;
            int size = pageSize ?? 5;       
            if (size < 1) size = 5; 
            IPagedList<FacilityDTO> pagedList = list.ToPagedList(pageNumber, size); 
            ViewBag.PageSize = size;
            ViewBag.TotalRecords = list.Count;
            ViewBag.StartRecord = ((pageNumber - 1) * size) + 1;
            ViewBag.EndRecord = Math.Min(pageNumber * size, list.Count);
            return View(pagedList);
            //return View(list);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("CreateFacility")]
        public IActionResult CreateFacility()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreateFacility")]
        public async Task<IActionResult> CreateFacility(FacilityCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _facilityService.CreateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Facility Created Successfully";
                    return RedirectToAction(nameof(IndexFacility));
                }
            }
            TempData["error"] = "Error encountered";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("UpdateFacility/{id:int}")]
        [Route("UpdateFacility")]
        public async Task<IActionResult> UpdateFacility(int id)
        {
            var response = await _facilityService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                FacilityDTO model = JsonConvert.DeserializeObject<FacilityDTO>(Convert.ToString(response.Result));
                return View(_mapper.Map<FacilityUpdateDTO>(model));
            }
            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost("UpdateFacility/{id:int}")] 
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> UpdateFacility(int id,FacilityUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _facilityService.UpdateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Facility Updated Successfully";
                    return RedirectToAction(nameof(IndexFacility));
                }
            }
            TempData["error"] = "Error encountered";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("DeleteFacility/{id:int}")] 
        
        public async Task<IActionResult> DeleteFacility(int id)
        {
            var response = await _facilityService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                FacilityDTO model = JsonConvert.DeserializeObject<FacilityDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [HttpPost("DeleteFacility/{id:int}")]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> DeleteFacility(int id,FacilityDTO model)
        {
            var response = await _facilityService.DeleteAsync<APIResponse>(model.Id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Facility Deleted Successfully";
                return RedirectToAction(nameof(IndexFacility));
            }
            TempData["error"] = "Error encountered";
            return View(model);
        }
    }
}