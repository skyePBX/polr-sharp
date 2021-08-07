// Copyright (c) 2021 Jonathan 'Pwn' Rainier / SkyPBX, LLC. / All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolrSharp.Models.Request.V2.Action;

namespace PolrSharp.Test
{
    [TestClass]
    public class PolrTest
    {
        private readonly Polr _client;
        private readonly IConfigurationRoot _configuration;
        private readonly string _testUrl;

        public PolrTest()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<PolrTest>();
            _configuration = builder.Build();
            _testUrl = _configuration["Tests:Url"];

            var endPoint = _configuration["Polr:EndPoint"];
            var apiKey = _configuration["Polr:ApiKey"];

            var createEndPointUrl = Uri.TryCreate(endPoint, UriKind.Absolute, out var endPointUrl);
            Assert.IsTrue(createEndPointUrl);

            _client = new Polr(endPointUrl, apiKey);
        }

        [TestMethod]
        public async Task GetLookup()
        {
            Assert.IsNotNull(_client);

            var urlEnding = _configuration["Tests:UrlEnding"];
            Assert.IsNotNull(urlEnding);

            var response = await _client.Lookup(urlEnding);
            Assert.IsNotNull(response);

            Debug.WriteLine($"[{urlEnding}] {response.Result.LongUrl} // {response.Result.Clicks}");
        }

        [TestMethod]
        public async Task GetShorten()
        {
            Assert.IsNotNull(_client);
            Assert.IsNotNull(_testUrl);

            var url = $"{_testUrl}#polr_sharp={DateTime.Now:O}";

            var responseNoSecret = await _client.Shorten(url);
            Assert.IsNotNull(responseNoSecret);

            var responseSecret = await _client.Shorten(url, true);
            Assert.IsNotNull(responseSecret);
            Assert.IsTrue(responseNoSecret.Result != responseSecret.Result);
        }

        [TestMethod]
        public async Task PostShortenBulk()
        {
            Assert.IsNotNull(_client);
            Assert.IsNotNull(_testUrl);

            var urlsToShorten = new List<Link>();
            for (var i = 0; i < 3; i++)
                urlsToShorten.Add(new Link
                {
                    Url = new Uri($"{_testUrl}#polr_sharp={DateTime.Now:O}-{i}")
                });

            var shortenBulkRequest = new ShortenBulkRequest {Links = urlsToShorten};

            var response = await _client.ShortenBulk(shortenBulkRequest);
            Assert.IsNotNull(response);
        }
    }
}