using System;
using System.Windows.Forms;
using Tangerine.BLL.Hooks;

namespace Tangerine.UI
{
    internal partial class frmEditHook : Form
    {
        private readonly MethodHook m_methodHook;

        private frmEditHook()
        {
            InitializeComponent();
        }

        public frmEditHook(MethodHook methodHook) : this()
        {
            m_methodHook = methodHook;
            LoadProperties();
        }

        private void LoadProperties()
        {
            Text += m_methodHook.ToShortString();

            lblMethod.Text = m_methodHook.ToLongString();

            chbLogMethods.Checked = m_methodHook.LogMethodName;
            chbLogParameters.Checked = m_methodHook.LogParameterValues;
            chbLogReturnValues.Checked = m_methodHook.LogReturnValues;
            chbRunCode.Checked = m_methodHook.RunCustomCode;

            bool replace = ((m_methodHook.HookType & HookType.ReplaceMethod) == HookType.ReplaceMethod);
            rbReplace.Checked = replace;
            
            bool addBegin = ((m_methodHook.HookType & HookType.OnMethodEnter) == HookType.OnMethodEnter);
            rbMethodEnter.Checked = addBegin;

            bool addEnd = ((m_methodHook.HookType & HookType.OnMethodExit) == HookType.OnMethodExit);
            rbMethodExit.Checked = addEnd;

            rtbCode.Text = m_methodHook.Code;
        }

        private void SaveProperties()
        {
            m_methodHook.LogMethodName = chbLogMethods.Checked;
            m_methodHook.LogParameterValues = chbLogParameters.Checked;
            m_methodHook.LogReturnValues = chbLogReturnValues.Checked;
            m_methodHook.RunCustomCode = chbRunCode.Checked;

            if (chbRunCode.Checked)
            {
                if (rbReplace.Checked)
                {
                    m_methodHook.HookType |= HookType.ReplaceMethod;
                }
                else if (rbMethodEnter.Checked)
                {
                    m_methodHook.HookType |= HookType.OnMethodEnter;
                }
                else if (rbMethodExit.Checked)
                {
                    m_methodHook.HookType |= HookType.OnMethodExit;
                }
            }
            else
            {
                m_methodHook.HookType &= ~HookType.ReplaceMethod;
                m_methodHook.HookType &= ~HookType.OnMethodEnter;
                m_methodHook.HookType &= ~HookType.OnMethodExit;
            }

            m_methodHook.Code = rtbCode.Text;
        }

        private void chbRunCode_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chbRunCode.Checked;
            
            rbReplace.Enabled = enabled;
            rbReplace.Checked = enabled;
            
            rbMethodEnter.Enabled = enabled;
            rbMethodEnter.Checked = false;

            rbMethodExit.Enabled = enabled;
            rbMethodExit.Checked = false;
            
            rtbCode.Enabled = enabled;
        }

        private void chbLogMethods_CheckedChanged(object sender, EventArgs e)
        {
            bool logMethodNames = chbLogMethods.Checked;
            chbLogParameters.Enabled = logMethodNames;
            chbLogReturnValues.Enabled = logMethodNames;
            if (!logMethodNames)
            {
                chbLogParameters.Checked = false;
                chbLogReturnValues.Checked = false;
            }
        }

        private void frmEditHook_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                SaveProperties();
            }
        }
    }
}
