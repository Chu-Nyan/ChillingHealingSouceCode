public abstract class Singleton<T> where T : Singleton<T>, new()
{
    private static T _instance;

    public static T Instance { get { return _instance; } }

    public Singleton()
    {
        _instance ??= (T)this;
    }
}