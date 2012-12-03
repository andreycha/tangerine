using System.Collections.Generic;

namespace Tangerine.UI.BLL
{
    internal interface IIncrementalSearchView
    {
        string GetSearchText();

        void SetSearchItems(IEnumerable<string> items);
    }
}
