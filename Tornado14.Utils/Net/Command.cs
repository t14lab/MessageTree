using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tornado14.Utils.Net
{
    public enum Command
    {
        Idle = 0,
        TestCommand = 1,
        GetTodo = 10,
        GetProject = 20,
        GetSprint = 30
    }
}
