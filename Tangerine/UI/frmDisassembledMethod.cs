using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mono.Cecil;
using Tangerine.BLL;

namespace Tangerine.UI
{
    public partial class frmDisassembledMethod : Form
    {
        private readonly MethodDefinition m_methodDefinition;

        public frmDisassembledMethod()
        {
            InitializeComponent();
        }

        public frmDisassembledMethod(MethodDefinition mMethodDefinition)
            : this()
        {
            m_methodDefinition = mMethodDefinition;
        }

        private void frmDisassembledMethod_Load(object sender, EventArgs e)
        {
            Text = m_methodDefinition.ToDisplayString();

            SetCode();
        }

        private void SetCode()
        {
            rtbCode.AppendText(m_methodDefinition.ToLongString());
            rtbCode.AppendText(Environment.NewLine);
            
            if (m_methodDefinition.Body.Variables.Count > 0)
            {
                rtbCode.AppendText(Environment.NewLine);
                rtbCode.AppendText("Locals: ");
            }
            for (int i = 0; i < m_methodDefinition.Body.Variables.Count; i++)
            {
                var variable = m_methodDefinition.Body.Variables[i];
                rtbCode.AppendText(String.Format("{0} {1}, ", variable.VariableType.Name, variable.Name));
            }
            int index = rtbCode.Text.LastIndexOf(", ");
            if (index > -1)
            {
                rtbCode.Text = rtbCode.Text.Remove(index);
                rtbCode.AppendText(Environment.NewLine);
            }

            for (int i = 0; i < m_methodDefinition.Body.Instructions.Count; i++)
            {
                rtbCode.AppendText(Environment.NewLine);
                rtbCode.AppendText(m_methodDefinition.Body.Instructions[i].ToString());
            }
        }
    }
}
