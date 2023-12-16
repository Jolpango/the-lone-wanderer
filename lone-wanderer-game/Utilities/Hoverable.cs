using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoneWandererGame.Utilities
{
    public abstract class Hoverable
    {
        public Action OnEnter { get; set; }
        public Action OnLeave { get; set; }
    }
}
