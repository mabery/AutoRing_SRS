using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Input;

namespace AutoRingSRS
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEsapiService _esapiService;
        private readonly IDialogService _dialogService;
        public MainViewModel(IEsapiService esapiService, IDialogService dialogService)
        {
            _esapiService = esapiService;
            _dialogService = dialogService;
        }
        private Struct[] _structures;
        public Struct[] Structures
        {
            get => _structures;
            set => Set(ref _structures, value);
        }
        private Struct[] _structuresRingInner;
        public Struct[] StructuresRingInner
        {
            get => _structuresRingInner;
            set => Set(ref _structuresRingInner, value);
        }
        private Struct[] _structuresRingMiddle;
        public Struct[] StructuresRingMiddle
        {
            get => _structuresRingMiddle;
            set => Set(ref _structuresRingMiddle, value);
        }
        private Struct[] _structuresRingOuter;
        public Struct[] StructuresRingOuter
        {
            get => _structuresRingOuter;
            set => Set(ref _structuresRingOuter, value);
        }
        private StructSet[] _structureSets;
        public StructSet[] StructureSets
        {
            get => _structureSets;
            set => Set(ref _structureSets, value);
        }
        private StructSet _selectedStructureSet;
        public StructSet SelectedStructureSet
        {
            get => _selectedStructureSet;
            set => Set(ref _selectedStructureSet, value);
        }
        private Struct _selectedStructure;
        public Struct SelectedStructure
        {
            get => _selectedStructure;
            set => Set(ref _selectedStructure, value);
        }
        private Struct _selectedStructureRingInner;
        public Struct SelectedStructureRingInner
        {
            get => _selectedStructureRingInner;
            set => Set(ref _selectedStructureRingInner, value);
        }
        private Struct _selectedStructureRingMiddle;
        public Struct SelectedStructureRingMiddle
        {
            get => _selectedStructureRingMiddle;
            set => Set(ref _selectedStructureRingMiddle, value);
        }
        private Struct _selectedStructureRingOuter;
        public Struct SelectedStructureRingOuter
        {
            get => _selectedStructureRingOuter;
            set => Set(ref _selectedStructureRingOuter, value);
        }
        public ICommand StartCommand => new RelayCommand(Start);
        public ICommand GetStructuresCommand => new RelayCommand(GetStructures);
        public ICommand GetRingsCommand => new RelayCommand(CreateRings);
        private async void Start()
        {
            StructureSets = await _esapiService.GetStructureSetsAsync();
        }

        private async void GetStructures()
        {
            Structures = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "TV");
            StructuresRingInner = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "Ring_Inner");
            StructuresRingMiddle = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "Ring_Middle");
            StructuresRingOuter = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "Ring_Outer");
        }

        private async void CreateRings()
        {
            string selectedStructureSetId = SelectedStructureSet?.StructureSetId;
            string ptvId = SelectedStructure?.StructureId;
            double ptvVolume = (double)(SelectedStructure?.StructureVolume);
            string ringInnerId = SelectedStructureRingInner?.StructureId;
            string ringMiddleId = SelectedStructureRingMiddle?.StructureId;
            string ringOuterId = SelectedStructureRingOuter?.StructureId;

            if (ringInnerId == "<Create new structure>")
                ringInnerId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "Ring_Inner");                      
            if (ringMiddleId == "<Create new structure>")
                ringMiddleId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "Ring_Middle");
            if (ringOuterId == "<Create new structure>")
                ringOuterId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "Ring_Outer");

            double innerMargin;
            double middleMargin;
            double outerMargin;

            if (ptvVolume <= 0.5)  //very small
            {
                innerMargin = 0.2;
                middleMargin = 0.5;
                outerMargin = 1.5;
            }
            else  //normal or large size shells
            {
                innerMargin = 0.5;
                middleMargin = 1.0;
                outerMargin = 3.0;
            }

            _dialogService.ShowProgressDialog("Adding rings...",
                async progress =>
                {
                    await _esapiService.AddRingAsync(selectedStructureSetId, ptvId, ringInnerId, ringMiddleId, ringOuterId, innerMargin * 10, middleMargin * 10, outerMargin * 10);
                });
        }
    }
}
