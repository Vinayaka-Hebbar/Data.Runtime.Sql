using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace SqlDb.Data.Utils
{
    sealed class ExpressionVisitor
    {
        private readonly SqlTable _table;

        public ExpressionVisitor(SqlTable table)
        {
            _table = table;
        }


        internal string Get(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.AndAlso:
                    return GetBinaryLogical((BinaryExpression)expression, "and");
                case ExpressionType.OrAssign:
                    return GetBinaryLogical((BinaryExpression)expression, "or");
                case ExpressionType.Equal:
                    return GetBinary((BinaryExpression)expression, "=");
                case ExpressionType.NotEqual:
                    return GetBinary((BinaryExpression)expression, "!=");
                case ExpressionType.MemberAccess:
                    return GetMemberName((MemberExpression)expression);
                case ExpressionType.Constant:
                    return ((ConstantExpression)expression).Value.ToString();
            }
            throw new FormatException("filter format invalid");
        }

        private object GetValue(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return GetMemberValue((MemberExpression)expression);
                case ExpressionType.Constant:
                    return ((ConstantExpression)expression).Value;
            }
            throw new FormatException("filter format invalid");
        }

        private string GetMemberName(MemberExpression expression)
        {
            var member = expression.Member;
            var attr = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute));
            if (attr != null && attr.Name != null)
            {
                return string.Format("`{0}`", attr.Name);
            }
            return string.Format("`{0}`", member.Name);
        }

        private object GetMemberValue(MemberExpression memberAcess)
        {
            var expression = memberAcess.Expression;
            object instance = GetValue(expression);
            var member = memberAcess.Member;
            switch (member)
            {
                case System.Reflection.FieldInfo field:
                    return field.GetValue(instance);
                case System.Reflection.PropertyInfo property:
                    return property.GetValue(instance, new object[0]);
            }
            return string.Empty;
        }

        private string GetBinaryLogical(BinaryExpression binary, string relation)
        {
            var left = binary.Left;
            var right = binary.Right;
            return string.Concat(Get(left), " ", relation, " ", Get(right));
        }

        private string GetBinary(BinaryExpression binary, string operation)
        {
            var left = binary.Left;
            var right = binary.Right;
            return string.Concat(Get(left), " ", operation, " ", _table.GetValue(GetValue(right)));
        }
    }
}
