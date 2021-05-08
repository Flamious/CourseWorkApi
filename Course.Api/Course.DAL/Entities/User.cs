using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Course.DAL.Entities
{
    public class User : IdentityUser
    {
        public User()
        {
            Post = new HashSet<Post>();
            Commentary = new HashSet<Commentary>();
            Assessment = new HashSet<Assessment>();
        }
        public bool IsCreatingPost { get; set; }
        public virtual ICollection<Post> Post { get; set; }
        public virtual ICollection<Commentary> Commentary { get; set; }
        public virtual ICollection<Assessment> Assessment { get; set; }
    }
}
