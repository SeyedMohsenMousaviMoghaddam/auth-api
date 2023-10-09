using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Application.Common;
public static class FluentValidationExtensions
{
    /// <summary>
    /// لیست خطاهای دریافتی را به 
    /// Dictionary
    /// تبدیل میکند و خطاهای تکراری را حذف میکند.
    /// </summary>
    /// <param name="errors">لیست خطاها</param>
    /// <param name="distinct">خطاهای تکراری حذف شود یا خیر</param>
    public static Dictionary<string, string[]> AsDictionary(this IEnumerable<ValidationFailure> errors, bool distinct = true)
    {
        var errorsAsDictionary = errors.GroupBy(
                x => x.PropertyName.ToCamelCase(),
                x => x.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = distinct ? errorMessages.Distinct().ToArray() : errorMessages.ToArray(),
                })
            .ToDictionary(x => x.Key, x => x.Values);

        return errorsAsDictionary;
    }
}