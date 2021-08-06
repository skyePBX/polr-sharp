// Copyright (c) 2021 Jonathan 'Pwn' Rainier / SkyPBX, LLC. / All rights reserved.

using Newtonsoft.Json;
using PolrSharp.Attributes;

namespace PolrSharp.Models.Response.V2.Action
{
    [PolrApiRequest("action/shorten")]
    public class Shorten
    {
        [JsonProperty("action")] public string Action { get; set; }

        [JsonProperty("result")] public string Result { get; set; }
    }
}