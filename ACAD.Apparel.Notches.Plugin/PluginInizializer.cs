using System;
using System.Linq;
using Autodesk.AutoCAD.Ribbon;
using Autodesk.Windows;

namespace ACAD.Apparel.Notches.Plugin
{
    public class PluginInizializer : Autodesk.AutoCAD.Runtime.IExtensionApplication
    {
        public void Initialize()
        {
            ConfigureRibbon();
        }

        public void Terminate()
        {
            Console.WriteLine("Cleaning up...");
        }

        private void ConfigureRibbon()
        {
            var ribbonControl = ComponentManager.Ribbon;
            var ribbonTab = ribbonControl.Tabs
                .Where(tab => tab.AutomationName == "Plug-ins" || tab.AutomationName == "Add-ins")
                .FirstOrDefault();
            if (ribbonTab == null)
            {
                ribbonTab = new RibbonTab();
                ribbonTab.Title = "Add-ins";
                ribbonTab.Id = "ID_CUSTOMRIBBONTAB";
                ribbonControl.Tabs.Add(ribbonTab);
            }
            var ribbonPanelSource = new RibbonPanelSource();
            ribbonPanelSource.Title = "Notches";

            var ribbonPanel = new RibbonPanel();
            ribbonPanel.Source = ribbonPanelSource;
            ribbonTab.Panels.Add(ribbonPanel);

            var showNotchesParamsButton = new RibbonCommandButton();
            showNotchesParamsButton.ShowImage = false;
            showNotchesParamsButton.ShowText = true;
            showNotchesParamsButton.Text = "Show Notches";
            showNotchesParamsButton.CommandHandler = new ShowNotchesParamsCommand();
            ribbonPanel.Items.Add(showNotchesParamsButton);

            // For testing purposes:
            ribbonTab.IsActive = true;
        }
    }
}
