using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace WorldCitiesAPI.Models
{
    public class ApiResult<T>
    {
        /// <summary>
        /// private constructor called by the AsyncCreate method
        /// </summary>
        private ApiResult(List<T> data, 
            int pageIndex, 
            int pageSize, 
            int totalCount, 
            string? sortColumn, 
            string? sortOrder, 
            string? filterColumn, 
            string? filterQuery)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            FilterColumn = filterColumn;
            FilterQuery = filterQuery;
        }

        /// <summary>
        /// page, sort and/or filters a IQueryable source
        /// </summary>
        /// <param name="source:IQueryable"></param>
        /// <param name="pageIndex:int"></param>
        /// <param name="pageSize:int"></param>
        /// <param name="sortColumn:string"></param>
        /// <param name="sortOrder:string"></param>
        /// <param name="filterColumn:string"></param>
        /// <param name="filterQuery:string"></param>
        /// <returns>
        /// <typeparamref name="ApiResult"/>
        /// </returns>
        public static async Task<ApiResult<T>> CreateAsync(
            IQueryable<T> source,
            int pageIndex,
            int pageSize,
            string? sortColumn = null,
            string? sortOrder = null,
            string? filterColumn = null,
            string? filterQuery = null
        )
        {
            if (isValidProperty(filterColumn) && !string.IsNullOrEmpty(filterQuery))
            {
                source = source.Where(
                        string.Format("{0}.StartsWith(@0)",
                        filterColumn),
                    filterQuery);
            }
            var count = await source.CountAsync();
            var isvalid = isValidProperty(sortColumn);
            if (isvalid)
            {
                sortOrder = !string.IsNullOrEmpty(sortOrder) &&
                    "ASC".Equals(sortOrder.ToUpper()) ? "ASC" : "DESC";
                var order = string.Format("{0} {1}", sortColumn, sortOrder);
                source = source.OrderBy(order);
            }
            source = source
                .Skip(pageSize * pageIndex)
                .Take(pageSize);
            var data = await source.ToListAsync();
            return new ApiResult<T>(
                data,
                pageIndex,
                pageSize,
                count,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
        }

        public static bool isValidProperty(string? propertyName, bool isThrowException=true)
        {
            if (string.IsNullOrEmpty(propertyName))
                return false;
            //var props = typeof(T)
            //    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
            //    .FirstOrDefault(p => p.Name.Equals(propertyName));
            var prop = typeof(T).GetProperty(
                    propertyName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                );
            if (prop == null && isThrowException)
                throw new NotSupportedException($"ERROR: Property '{propertyName}' does not exist.");
            return (prop!=null);
        }

        public List<T> Data { get; private set; }

        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        public int TotalPages { get; private set; }

        public bool HasPreviousPage 
        { 
            get
            {
                return (PageIndex > 0);
            } 
        }

        public bool HastNextPage
        {
            get
            {
                return ((PageIndex + 1) < TotalPages);
            }
        }

        public string? SortColumn {get; set;}

        public string? SortOrder {get; set;}

        public string? FilterColumn { get; set; }

        public string? FilterQuery { get; set; }
    }
}
