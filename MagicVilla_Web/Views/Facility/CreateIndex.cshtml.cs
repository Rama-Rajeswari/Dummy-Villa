using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MagicVilla_Web.Views.Facility
{
    public class CreateIndex : PageModel
    {
        private readonly ILogger<CreateIndex> _logger;

        public CreateIndex(ILogger<CreateIndex> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}