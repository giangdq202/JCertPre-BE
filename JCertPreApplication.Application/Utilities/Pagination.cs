namespace JCertPreApplication.Application.Utilities
{
    public class Pagination<T>
    {
        public int TotalItemsCount { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 1;

        public int TotalPagesCount
        {
            get
            {
                var temp = TotalItemsCount / PageSize;
                if (TotalItemsCount % PageSize == 0)
                {
                    return temp;
                }
                return temp + 1;
            }
        }

        /// <summary>
        /// page number start from 0
        /// </summary>
        public bool Next => PageIndex + 1 < TotalPagesCount;
        public bool Previous => PageIndex > 0;
        public ICollection<T> Items { get; set; }

    }
}
