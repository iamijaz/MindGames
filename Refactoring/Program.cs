using System.Collections.Generic;

namespace Refactoring
{
    internal class Program
    {
        private static void Main()
        {
            int[] arrayToSort = { 45, 80, -1, 70, 0, 10, 90 };
            QuickSort(arrayToSort, 0, arrayToSort.Length - 1);
        }

        // To sort array arrayToSort[] of size n: qsort(arrayToSort,0,n-1)

        private static void QuickSort(IList<int> arrayToSort, int firstValueIndex, int lastValueIndex)
        {
            if (firstValueIndex < lastValueIndex)
            {
                var firstIndex = firstValueIndex;
                var lastIndex = lastValueIndex;
                var pivotValue = arrayToSort[lastValueIndex];

                do
                {
                    firstIndex = FindFirstIndexOfValueSmallerThanPivotValue(arrayToSort, firstIndex, lastIndex, pivotValue);
                    lastIndex = FindLastIndexOfValueBIggerThanPivotValue(arrayToSort, lastIndex, firstIndex, pivotValue);
                    SwapValues(arrayToSort, firstIndex, lastIndex);
                } while (firstIndex < lastIndex);

                arrayToSort[lastValueIndex] = arrayToSort[firstIndex];
                arrayToSort[firstIndex] = pivotValue;

                QuickSort(arrayToSort, firstValueIndex, firstIndex - 1);
                QuickSort(arrayToSort, firstIndex + 1, lastValueIndex);
            }
        }

        private static void SwapValues(IList<int> arrayToSort, int firstIndex, int lastIndex)
        {
            if (firstIndex < lastIndex)
            {
                var temp = arrayToSort[firstIndex];
                arrayToSort[firstIndex] = arrayToSort[lastIndex];
                arrayToSort[lastIndex] = temp;
            }
        }

        private static int FindLastIndexOfValueBIggerThanPivotValue(IList<int> arrayToSort, int lastIndex, int firstIndex, int pivotValue)
        {
            while ((lastIndex > firstIndex) && (arrayToSort[lastIndex] >= pivotValue))
            {
                lastIndex = lastIndex - 1;
            }

            return lastIndex;
        }

        private static int FindFirstIndexOfValueSmallerThanPivotValue(IList<int> arrayToSort, int firstIndex, int lastIndex, int pivotValue)
        {
            while ((firstIndex < lastIndex) && (arrayToSort[firstIndex] <= pivotValue))
            {
                firstIndex = firstIndex + 1;
            }
            return firstIndex;
        }
    }
}
