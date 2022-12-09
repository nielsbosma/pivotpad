using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PivotPad;

public class PivotField
{
    public string UniqueName { get; set; }
	
    public string Caption { get; set; }
	
    public PivotSort Sort { get; set; } = PivotSort.Unsorted;
}