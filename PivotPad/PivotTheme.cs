using System.ComponentModel;

namespace PivotPad;

public enum PivotTheme
{
    [Description("https://cdn.webdatarocks.com/latest/theme/dark/webdatarocks.min.css")]
    Dark,
    
    [Description("https://cdn.webdatarocks.com/latest/webdatarocks.min.css")]
    Default,
    
    [Description("https://cdn.webdatarocks.com/latest/theme/lightblue/webdatarocks.min.css")]
    LightBlue,
    
    [Description("https://cdn.webdatarocks.com/latest/theme/orange/webdatarocks.min.css")]
    Orange,
    
    [Description("https://cdn.webdatarocks.com/latest/theme/teal/webdatarocks.min.css")]
    Teal,
    
    [Description("https://cdn.webdatarocks.com/latest/theme/green/webdatarocks.min.css")]
    Green,
    
    [Description("https://cdn.webdatarocks.com/latest/theme/stripedblue/webdatarocks.min.css")]
    StripedBlue,
    
    [Description("https://cdn.webdatarocks.com/latest/theme/stripedteal/webdatarocks.min.css")]
    StripedTeal
}