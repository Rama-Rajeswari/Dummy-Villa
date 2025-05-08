using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.Destination
{
    public class DeleteDestination : PageModel
    {
        private readonly ILogger<DeleteDestination> _logger;

        public DeleteDestination(ILogger<DeleteDestination> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}