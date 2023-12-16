using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoneWandererGame.Utilities
{
    public abstract class Clickable
    {
        public Action OnClick { get; set; }
        public Action OnRelease { get; set; }
        public Action OnPress { get; set; }
    }
}
