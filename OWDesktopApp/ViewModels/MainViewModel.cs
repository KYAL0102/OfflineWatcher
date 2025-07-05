using Core.Contracts;
using Microsoft.Win32;
using MVVMBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Core.DataTransferObjects;
using Core.Entities;
using WpfDesktopApp.Views;

namespace WpfDesktopApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IUnitOfWork _uow;
        public ObservableCollection<PatientDTO> Patients { get; set; } = new();

        private bool _isPatientSelected = false;
        public bool IsPatientSelected
        {
            get => _isPatientSelected;
            set
            {
                _isPatientSelected = value;
                OnPropertyChanged();
            }
        }
        private PatientDTO? _selectedPatient;
        public PatientDTO? SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                IsPatientSelected = value != null;
                _selectedPatient = value;
                OnPropertyChanged();
                
                var firstName = _selectedPatient?.FirstName ?? string.Empty;
                var lastName = _selectedPatient?.LastName ?? string.Empty;
                _originalFirstName = firstName;
                _originalLastName = lastName;
                PatientFirstName = firstName;
                PatientLastName = lastName;
            }
        }
        
        private string _originalFirstName = string.Empty;
        private string _firstName = string.Empty;
        public string PatientFirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }
        
        private string _originalLastName = string.Empty;
        private string _lastName = string.Empty;
        public string PatientLastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }
        
        public RelayCommand DetailsCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }
        public RelayCommand ChangeCommand { get; set; }
        bool _checkBox = false;
        public RelayCommand CheckBoxCommand { get; set; }
        public RelayCommand FilterNoNamePatientsCommand { get; set; }
        public MainViewModel(WindowController? controller, IUnitOfWork uow) : base(controller)
        {
            _uow = uow;

            DetailsCommand = new RelayCommand(
                async (_) => await OpenExaminationDetails(SelectedPatient!.SSID),
                (_) => SelectedPatient != null);
            ImportCommand = new RelayCommand(
                (_) => ImportExamination(),
                (_) => true);
            ChangeCommand = new RelayCommand(
                async (_) => await ChangeFirstAndLastNameAsync(),
                (_) => SelectedPatient != null && (_originalFirstName != PatientFirstName || _originalLastName != PatientLastName));
            CheckBoxCommand = new RelayCommand(
                (_) => _checkBox = !_checkBox,
                (_) => true);
            FilterNoNamePatientsCommand = new RelayCommand(
                async (_) => await FilterNoNamePatients(),
                (_) => true);
        }

        public async Task InitializeDataAsync()
        {
            await UpdatePatientsAsync();
            
            await Task.CompletedTask;
        }

        private async Task UpdatePatientsAsync()
        {
            Patients.Clear();
            var patients = (await _uow.Patients.GetAllPatients()).OrderBy(p => p.SSID).ToList();
            foreach (var patient in patients) Patients.Add(patient);
        }

        public async Task ChangeFirstAndLastNameAsync()
        {
            await _uow.Patients.UpdateFirstAndLastNameOfPatient(SelectedPatient!.SSID, PatientFirstName, PatientLastName);
            SelectedPatient = null;
            PatientFirstName = string.Empty;
            PatientLastName = string.Empty;

            await UpdatePatientsAsync();
        }

        public async Task OpenExaminationDetails(string ssid)
        {
            var examinationController = new WindowController(new ExaminationWindow());
            var examinationViewModel = new ExaminationViewModel(examinationController, _uow, ssid);
            await examinationViewModel.InitializeDataAsync();
            examinationController.ShowDialog(examinationViewModel);
        }

        public void ImportExamination()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            if (fileDialog.ShowDialog() == true)
            {
                string filePath = fileDialog.FileName;
                
            }
        }
        
        public async Task FilterNoNamePatients()
        {
            if (_checkBox)
            {
                Patients.Where(p => string.IsNullOrEmpty(p.FirstName) || string.IsNullOrEmpty(p.LastName)).ToList()
                    .ForEach(p => Patients.Remove(p));
            }
            else await UpdatePatientsAsync();
        }
    }
}
