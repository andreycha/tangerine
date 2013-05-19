using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tangerine.Common
{
    public static class Util
    {
        /// <summary>
        /// Returns enum member description from DescriptionAttribute.
        /// If it's not defined, returns string representation.
        /// </summary>
        public static string GetEnumDescription(Enum member)
        {
            FieldInfo fi = member.GetType().GetField(member.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return member.ToString();
            }
        }
    }
}
