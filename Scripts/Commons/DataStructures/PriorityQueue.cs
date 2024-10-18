namespace ChillingHealing.DataStructure
{
    public class PriorityQueue<T> where T : class, IPrioritizable
    {
        private readonly T[] _datas;
        private int _count;

        public int Count { get { return _count; } }

        public PriorityQueue(int Capacity)
        {
            _count = 0;
            _datas = new T[Capacity];
        }

        public void Enqueue(T node)
        {
            _count++;
            _datas[_count] = node;
            node.QueueIndex = _count;
            CompareParent(node);
        }

        public T Dequeue()
        {
            var firstNode = _datas[1];
            if (_count > 1)
            {
                _datas[1] = _datas[_count];
                _datas[1].QueueIndex = 1;
                CompareChildren(_datas[1]);
            }
            _count--;
            return firstNode;
        }

        public void Clear()
        {
            while (_count > 0) 
            { 
                Dequeue().ResetPriorityData();
            }
            _count = 0;
        }

        public bool Contains(T node)
        {
            return _datas[node.QueueIndex] == node;
        }

        private void CompareParent(T node)
        {
            while (node.QueueIndex > 1)
            {
                int parentIndex = node.QueueIndex >> 1;
                T parentNode = _datas[parentIndex];
                if (node.Priority >= parentNode.Priority)
                    break;

                SwapIndex(node, parentNode);
            }
        }

        private void CompareChildren(T node)
        {
            int leftIndex = node.QueueIndex << 1; ;
            int rightIndex;
            int childIndex;

            while (leftIndex <= _count)
            {
                if (leftIndex == _count)
                    childIndex = leftIndex;
                else
                {
                    rightIndex = leftIndex + 1;
                    childIndex = _datas[leftIndex].Priority < _datas[rightIndex].Priority ? leftIndex : rightIndex;
                }

                if (_datas[childIndex].Priority >= node.Priority)
                    break;

                SwapIndex(node, _datas[childIndex]);
                leftIndex = node.QueueIndex << 1;
            }
        }

        private void SwapIndex(T A, T B)
        {
            int temp = A.QueueIndex;
            A.QueueIndex = B.QueueIndex;
            B.QueueIndex = temp;

            _datas[A.QueueIndex] = A;
            _datas[B.QueueIndex] = B;
        }
    }
}
