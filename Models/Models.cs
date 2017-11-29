using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace HelloAspNetCore.Models{

    public enum BlogStatus
    {
        Draft = 10,
        Released = 20,
        Archived = 30,
        Deleted = 50
    }

    public class Blog
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryID { get; set; }
        public string Status { get; set; }

        [DisplayFormat(DataFormatString = "{0:yy-MMM-dd HH:mm:ss}")]
        public DateTime CreatedTime { get; set; }
        public string CreatedUser { get; set; }


        public Category Category { get; set; }

        public Blog()
        {
            ID = "100"; //IdGenerator.GenerateId(context, "sp_CreateUniqueID");
            Status = BlogStatus.Draft.ToString();
            CreatedTime = DateTime.Now;
            CreatedUser = "NetCoreAuthor";
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
}
