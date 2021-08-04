using System;
using System.Collections.Generic;

namespace Pahtfinding
{
    public class Path<T> : List<T>
    {
        public T First => this[0];

        public T Destination => this[Count - 1];

        public T Next()
        {
            if (Count == 0) throw new Exception("Path has no next item");
            Remove(this[0]);
            return First;
        }
    }
}