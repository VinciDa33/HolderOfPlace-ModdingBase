using ADV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModUtils
{
    public class UIButton_Generic : UIButton_Standard
    {
        public Action action;

        public override void MouseDownEffect()
        {
            action();
        }
    }
}
