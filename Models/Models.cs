using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Http;


namespace HelloAspNetCore.Models{

    public enum BlogStatus
    {
        Created=10,
        Draft = 20,
        Released = 30,
        Archived = 40,
        Deleted = 50
    }

    public class Blog
    {
        public long ID { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        public string Content { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Required]
        public string Status { get; set; }

        [DisplayFormat(DataFormatString = "{0:yy-MMM-dd HH:mm:ss}")]
        public DateTime CreatedTime { get; set; }
        public string CreatedUser { get; set; }

        [NotMapped]
        public IFormFile Attachment { get; set; }


        public Category Category { get; set; }

        public ICollection<Attachment> UploadedFiles { get; set; }

        public Blog()
        {
            ID = IdGenerator.NextId();
            Title = " ";
            Content = " ";
            Status = BlogStatus.Created.ToString();
            CategoryID = 1;
            CreatedTime = DateTime.Now;
            CreatedUser = "NetCoreAuthor";

            UploadedFiles = new List<Attachment>();

        }
    }

    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }

        public ICollection<Blog> Blogs { get; set; }
    }

    public class Attachment
    {
        public long ID { get; set; }
        public long BlogID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedTime { get; set; }
        public string UploadedBy { get; set;}

        public Attachment()
        {
            ID = IdGenerator.NextId();
            UploadedTime = DateTime.Now;
            UploadedBy = "NetCoreUser";
        }
    }

    public static class IdGenerator
    {
        private static IdWorker idWorker;
        static IdGenerator()
        {
            idWorker = new IdWorker(1, 1);
        }

        public static long NextId()
        {
            return idWorker.NextId();
        }

    }
}
