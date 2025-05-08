using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.Facility
{
    public class IndexFacility : PageModel
    {
        private readonly ILogger<IndexFacility> _logger;

        public IndexFacility(ILogger<IndexFacility> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}