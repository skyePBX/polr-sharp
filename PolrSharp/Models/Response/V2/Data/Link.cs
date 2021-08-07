// Copyright (c) 2021 Jonathan 'Pwn' Rainier / SkyPBX, LLC. / All rights reserved.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PolrSharp.Attributes;

namespace PolrSharp.Models.Response.V2.Data
{
    [PolrApiRequest("data/link")]
    public class Link
    {
        [JsonProperty("action")] public string Action { get; set; }

        [JsonProperty("result")] public Result Result { get; set; }
    }

    public class Result
    {
        [JsonProperty("url_ending")] public string UrlEnding { get; set; }

        [JsonProperty("data")] public List<Datum> Data { get; set; }
    }

    public class Datum
    {
        [JsonProperty("x")]
        private DateTime X
        {
            set => Date = value;
        }

        [JsonProperty("date")] public DateTime Date { get; set; }

        [JsonProperty("label")] public string Label { get; set; }

        [JsonProperty("clicks")] public long Clicks { get; set; }

        [JsonProperty("y")]
        private long ClicksX
        {
            set => Clicks = value;
        }
    }
}