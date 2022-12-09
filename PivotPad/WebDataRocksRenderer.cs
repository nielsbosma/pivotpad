using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;

namespace PivotPad;

public class WebDataRocksRenderer : IPivotRenderer
{
    public WebDataRocksRenderer()
    {
    }

    public string Render(PivotReport report, string csvHost)
    {
        var tmpHtmlPath = Path.GetTempFileName() + ".html";

        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
    <title></title>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
	<link href='{Utils.GetDescriptionFromEnum(report.Theme)}' rel='stylesheet'/>
    <script src='https://cdn.webdatarocks.com/latest/webdatarocks.toolbar.min.js'></script>
    <script src='https://cdn.webdatarocks.com/latest/webdatarocks.js'></script>
    <style>
        html {{
            margin: 0;
        }}
    </style>
</head>
<body>
<div style='height:100vh; width:100%;'>
    <div id='wdr-component'></div>
</div>
<script>
var pivot = new WebDataRocks({{
    container: '#wdr-component',
    toolbar: true,
    height: '100%',
    beforetoolbarcreated: customizeToolbar,
    report: {{
		dataSourceType: 'csv',
        dataSource: {{
            filename: '{csvHost}data.csv'
        }},
		slice: {{
			reportFilters: {ToJson(report.Filters)},
		    rows: {ToJson(report.Rows)},
		    columns: {ToJson(report.Columns)},
		    measures: {ToJson(report.Measures)}
		}},
		'formats': [
        {{
            name: '',
            decimalPlaces: 0,
            //currencySymbol: ' kr',
			//currencySymbolAlign: 'right'
        }}
    ],
    }}
}});
function customizeToolbar(toolbar) {{
    var tabs = toolbar.getTabs(); 
    toolbar.getTabs = function() {{
        delete tabs[0]; 
        tabs.push({{
            id: 'wdr-tab-feedback',
            title: 'Github',
            handler: handlerFeedback,
            icon: this.icons.options
        }});
        return tabs;
    }}
}}
var handlerFeedback = function() {{
    window.open('https://bit.ly/pivotpad');
}};

</script>
</body>
</html>		
";
        File.WriteAllText(tmpHtmlPath, htmlBody);

        return tmpHtmlPath;
    }
    
    private JsonArray ToJson(IEnumerable<PivotField> fields)
    {
        return new JsonArray(fields.Select(ToJson).ToArray());
    }
    
    private JsonObject ToJson(PivotField field)
    {
        return new JsonObject(new[] {
            KeyValuePair.Create<string, JsonNode>("uniqueName", field.UniqueName + (field.Segment != null ? "." + field.Segment : "")),
            KeyValuePair.Create<string, JsonNode>("caption", field.Caption??field.UniqueName),
            KeyValuePair.Create<string, JsonNode>("sort", field.Sort.ToString().ToLower()),
        });
    }
    
    private JsonArray ToJson(IEnumerable<PivotMeasure> fields)
    {
        return new JsonArray(fields.Select(ToJson).ToArray());
    }
    
    private JsonObject ToJson(PivotMeasure field)
    {
        var pairs = new List<KeyValuePair<string, JsonNode>>(new[]
        {
            KeyValuePair.Create<string, JsonNode>("uniqueName", field.UniqueName),
            KeyValuePair.Create<string, JsonNode>("caption", field.Caption ?? field.UniqueName),
            KeyValuePair.Create<string, JsonNode>("aggregation", Utils.GetDescriptionFromEnum(field.Aggregation)),
            KeyValuePair.Create<string, JsonNode>("active", field.Active),
            KeyValuePair.Create<string, JsonNode>("individual", field.Individual)
        });

        if (!string.IsNullOrEmpty(field.GrandTotalCaption))
        {
            pairs.Add(KeyValuePair.Create<string, JsonNode>("grandTotalCaption", field.GrandTotalCaption));
        }
        
        if (!string.IsNullOrEmpty(field.Formula))
        {
            pairs.Add(KeyValuePair.Create<string, JsonNode>("formula", field.Formula));
        }
        
        return new JsonObject(pairs);
    }
}