using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.GuestType
{
    public class IndexGuestType : PageModel
    {
        private readonly ILogger<IndexGuestType> _logger;

        public IndexGuestType(ILogger<IndexGuestType> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}