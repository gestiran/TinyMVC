using System;

namespace TinyMVC.Utilities {
    public static class EnumUtility {
        public static void RunAll<T>(Action<T> action) where T : struct {
            string[] locations = Enum.GetNames(typeof(T));

            for (int locationId = 0; locationId < locations.Length; locationId++) {
                action(Enum.Parse<T>(locations[locationId]));
            }
        }

        public static int Count<T>() where T : struct => Enum.GetNames(typeof(T)).Length;
    }
}