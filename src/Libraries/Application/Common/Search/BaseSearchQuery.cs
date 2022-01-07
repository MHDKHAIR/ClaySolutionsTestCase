using System;
using Application.Common.Enums;
using Application.Common.Interfaces;

namespace Application.Common.Search
{
    public class BaseSearchQuery<S, T> where S : IGeneralSearch where T : Enum
    {
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public GeneralOrderEnum Order { get; set; }
        public S Search { get; set; }
        public T Sort { get; set; }
    }
}
