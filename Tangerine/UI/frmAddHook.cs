using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Tangerine.UI.BLL;

namespace Tangerine.UI
{
    public partial class frmAddHook : Form, IIncrementalSearchView
    {
        private readonly IncrementalSearchPresenter m_presenter;

        protected frmAddHook()
        {
            InitializeComponent();
        }

        public frmAddHook(IEnumerable<string> searchItems)
            : this()
        {
            m_presenter = new IncrementalSearchPresenter(this, searchItems);
        }

        private void frmAddHook_Load(object sender, EventArgs e)
        {
            ActiveControl = txtSearchText;
        }

        public string GetSelectedItem()
        {
            return lbSearchItems.SelectedItem.ToString();
        }

        string IIncrementalSearchView.GetSearchText()
        {
            return txtSearchText.Text;
        }

        void IIncrementalSearchView.SetSearchItems(IEnumerable<string> items)
        {
            lbSearchItems.Items.Clear();
            lbSearchItems.Items.AddRange(items.ToArray());
            if (lbSearchItems.Items.Count > 0)
            {
                lbSearchItems.SelectedIndex = 0;
            }
        }

        private void txtSearchText_TextChanged(object sender, EventArgs e)
        {
            m_presenter.Search();
        }
    }
}
