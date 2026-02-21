using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModUtils
{ 
    public abstract class HopMod
    {
        public string modPath;
        internal bool enabled;

        public virtual string guid => "name.mod";
        public virtual string title => "Mod Name";

        public HopMod(string path)
        {
            modPath = path;
        }

        protected virtual void CreateAssets()
        {

        }

        protected virtual void OnEnable()
        {
            enabled = true;
        }

        protected virtual void OnDisable()
        {
            enabled = false;
        }
    }
}
