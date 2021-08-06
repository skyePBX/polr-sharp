// Copyright (c) 2021 Jonathan 'Pwn' Rainier / SkyPBX, LLC. / All rights reserved.

using System;

namespace PolrSharp.Extensions
{
    public static class EnumExtension
    {
        public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0 ? (T) attributes[0] : null;
        }
    }
}