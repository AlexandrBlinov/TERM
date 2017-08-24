using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace Term.Web.Services
{

    internal class SubstExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
    {
        public Dictionary<Expression, Expression> subst = new Dictionary<Expression, Expression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Expression newValue;
            if (subst.TryGetValue(node, out newValue))
            {
                return newValue;
            }
            return node;
        }
    }

    /// <summary>
    /// Class used to join delegates via And / Or conditions
    /// http://stackoverflow.com/questions/457316/combining-two-expressions-expressionfunct-bool
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// Testing predicate if TEntity.propertyName!=null
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>

        public static Expression<Func<TEntity, bool>> GetFilterNotNull<TEntity>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            Type t = typeof(TEntity).GetProperty(propertyName).PropertyType;
            var property = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant;
            Expression predicate;


            constant = Expression.Constant(null);
            var toString = Expression.Call(property, "ToString", null);

            predicate = Expression.Call(toString, "Equals", null, constant);





            return Expression.Lambda<Func<TEntity, bool>>(predicate, parameter);

        }

        public static Expression<Func<TEntity, bool>> GetFilter<TEntity>(string data, string propertyName)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            Type t = typeof(TEntity).GetProperty(propertyName).PropertyType;
            var property = Expression.PropertyOrField(parameter, propertyName);
            ConstantExpression constant;
            Expression predicate;

            if (t == typeof(int))
            {
                int temp;
                bool check = int.TryParse(data, out temp);

                if (!check)
                {
                    return ((TEntity x) => false);
                }

                constant = Expression.Constant(temp);


                predicate = Expression.Call(property, "Equals", null, constant);
            }
            else
            {
                constant = Expression.Constant(data);
                var toString = Expression.Call(property, "ToString", null);

                predicate = Expression.Call(toString, "Contains", null, constant);



            }


            return Expression.Lambda<Func<TEntity, bool>>(predicate, parameter);

        }

        public static Expression<Func<T, string>> MakeGetter<T>(string propertyName)
        {
            ParameterExpression input = Expression.Parameter(typeof(T));

            var expr = Expression.Property(input, typeof(T).GetProperty(propertyName));

            return Expression.Lambda<Func<T, string>>(expr, input);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {

            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[b.Parameters[0]] = p;

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {

            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[b.Parameters[0]] = p;

            Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
    }
}