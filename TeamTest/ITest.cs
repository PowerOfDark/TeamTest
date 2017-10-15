namespace TeamTest
{
    public interface ITest
    {
        string JobName { get; }
        string GenerateTest(int id, string directory);
        bool HasNext(int nextId);
    }
}