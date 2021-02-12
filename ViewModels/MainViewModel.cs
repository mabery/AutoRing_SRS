using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
        public ICommand NewRingInnerCommand => new RelayCommand(NewRingInner);
        public ICommand NewRingMiddleCommand => new RelayCommand(NewRingMiddle);
        public ICommand NewRingOuterCommand => new RelayCommand(NewRingOuter);
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

        private void CreateRings()
        {
            string selectedStructureSetId = SelectedStructureSet?.StructureSetId;
            string ptvId = SelectedStructure?.StructureId;
            double ptvVolume = (double)(SelectedStructure?.StructureVolume);
            string ringInnerId = SelectedStructureRingInner?.StructureId;
            string ringMiddleId = SelectedStructureRingMiddle?.StructureId;
            string ringOuterId = SelectedStructureRingOuter?.StructureId;

            if (ringInnerId == null)
                ringInnerId = "Ring_Inner";
            else
            {
                if (SelectedStructureRingInner.CanModify == false)
                {
                    ringInnerId = "Ring_Inner1";
                }
                else
                {
                    for (int i = 2; i < 5; i++)
                    {
                        var possibleStructureId = "Ring_Inner" + i.ToString();
                        if (SelectedStructureRingInner.StructureId == possibleStructureId)
                        {
                            if (SelectedStructureRingInner.CanModify == false)
                                continue;
                            else
                            {
                                ringInnerId = possibleStructureId;
                                break;
                            }
                        }
                    }
                }    
                            
            }
            if (ringMiddleId == null)
                ringMiddleId = "Ring_Middle";
            else
            {
                if (SelectedStructureRingMiddle.CanModify == false)
                {
                    ringInnerId = "Ring_Middle1";
                }
                else
                {
                    for (int i = 2; i < 5; i++)
                    {
                        var possibleStructureId = "Ring_Middle" + i.ToString();
                        if (SelectedStructureRingMiddle.StructureId == possibleStructureId)
                        {
                            if (SelectedStructureRingMiddle.CanModify == false)
                                continue;
                            else
                            {
                                ringInnerId = possibleStructureId;
                                break;
                            }
                        }
                    }
                }

            }
            if (ringOuterId == null)
                ringOuterId = "Ring_Outer";
            else
            {
                if (SelectedStructureRingOuter.CanModify == false)
                {
                    ringInnerId = "RingOuter1";
                }
                else
                {
                    for (int i = 2; i < 5; i++)
                    {
                        var possibleStructureId = "Ring_Outer" + i.ToString();
                        if (SelectedStructureRingOuter.StructureId == possibleStructureId)
                        {
                            if (SelectedStructureRingOuter.CanModify == false)
                                continue;
                            else
                            {
                                ringInnerId = possibleStructureId;
                                break;
                            }
                        }
                    }
                }
            }
            double innerMargin;
            double middleMargin;
            double outerMargin;

            if (ptvVolume < 0.5)  //very small
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

        private void NewRingInner()
        {
            
        }

        private void NewRingMiddle()
        {

        }
        private void NewRingOuter()
        {

        }
    }
}
