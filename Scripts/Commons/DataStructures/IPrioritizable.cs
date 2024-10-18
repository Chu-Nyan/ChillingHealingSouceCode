
public interface IPrioritizable
{
    public int QueueIndex { get; set; }
    public int Priority { get; }

    public void ResetPriorityData();
}
