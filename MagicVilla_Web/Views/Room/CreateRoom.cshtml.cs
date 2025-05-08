using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.Room
{
    public class CreateRoom : PageModel
    {
        private readonly ILogger<CreateRoom> _logger;

        public CreateRoom(ILogger<CreateRoom> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}