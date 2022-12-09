using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace PivotPad;

public class Utils
{
    public static string GetNameFromMemberExpression(Expression expression)
    {
        while (true)
        {
            switch (expression)
            {
                case MemberExpression memberExpression:
                    return memberExpression.Member.Name;
                case UnaryExpression unaryExpression:
                    expression = unaryExpression.Operand;
                    continue;
            }
            throw new ArgumentException("Invalid expression.");
        }
    }

    public static string GetDescriptionFromEnum(Enum @enum)
    {
        return @enum.GetType()
            .GetField(@enum.ToString())
            ?.GetCustomAttributes(false)
            .OfType<DescriptionAttribute>()
            .FirstOrDefault()
            ?.Description ?? @enum.ToString();
    }
    
    public static string Capitalize(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        return char.ToUpper(s[0]) + s[1..].ToLower();
    }
    
}