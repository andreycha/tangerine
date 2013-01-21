using System.ComponentModel;

namespace Tangerine.Common
{
    public enum ScreenResolution
    {
        [Description("Resolution - 480x800, Aspect Ratio - 15:9")]
        WVGA,
        [Description("Resolution - 768x1280, Aspect Ratio - 15:9")]
        WXGA,
        [Description("Resolution - 720x1280, Aspect Ratio - 16:9")]
        HD720P
    }
}
