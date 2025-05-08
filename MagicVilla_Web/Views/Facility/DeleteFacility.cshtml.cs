using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.Facility
{
    public class DeleteFacility : PageModel
    {
        private readonly ILogger<DeleteFacility> _logger;

        public DeleteFacility(ILogger<DeleteFacility> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}