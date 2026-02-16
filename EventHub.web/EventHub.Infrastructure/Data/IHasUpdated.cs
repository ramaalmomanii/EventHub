using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Infrastructure.Data
{
    public interface IHasUpdated
    {
        DateTime? Updated { get; set; }
    }

}
