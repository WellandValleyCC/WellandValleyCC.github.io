namespace ClubProcessor.Interfaces
{
    public interface IVetsHandicapProvider
    {
        int GetHandicapSeconds(double distanceMiles, bool isFemale, int vetsBucket);
    }
}
