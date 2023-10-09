using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Application.Common
{
    public abstract class Search
    {
        public string? Filter { get; set; }
        public int Top { get; set; } = 10;
    }
}
