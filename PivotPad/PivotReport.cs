using System.Collections.Generic;

namespace PivotPad;

public class PivotReport
{
	public List<PivotField> Rows { get; } = new();
	
    public List<PivotField> Columns { get; } = new();
	
    public List<PivotField> Measures { get; } = new();
	
    public List<PivotField> Filters { get; } = new();
    
    public PivotTheme Theme { get; set; } = PivotTheme.Default;
}