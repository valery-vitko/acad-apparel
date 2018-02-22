using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
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

                var paramsView = new UI.ParamsView();
                paramsView.DataContext = new UI.TestParamsViewModel();

                var host = new ElementHost();
                host.AutoSize = true;
                host.Dock = DockStyle.Fill;
                host.Child = paramsView;

                PaletteSet.Add("ParamsView ElementHost", host);
            }

            PaletteSet.KeepFocus = true;
            PaletteSet.Visible = true;
        }
    }
}
