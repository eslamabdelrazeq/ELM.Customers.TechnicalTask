using System;
using System.Collections.Generic;
using System.Text;

namespace ELM.Common.BaseRequestResponse
{
    public class BaseRequestResponseHeader
    {
        public int MessageId { get; set; }
        public DateTime timeStamp { get; set; } = new DateTime();

    }
}
