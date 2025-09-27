public static class Scenes
{
    public const string Loading = "Loading";
    public const string CorridorPass1 = "Corridor_Pass1";
    public const string CorridorPass2 = "Corridor_Pass2";
    public const string CorridorPass3 = "Corridor_Pass3";
    public const string STREET = "STREET";
    public const string FOREST = "FOREST";
}

public static class SpawnIds
{
    // Where corridor exits land you in gameplay scenes
    public const string From_Corridor = "From_Corridor";  // in STREET/FOREST

    // Where you appear inside each corridor pass
    public const string From_Start = "From_Start";     // first time (Pass1)
    public const string From_Street = "From_Street";    // when coming from STREET (Pass2)
    public const string From_Forest = "From_Forest";    // when coming from FOREST (Pass3)
}
