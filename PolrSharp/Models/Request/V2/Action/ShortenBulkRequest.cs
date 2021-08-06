// Copyright (c) 2021 Pwn (Jonathan) / SkyPBX, LLC. / All rights reserved.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PolrSharp.Models.Request.V2.Action
{
    public class ShortenBulkRequest
    {
        [JsonProperty("links")] public List<Link> Links { get; set; }
    }

    public class Link
    {
        [JsonProperty("url")] public Uri Url { get; set; }

        [JsonProperty("is_secret")] public bool IsSecret { get; set; }

        [JsonProperty("custom_ending")] public string CustomEnding { get; set; }
    }
}