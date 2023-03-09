using System.Collections.Generic;

namespace IGDB.Models
{
    public class Response<T>
    {
        public List<T> MyProperty { get; set; }

        public int TotalCount { get; set; }
    }
}