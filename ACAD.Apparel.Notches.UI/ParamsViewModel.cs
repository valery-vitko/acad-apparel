using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ACAD.Apparel.Notches.UI
{
    public class ParamsViewModel : ObservableObject
    {
        public double LengthSource { get; set; }
        public double LengthDestination { get; set; }
        public double Difference => LengthSource - LengthDestination;

        public ObservableCollection<NotchFacetViewModel> Facets { get; set; } = new ObservableCollection<NotchFacetViewModel>();
    }

    public class NotchFacetViewModel : ObservableObject
    {
        public int FromNotch { get; set; }
        public int ToNotch { get; set; }
        public string Label => $"{FromNotch} - {ToNotch}";
        public double SourceLength { get; set; }
        public double DestinationLength { get; set; }
        public double AdjustmentPercentage { get; set; } = 0;
    }

    public class TestParamsViewModel : ParamsViewModel
    {
        public TestParamsViewModel()
        {
            LengthSource = 100.1;
            LengthDestination = 110;

            Facets.Add(new NotchFacetViewModel { FromNotch = 0, ToNotch = 1, SourceLength = 5 });
            Facets.Add(new NotchFacetViewModel { FromNotch = 1, ToNotch = 3, SourceLength = 5.1 });
            Facets.Add(new NotchFacetViewModel { FromNotch = 3, ToNotch = 4, SourceLength = 5.1 });
            Facets.Add(new NotchFacetViewModel { FromNotch = 4, ToNotch = 5, SourceLength = 5.1 });
            Facets.Add(new NotchFacetViewModel { FromNotch = 5, ToNotch = 6, SourceLength = 5.1 });
        }
    }
}
