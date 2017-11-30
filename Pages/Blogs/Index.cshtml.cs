using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HelloAspNetCore.Models;

namespace HelloAspNetCore.Pages.Blogs
{
    public class IndexModel : PageModel
    {
        private readonly HelloAspNetCore.Models.NetCoreBlogContext _context;

        public IndexModel(HelloAspNetCore.Models.NetCoreBlogContext context)
        {
            _context = context;
        }

        public IList<Blog> Blog { get;set; }

        public async Task OnGetAsync()
        {
            Blog = await _context.Blog.OrderByDescending(a=>a.ID).Where(b=>b.Status == BlogStatus.Released.ToString())
                .Include(b => b.Category).Include(x=>x.UploadedFiles).ToListAsync();

        }
    }
}
