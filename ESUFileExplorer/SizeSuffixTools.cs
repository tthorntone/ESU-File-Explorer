using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESUFileExplorer
{
    class SizeSuffixTools
    {
    private static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
    public static string SizeSuffix(Int64 value)
    {
        if (value < 0) { return "-" + SizeSuffix(-value); }
        if (value == 0) { return "0.0 bytes"; }

        int mag = (int)Math.Log(value, 1024);
        decimal adjustedSize = (decimal)value / (1L << (mag * 10));

        return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
    }
    }
}
