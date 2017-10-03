using System;

namespace Tornado14.Utils.Net
{
    interface IFilter
    {
        bool applyFilter(Package package);
    }
}
