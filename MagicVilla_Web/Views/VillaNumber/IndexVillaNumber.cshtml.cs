using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.VillaNumber
{
    public class IndexVillaNumber : PageModel
    {
        private readonly ILogger<IndexVillaNumber> _logger;

        public IndexVillaNumber(ILogger<IndexVillaNumber> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}