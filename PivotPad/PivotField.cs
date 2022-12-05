using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PivotPad;

public class PivotField
{
    public string UniqueName { get; set; }
	
    public string Caption { get; set; }
	
    public PivotSort Sort { get; set; } = PivotSort.Unsorted;
	
    //Filter?

    public JsonObject ToJson()
    {
        return new JsonObject(new[] {
            KeyValuePair.Create<string, JsonNode>("uniqueName", UniqueName),
            KeyValuePair.Create<string, JsonNode>("caption", Caption??UniqueName),
            KeyValuePair.Create<string, JsonNode>("sort", Sort.ToString().ToLower()),
        });
    }
}