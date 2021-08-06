// Copyright (c) 2021 Pwn (Jonathan) / SkyPBX, LLC. / All rights reserved.

using System;
using System.Reflection;
using PolrSharp.Attributes;

namespace PolrSharp.Extensions
{
    public static class TypeExtension
    {
        public static PolrApiRequestAttribute GetRequestAttributeFromType(this Type type)
        {
            return type.GetCustomAttribute<PolrApiRequestAttribute>();
        }
    }
}