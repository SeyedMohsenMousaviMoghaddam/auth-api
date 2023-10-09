using Service.Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Infrastructure.Util
{
    public static class QueryableExtention
    {
        public static IPagedEntities<T> WithPaging<T>(this IQueryable<T> query,
                                                      string? searchValue,
                                                      int? page = 1,
                                                      int? rowsPerPage = 20,
                                                      string? sortBy = null,
                                                      bool descending = true) where T : class
        {
            var desc = descending ? "DESC" : "ASC";
            page = page ?? 1;
            rowsPerPage = rowsPerPage ?? 20;

            var filteredQuery = !string.IsNullOrEmpty(searchValue) ? SearchPredicate<T>(query, searchValue) : query;
            var orderedQuery = !string.IsNullOrEmpty(sortBy) ? filteredQuery.OrderBy($"{sortBy} {desc}") : filteredQuery.OrderBy($"Id {desc}");
            var rowsNumber = orderedQuery.Count();
            var entities = orderedQuery.Skip((page!.Value - 1) * rowsPerPage!.Value).Take(rowsPerPage.Value);
            var result = new PagedEntities<T>()
            {
                Data = entities,
                Pagination = new Pagination()
                {
                    Descending = descending,
                    Page = page.Value,
                    RowsNumber = rowsNumber,
                    SortBy = sortBy,
                    RowsPerPage = rowsPerPage.Value
                },
            };

            return result;
        }

        public static IPagedEntities<T, TotalSchema?> WithPaging<T, TotalSchema>(this IQueryable<T> query,
                                                                                string? searchValue,
                                                                                int? page = 1,
                                                                                int? rowsPerPage = 20,
                                                                                string? sortBy = null,
                                                                                bool descending = true) where T : class
        {
            var desc = descending ? "DESC" : "ASC";
            page = page ?? 1;
            rowsPerPage = rowsPerPage ?? 20;

            var filteredQuery = !string.IsNullOrEmpty(searchValue) ? SearchPredicate<T>(query, searchValue) : query;
            var orderedQuery = !string.IsNullOrEmpty(sortBy) ? filteredQuery.OrderBy($"{sortBy} {desc}") : filteredQuery.OrderBy($"Id {desc}");
            var rowsNumber = orderedQuery.Count();
            var aggregationStr = typeof(TotalSchema).GetProperties().Aggregate("", (current, prop) => current + $"Sum({prop.Name}) as {prop.Name},");
            var totals = orderedQuery.GroupBy("1").Select<TotalSchema>($"new({aggregationStr.Remove(aggregationStr.Length - 1)})").FirstOrDefault();
            var entities = orderedQuery.Skip((page!.Value - 1) * rowsPerPage!.Value).Take(rowsPerPage.Value);
            var result = new PagedEntities<T, TotalSchema?>()
            {
                Data = entities,
                Total = totals,
                Pagination = new Pagination()
                {
                    Descending = descending,
                    Page = page.Value,
                    RowsNumber = rowsNumber,
                    SortBy = sortBy,
                    RowsPerPage = rowsPerPage.Value
                },
            };

            return result;
        }

        private static Expression<Func<T, string>> CreateSelectorExpression<T>(string propertyName)
        {
            var paramterExpression = Expression.Parameter(typeof(T));
            return (Expression<Func<T, string>>)Expression.Lambda(
                Expression.PropertyOrField(paramterExpression, propertyName),
                paramterExpression);
        }

        private static IQueryable<T> OrderBy<T>(this IQueryable<T> source, Expression<Func<T, string>> keySelector, bool descending)
        {
            var selectorBody = keySelector.Body;
            if (selectorBody.NodeType == ExpressionType.Convert)
                selectorBody = ((UnaryExpression)selectorBody).Operand;
            var selector = Expression.Lambda(selectorBody, keySelector.Parameters);
            var queryBody = Expression.Call(typeof(Queryable),
                descending ? "OrderByDescending" : "OrderBy",
                new Type[] { typeof(T), selectorBody.Type },
                source.Expression, Expression.Quote(selector));
            return source.Provider.CreateQuery<T>(queryBody);
        }

        public static IQueryable<T> MustBeInTenant2_Branch<T>(this IQueryable<T> source, long tenantId, long branchId)
        {
            return source.Where("TenantId2== @0 && BranchId==@1", tenantId, branchId);
        }
        public static IQueryable<T> MayBeInTenant<T>(this IQueryable<T> source, Guid? tenantId)
        {
            return source.Where("TenantId==null || TenantId== @0", tenantId);
        }
        public static IQueryable<T> MayBeInTenant2<T>(this IQueryable<T> source, long? tenantId)
        {
            return source.Where("TenantId2==null || TenantId2== @0", tenantId);
        }
        public static IQueryable<T> ExcludeSoftDelete<T>(this IQueryable<T> source)
        {
            var xType = typeof(T);
            var deleteColumn = xType.GetProperties().FirstOrDefault(p => p.Name == "IsDeleted");
            return deleteColumn != null ? source.Where("IsDeleted !=true") : source;
        }

        public static IQueryable<T> IncludeActive<T>(this IQueryable<T> source)
        {
            var xType = typeof(T);
            var isActiveColumn = xType.GetProperties().FirstOrDefault(p => p.Name == "IsActive");
            return isActiveColumn != null ? source.Where("IsActive ==true") : source;
        }

        private static IQueryable<T> SearchPredicate<T>(IQueryable<T> source, string searchValue)
        {
            try
            {
                var columns = source.GetType().GenericTypeArguments[0]
                                    .GetProperties()
                                    .Where(p => p.CustomAttributes.Any(c => c.AttributeType == typeof(SearchableAttribute)) &&
                                            (p.PropertyType == typeof(byte?) ||
                                            p.PropertyType == typeof(int?) ||
                                            p.PropertyType == typeof(long?) ||
                                            p.PropertyType == typeof(decimal?) ||
                                            p.PropertyType == typeof(float?) ||
                                            p.PropertyType == typeof(byte) ||
                                            p.PropertyType == typeof(int) ||
                                            p.PropertyType == typeof(long) ||
                                            p.PropertyType == typeof(decimal) ||
                                            p.PropertyType == typeof(float) ||
                                            p.PropertyType == typeof(string)))
                                    .Select(x => new { x.Name, Type = x.PropertyType })
                                    .ToArray();

                var queries = new string[columns.Length];

                for (int i = 0; i < columns.Length; i++)
                {
                    if (columns[i].Type == typeof(string))
                        queries[i] = $"{columns[i].Name}.Contains(\"{searchValue}\")";
                    else
                        queries[i] = $"{columns[i].Name}.ToString().Contains(\"{searchValue}\")";
                }
                string expression = string.Join(" || ", queries);
                if (expression.Length > 0)
                    return source.Where(expression);
                else
                    return source;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException is not null ? ex.InnerException.Message : ex.Message);
                throw;
            }
        }

    }
}
