using System;
using System.Collections.Generic;
using System.Text;

namespace Course.BLL.Models
{
    public class Commentary
    {
        public Commentary() { }
        public Commentary(DAL.Entities.Commentary commentary)
        {
            CommentaryId = commentary.CommentaryId;
            Message = commentary.Message;
            PostId = commentary.PostId;
            UserId = commentary.UserId;
            if (commentary.User != null)
                User = new User()
                {
                    Id = commentary.User.Id,
                    UserName = commentary.User.UserName
                };
        }
        public int CommentaryId { get; set; }
        public string Message { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
