// Copyright (c) 2021 Pwn (Jonathan) / SkyPBX, LLC. / All rights reserved.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolrSharp.Test
{
    [TestClass]
    public class PolrTest
    {
        private readonly Polr _client;
        private readonly IConfigurationRoot _configuration;

        public PolrTest()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<PolrTest>();
            _configuration = builder.Build();

            var endPoint = _configuration["Polr:EndPoint"];
            var apiKey = _configuration["Polr:ApiKey"];

            var createEndPointUrl = Uri.TryCreate(endPoint, UriKind.Absolute, out var endPointUrl);
            Assert.IsTrue(createEndPointUrl);

            _client = new Polr(endPointUrl, apiKey);
        }

        [TestMethod]
        public async Task GetLookup()
        {
            var urlEnding = _configuration["Tests:UrlEnding"];
            Assert.IsNotNull(urlEnding);
            Assert.IsNotNull(_client);

            var response = await _client.Lookup(urlEnding);
            Assert.IsNotNull(response);

            Debug.WriteLine($"[{urlEnding}] {response.Result.LongUrl} // {response.Result.Clicks}");
        }
    }
}