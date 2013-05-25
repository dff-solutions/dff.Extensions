using System;
using dff.Extensions.HelperClasses;

namespace dff.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Git für ein beliebiges Objekt einen kompletten Dump aus, der alle Ebenen überspannt.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Dump(this Object obj)
        {
            return ObjectDumper.Dump(obj);
        }
    }
}