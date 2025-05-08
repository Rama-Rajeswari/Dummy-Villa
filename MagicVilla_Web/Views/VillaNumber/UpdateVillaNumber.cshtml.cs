using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.VillaNumber
{
    public class UpdateVillaNumber : PageModel
    {
        private readonly ILogger<UpdateVillaNumber> _logger;

        public UpdateVillaNumber(ILogger<UpdateVillaNumber> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}