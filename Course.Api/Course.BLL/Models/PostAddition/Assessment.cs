using System;
using System.Collections.Generic;
using System.Text;

namespace Course.BLL.Models.PostAddition
{
    public class Assessment
    {
        public bool Like => string.Compare(AssessmentString, "true") == 0;
        public string AssessmentString { get; set; }
    }
}
