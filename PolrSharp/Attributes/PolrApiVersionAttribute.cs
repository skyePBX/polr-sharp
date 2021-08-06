// Copyright (c) 2021 Pwn (Jonathan) / SkyPBX, LLC. / All rights reserved.

using System;

namespace PolrSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PolrApiVersionAttribute : Attribute
    {
        public PolrApiVersionAttribute(string basePath)
        {
            BasePath = basePath;
        }

        public string BasePath { get; }
    }
}