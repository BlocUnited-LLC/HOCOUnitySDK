namespace Hoco.Samples
{
    [System.Serializable]
    public enum EquipmentType
    {
        None, //Cant be Equipped
        Hand,//Goes in the Hand
        Head,//Goes on the Head like a Hat
        Back,//Goes on the Back but over the other offHand Item if any
    }
}