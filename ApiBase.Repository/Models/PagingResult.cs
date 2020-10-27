using System.Collections.Generic;

namespace ApiBase.Repository.Models
{
    public class PagingResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRow { get; set; }
        public string Keywords { get; set; }
    }
}