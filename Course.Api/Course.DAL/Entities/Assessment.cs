using System;
using System.Collections.Generic;
using System.Text;

namespace Course.DAL.Entities
{
    public class Assessment
    {
        public int AssessmentId { get; set; }
        public bool Like { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public int PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
