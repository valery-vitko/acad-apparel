using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ACAD.Apparel.Notches.UI
{
    public class ParamsViewModel : ObservableObject
    {
        public double SourceLength { get; set; }
        public double TargetLength { get; set; }
        public double Adjustment => TargetLength - SourceLength;

        public bool IsInverted { get; set; } = false;

        public ObservableCollection<NotchFacetViewModel> Facets { get; } = new ObservableCollection<NotchFacetViewModel>();

        public ICommand ReadSelection { get; set; }
        public ICommand UpdateDestinationNotches { get; set; }

        public void Regenerate(double sourceLength, double targetLength, IList<double> sourceFacetLengths)
        {
            SourceLength = sourceLength;
            TargetLength = targetLength;

            var totalAdjustment = Adjustment;

            Facets.Clear();

            for (int i = 0; i < sourceFacetLengths.Count; i++)
                Facets.Add(new NotchFacetViewModel(i, sourceFacetLengths[i], totalAdjustment));
        }

        public void Clear()
        {
            SourceLength = 0;
            TargetLength = 0;
            Facets.Clear();
        }

        public void Error(string message)
        {
            // TODO
        }
    }

    public class NotchFacetViewModel : ObservableObject
    {
        private readonly int index;
        private readonly double totalAdjustment;

        public NotchFacetViewModel(int index, double sourceLength, double totalAdjustment)
        {
            this.index = index;
            SourceLength = sourceLength;
            this.totalAdjustment = totalAdjustment;
        }

        public string Label => $"{index} - {index + 1}";
        public double SourceLength { get; }
        public double AdjustmentPercentage { get; set; } = 0;
        public double TargetLength => SourceLength + totalAdjustment * AdjustmentPercentage / 100.0;
    }

    public class TestParamsViewModel : ParamsViewModel
    {
        public TestParamsViewModel()
        {
            SourceLength = 100.1;
            TargetLength = 110;

            Facets.Add(new NotchFacetViewModel(0, 5, Adjustment));
            Facets.Add(new NotchFacetViewModel(1, 5.1, Adjustment));
            Facets.Add(new NotchFacetViewModel(3, 5.1, Adjustment));
            Facets.Add(new NotchFacetViewModel(4, 5.1, Adjustment));
            Facets.Add(new NotchFacetViewModel(5, 5.1, Adjustment));
        }
    }
}
