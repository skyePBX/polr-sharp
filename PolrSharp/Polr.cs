// Copyright (c) 2021 Jonathan 'Pwn' Rainier / SkyPBX, LLC. / All rights reserved.

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PolrSharp.Enums;
using PolrSharp.Extensions;
using PolrSharp.Models.Request.V2.Action;
using PolrSharp.Models.Response.V2.Action;

namespace PolrSharp
{
    public class Polr
    {
        private readonly string _apiKey;
        private readonly JsonSerializerSettings _serializer;
        private readonly PolrApiVersion _version;
        private Uri _host;

        public Polr(Uri host, string apiKey, PolrApiVersion version = PolrApiVersion.V2)
        {
            _host = host;
            _apiKey = apiKey;
            _version = version;
            _serializer = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task<Lookup> Lookup(string urlEnding)
        {
            return await Get<Lookup>(new NameValueCollection
            {
                {"url_ending", urlEnding}
            });
        }

        public async Task<Shorten> Shorten(string url, bool isSecret = false, string customEnding = default)
        {
            var parameters = new NameValueCollection
            {
                {"url", url},
                {"is_secret", isSecret.ToString().ToLower()}
            };

            if (customEnding != default)
                parameters.Add("custom_ending", customEnding);

            return await Get<Shorten>(parameters);
        }

        public async Task<ShortenBulk> ShortenBulk(ShortenBulkRequest shortenBulkRequest)
        {
            var parameters = new NameValueCollection
            {
                {"data", JsonConvert.SerializeObject(shortenBulkRequest, _serializer)}
            };

            return await Post<ShortenBulk>(default, parameters);
        }

        private async Task<T> Get<T>(NameValueCollection parameters = default)
        {
            var requestClass = typeof(T).GetRequestAttributeFromType();
            if (requestClass == null || string.IsNullOrEmpty(requestClass.Path))
                return default;

            var response = await GenerateHttpClient(requestClass.Path, parameters).GetAsync(string.Empty);
            if (response.IsSuccessStatusCode) return await ParseResponse<T>(response);

            return default;
        }

        private async Task<T> Post<T>(object data = default, NameValueCollection parameters = default)
        {
            var requestClass = typeof(T).GetRequestAttributeFromType();
            if (requestClass == null || string.IsNullOrEmpty(requestClass.Path))
                return default;

            var response = await GenerateHttpClient(requestClass.Path, parameters).PostAsJsonNetAsync(string.Empty, data, _serializer);
            if (response.IsSuccessStatusCode) return await ParseResponse<T>(response);

            return default;
        }

        private async Task<T> ParseResponse<T>(HttpResponseMessage response)
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseJson))
            {
                DebugLog(response: response);
                return default;
            }

            DebugLog(response: response);

            try
            {
                var deserializedObject = JsonConvert.DeserializeObject<T>(responseJson, _serializer);
                return deserializedObject;
            }
            catch (Exception e)
            {
                DebugLog(e.Message);
                return default;
            }
        }

        private HttpClient GenerateHttpClient(string path, NameValueCollection parameters = default)
        {
            var uriBuilder = new UriBuilder(_host)
            {
                Path = _version switch
                {
                    PolrApiVersion.V2 => "/api/v2/",
                    _ => throw new ArgumentOutOfRangeException(nameof(_version), _version, null)
                }
            };

            uriBuilder.Path += path;

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("key", _apiKey);
            queryString.Add("response_type", "json");

            if (parameters != default)
                foreach (string parameter in parameters)
                    queryString[parameter] = parameters[parameter];

            uriBuilder.Query = queryString.ToString() ?? string.Empty;

            _host = uriBuilder.Uri;

            var httpClient = new HttpClient
            {
                BaseAddress = _host,
                Timeout = TimeSpan.FromSeconds(5)
            };

            httpClient.DefaultRequestHeaders.Clear();

            return httpClient;
        }

        private static async void DebugLog(object data = default, HttpResponseMessage response = default)
        {
            if (response != default)
            {
                var request = response.RequestMessage;
                Debug.WriteLine($"[{request?.Method}] [{response.StatusCode}] {request?.RequestUri}");

                if (request?.Content != default)
                {
                    var requestContent = await request.Content?.ReadAsStringAsync();
                    Debug.WriteLine($"[REQ] {requestContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"[RES] {responseContent}");
            }

            if (data != default)
                Debug.WriteLine($"[OBJ] {data}");
        }
    }
}