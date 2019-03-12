namespace IdentityServer4.Admin.Infrastructure
{
    public class PagedQuery
    {
        private int _page = 1;
        private int _size = 10;

        public int? Page
        {
            get => _page;
            set
            {
                if (value == null || value <= 0)
                {
                    _page = 1;
                }
                else
                {
                    _page = value.Value;
                }
            }
        }

        public int? Size
        {
            get => _size;
            set
            {
                if (value == null || value <= 0)
                {
                    _size = 30;
                }
                else
                {
                    _size = value.Value;
                }
            }
        } 
        
        public int GetPage()
        {
            return _page;
        }

        public int GetSize()
        {
            return _size;
        }
    }
}