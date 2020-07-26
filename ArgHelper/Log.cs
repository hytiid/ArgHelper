using System;

namespace ArgHelper
{
    public static class Log
    {
        public static void Require(bool expected, string errorMassage)
        {
            if (!expected)
                throw new Exception(errorMassage);
        }
    }
}