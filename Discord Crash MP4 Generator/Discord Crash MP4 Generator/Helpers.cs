using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Crash_MP4_Generator
{
    static class Helpers
    {
        public static bool HasMethod(this object objectToCheck, string methodName)
        {
            try
            {
                var type = objectToCheck.GetType();
                return type.GetMethod(methodName) != null;
            }
            catch (AmbiguousMatchException)
            {
                return true;
            }
        }
    }
}
