namespace LMS.Helpers
{
    public class Pager
    {
        public int TotalItems { get; private set; }         // total # of items
        public int CurrentPage { get; private set; }       // current on page
        public int PageSize { get; private set; }        // itmes per page

        public int TotalPages { get; private set; }  // All pages ( displayed and not displayed)
        public int StartPage { get; private set; }   // displayed first in the bar
        public int EndPage { get; private set; }    // displayed last in the bar

        public Pager() { }

        public Pager(int totalItems, int? page, int pageSize = 10)
        {
            // calculate total, start and end pages
            int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            int currentPage = page != null ? (int)page : 1;
            int startPage = currentPage - 5;
            int endPage = currentPage + 4;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }

}