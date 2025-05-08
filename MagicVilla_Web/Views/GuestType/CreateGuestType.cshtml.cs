using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.GuestType
{
    public class CreateGuestType : PageModel
    {
        private readonly ILogger<CreateGuestType> _logger;

        public CreateGuestType(ILogger<CreateGuestType> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}