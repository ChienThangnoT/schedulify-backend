using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.Commons
{
    public class Pagination<T>
    {
        public int TotalItemCount { get; set; }

        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }

        // Set the maximum amount of items in one page
        private const int MaxPageSize = 100;

        public int TotalPagesCount
        {
            get
            {
                var tmp = TotalItemCount / PageSize;
                if (TotalItemCount % PageSize == 0)
                {
                    return tmp;
                }
                return tmp + 1;
            }
        }

        private int _pageIndex = 0;

        // Auto re-assign pageIndex
        // if pageIndex is greater than or equal to TotalPagesCount
        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                _pageIndex = (value < 0) ? 0 : (value >= TotalPagesCount) ? TotalPagesCount - 1 : value;
            }
        }

        public bool Next => PageIndex + 1 < TotalPagesCount;
        public bool Previous => PageIndex > 0;
        public ICollection<T> Items { get; set; }
    }
}
