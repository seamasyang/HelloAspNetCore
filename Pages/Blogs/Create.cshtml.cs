using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using HelloAspNetCore.Models;

namespace HelloAspNetCore.Pages.Blogs
{
    public class CreateModel : PageModel
    {
        private readonly HelloAspNetCore.Models.NetCoreBlogContext _context;

        public CreateModel(HelloAspNetCore.Models.NetCoreBlogContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "ID");

            Blog = new Blog();

            return Page();
        }

        [BindProperty]
        public Blog Blog { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var userAction = Request.Query["action"];

            if (string.Compare(userAction, "create", true) == 0)
            {
                await CreateBlogAsync();
            }
            else
            {
                // save blog first; otherwise FOREIGN KEY constraint failed exception
                if (string.IsNullOrWhiteSpace(Blog.Title))
                    Blog.Title = string.Empty;
                if (string.IsNullOrWhiteSpace(Blog.Content))
                    Blog.Content = string.Empty;

                _context.Blog.Add(Blog);
                await _context.SaveChangesAsync();

                // save attachment (file & DB)
                await UploadFileAsync(Blog.Attachment);

                // refresh attachment uploaded?
            }

            return Page();

        }

        public async Task<IActionResult> UploadFileAsync(IFormFile formFile)
        {

            if (formFile == null || string.IsNullOrWhiteSpace(formFile.FileName))
            {
                ModelState.AddModelError("Blog.Attachment", $"No file selected to upload.");
                return Page();
            }

            var fileName = WebUtility.UrlEncode(Path.GetFileName(formFile.FileName));

            // allowed file extension check ignored


            if (formFile.Length == 0)
            {
                ModelState.AddModelError(formFile.Name, $"The file ({fileName}) is empty.");
                return Page();
            }


            try
            {
                Attachment attachment = new Attachment();

                // save to server side
                string newName = string.Format("{0}{1}", attachment.ID, Path.GetExtension(formFile.FileName));
                var path = string.Format(@"{0}\Attachments\{1}", Environment.CurrentDirectory, newName);
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    await formFile.CopyToAsync(fs);
                }


                // update to database                   
                attachment.FileName = fileName;
                attachment.FilePath = path;
                attachment.BlogID = Blog.ID;
                _context.Attachment.Add(attachment);
                await _context.SaveChangesAsync();

            }
            catch (IOException ex)
            {
                ModelState.AddModelError(formFile.Name, $"The file ({fileName}) upload failed {ex.Message}. Please contact the Help Desk for support.");
                // Log the exception

                ViewData["UploadException"] = $"The file ({fileName}) upload failed {ex.Message}. Please contact the Help Desk for support.";
            }

            return Page();
        }

        public async Task<IActionResult> CreateBlogAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Blog.Status = BlogStatus.Released.ToString();
            _context.Attach(Blog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogExists(Blog.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Blogs/Index");
        }

        private bool BlogExists(long id)
        {
            return _context.Blog.Any(e => e.ID == id);
        }
    }


}