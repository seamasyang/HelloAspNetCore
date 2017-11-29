using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HelloAspNetCore.Models;

namespace HelloAspNetCore.Pages.Movies
{
    public class DetailsModel : PageModel
    {
        private readonly HelloAspNetCore.Models.MovieContext _context;

        public DetailsModel(HelloAspNetCore.Models.MovieContext context)
        {
            _context = context;
        }

        public Models Movie { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Movie = await _context.Movie.SingleOrDefaultAsync(m => m.ID == id);

            if (Movie == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
