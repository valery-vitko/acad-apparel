using System;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Ribbon;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;

[assembly: ExtensionApplication(typeof(ACAD.Apparel.Notches.Plugin.ExtensionApplication))]
[assembly: CommandClass(typeof(ACAD.Apparel.Notches.Plugin.ExtensionApplication))]

namespace ACAD.Apparel.Notches.Plugin
{
    public class ExtensionApplication : Autodesk.AutoCAD.Runtime.IExtensionApplication
    {
        public void Initialize()
        {
            PrepareRibbonLoading();
        }

        private void PrepareRibbonLoading()
        {
            if (RibbonServices.RibbonPaletteSet == null)
            {
                RibbonServices.RibbonPaletteSetCreated += RibbonServices_RibbonPaletteSetCreated;
            }
            else
            {
                RibbonServices.RibbonPaletteSet.WorkspaceLoaded += Workspace_Loaded;
                ConfigureRibbon();
            }
        }

        private void RibbonServices_RibbonPaletteSetCreated(object sender, EventArgs e)
        {
            RibbonServices.RibbonPaletteSet.WorkspaceLoaded += Workspace_Loaded;
            ConfigureRibbon();
        }

        private void Workspace_Loaded(object sender, EventArgs e)
        {
            ConfigureRibbon();
        }

        public void Terminate()
        {
            Console.WriteLine("Cleaning up...");
        }

        [CommandMethod("Notches")]
        public void ShowNotchesParamsCommand()
        {
            var command = new ShowNotchesParamsCommand();
            if (command.CanExecute(null))
                command.Execute(null);
        }

        private void ConfigureRibbon()
        {
            var ribbonControl = ComponentManager.Ribbon;
            if (ribbonControl == null)
                return;

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

            if (ribbonTab.Panels.Any(panel => panel.Source?.Name == "Notches"))
                return;

            var ribbonPanelSource = new RibbonPanelSource
            {
                Name = "Notches",
                Title = "Notches"
            };

            var ribbonPanel = new RibbonPanel();
            ribbonPanel.Source = ribbonPanelSource;
            ribbonTab.Panels.Add(ribbonPanel);

            var showNotchesParamsButton = new Autodesk.AutoCAD.Ribbon.RibbonCommandButton
            {
                Size = RibbonItemSize.Large,
                Orientation = System.Windows.Controls.Orientation.Vertical,
                ShowImage = true,
                LargeImage = ImageHelper.ImageSourceForBitmap(Properties.Resources.ShowNotchesParamsRibbonButtonIcon),
                ShowText = true,
                Text = $"Show{Environment.NewLine}Notches",
                CommandHandler = new ShowNotchesParamsCommand()
            };
            ribbonPanel.Items.Add(showNotchesParamsButton);

            // For testing purposes:
            //ribbonTab.IsActive = true;
        }
    }
}
