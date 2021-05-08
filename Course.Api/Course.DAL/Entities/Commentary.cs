using System;
using System.Collections.Generic;
using System.Text;

namespace Course.DAL.Entities
{
    public class Commentary
    {
        public int CommentaryId { get; set; }
        public DateTime CreatingDate { get; set; }
        public string Message { get; set; }
        public int PostId { get; set; }
        public virtual Post Post { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
