// Copyright (c) 2021 Jonathan 'Pwn' Rainier / SkyPBX, LLC. / All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolrSharp.Enums;
using PolrSharp.Models.Request.V2.Action;

namespace PolrSharp.Test
{
    [TestClass]
    public class PolrTest
    {
        private readonly Polr _client;
        private readonly IConfigurationRoot _configuration;
        private readonly string _testUrl;
        private readonly string _urlEnding;

        public PolrTest()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<PolrTest>();
            _configuration = builder.Build();
            _testUrl = _configuration["Tests:Url"];
            _urlEnding = _configuration["Tests:UrlEnding"];

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

            Assert.IsNotNull(_urlEnding);

            var response = await _client.Lookup(_urlEnding);
            Assert.IsNotNull(response);

            Debug.WriteLine($"[{_urlEnding}] {response.Result.LongUrl} // {response.Result.Clicks}");
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

        [TestMethod]
        public async Task GetDataLink()
        {
            Assert.IsNotNull(_client);
            Assert.IsNotNull(_testUrl);

            var now = DateTime.Now;
            var start = now.Subtract(TimeSpan.FromDays(10));

            var responseDay = await _client.Link(_urlEnding, start, now);
            Assert.IsNotNull(responseDay);
            var responseCountry = await _client.Link(_urlEnding, start, now, PolrDataLinkStatsType.Country);
            Assert.IsNotNull(responseCountry);
            var responseReferer = await _client.Link(_urlEnding, start, now, PolrDataLinkStatsType.Referer);
            Assert.IsNotNull(responseReferer);
        }
    }
}