using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.Room
{
    public class UpdateRoom : PageModel
    {
        private readonly ILogger<UpdateRoom> _logger;

        public UpdateRoom(ILogger<UpdateRoom> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}