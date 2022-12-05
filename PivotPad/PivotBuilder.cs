using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using LINQPad;
using LINQPad.FSharpExtensions;

namespace PivotPad;

public class PivotBuilder<T>
{
	private PivotReport _report;
	
	public PivotBuilder()
	{
		_report = new PivotReport();
	}

	public PivotBuilder<T> Row(Expression<Func<T, object>> field, PivotSort sort = PivotSort.Unsorted, string caption = null)
	{
		var name = Utils.GetNameFromMemberExpression(field.Body);
		_report.Rows.Add(new PivotField()
		{
			UniqueName = name,
			Caption = caption,
			Sort = sort
		});
		return this;
	}

	public PivotBuilder<T> Rows(params Expression<Func<T, object>>[] fields)
	{
		foreach(var fieldExpr in fields)
		{
			Row(fieldExpr);
		}
		return this;
	}

	public PivotBuilder<T> Column(Expression<Func<T, object>> field, PivotSort sort = PivotSort.Unsorted, string caption = null)
	{
		var name = Utils.GetNameFromMemberExpression(field.Body);
		_report.Columns.Add(new PivotField()
		{
			UniqueName = name,
			Caption = caption,
			Sort = sort
		});
		return this;
	}

	public PivotBuilder<T> Columns(params Expression<Func<T, object>>[] fields)
	{
		foreach (var fieldExpr in fields)
		{
			Column(fieldExpr);
		}
		return this;
	}

	public PivotBuilder<T> Filters(params Expression<Func<T, object>>[] fields)
	{
		foreach (var fieldExpr in fields)
		{
			var name = Utils.GetNameFromMemberExpression(fieldExpr.Body);
			_report.Filters.Add(new PivotField()
			{
				UniqueName = name
			});
		}
		return this;
	}

	public PivotBuilder<T> Measures(params Expression<Func<T, object>>[] fields)
	{
		foreach (var fieldExpr in fields)
		{
			var name = Utils.GetNameFromMemberExpression(fieldExpr.Body);
			_report.Measures.Add(new PivotField()
			{
				UniqueName = name
			});
		}
		return this;
	}

	public void Build(IEnumerable<T> data)
	{
		if(!_report.Columns.Any() && _report.Measures.Any())
		{
			_report.Columns.Add(new PivotField() {
				UniqueName = "Measures"
			});
		}
		
		var tmpHtmlPath = Path.GetTempFileName() + ".html";

		var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
    <title></title>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
	<!--<link rel='stylesheet' type='text/css' href='https://cdn.webdatarocks.com/latest/theme/stripedblue/webdatarocks.min.css'/>-->
	<link href='https://cdn.webdatarocks.com/latest/webdatarocks.min.css' rel='stylesheet'/>
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
            filename: 'http://localhost:8001/data.csv'
        }},
		slice: {{
			reportFilters: {ToJson(_report.Filters)},
		    rows: {ToJson(_report.Rows)},
		    columns: {ToJson(_report.Columns)},
		    measures: {ToJson(_report.Measures)}
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
        //delete tabs[1];
        //delete tabs[2];
        return tabs;
    }}
}}
</script>
</body>
</html>		
";
		File.WriteAllText(tmpHtmlPath, htmlBody);

		//Process.Start("code.exe", tmpHtmlPath);

		Util.ClearResults();
		Util.RawHtml(@$"
<style>
body {{
margin:0px !important;
overflow:hidden;
}}
</style>
<iframe style='width:100vw;height:100vh;overflow-x:hidden' src='{tmpHtmlPath}'></iframe>
		").Dump();
		
		new CsvServer().Serve(Util.ToCsvString(data)).GetAwaiter().GetResult();
	}
	
	private JsonArray ToJson(IEnumerable<PivotField> fields)
	{
		return new JsonArray(fields.Select(e => e.ToJson()).ToArray());
	}

	

	
}