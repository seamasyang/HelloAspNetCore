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

            if (!BlogExists(Blog.ID))
                await CreateBlogAsync();

            if (string.Compare(userAction, "Save", true) == 0)
            {
                await SaveBlogAsync();
                return Page();
            }
            else if (string.Compare(userAction, "Submit", true) == 0)
            {
                if (string.IsNullOrWhiteSpace(Blog.Title))
                    ModelState.AddModelError("Blog.Title", "Title is blank.");
                if (string.IsNullOrWhiteSpace(Blog.Content))
                    ModelState.AddModelError("Blog.Content", "Content is blank.");


                if (!ModelState.IsValid)
                {
                    // created->draft
                    if (string.IsNullOrWhiteSpace(Blog.Title))
                        Blog.Title = string.Empty;
                    if (string.IsNullOrWhiteSpace(Blog.Content))
                        Blog.Content = string.Empty;
                    Blog.Status = BlogStatus.Draft.ToString();
                    _context.Attach(Blog).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Page();
                }
                else
                {
                    Blog.Status = BlogStatus.Released.ToString();
                    _context.Attach(Blog).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
            }
            else
            {
                await UploadFileAsync(Blog.Attachment);
                return Page();
            }
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

                // update memory object
                Blog.UploadedFiles = _context.Attachment.Where(a => a.BlogID == Blog.ID).ToList();

            }
            catch (IOException ex)
            {
                ModelState.AddModelError(formFile.Name, $"The file ({fileName}) upload failed {ex.Message}. Please contact the Help Desk for support.");
                // Log the exception

                ViewData["UploadException"] = $"The file ({fileName}) upload failed {ex.Message}. Please contact the Help Desk for support.";
            }

            return Page();
        }

        private async Task<IActionResult> CreateBlogAsync()
        {
            if (string.IsNullOrWhiteSpace(Blog.Title))
                Blog.Title = string.Empty;
            if (string.IsNullOrWhiteSpace(Blog.Content))
                Blog.Content = string.Empty;

            _context.Blog.Add(Blog);
            await _context.SaveChangesAsync();
            return Page();
        }

        private async Task<IActionResult> SaveBlogAsync()
        {

            Blog.Status = BlogStatus.Draft.ToString();
            _context.Attach(Blog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                Blog.UploadedFiles = _context.Attachment.Where(a => a.BlogID == Blog.ID).ToList();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BlogExists(Blog.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw ex;
                }
            }

            return Page();
        }


        private bool BlogExists(long id)
        {
            return _context.Blog.Any(e => e.ID == id && e.Status == BlogStatus.Draft.ToString());
        }
    }


}