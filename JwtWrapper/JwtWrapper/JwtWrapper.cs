using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using JwtWrapper.DTO.Output;
using JwtWrapper.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace JwtWrapper
{
    public class JwtWrapper : IJwtWrapper
    {
        private readonly IJwtEncoder _jwtEncoder;
        private readonly IJwtDecoder _jwtDecoder;

        public JwtWrapper()
        {
            //Dependencies
            IJwtAlgorithm jwtAlgorithm = new HMACSHA256Algorithm();
            IJsonSerializer jsonSerializer = new JsonNetSerializer();
            IBase64UrlEncoder base64UrlEncoder = new JwtBase64UrlEncoder();
            IDateTimeProvider dateTimeProvider = new UtcDateTimeProvider();
            IJwtValidator jwtValidator = new JwtValidator(jsonSerializer, dateTimeProvider);

            //Initialize
            _jwtEncoder = new JwtEncoder(jwtAlgorithm, jsonSerializer, base64UrlEncoder);
            _jwtDecoder = new JwtDecoder(jsonSerializer, jwtValidator, base64UrlEncoder);
        }

        /// <summary>
        /// Encode object with secret
        /// </summary>
        /// <param name="secretToken"></param>
        /// <param name="objectToEncode"></param>
        /// <returns></returns>
        public OutputDto<string> Encode(string secretToken, object objectToEncode, DateTime? tokenExpireDate = null, IJwtAlgorithm algorithm = null)
        {
            try
            {
                //Add 1 hour of token expiration by default
                DateTimeOffset expireDate = DateTimeOffset.UtcNow.AddHours(1);

                //Add default algorithm
                if (algorithm == null)
                {
                    algorithm = new HMACSHA256Algorithm();
                }

                //Encode token
                var tokenBuilder = new JwtBuilder()
                                    .WithAlgorithm(algorithm)
                                    .WithSecret(secretToken)
                                    .AddClaim("content", objectToEncode);

                //Validate expire date
                if (tokenExpireDate.HasValue)
                {
                    //Parse date to UTC
                    expireDate = tokenExpireDate.Value.Kind != DateTimeKind.Utc ? tokenExpireDate.Value.ToUniversalTime() : tokenExpireDate.Value;

                    tokenBuilder = tokenBuilder.AddClaim("exp", expireDate.ToUnixTimeSeconds());
                }

                //Build token
                var token = tokenBuilder.Build();

                //Validate token
                if (string.IsNullOrWhiteSpace(token))
                {
                    return new OutputDto<string>(HttpStatusCode.Unauthorized, "The generated token is empty", null);
                }
                
                if (tokenExpireDate.HasValue)
                {
                    //Return converted Object
                    return new OutputDto<string>(token, HttpStatusCode.OK, ToUtc(expireDate.DateTime));
                }
                else
                {
                    return new OutputDto<string>(token);
                }

            }
            catch (Exception ex)
            {
                return new OutputDto<string>(HttpStatusCode.Unauthorized, ex.ToString());
            }
        }

        /// <summary>
        /// Decode object with secret
        /// </summary>
        /// <param name="secretToken"></param>
        /// <param name="token"></param>
        /// <returns>Complete object with expire date</returns>
        public OutputDto<string> Decode(string secretToken, string token)
        {
            try
            {
                //Decode JWT token
                string decodedToken = _jwtDecoder.Decode(token, secretToken, verify: true);

                //Decode Token
                DecodeToken(decodedToken, out string content, out DateTime? parsedDateTime);

                if (parsedDateTime.HasValue)
                {
                    //Return converted Object
                    return new OutputDto<string>(content, HttpStatusCode.OK, ToUtc(parsedDateTime.Value));
                }
                else
                {
                    return new OutputDto<string>(content);
                }
               
            }
            catch (TokenExpiredException ex)
            {
                return new OutputDto<string>(HttpStatusCode.Unauthorized, "Token has expired", ex.ToString());
            }
            catch (SignatureVerificationException ex)
            {
                return new OutputDto<string>(HttpStatusCode.Unauthorized, "Token has invalid signature", ex.ToString());
            }
        }

        /// <summary>
        /// Decode object with secret
        /// </summary>
        /// <param name="secretToken"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public OutputDto<T> Decode<T>(string secretToken, string token)
        {
            try
            {
                //Decode token
                string decodedToken = _jwtDecoder.Decode(token, secretToken, verify: true);

                //Decode Token
                DecodeToken(decodedToken, out string content, out DateTime? parsedDateTime);

                //convert json to object
                var convertedObject = JsonConvert.DeserializeObject<T>(content);
                
                if (parsedDateTime.HasValue)
                {
                    //Return converted Object
                    return new OutputDto<T>(convertedObject, HttpStatusCode.OK, ToUtc(parsedDateTime.Value));
                }
                else
                {
                    return new OutputDto<T>(convertedObject);
                }
            }
            catch (TokenExpiredException ex)
            {
                return new OutputDto<T>(HttpStatusCode.Unauthorized, "Token has expired", ex.ToString());
            }
            catch (SignatureVerificationException ex)
            {
                return new OutputDto<T>(HttpStatusCode.Unauthorized, "Token has invalid signature", ex.ToString());
            }
        }

        /// <summary>
        /// Convert a DateTime to Utc
        /// </summary>
        /// <param name="parseDateTime"></param>
        /// <returns></returns>
        private string ToUtc(DateTime parseDateTime)
        {
            return parseDateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");
        }

        /// <summary>
        /// Decode token
        /// </summary>
        /// <param name="decodedToken"></param>
        /// <param name="content"></param>
        /// <param name="parseDateTime"></param>
        private void DecodeToken(string decodedToken, out string content, out DateTime? parseDateTime)
        {
            //Get content
            var decodedObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(decodedToken);

            parseDateTime = null;

            //Get Expire date as long
            if (decodedObject.ContainsKey("exp") && long.TryParse(decodedObject["exp"]?.ToString(), out long expireDate))
            {
                //Convert expire date
                parseDateTime = DateTimeOffset.FromUnixTimeSeconds(expireDate).DateTime;
            }
          

            //Get content
            content = decodedObject["content"].ToString();
        }
    }
}
