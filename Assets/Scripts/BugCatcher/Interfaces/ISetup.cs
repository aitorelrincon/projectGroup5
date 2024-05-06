using BugCatcher.Utils;

namespace BugCatcher.Interfaces
{
    public interface ISingleSetup<T>
        where T : MonoSingle<T>
    {
        bool Done { get; }
        bool Setup();
    }
}