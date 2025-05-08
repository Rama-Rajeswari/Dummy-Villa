using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_VillaAPI.Models.Dto;
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
    public class RoomController:Controller
    {
        private readonly IRoomService _roomService;
        private readonly IVillaService _villaService;
        private readonly IGuestTypeService _guestTypeService;
        private readonly IMapper _mapper;

        public RoomController(IRoomService roomService, IMapper mapper,IVillaService villaService,IGuestTypeService guestTypeService)
        {
            _roomService = roomService;
            _villaService=villaService;
            _guestTypeService=guestTypeService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        [Route("IndexRoom")]
        public async Task<IActionResult> IndexRoom(int? page, int? pageSize)
        {
            List<RoomDTO> list = new();
            var response = await _roomService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<RoomDTO>>(Convert.ToString(response.Result));
            }
            int pageNumber = page ?? 1;
            int size = pageSize ?? 5;       
            if (size < 1) size = 5; 
            IPagedList<RoomDTO> pagedList = list.ToPagedList(pageNumber, size); 
            ViewBag.PageSize = size;
            ViewBag.TotalRecords = list.Count;
            ViewBag.StartRecord = ((pageNumber - 1) * size) + 1;
            ViewBag.EndRecord = Math.Min(pageNumber * size, list.Count);
            return View(pagedList);
            //return View(list);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("CreateRoom")]
        public IActionResult CreateRoom()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreateRoom")]
        public async Task<IActionResult> CreateRoom(RoomCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _roomService.CreateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Room Created Successfully";
                    return RedirectToAction(nameof(IndexRoom));
                }
            }
            TempData["error"] = "Error encountered";
            return View(model);
        }
        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("UpdateRoom")]
        public async Task<IActionResult> UpdateRoom(int roomId)
        {
             var response = await _roomService.GetAsync<APIResponse>(roomId, HttpContext.Session.GetString(SD.SessionToken));
             if (response != null && response.IsSuccess)
             {
                 RoomDTO model = JsonConvert.DeserializeObject<RoomDTO>(Convert.ToString(response.Result));
                 var roomUpdateDto = _mapper.Map<RoomUpdateDTO>(model);
                 var guestTypeResponse = await _guestTypeService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
                 if (guestTypeResponse != null && guestTypeResponse.IsSuccess && guestTypeResponse.Result != null)
                 {
                    ViewBag.GuestTypes = JsonConvert.DeserializeObject<List<GuestTypeDTO>>(Convert.ToString(guestTypeResponse.Result));
                 }
                 else
                 {
                    ViewBag.GuestTypes = new List<GuestTypeDTO>(); // Avoid null reference
                 }  
                 var villaResponse = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
                 if (villaResponse != null && villaResponse.IsSuccess && villaResponse.Result != null)
                 {
                    ViewBag.Villas = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(villaResponse.Result));
                 }
                 else
                 {
                    ViewBag.Villas = new List<VillaDTO>();
                 }
                  return View(roomUpdateDto);
            }
            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UpdateRoom")]
        public async Task<IActionResult> UpdateRoom(RoomUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
               var response = await _roomService.UpdateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
               if (response != null && response.IsSuccess)
               {
                   TempData["success"] = "Room Updated Successfully";
                   return RedirectToAction(nameof(IndexRoom));
               }
            }
             TempData["error"] = "Error encountered";
             return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [Route("DeleteRoom")]
        public async Task<IActionResult> DeleteRoom(int roomId)
        {
            var response = await _roomService.GetAsync<APIResponse>(roomId, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                RoomDTO model = JsonConvert.DeserializeObject<RoomDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteRoom")]
        public async Task<IActionResult> DeleteRoom(RoomDTO model)
        {
            var response = await _roomService.DeleteAsync<APIResponse>(model.Id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Room Deleted Successfully";
                return RedirectToAction(nameof(IndexRoom));
            }
            TempData["error"] = "Error encountered";
            return View(model);
        }
    }
}