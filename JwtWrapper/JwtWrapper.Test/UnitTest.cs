using JwtWrapper.DTO.Output;
using JwtWrapper.DTO.Status;
using JwtWrapper.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace JwtWrapper.Test
{
    public class UnitTest
    {
        private readonly IJwtWrapper _jwtWrapper;
        private StatusDto _objectToEncode;
        private const string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";

        /// <summary>
        /// Initialize dependencies
        /// </summary>
        public UnitTest()
        {
            _jwtWrapper = new JwtWrapper();

            _objectToEncode = new StatusDto
            {
                Message = "Space Dust",
                Code = HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Encode StatusDto 
        /// </summary>
        [Fact]
        public void Encode()
        {
            // Encode object
            var encodeOutput = EncodeObject();

            //Validate token
            bool validTest = encodeOutput != null && encodeOutput.Status.Code == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(encodeOutput.Content);

            //Validate test
            Assert.True(validTest);
        }

        /// <summary>
        /// Decode Status Dto to string
        /// </summary>
        [Fact]
        public void Decode()
        {
            // Encode object
            var tokenOutput = EncodeObject();

            //Decode object
            var decodeOutput = _jwtWrapper.Decode(secret, tokenOutput?.Content);

            //Validate token
            bool validTest = decodeOutput != null && decodeOutput.Status.Code == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(decodeOutput.Content);

            //Deserialize object
            var deserialiedObject = JsonConvert.DeserializeObject<StatusDto>(decodeOutput.Content);

            //Compare objects
            bool validObject = deserialiedObject.Message == _objectToEncode.Message
                                && deserialiedObject.Code == _objectToEncode.Code
                                && deserialiedObject.Exception == _objectToEncode.Exception;

            //Validate test
            Assert.True(validTest && validObject);
        }

        /// <summary>
        /// Decode Status Dto
        /// </summary>
        [Fact]
        public void Decode_With_Type()
        {
            // Encode object
            var tokenOutput = EncodeObject();

            //Decode object
            var decodeOutput = _jwtWrapper.Decode<StatusDto>(secret, tokenOutput?.Content);

            //Validate token
            bool validTest = decodeOutput != null && decodeOutput.Status.Code == HttpStatusCode.OK && decodeOutput.Content != null;
            
            //Compare objects
            bool validObject = decodeOutput.Content.Message == _objectToEncode.Message
                                && decodeOutput.Content.Code == _objectToEncode.Code
                                && decodeOutput.Content.Exception == _objectToEncode.Exception;

            //Validate test
            Assert.True(validTest && validObject);
        }

        /// <summary>
        /// Decode Status Dto to string
        /// </summary>
        [Fact]
        public void Decode_With_ExpiredToken_And_Fail()
        {
            //Set the token expiration day to yesterday
            DateTime yesterday = DateTime.Now.AddDays(-1);

            // Encode object
            var tokenOutput = EncodeObject(yesterday);

            //Decode object
            var decodeOutput = _jwtWrapper.Decode(secret, tokenOutput?.Content);

            //Validate token
            bool validTest = decodeOutput != null 
                            && decodeOutput.Status.Code != HttpStatusCode.OK 
                            && string.IsNullOrWhiteSpace(decodeOutput.Content)
                            && decodeOutput.Status.Message.Contains("Token has expired");

            //Validate test
            Assert.True(validTest);
        }

        /// <summary>
        /// Encode object
        /// </summary>
        /// <returns></returns>
        private OutputDto<string> EncodeObject(DateTime? datetime = null)
        {
            //Encode object
            return _jwtWrapper.Encode(secret, _objectToEncode, datetime);
        }
    }
}
