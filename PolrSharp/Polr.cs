// Copyright (c) 2021 Pwn (Jonathan) / SkyPBX, LLC. / All rights reserved.

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using PolrSharp.Enums;
using PolrSharp.Extensions;
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
                DefaultValueHandling = DefaultValueHandling.Include
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
                {"is_secret", isSecret.ToString()}
            };

            if (customEnding != default)
                parameters.Add("custom_ending", customEnding);

            return await Get<Shorten>(parameters);
        }

        private async Task<T> Get<T>(NameValueCollection parameters = default)
        {
            var requestClass = typeof(T).GetRequestAttributeFromType();
            if (requestClass == null || string.IsNullOrEmpty(requestClass.Path))
                return default;

            var response = await GenerateHttpClient(requestClass.Path, parameters).GetAsync(string.Empty);
            if (!response.IsSuccessStatusCode)
            {
                DebugLog(response: response);
                return default;
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseJson))
            {
                DebugLog(response: response);
                return default;
            }

            try
            {
                var deserializedObject = JsonConvert.DeserializeObject<T>(responseJson, _serializer);
                return deserializedObject;
            }
            catch (Exception e)
            {
                DebugLog($"\"{responseJson}\" // {e.Message}");
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

        private static void DebugLog(object data = default, HttpResponseMessage response = default)
        {
            var message = string.Empty;
            if (response != default)
            {
                var request = response.RequestMessage;
                message = $" [{request?.Method}] [{response.StatusCode}] {request?.RequestUri}";

                if (request?.Content != default)
                    message += $" // {response.RequestMessage?.Content}";
            }

            if (data != default)
                message += $" // {data}";

            Debug.WriteLine(message);
        }
    }
}