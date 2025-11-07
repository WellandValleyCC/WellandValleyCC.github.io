namespace ClubProcessor.Interfaces
{
    public interface IVetsHandicapProvider
    {
        int GetHandicapSeconds(int year, double distanceMiles, bool isFemale, int vetsBucket);
    }
}
