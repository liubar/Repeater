using System;

namespace App
{
    public static class Utils
    {
        public static void ThrowIfNull(Object obj, string message)
        {
            if (obj == null)
            {
                throw new NullReferenceException(message);
            }
        }
    }
}
