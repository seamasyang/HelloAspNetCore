using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HelloAspNetCore.Models;

namespace HelloAspNetCore.Pages.Categories
{
    public class DetailsModel : PageModel
    {
        private readonly HelloAspNetCore.Models.NetCoreBlogContext _context;

        public DetailsModel(HelloAspNetCore.Models.NetCoreBlogContext context)
        {
            _context = context;
        }

        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category = await _context.Category.SingleOrDefaultAsync(m => m.ID == id);

            if (Category == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
