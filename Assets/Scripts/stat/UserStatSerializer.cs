using System.IO;
using UnityEngine;

namespace stat
{
    public static class UserStatSerializer
    {
        private static readonly string path = "users-stat.json";
        
        public static void Save(UsersStatHolder holder)
        {
            using (var writer = new StreamWriter(path))
            {
                var json = JsonUtility.ToJson(holder);
                writer.Write(json);
            }
        }

        public static UsersStatHolder Load()
        {
            try
            {
                using (var reader = new StreamReader(path))
                {
                    var json = reader.ReadToEnd();
                    return JsonUtility.FromJson<UsersStatHolder>(json);
                }
            }
            catch (IOException exc)
            {
                return null;
            }
        }
    }
}