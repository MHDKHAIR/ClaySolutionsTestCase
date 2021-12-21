using System;
using Domain.Enums;

namespace Domain.Common.Search
{
    public class UserDoorLocksSearch : GeneralSearch
    {
        public string Name { get; set; }
        public DateTime? LastAccessed { get; set; }
    }
    public class UserAccessLocksHistorySearch : UserDoorLocksSearch
    {
        public LockAccessStatusEnum? LockAccessStatus { get; set; }
    }
}
