namespace Assets.Pathfinder.Scripts
{
    public class Heap<T> where T : IPathable
    {

        T[] _items;
        int _currentItemCount;

        public Heap(int maxHeapSize)
        {
            _items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = _currentItemCount;
            _items[_currentItemCount] = item;
            SortUp(item);
            _currentItemCount++;
        }

        public T RemoveFirst()
        {
            T firstItem = _items[0];
            _currentItemCount--;
            _items[0] = _items[_currentItemCount];
            _items[0].HeapIndex = 0;
            SortDown(_items[0]);
            return firstItem;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1)/2;

            while (true)
            {
                T parentItem = _items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                    Swap(item, parentItem);
                else
                    break;
                parentIndex = (item.HeapIndex - 1)/2;
            }
        }

        void SortDown(T item)
        {
            while (true)
            {
                int leftChildIndex = item.HeapIndex*2 + 1;
                int rightChildIndex = item.HeapIndex*2 + 2;
                int swapIndex = 0;

                if (leftChildIndex < _currentItemCount)
                {
                    swapIndex = leftChildIndex;
                    if (rightChildIndex < _currentItemCount)
                    {
                        if (_items[leftChildIndex].CompareTo(_items[rightChildIndex]) < 0)
                            swapIndex = rightChildIndex;
                    }
                    if (item.CompareTo(_items[swapIndex]) < 0)
                        Swap(item, _items[swapIndex]);
                    else
                        return;

                }
                else 
                    return;

            }
        }

        void Swap(T itemA, T itemB)
        {
            _items[itemA.HeapIndex] = itemB;
            _items[itemB.HeapIndex] = itemA;
            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }

        public int Count
        {
            get { return _currentItemCount; }
        }

        public bool Contains(T item)
        {
            return Equals(_items[item.HeapIndex], item);
        }



    }
}


