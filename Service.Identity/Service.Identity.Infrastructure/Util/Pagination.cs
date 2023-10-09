using Service.Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Infrastructure.Util;
public class Pagination : IPagination
{
    public int Page { set; get; }
    public int RowsPerPage { set; get; }
    public int RowsNumber { set; get; }
    public bool Descending { set; get; }
    public string? SortBy { set; get; }
}