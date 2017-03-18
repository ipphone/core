using System;
using System.Linq.Expressions;

namespace ContactPoint.BaseDesign.Components
{
    public static class PropertyAccessor
    {
        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            return GetPropertyName(expression.Body);
        }

        public static string GetPropertyName<TItem, TResult>(Expression<Func<TItem, TResult>> expression)
        {
            return GetPropertyName(expression.Body);
        }

        private static string GetPropertyName(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;
                    var supername = GetPropertyName(memberExpression.Expression);

                    if (String.IsNullOrEmpty(supername))
                        return memberExpression.Member.Name;

                    return String.Concat(supername, '.', memberExpression.Member.Name);

                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    return callExpression.Method.Name;

                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    return GetPropertyName(unaryExpression.Operand);

                case ExpressionType.Parameter:
                    return String.Empty;

                default:
                    throw new ArgumentException("The expression is not a member access or method call expression");
            }
        }
    }

    public static class PropertyAccessorExtensions
    {
        public static string GetPropertyName<T>(this T instance, Expression<Func<T, object>> propertyAccesor)
        {
            return PropertyAccessor.GetPropertyName(propertyAccesor);
        }
    }
}
