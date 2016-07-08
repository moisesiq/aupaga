using System;
using System.ComponentModel;

namespace Refaccionaria.App
{
    static class General2
    {
        public static void CustomInvoke<T>(this T controlToInvoke, Action<T> actionToPerform) where T : ISynchronizeInvoke
        {
            if (controlToInvoke.InvokeRequired)
                controlToInvoke.Invoke(actionToPerform, new object[] { controlToInvoke });
            else
                actionToPerform(controlToInvoke);
        }
    }
}
