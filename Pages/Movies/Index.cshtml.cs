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
    public class IndexModel : PageModel
    {
        private readonly HelloAspNetCore.Models.MovieContext _context;

        public IndexModel(HelloAspNetCore.Models.MovieContext context)
        {
            _context = context;
        }

        public IList<Models.Models> Movie { get;set; }

        /*public async Task OnGetAsync()
        {
            Movie = await _context.Movie.ToListAsync();
        }*/

        public async Task OnGetAsync(string searchString)
        {
            var movies = from m in _context.Movie
                        select m;

        

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString));
            }

            Movie = await movies.ToListAsync();
        }

    }
}
