using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Domain.Common;
public interface IPagedEntities<out T>
{
    IQueryable<T> Data { get; }
    IPagination Pagination { get; set; }
}

public interface IPagedEntities<out T, out U>
{
    IQueryable<T> Data { get; }
    U Total { get; }
    IPagination Pagination { get; set; }
}

public interface IPagedListEntities<out T>
{
    IEnumerable<T> Data { get; }
    IPagination Pagination { get; set; }
}

public interface IPagedListEntities<out T, out U>
{
    IEnumerable<T> Data { get; }
    U Total { get; }
    IPagination Pagination { get; set; }
}