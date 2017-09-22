using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mango.Data.Extensions;

namespace Mango.Data
{
    public partial class mangoEntities
    {
        public mangoEntities(IsolationLevel isolationLevel)
            : base()
        {
            //   this.Configuration.LazyLoadingEnabled = false;
            //this.Configuration.LazyLoadingEnabled = true;
            this.SetIsolationLevel(isolationLevel);
        }
    }
}
