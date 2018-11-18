using JwtWrapper.DTO.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace JwtWrapper.Interface
{
    public interface IJwtWrapper
    {
        /// <summary>
        /// Encode object with secret
        /// </summary>
        /// <param name="secretToken"></param>
        /// <param name="objectToEncode"></param>
        /// <param name="tokenExpireDate"></param>
        /// <returns></returns>
        OutputDto<string> Encode(string secretToken, object objectToEncode, DateTime? tokenExpireDate = null);

        /// <summary>
        /// Decode object with secret
        /// </summary>
        /// <param name="secretToken"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        OutputDto<string> Decode(string secretToken, string token);

        /// <summary>
        /// Decode object with secret
        /// </summary>
        /// <param name="secretToken"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        OutputDto<T> Decode<T>(string secretToken, string token);
    }
}
