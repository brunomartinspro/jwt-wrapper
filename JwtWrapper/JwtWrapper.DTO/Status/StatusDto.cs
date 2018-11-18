using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace JwtWrapper.DTO.Status
{
    public class StatusDto
    {
        public string Message { get; set; }
        public string Exception { get; set; }
        public HttpStatusCode Code { get; set; }
    }
}
