using System.Collections.Generic;

namespace Application.Common.Results
{
    public class DataList<T>
    {
        public IEnumerable<T> Results { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int Total { get; set; }
    }
}
