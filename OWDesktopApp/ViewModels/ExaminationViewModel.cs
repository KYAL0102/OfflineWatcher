using Core.Contracts;
using Core.Entities;
using MVVMBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDesktopApp.ViewModels
{
    public class ExaminationViewModel : BaseViewModel
    {

        private readonly IUnitOfWork _uow;
        
        private string _patientSsid = string.Empty;
        public string PatientSsid
        {
            get => _patientSsid;
            set
            {
                _patientSsid = value;
                OnPropertyChanged();
            }
        }
        
        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }
        
        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        private Examination? _selectedExamination = null;
        public Examination? SelectedExamination
        {
            get => _selectedExamination;
            set
            {
                _selectedExamination = value;
                if(_selectedExamination != null) DataStreams = _selectedExamination.DataStreams;
                OnPropertyChanged();
            }
        }
        
        private IList<DataStream> _dataStreams = new List<DataStream>();
        public IList<DataStream> DataStreams
        {
            get => _dataStreams;
            set
            {
                _dataStreams = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Examination> Examinations { get; set; } = new();
        
        public RelayCommand CloseCommand { get; set; }
        public ExaminationViewModel(WindowController? controller, IUnitOfWork uow, string ssid) : base(controller)
        {
            _uow = uow;
            PatientSsid = ssid;
            CloseCommand = new RelayCommand(
                (_) => Controller!.CloseWindow(),
                (_) => true);
        }

        public async Task InitializeDataAsync()
        {
            var patient = await _uow.Patients.GetPatientBySSID(PatientSsid);

            if (patient != null)
            {
                FirstName = patient.FirstName ?? string.Empty;
                LastName = patient.LastName ?? string.Empty;

                if (patient.Examinations != null) foreach (var examination in patient.Examinations) Examinations.Add(examination);
            }
            
            await Task.CompletedTask;
        }
    }
}
