using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Course.BLL.Models
{
    public class Post
    {
        public Post() { }
        public Post(DAL.Entities.Post post)
        {
            PostId = post.PostId;
            Content = post.Content;
            UserId = post.UserId;
            FileName = post.FileName;
            ImageName = post.ImageName;
            if (post.User != null)
                User = new User()
                {
                    Id = post.User.Id,
                    UserName = post.User.UserName
                };
            Commentary = new List<Commentary>();
            foreach (DAL.Entities.Commentary commentary in post.Commentary)
            {
                Commentary.Add(new Commentary(commentary));
            }
            Assessment = new List<Assessment>();
            foreach (DAL.Entities.Assessment assessment in post.Assessment)
            {
                Assessment.Add(new Assessment(assessment));
            }
            DateTime now = DateTime.Now;
            TimeSpan difference = now - post.CreatingDate;
            if(difference.TotalMinutes < 2)
            {
                CreatingDate = "1 minute ago";
            }
            else
            {
                if(difference.TotalHours < 1)
                {
                    CreatingDate = Math.Floor(difference.TotalMinutes).ToString() + " minutes ago";
                }
                else
                {
                    if(difference.TotalHours < 2)
                    {
                        CreatingDate = "1 hour ago";
                    }
                    else
                    {
                        if (difference.TotalDays < 1)
                        {
                            CreatingDate = Math.Floor(difference.TotalHours).ToString() + " hours ago";
                        }
                        else
                        {
                            if(post.CreatingDate.Year < DateTime.Now.Year)
                            {
                                CreatingDate = post.CreatingDate.ToString("dd.MM.yyyy HH:mm");
                            }
                            else
                            {
                                CreatingDate = post.CreatingDate.ToString("dd.MM HH:mm");
                            }
                        }
                    }
                }
            }

        }
        public int PostId { get; set; }
        public string CreatingDate { get; set; }
        public string FileName { get; set; }
        public string ImageName { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public virtual ICollection<Commentary> Commentary { get; set; }
        public virtual ICollection<Assessment> Assessment { get; set; }
    }
}
