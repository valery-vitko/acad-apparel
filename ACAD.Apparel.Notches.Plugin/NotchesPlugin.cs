using System;
using System.Linq;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace ACAD.Apparel.Notches.Plugin
{
    internal class NotchesPlugin
    {
        private static Logger logger = new Logger("Notches");

        public NotchesPlugin()
        {
            updateDestinationNotchesCommand = new UpdateDestinationNotchesCommand(this);

            Params = new UI.ParamsViewModel
            {
                ReadSelection = new ReadSelectionCommand(this),
                UpdateDestinationNotches = updateDestinationNotchesCommand
            };
        }

        private readonly UpdateDestinationNotchesCommand updateDestinationNotchesCommand;

        private Projector projector;

        public UI.ParamsViewModel Params { get; }


        public void Init()
        {
        }

        public void ReadSelection()
        {
            Params.Clear();
            updateDestinationNotchesCommand.AllowExecution = false;

            var document = Application.DocumentManager.MdiActiveDocument;
            var editor = document.Editor;

            var selectionResult = editor.SelectImplied();
            if (selectionResult.Status == Autodesk.AutoCAD.EditorInput.PromptStatus.Error)
            {
                logger.Error("Error getting selection");
                Params.Error("Error getting selection");
                return;
            }

            var selection = selectionResult.Value;
            using (var tx = document.TransactionManager.StartTransaction())
            {
                var objects = selection.GetObjectIds()
                    .Select(objectId => tx.GetObject(objectId, OpenMode.ForRead))
                    .ToList();

                var curves = objects.OfType<Polyline>().ToList();
                const int polylinesNeededCount = 2;
                if (curves.Count != polylinesNeededCount) // TODO: Add loggog
                {
                    logger.Error($"Error getting selection: {curves.Count} polyline(s) selected instead of {polylinesNeededCount} needed");
                    Params.Error($"Error getting selection: {curves.Count} polyline(s) selected instead of {polylinesNeededCount} needed");
                    return;
                }

                var sourceCurve = curves[0];
                var targetCurve = curves[1];
                var notchLines = objects.OfType<Line>();

                projector = Projector.FromNotchLines(sourceCurve, notchLines, targetCurve);
                UpdateParamsFromProjector();

                tx.Commit();
            }

            updateDestinationNotchesCommand.AllowExecution = true;
        }

        private void UpdateParamsFromProjector()
        {
            var facetStats = projector.GetFacetStats();
            Params.Regenerate(facetStats.SourceLength, facetStats.TargetLength, facetStats.SourceFacetLengths);
        }

        public void UpdateDestinationNotches()
        {
            UpdateProjectorFromParams();

            var document = Application.DocumentManager.MdiActiveDocument;
            using (document.LockDocument())
            {
                using (var tx = document.TransactionManager.StartTransaction())
                {
                    var db = document.Database;
                    var targetNotches = projector.Project();
                    AcadHelpers.GeneratePoints(db, tx, targetNotches);
                    AcadHelpers.ConfigurePointsToBeVisible(db);

                    tx.Commit();
                }
            }
        }

        private void UpdateProjectorFromParams()
        {
            projector.IsInverted = Params.IsInverted;
            var adj = Params.Facets.Select(facet => facet.AdjustmentPercentage);
            projector.TargetFacetAdjustmentPercentages.Clear();
            projector.TargetFacetAdjustmentPercentages.AddRange(adj);
        }

        internal class ReadSelectionCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            private NotchesPlugin state;

            public ReadSelectionCommand(NotchesPlugin state)
            {
                this.state = state;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                state.ReadSelection();
            }
        }

        internal class UpdateDestinationNotchesCommand : ICommand
        {
            private NotchesPlugin state;
            private bool canExecute = false;

            public UpdateDestinationNotchesCommand(NotchesPlugin state)
            {
                this.state = state;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return canExecute;
            }

            public bool AllowExecution
            {
                get { return canExecute; }
                set
                {
                    canExecute = true;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public void Execute(object parameter)
            {
                state.UpdateDestinationNotches();
            }
        }
    }
}
