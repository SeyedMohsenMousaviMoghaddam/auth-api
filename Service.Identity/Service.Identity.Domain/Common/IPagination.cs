using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Domain.Common
{
    public interface IPagination
    {
        public int Page { set; get; }
        public int RowsPerPage { set; get; }
        public int RowsNumber { set; get; }
        public bool Descending { get; set; }
        public string? SortBy { get; set; }
    }
}
