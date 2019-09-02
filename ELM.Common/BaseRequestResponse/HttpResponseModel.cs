using System;
using System.Collections.Generic;
using System.Text;

namespace ELM.Common.BaseRequestResponse
{
    public class ResponseBody<T>
    {
        public T Data { get; set; }
        public List<string> Errors { get; set; }
    }
    public class HttpResponseModel<T>
    {
        public BaseRequestResponseHeader Header { get; set; }
        public ResponseBody<T> Body { get; set; }
    }
}
