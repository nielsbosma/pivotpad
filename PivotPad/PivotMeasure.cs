namespace PivotPad;

public class PivotMeasure
{
    public string UniqueName { get; set; }
    
    public string Caption { get; set; }
    
    public PivotAggregation Aggregation { get; set; }
    
    public string Formula { get; set; }
    
    public string GrandTotalCaption { get; set; }

    public bool Individual { get; set; } = false;

    public bool Active { get; set; } = true;
}