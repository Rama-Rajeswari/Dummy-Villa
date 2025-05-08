using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.VM;
using MagicVilla_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;



namespace MagicVilla_Web.Controllers
{
    [Route("VillaNumber")]
    public class VillaNumberController:Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public VillaNumberController(IVillaNumberService villaNumberService,IMapper mapper,IVillaService villaService)
        {
            _villaNumberService=villaNumberService;
            _mapper=mapper;
            _villaService=villaService;
        }
        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO>list=new();
            var response=await _villaNumberService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                list=JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
            }
            return View(list);
        }
        [Authorize(Roles ="admin")]
        [HttpGet("CreateVillaNumber")]
        public async Task<IActionResult> CreateVillaNumber()
        {
           
            VillaNumberCreateVM villaNumberVM=new();
            var response=await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                villaNumberVM.VillaList=JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result)).Select(i=>new SelectListItem
                {
                    Text=i.Name,
                    Value=i.Id.ToString()
                });;
            }
            return View(villaNumberVM);
        }
        [Authorize(Roles ="admin")]
        [HttpPost("CreateVillaNumber")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {
           if(ModelState.IsValid)
           {
            var response=await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumber,HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess )
            {
                return RedirectToAction(nameof(IndexVillaNumber));
            }
            else
            {
                if(response.ErrorMessages!=null&&response.ErrorMessages.Count>0 )
                {
                    ModelState.AddModelError("ErrorMessage",response.ErrorMessages.FirstOrDefault());
                }
            }
           }
            var resp=await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(resp!=null && resp.IsSuccess)
            {
                model.VillaList=JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(resp.Result)).Select(i=>new SelectListItem
                {
                    Text=i.Name,
                    Value=i.Id.ToString()
                });;
            }
           
            return View(model);
        }
        [Authorize(Roles ="admin")]
        [HttpGet("UpdateVillaNumber")]
        public async Task<IActionResult> UpdateVillaNumber(int VillaNo)
        {
            VillaNumberUpdateVM villaNumberVM=new();
            var response=await _villaNumberService.GetAsync<APIResponse>(VillaNo,HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                VillaNumberDTO model=JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                villaNumberVM.VillaNumber=_mapper.Map<VillaNumberUpdateDTO>(model);
            }

            response=await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                villaNumberVM.VillaList=JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result)).Select(i=>new SelectListItem
                {
                    Text=i.Name,
                    Value=i.Id.ToString()
                });
                return View(villaNumberVM);
            }

            return NotFound();
        }
        [Authorize(Roles ="admin")]
        [HttpPost("UpdateVillaNumber")]
         [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
        {
           if(ModelState.IsValid)
           {
            var response=await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber,HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess )
            {
                return RedirectToAction(nameof(IndexVillaNumber));
            }
            else
            {
                if(response.ErrorMessages.Count>0)
                {
                    ModelState.AddModelError("ErrorMessage",response.ErrorMessages.FirstOrDefault());
                }
            }
           }
            var resp=await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(resp!=null && resp.IsSuccess)
            {
                model.VillaList=JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(resp.Result)).Select(i=>new SelectListItem
                {
                    Text=i.Name,
                    Value=i.Id.ToString()
                });;
            }
           
            return View(model);
        }
        [Authorize(Roles ="admin")]
        [HttpGet("DeleteVillaNumber")]
        public async Task<IActionResult> DeleteVillaNumber(int VillaNo)
        {
            VillaNumberDeleteVM villaNumberVM=new();
            var response=await _villaNumberService.GetAsync<APIResponse>(VillaNo,HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                VillaNumberDTO model=JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                villaNumberVM.VillaNumber=model;
            }

            response=await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                villaNumberVM.VillaList=JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result)).Select(i=>new SelectListItem
                {
                    Text=i.Name,
                    Value=i.Id.ToString()
                });
                return View(villaNumberVM);
            }

            return NotFound();
        }
        [Authorize(Roles ="admin")]
        [HttpPost("DeleteVillaNumber")]
         [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
        {
            var response=await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumber.VillaNo,HttpContext.Session.GetString(SD.SessionToken));
            if(response!=null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexVillaNumber));
            }
             return View(model);
        }
    }
}