using Service.Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Infrastructure.Util;

public class PagedEntities<T> : IPagedEntities<T>
{
    public IQueryable<T> Data { set; get; }
    public IPagination Pagination { set; get; }
}

public class PagedEntities<T, U> : IPagedEntities<T, U>
{
    public IQueryable<T> Data { set; get; }
    public U Total { set; get; }
    public IPagination Pagination { set; get; }
}

public class PagedListEntities<T> : IPagedListEntities<T>
{
    public IEnumerable<T> Data { get; init; }
    public IPagination Pagination { get; set; }
}

public class PagedListEntities<T, U> : IPagedListEntities<T, U>
{
    public IEnumerable<T> Data { get; init; }
    public U Total { get; init; }
    public IPagination Pagination { get; set; }
}