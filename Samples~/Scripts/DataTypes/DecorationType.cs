namespace Hoco.Samples
{
    [System.Serializable]
    public enum DecorationType
    {
        Ground, //Placable on anything flat
        GroundAndWalls, //Placable on anything flat or a wall
        Wall  //Only on Wall
    }
}