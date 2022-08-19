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

        public TableModelDTO SelectedTable { get; set; }

        public ObservableCollection<TableModelDTO> Tables { get; set; } = new();

        public EmployeeModelDTO SelectedEmployee { get; set; }

        public ObservableCollection<EmployeeModelDTO> Employees { get; set; } = new();

        public bool CanAssingReservation { get; set; }

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand DeclineCommand { get; }

        private ICommand? _assignReservationCommand;
        public ICommand AssignReservationCommand => _assignReservationCommand ??= new AsyncCommand(OnAssignReservationCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName
                is nameof(SelectedTable)
                or nameof(SelectedEmployee))
            {
                CanAssingReservation = SelectedTable is not null && SelectedEmployee is not null;
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
                SelectedTable = table;
                SelectedEmployee = employee;

                Tables = new(tables);
                Employees = new(employees);
            }
        }

        private Task OnAssignReservationCommandAsync()
        {
            var parameters = new DialogParameters();

            if (CanAssingReservation)
            {
                parameters.Add(Constants.DialogParameterKeys.EMPLOYEE, SelectedEmployee);
                parameters.Add(Constants.DialogParameterKeys.TABLE, SelectedTable);
            }

            RequestClose(parameters);

            return Task.CompletedTask;
        }

        #endregion
    }
}
