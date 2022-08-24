using Next2.Models.API.DTO;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Tablet.Dialogs
{
    public class AssignReservationDialogViewModel : BindableBase
    {
        public AssignReservationDialogViewModel(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;

            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters()));

            InitData(parameters);
        }

        #region -- Public properties --

        private TableModelDTO _selectedTable;
        public TableModelDTO SelectedTable
        {
            get => _selectedTable;
            set => SetProperty(ref _selectedTable, value);
        }

        public int SelectedTableNumber { get; set; }

        public ObservableCollection<TableModelDTO> Tables { get; set; } = new();

        private EmployeeModelDTO _selectedEmployee;
        public EmployeeModelDTO SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetProperty(ref _selectedEmployee, value);
        }

        public string SelectedEmployeeUserName { get; set; } = string.Empty;

        public ObservableCollection<EmployeeModelDTO> Employees { get; set; } = new();

        public bool IsValidSelectedEmployee { get; set; } = true;

        public bool IsValidSelectedTable { get; set; } = true;

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand DeclineCommand { get; }

        private ICommand? _assignReservationCommand;
        public ICommand AssignReservationCommand => _assignReservationCommand ??= new AsyncCommand(OnAssignReservationCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(SelectedEmployee):
                    IsValidSelectedEmployee = SelectedEmployee is not null;
                    SelectedEmployeeUserName = SelectedEmployee.UserName;

                    break;
                case nameof(SelectedTable):
                    IsValidSelectedTable = SelectedTable is not null;
                    SelectedTableNumber = SelectedTable.Number;

                    break;
            }
        }

        #endregion

        #region -- Private helpers --

        private void InitData(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.TABLES, out IEnumerable<TableModelDTO> tables)
                && parameters.TryGetValue(Constants.DialogParameterKeys.EMPLOYEES, out IEnumerable<EmployeeModelDTO> employees)
                && parameters.TryGetValue(Constants.DialogParameterKeys.TABLE, out TableModelDTO table)
                && parameters.TryGetValue(Constants.DialogParameterKeys.EMPLOYEE, out EmployeeModelDTO employee))
            {
                _selectedTable = table;
                _selectedEmployee = employee;

                if (table is not null)
                {
                    SelectedTableNumber = table.Number;
                }

                if (employee is not null)
                {
                    SelectedEmployeeUserName = employee.UserName;
                }

                Tables = new(tables);
                Employees = new(employees);
            }
        }

        private Task OnAssignReservationCommandAsync()
        {
            IsValidSelectedEmployee = SelectedEmployee is not null;
            IsValidSelectedTable = SelectedTable is not null;

            if (IsValidSelectedTable && IsValidSelectedEmployee)
            {
                var parameters = new DialogParameters()
                {
                    { Constants.DialogParameterKeys.EMPLOYEE, SelectedEmployee },
                    { Constants.DialogParameterKeys.TABLE, SelectedTable },
                };

                RequestClose(parameters);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
