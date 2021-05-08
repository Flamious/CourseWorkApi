using System;
using System.Collections.Generic;
using System.Text;

namespace Course.BLL.Models
{
    public class Assessment
    {
        public Assessment() { }
        public Assessment(DAL.Entities.Assessment assessment) 
        {
            AssessmentId = assessment.AssessmentId;
            Like = assessment.Like;
            UserId = assessment.UserId;
            PostId = assessment.PostId;
        }
        public int AssessmentId { get; set; }
        public bool Like { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
    }
}
