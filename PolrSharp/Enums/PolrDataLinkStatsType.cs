// Copyright (c) 2021 Jonathan 'Pwn' Rainier / SkyPBX, LLC. / All rights reserved.

using System.ComponentModel;

namespace PolrSharp.Enums
{
    public enum PolrDataLinkStatsType
    {
        [Description("day")] Day,
        [Description("country")] Country,
        [Description("referer")] Referer
    }
}