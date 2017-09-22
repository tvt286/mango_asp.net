using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Common
{
    public class CommandResult
    {
        public CommandResult(ResultCode code, string message)
        {
            Code = code;
            Message = message;
        }

        public CommandResult()
        {
            Code = ResultCode.Success;
            Message = string.Empty;
        }

        public ResultCode Code { get; set; }
        public string Message { get; set; }
    }

    public class RedirectCommand
    {
        public ResultCode Code { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
    }

    public enum ResultCode
    {
        Success = 0,
        Fail = 1,

    }

    public enum CreateEdit
    {
        Create = 0,
        Edit = 1,
    }
}
