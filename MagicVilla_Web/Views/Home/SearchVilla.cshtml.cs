using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.Home
{
    public class SearchVilla : PageModel
    {
        private readonly ILogger<SearchVilla> _logger;

        public SearchVilla(ILogger<SearchVilla> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}