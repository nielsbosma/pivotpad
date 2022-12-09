namespace PivotPad;

public class PivotSettings
{
    public enum SortingOptions
    {
        Columns,
        Rows,
        On,
        Off
    }

    public SortingOptions Sorting { get; set; } = SortingOptions.On;
    
}