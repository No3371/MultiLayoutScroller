// MIT License
// Original author: Mohammed Iqubal Hussain (Polyandcode.com)

namespace BAStudio.MultiLayoutScroller
{
    public interface IMultiLayoutScrollerDataSource
    {
        int RuntimeVersion { get; }
        object Pull(int key);
    }
}