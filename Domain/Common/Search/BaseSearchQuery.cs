using System;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Common.Search
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
