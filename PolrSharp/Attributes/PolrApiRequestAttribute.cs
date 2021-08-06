// Copyright (c) 2021 Jonathan 'Pwn' Rainier / SkyPBX, LLC. / All rights reserved.

using System;

namespace PolrSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PolrApiRequestAttribute : Attribute
    {
        public PolrApiRequestAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}