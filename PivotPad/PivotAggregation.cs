using System.ComponentModel;

namespace PivotPad;

public enum PivotAggregation
{
    Sum,
    Count,
    DistinctCount,
    Average,
    Max,
    Min,
    Percent,
    PercentOfRow,
    PercentOfColumn,
    Index,
    Difference,
    [Description("difference")]
    PercentDifference,
    [Description("none")]
    Formula
}