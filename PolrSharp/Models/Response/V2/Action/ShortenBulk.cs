// Copyright (c) 2021 Pwn (Jonathan) / SkyPBX, LLC. / All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;
using PolrSharp.Attributes;

namespace PolrSharp.Models.Response.V2.Action
{
    [PolrApiRequest("shorten_bulk")]
    public class ShortenBulk
    {
        [JsonProperty("action")] public string Action { get; set; }

        [JsonProperty("result")] public Result Result { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("shortened_links")] public List<ShortenedLink> ShortenedLinks { get; set; }
    }

    public class ShortenedLink
    {
        [JsonProperty("long_url")] public string LongUrl { get; set; }

        [JsonProperty("short_url")] public string ShortUrl { get; set; }
    }
}