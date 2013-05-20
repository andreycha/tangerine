using System;
using System.Reflection;

namespace Tangerine.Common
{
    public static class ExceptionHelper
    {
        public static Exception GetRealExceptionWithStackTrace(TargetInvocationException tiex)
        {
            var remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);

            remoteStackTraceString.SetValue(
                tiex.InnerException,
                tiex.StackTrace + Environment.NewLine
                );

            return tiex.InnerException;
        }
    }
}
