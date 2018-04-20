using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESUFileExplorer
{
    public enum FilterType { FIND, SEARCH, NONE }

    public static class FilterInfo
    {
        public const string FIND_STRING = "find:";
        public const string SEARCH_STRING = "search:";

        public static T StringToEnum<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        public static string ToDescriptionString(this FilterType value)
        {
            switch (value)
            {
                case FilterType.NONE:
                default:
                    return "";
                case FilterType.FIND:
                    return FIND_STRING;
                case FilterType.SEARCH:
                    return SEARCH_STRING;
            }
        }
    }
}
