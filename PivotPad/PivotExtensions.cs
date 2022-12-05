using System;
using System.Collections.Generic;

namespace PivotPad;

public static class PivotExtensions
{
    public static void Pivot<T>(this IEnumerable<T> data, Action<PivotBuilder<T>> build = null)
    {
        var builder = new PivotBuilder<T>();
	
        if(build != null)
        {
            build(builder);
        }
		
        builder.Build(data);
    }
}