using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace stat
{
    public class UserStatController : MonoBehaviour
    {
        private static UserStatController instance;

        private UsersStatHolder holder = new UsersStatHolder();

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            holder = UserStatSerializer.Load();
            if (holder == null)
            {
                holder = new UsersStatHolder();
            }
        }

        public static List<UserStatHolder> GetBestScore(int limit)
        {
            return instance.holder.bestScoreRecords
                .OrderByDescending(holder => holder.bestScore)
                .Take(limit)
                .ToList();
        }

        public static UserStatHolder SetCurrentUser(string name)
        {
            instance.holder.lastScore = 0;
            var existingIndex = instance.holder.bestScoreRecords.FindIndex(holder => holder.name.Equals(name));
            if (existingIndex < 0)
            {
                var holder = new UserStatHolder()
                {
                    name = name,
                    bestScore = 0
                };
                instance.holder.bestScoreRecords.Add(holder);
                instance.holder.currentIndex = instance.holder.bestScoreRecords.Count - 1;
            }
            else
            {
                instance.holder.currentIndex = existingIndex;
            }
            
            UserStatSerializer.Save(instance.holder);
            return instance.holder.bestScoreRecords[instance.holder.currentIndex];
        }

        public static UserStatHolder GetCurrentUserHolder()
        {
            //Assert.IsTrue(instance.currentIndex >= 0, "currentPlayerIndex is not valid");
            if (instance.holder.currentIndex < 0 || instance.holder.currentIndex >= instance.holder.bestScoreRecords.Count)
            {
                return null;
            }
            return instance.holder.bestScoreRecords[instance.holder.currentIndex];
        }

        public static void UpdateBestScore(int score)
        {
            var holder = GetCurrentUserHolder();
            if (score > holder.bestScore)
            {
                holder.bestScore = score;
            }
        }

        public static int GetLastScore()
        {
            return instance.holder.lastScore;
        }

        public static void SetLastScore(int lastScore)
        {
            instance.holder.lastScore = lastScore;
            UpdateBestScore(lastScore);
        }

        public static int IncreaseLastScore(int increment)
        {
            instance.holder.lastScore += increment;
            UpdateBestScore(instance.holder.lastScore);
            return instance.holder.lastScore;
        }

        public static void Flush()
        {
            UserStatSerializer.Save(instance.holder);
        }
    }
}