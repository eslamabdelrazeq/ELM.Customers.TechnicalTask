using System;
using System.Collections.Generic;
using System.Text;

namespace ELM.Common.BaseRequestResponse
{
    public class RequestModel<T>
    {
        public BaseRequestResponseHeader Header { get; set; }
        public T Body { get; set; }
    }
}
