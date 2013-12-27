using System.Collections.Generic;

namespace CDSReviewerCore
{
    static class Utils
    {
        /// <summary>
        /// Given a single item, return it as the contents of a single list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsSingleList<T>(this T item)
        {
            yield return item;
        }
    }
}
