using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModUtils
{
    public static class KeyLib
    {
        public static string TARGET(int selfOrTarget) => "Target[" + selfOrTarget;
        public static string TARGET_SELF => TARGET(0);
        public static string TARGET_OTHER => TARGET(1);
        public static string ICHANNEL(float f) => "InvokeChannel[" + f;
        public static string ON_RECRUIT => ICHANNEL(0);
        public static string ON_SUMMON => ICHANNEL(0.2f);
        public static string ON_ATTACK => ICHANNEL(1);
        public static string AFTER_ATTACK => ICHANNEL(1.5f);
        public static string PERMANENT => "Permanent[1";
        public static string TRAIT => "Trait[1";
        
    }
}
