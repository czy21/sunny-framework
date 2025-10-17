using System.Linq.Expressions;

namespace Sunny.Framework.Core.Util;

public static class ReflectionUtil
{
    public static string GetPropertyName<T, TProp>(Expression<Func<T, TProp>> expression)
    {
        if (expression.Body is MemberExpression member)
            return member.Member.Name;

        if (expression.Body is UnaryExpression unary && unary.Operand is MemberExpression unaryMember)
            return unaryMember.Member.Name;

        throw new ArgumentException("Expression is not a member access", nameof(expression));
    }
}