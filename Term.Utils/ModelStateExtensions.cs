using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Term.Utils
{
    /// <summary>
    /// Класс упрощающий работу с валидацией моделей списков 
    /// со списками
    /// </summary>
    public static class ModelStateExtensions
    {
            public static void AddModelError<TModel, TProperty>(
                this ModelStateDictionary modelState,
                Expression<Func<TModel, TProperty>> ex,
                string message
            )
            {
                var key = ExpressionHelper.GetExpressionText(ex);
                modelState.AddModelError(key, message);
            }


            public static void AddModelErrorIf<TModel, TProperty>(
                    this ModelStateDictionary modelState,
                    Expression<Func<TModel, TProperty>> ex,
                Func<TModel, bool> predicate,
                TModel model,
                    string message
                )
            {
                
                    if(  predicate(model))
                    {
                        var key = ExpressionHelper.GetExpressionText(ex);
                        modelState.AddModelError(key, message);
                    }
            }
    }
}
