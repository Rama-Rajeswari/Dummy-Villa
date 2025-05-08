using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Web.Models.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace MagicVilla_Web.Models.VM
{
    public class VillaNumberDeleteVM
    {
        public VillaNumberDeleteVM()
        {
            VillaNumber=new VillaNumberDTO();
        }
        public VillaNumberDTO VillaNumber{get;set;}
        public IEnumerable<SelectListItem> VillaList{get;set;}
    }
}