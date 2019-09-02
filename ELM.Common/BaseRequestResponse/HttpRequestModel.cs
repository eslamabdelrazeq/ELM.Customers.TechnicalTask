using System;
using System.Collections.Generic;
using System.Text;

namespace ELM.Common.BaseRequestResponse
{
    public class HttpRequestModel<T>
    {
        public BaseRequestResponseHeader Header { get; set; }
        public T Body { get; set; }
    }
}
