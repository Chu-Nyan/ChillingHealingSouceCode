public struct PathFindingData<T>
{
    public T BeforeNode;
    public int QueueIndex;

    public int G;
    public int H;
    public int F;

    public PathFindingData(int g, int h, int f)
    {
        BeforeNode = default;
        QueueIndex = 0;
        G = g;
        H = h;
        F = f;
    }

    public PathFindingData(T beforeNode, int queueIndex, int g, int h, int f)
    {
        BeforeNode = beforeNode;
        QueueIndex = queueIndex;
        G = g;
        H = h;
        F = f;
    }
}
