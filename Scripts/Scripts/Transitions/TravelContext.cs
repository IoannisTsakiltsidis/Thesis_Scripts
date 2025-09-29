public static class TravelContext
{
    public static string PreviousScene { get; private set; }
    public static string NextScene { get; private set; }

    public static void Set(string previousScene, string nextScene)
    {
        PreviousScene = previousScene;
        NextScene = nextScene;
    }

    public static void Clear() { PreviousScene = null; NextScene = null; }
}
