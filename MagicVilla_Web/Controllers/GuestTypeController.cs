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
    public class GuestTypeController:Controller
    {
         private readonly IGuestTypeService _guestTypeService;
        private readonly IMapper _mapper;

        public GuestTypeController(IGuestTypeService guestTypeService, IMapper mapper)
        {
            _guestTypeService = guestTypeService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        [Route("IndexGuestType")]
        public async Task<IActionResult> IndexGuestType(int? page, int? pageSize)
        {
            List<GuestTypeDTO> list = new();
            var response = await _guestTypeService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<GuestTypeDTO>>(Convert.ToString(response.Result));
            }
            int pageNumber = page ?? 1;
            int size = pageSize ?? 5;       
            if (size < 1) size = 5; 
            IPagedList<GuestTypeDTO> pagedList = list.ToPagedList(pageNumber, size); 
            ViewBag.PageSize = size;
            ViewBag.TotalRecords = list.Count;
            ViewBag.StartRecord = ((pageNumber - 1) * size) + 1;
            ViewBag.EndRecord = Math.Min(pageNumber * size, list.Count);
            return View(pagedList);
            //return View(list);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("CreateGuestType")]
        public IActionResult CreateGuestType()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreateGuestType")]
        public async Task<IActionResult> CreateGuestType(GuestTypeCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _guestTypeService.CreateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Guest Type Created Successfully";
                    return RedirectToAction(nameof(IndexGuestType));
                }
            }
            TempData["error"] = "Error encountered";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("UpdateGuestType/{id:int}")]

        public async Task<IActionResult> UpdateGuestType(int id)
        {
            var response = await _guestTypeService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                GuestTypeDTO model = JsonConvert.DeserializeObject<GuestTypeDTO>(Convert.ToString(response.Result));
                return View(_mapper.Map<GuestTypeUpdateDTO>(model));
            }
            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost("UpdateGuestType/{id:int}")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> UpdateGuestType(int id,GuestTypeUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _guestTypeService.UpdateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Guest Type Updated Successfully";
                    return RedirectToAction(nameof(IndexGuestType));
                }
            }
            TempData["error"] = "Error encountered";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("DeleteGuestType/{id:int}")] 
        
        public async Task<IActionResult> DeleteGuestType(int id)
        {
            var response = await _guestTypeService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                GuestTypeDTO model = JsonConvert.DeserializeObject<GuestTypeDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [HttpPost("DeleteGuestType/{id:int}")] 
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> DeleteGuestType(int id,GuestTypeDTO model)
        {
            var response = await _guestTypeService.DeleteAsync<APIResponse>(model.Id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Guest Type Deleted Successfully";
                return RedirectToAction(nameof(IndexGuestType));
            }
            TempData["error"] = "Error encountered";
            return View(model);
        }
    }
}