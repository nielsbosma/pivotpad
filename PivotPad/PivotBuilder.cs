using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using LINQPad;
using LINQPad.FSharpExtensions;

namespace PivotPad;

public class PivotBuilder<T>
{
    private readonly PivotReport _report;

    private readonly IPivotRenderer _renderer = new WebDataRocksRenderer();

    public PivotBuilder()
    {
        _report = new PivotReport();
    }

    public PivotBuilder<T> Theme(PivotTheme theme)
    {
        _report.Theme = theme;
        return this;
    }

    public PivotBuilder<T> DateRow(Expression<Func<T, object>> field, PivotDatePart datePart = PivotDatePart.Year,
        PivotSort sort = PivotSort.Unsorted,
        string caption = null)
    {
        if ((datePart & PivotDatePart.Year) == PivotDatePart.Year)
        {
            Row(field, sort, caption ?? "Year", Utils.Capitalize(PivotDatePart.Year.ToString()));
        }

        if ((datePart & PivotDatePart.Month) == PivotDatePart.Month)
        {
            Row(field, sort, caption ?? "Month", Utils.Capitalize(PivotDatePart.Month.ToString()));
        }

        if ((datePart & PivotDatePart.Day) == PivotDatePart.Day)
        {
            Row(field, sort, caption ?? "Day", Utils.Capitalize(PivotDatePart.Day.ToString()));
        }

        return this;
    }

    public PivotBuilder<T> Row(Expression<Func<T, object>> field, PivotSort sort = PivotSort.Unsorted,
        string caption = null, string segment = null)
    {
        var name = Utils.GetNameFromMemberExpression(field.Body);
        _report.Rows.Add(new PivotField()
        {
            UniqueName = name,
            Caption = caption,
            Sort = sort,
            Segment = segment
        });
        return this;
    }

    public PivotBuilder<T> Rows(params Expression<Func<T, object>>[] fields)
    {
        foreach (var fieldExpr in fields)
        {
            Row(fieldExpr);
        }

        return this;
    }

    public PivotBuilder<T> DateColumn(Expression<Func<T, object>> field, PivotDatePart datePart = PivotDatePart.Day,
        PivotSort sort = PivotSort.Unsorted,
        string caption = null)
    {
        if ((datePart & PivotDatePart.Year) == PivotDatePart.Year)
        {
            Column(field, sort, caption ?? "Year", Utils.Capitalize(PivotDatePart.Year.ToString()));
        }

        if ((datePart & PivotDatePart.Month) == PivotDatePart.Month)
        {
            Column(field, sort, caption ?? "Month", Utils.Capitalize(PivotDatePart.Month.ToString()));
        }

        if ((datePart & PivotDatePart.Day) == PivotDatePart.Day)
        {
            Column(field, sort, caption ?? "Day", Utils.Capitalize(PivotDatePart.Day.ToString()));
        }

        return this;
    }

    public PivotBuilder<T> Column(Expression<Func<T, object>> field, PivotSort sort = PivotSort.Unsorted,
        string caption = null, string segment = null)
    {
        var name = Utils.GetNameFromMemberExpression(field.Body);
        _report.Columns.Add(new PivotField()
        {
            UniqueName = name,
            Caption = caption,
            Sort = sort,
            Segment = segment
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
            Measure(fieldExpr);
        }

        return this;
    }
    
    public PivotBuilder<T> Measure(Expression<Func<T, object>> field, PivotAggregation aggregation = PivotAggregation.Sum,
        string caption = null, string formula = null, string grandTotalCaption = null, bool active = true, bool individual = false)
    {
        if (!string.IsNullOrEmpty(formula))
        {
            aggregation = PivotAggregation.Formula;
        }
        
        var name = Utils.GetNameFromMemberExpression(field.Body);
        _report.Measures.Add(new PivotMeasure()
        {
            UniqueName = name,
            Aggregation = aggregation,
            Caption = caption,
            GrandTotalCaption = grandTotalCaption,
            Active = active,
            Individual = individual,
            Formula = formula
        });
        return this;
    }

    public void Build(IEnumerable<T> data)
    {
        if (!_report.Columns.Any() && _report.Measures.Any())
        {
            _report.Columns.Add(new PivotField()
            {
                UniqueName = "Measures"
            });
        }

        var csvHost = "http://localhost:8081/";

        var tmpHtmlPath = _renderer.Render(_report, csvHost);

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

        new CsvServer(csvHost).Serve(Util.ToCsvString(data)).GetAwaiter().GetResult();
    }
}