// Copyright (c) 2021 Jonathan 'Pwn' Rainier / SkyPBX, LLC. / All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;
using PolrSharp.Attributes;

namespace PolrSharp.Models.Response.V2.Action
{
    [PolrApiRequest("action/lookup")]
    public class Lookup
    {
        [JsonProperty("action")] public string Action { get; set; }

        [JsonProperty("result")] public Result Result { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("long_url")] public string LongUrl { get; set; }

        [JsonProperty("created_at")] public List<CreatedAt> CreatedAt { get; set; }

        [JsonProperty("clicks")] public string Clicks { get; set; }
    }

    public class CreatedAt
    {
        [JsonProperty("date")] public string Date { get; set; }

        [JsonProperty("timezone_type")] public long TimezoneType { get; set; }

        [JsonProperty("timezone")] public string Timezone { get; set; }
    }
}