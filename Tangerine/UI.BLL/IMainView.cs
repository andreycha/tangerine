using System;
using System.Collections.Generic;
using Tangerine.BLL;
using Tangerine.BLL.Hooks;
using Aga.Controls.Tree;

namespace Tangerine.UI.BLL
{
    interface IMainView
    {
        string ShowSearchForm(IEnumerable<string> searchItems);

        void AddMethod(MethodHook methodHook);

        void AddOutputText(string text);

        void ResetButton(string target);

        bool GetLogMethodNames();

        bool GetLogParameterValues();

        MethodHook GetSelectedHook();

        IEnumerable<MethodHook> GetHooks();

        void ShowEditHookForm(MethodHook methodHook);

        void SetManifestInformation(IManifest manifest);

        void InitializeAssemblyTree(IEnumerable<string> assemblies);

        void SetTreeModel(AssemblyTreeModel treeModel);

        void SetExpanded(Node node);

        void ShowError(Exception exception);

        bool GetLogReturnValues();
    }
}
