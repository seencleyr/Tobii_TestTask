using System.Threading;

namespace Tobii_TestTask.Core.Helper
{
    /// <summary>
    /// Helper class with autoincrementor logic. Can be used for creating unique ID
    /// </summary>
    public static class Autoincrementor
    {
        private static int _lastValue;

        public static int GetNextUniqueValue()
        {
            return Interlocked.Increment(ref _lastValue);
        }
    }
}