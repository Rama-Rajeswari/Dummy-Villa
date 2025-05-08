using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.VillaNumber
{
    public class CreateVillaNumber : PageModel
    {
        private readonly ILogger<CreateVillaNumber> _logger;

        public CreateVillaNumber(ILogger<CreateVillaNumber> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}