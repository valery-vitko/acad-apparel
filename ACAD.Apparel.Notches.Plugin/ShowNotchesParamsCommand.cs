using System;
using System.Drawing;
using System.Windows.Input;
using Autodesk.AutoCAD.Windows;

namespace ACAD.Apparel.Notches.Plugin
{
    public class ShowNotchesParamsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ShowPalette();
        }

        private static PaletteSet PaletteSet = null;

        private void ShowPalette()
        {
            if (PaletteSet == null)
            {
                PaletteSet = new PaletteSet("Notches")
                {
                    Size = new Size(500, 300),
                    DockEnabled = DockSides.Left | DockSides.Right | DockSides.Bottom
                };

                var plugin = new NotchesPlugin();
                var paramsView = new UI.ParamsView { DataContext = plugin.Params };

                var host = new System.Windows.Forms.Integration.ElementHost();
                host.AutoSize = true;
                host.Dock = System.Windows.Forms.DockStyle.Fill;
                host.Child = paramsView;

                PaletteSet.Add("ParamsView ElementHost", host);

                plugin.Init();
            }

            PaletteSet.KeepFocus = true;
            PaletteSet.Visible = true;
        }
    }
}
