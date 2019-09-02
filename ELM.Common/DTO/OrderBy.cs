using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ELM.Common.DTO
{
    public class OrderBy<T> where T : class
    {
        public string Column { get; set; }
        public Expression<Func<T, Object>> Property { get; set; }
        public bool Desc = false;
    }
}
