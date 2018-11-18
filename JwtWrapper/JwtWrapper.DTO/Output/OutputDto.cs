using JwtWrapper.DTO.Status;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace JwtWrapper.DTO.Output
{
   public class OutputDto<T>
    {
        public T Content { get; set; }
        public StatusDto Status { get; set; }
        public string ExpireDate { get; set; }

        /// <summary>
        /// Create Output
        /// </summary>
        /// <param name="content"></param>
        /// <param name="httpStatusCode"></param>
        public OutputDto(T content, HttpStatusCode httpStatusCode, string expireDate)
        {
            Content = content;
            Status = new StatusDto
            {
                Message = httpStatusCode.ToString(),
                Code = httpStatusCode
            };
            ExpireDate = expireDate;
        }

        /// <summary>
        /// Create Output
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <param name="exception"></param>
        public OutputDto(HttpStatusCode httpStatusCode, string exception)
        {
            Status = new StatusDto
            {
                Exception = exception,
                Code = httpStatusCode
            };
        }

        /// <summary>
        /// Create Output
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public OutputDto(HttpStatusCode httpStatusCode, string message, string exception)
        {
            Status = new StatusDto
            {
                Message = message,
                Exception = exception,
                Code = httpStatusCode
            };
        }
    }
}
