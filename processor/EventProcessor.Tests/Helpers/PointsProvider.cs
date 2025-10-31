namespace EventProcessor.Tests.Helpers
{
    internal static class PointsProvider
    {
        private static readonly IReadOnlyDictionary<int, int> PointsMap = BuildPointsMap();

        public static int GetPointsForPosition(int position)
        {
            var clamped = Math.Clamp(position, 1, 100);
            return PointsMap.TryGetValue(clamped, out var pts) ? pts : 0;
        }

        public static Func<int, int> AsDelegate() => GetPointsForPosition;

        private static IReadOnlyDictionary<int, int> BuildPointsMap()
        {
            int[] top = {
            60,55,51,48,46,44,42,40,39,38,37,36,35,34,33,32,31,30,29,28,
            27,26,25,24,23,22,21,20,19,18,17,16,15,14,13,12,11,10,9,8,
            7,6,5,4,3,2,1
        };

            const int maxPosition = 100;
            var map = new Dictionary<int, int>(capacity: maxPosition);

            for (int i = 0; i < top.Length; i++)
                map[i + 1] = top[i];

            for (int pos = top.Length + 1; pos <= maxPosition; pos++)
                map[pos] = 1;

            return map;
        }
    }
}

