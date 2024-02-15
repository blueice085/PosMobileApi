namespace PosMobileApi.Models.Responses
{
    public class BaseListResDto<T>
    {
        public T Data { get; private set; }
        public int Current { get; private set; }
        public int PageSize { get; private set; }
        public int Total { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPrevPage
        {
            get
            {
                return (Current > 1);
            }
        }
        public bool HasNextPage
        {
            get
            {
                return (Current < TotalPages);
            }
        }

        public BaseListResDto(T data, int count, int pageIndex, int pageSize)
        {
            Data = data;
            Current = pageIndex;
            PageSize = pageSize;
            Total = count;
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
        }
    }
}
