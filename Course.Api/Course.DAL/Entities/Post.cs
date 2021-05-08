using System;
using System.Collections.Generic;
using System.Text;

namespace Course.DAL.Entities
{
    public class Post
    {
        public Post()
        {
            Commentary = new HashSet<Commentary>();
            Assessment = new HashSet<Assessment>();
        }
        public int PostId { get; set; }
        public DateTime CreatingDate { get; set; }
        public string FileName { get; set; }
        public string ImageName { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Commentary> Commentary { get; set; }
        public virtual ICollection<Assessment> Assessment { get; set; }
    }
}
