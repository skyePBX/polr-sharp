// Copyright (c) 2021 Pwn (Jonathan) / SkyPBX, LLC. / All rights reserved.

using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PolrSharp.Extensions
{
    public static class HttpClientExtension
    {
        public static async Task<HttpResponseMessage> PostAsJsonNetAsync<T>(this HttpClient httpClient, string requestUrl, T data,
            JsonSerializerSettings serializer = default)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data, serializer));
            return await httpClient.PostAsync(requestUrl, content);
        }
    }
}