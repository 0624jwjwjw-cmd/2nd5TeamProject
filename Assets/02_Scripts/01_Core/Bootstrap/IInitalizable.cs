public interface IInitializable
{
    int Priority { get; }
    void Initialize();
}