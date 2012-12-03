using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangerine.UI.BLL
{
    internal sealed class IncrementalSearchPresenter
    {
        private readonly IIncrementalSearchView m_view;
        private readonly IEnumerable<string> m_searchItems;

        public IncrementalSearchPresenter(IIncrementalSearchView view, IEnumerable<string> searchItems)
        {
            m_view = view;
            m_searchItems = searchItems;

            m_view.SetSearchItems(searchItems);
        }

        public void Search()
        {
            string searchText = m_view.GetSearchText().ToLower();

            List<string> filteredItems = new List<string>();

            foreach (string searchItem in m_searchItems)
            {
                if (searchItem.ToLower().Contains(searchText))
                {
                    filteredItems.Add(searchItem);
                }
            }

            m_view.SetSearchItems(filteredItems.ToArray());
        }
    }
}
