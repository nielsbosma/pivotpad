using System;
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
}