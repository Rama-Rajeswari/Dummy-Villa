using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Web.Models.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Models.VM
{
    public class VillaNumberUpdateVM
    {
        public VillaNumberUpdateVM()
        {
            VillaNumber=new VillaNumberUpdateDTO();
        }
        public VillaNumberUpdateDTO VillaNumber{get;set;}
        public IEnumerable<SelectListItem> VillaList{get;set;}
    }
}