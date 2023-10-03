using System;
using System.Collections.Generic;

namespace stat
{
    [Serializable]
    public class UsersStatHolder
    {
        public List<UserStatHolder> bestScoreRecords = new List<UserStatHolder>(10);
        public int currentIndex = -1;
        public int lastScore;
    }
}