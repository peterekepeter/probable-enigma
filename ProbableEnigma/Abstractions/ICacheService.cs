namespace ProbableEnigma
{
    public interface ICacheService
    {
        IStore FindStore(string key);
    }
}