using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.GuestType
{
    public class DeleteGuestType : PageModel
    {
        private readonly ILogger<DeleteGuestType> _logger;

        public DeleteGuestType(ILogger<DeleteGuestType> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}