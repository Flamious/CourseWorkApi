using System;
using System.Collections.Generic;
using System.Text;

namespace Course.BLL.Models
{
    public class ErrorResponse
    {
        public ErrorResponse() { }
        public ErrorResponse(string message) 
        {
            Message = message;
            Error = null;
        }
        public ErrorResponse(string message, List<string> error) 
        {
            Message = message;
            Error = error;
        }
        public string Message { get; set; }
        public List<string> Error { get; set; }
    }
}
