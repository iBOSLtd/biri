using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.EntityFrameworkCore;
using PeopleDesk.Models.Employee;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PeopleDesk.Helper
{
    public static class PaginationHelper
    {
        public class PaginatedVM
        {
            public long CurrentPage { get; set; }
            public long TotalCount { get; set; }
            public long PageSize { get; set; }
            public dynamic Data { get; set; }
        }
        public class EmployeeHeaderDataHelper
        {
            public dynamic StrDesignationList { get; set; }
            public dynamic StrDepartmentList { get; set; }
            public dynamic StrSupervisorNameList { get; set; }
            public dynamic StrLinemanagerList { get; set; }
            public dynamic StrEmploymentTypeList { get; set; }
        }

        public static IQueryable<T> ApplyPaginationReturnQuery<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 1 : pageSize;
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
        public static async Task<PaginatedVM> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 1 : pageSize;
            PaginatedVM returnObj = new()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = await query.CountAsync(),
                Data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()
            };

            return returnObj;
        }
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortBy, bool isDescending)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, sortBy);
            var lambda = Expression.Lambda(property, parameter);
            var methodName = isDescending ? "OrderByDescending" : "OrderBy";
            var expression = Expression.Call(typeof(Queryable), methodName, new[] { typeof(T), property.Type }, query.Expression, lambda);
            return query.Provider.CreateQuery<T>(expression);
        }

        public static IQueryable<T> ApplySearch<T>(this IQueryable<T> query, string searchTerm, params Expression<Func<T, string>>[] searchProperties)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchProperties.Length == 0)
                return query;

            var searchExpressions = searchProperties
                .Select(prop => Expression.Call(prop.Body, "Contains", Type.EmptyTypes, Expression.Constant(searchTerm)))
                .ToArray();

            var parameter = searchProperties[0].Parameters[0];
            var combinedExpression = searchExpressions.Aggregate((Expression)null, (current, expression) => current != null ? Expression.Or(current, expression) : expression);

            var searchPredicate = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

            return query.Where(searchPredicate);
        }

        public static IQueryable<TResult> ApplyProjection<TSource, TResult>(this IQueryable<TSource> query, Expression<Func<TSource, TResult>> projection)
        {
            return query.Select(projection);
        }

    }

    /*
    var pageNumber = 1;
    var pageSize = 10;
    var sortBy = "Name";
    var isDescending = false;
    var searchTerm = "example";
    var searchProperties = new Expression<Func<YourModel, string>>[]
    {
        model => model.Name,
        model => model.Description
    };

    var results = dbContext.YourModels
        .ApplySearch(searchTerm, searchProperties)
        .ApplySorting(sortBy, isDescending)
        .ApplyPagination(pageNumber, pageSize)
        .ApplyProjection(model => new
        {
            model.Id,
            model.Name
        })
        .ToList();
     */

}
