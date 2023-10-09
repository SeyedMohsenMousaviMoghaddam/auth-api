using Service.Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Application.Common
{
    public class PaginationRequest<T> where T : IAdvancedFilter?, new()
    {
        public int? Page { get; set; }
        public int? RowsPerPage { get; set; }
        public string? SortBy { get; set; }
        public bool Descending { set; get; }
        public string? Search { set; get; }
        public T? Filters { get; set; } = new();
    }
}
