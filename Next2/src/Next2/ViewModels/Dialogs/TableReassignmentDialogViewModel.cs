using Next2.Models.API.DTO;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class TableReassignmentDialogViewModel : BindableBase
    {
        private IEnumerable<Guid> _collectionOfOrderIdsBeUpdated = Enumerable.Empty<Guid>();
        private EmployeeModelDTO? _tempEmployeeToAssignFrom;

        public TableReassignmentDialogViewModel(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            LoadPageData(parameters);

            RequestClose = requestClose;

            CloseCommand = new Command(() => RequestClose(new DialogParameters()));

            AcceptCommand = new Command(OnAcceptCommand);
        }

        #region -- Public properties --

        public bool IsAllTablesChecked { get; set; }

        public OccupiedTableModelDTO? SelectedTable { get; set; }

        public int SelectedTableNumberClue { get; set; }

        public EmployeeModelDTO? SelectedEmployeeToAssignFrom { get; set; }

        public EmployeeModelDTO? SelectedEmployeeToAssignTo { get; set; }

        public ObservableCollection<OccupiedTableModelDTO> Tables { get; set; } = new();

        public ObservableCollection<EmployeeModelDTO> Employees { get; set; } = new();

        public Action<IDialogParameters> RequestClose;

        public ICommand CloseCommand { get; }

        public ICommand AcceptCommand { get; }

        private ICommand? _selectDeselectTablesCommand;
        public ICommand? SelectDeselectTablesCommand => _selectDeselectTablesCommand ??= new AsyncCommand(OnSelectDeselectTablesCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _selectEmployeeToAssignFromCommand;
        public ICommand? SelectEmployeeToAssignFromCommand => _selectEmployeeToAssignFromCommand ??= new AsyncCommand(OnSelectEmployeeToAssignFromCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(SelectedTable) && SelectedTable is not null)
            {
                SelectedTableNumberClue = SelectedTable.Number;
            }
        }

        #endregion

        #region -- Private helpers --

        private void LoadPageData(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.EMPLOYEES, out IEnumerable<EmployeeModelDTO> employees))
            {
                Employees = new(employees);
            }
        }

        private Task OnSelectDeselectTablesCommandAsync()
        {
            IsAllTablesChecked = !IsAllTablesChecked;

            return Task.CompletedTask;
        }

        private Task OnSelectEmployeeToAssignFromCommandAsync()
        {
            SelectedEmployeeToAssignTo = null;

            SelectedTable = null;

            if (SelectedEmployeeToAssignFrom is not null)
            {
                if (_tempEmployeeToAssignFrom is not null)
                {
                    Employees.Add(_tempEmployeeToAssignFrom);
                }

                Employees.Remove(SelectedEmployeeToAssignFrom);

                Employees = new(Employees);

                Tables = new(SelectedEmployeeToAssignFrom.Tables?.OrderBy(row => row?.Number));

                _tempEmployeeToAssignFrom = new()
                {
                    EmployeeId = SelectedEmployeeToAssignFrom.EmployeeId,
                    UserName = SelectedEmployeeToAssignFrom.UserName,
                    Tables = SelectedEmployeeToAssignFrom.Tables,
                };
            }

            return Task.CompletedTask;
        }

        private void OnAcceptCommand()
        {
            if (IsAllTablesChecked)
            {
                var collectionOfOrdersIdsOfEachTable = Tables.Select(row => row.OrdersId);

                foreach (var collectionOfOrdersIdsOfSingleTable in collectionOfOrdersIdsOfEachTable)
                {
                    if (collectionOfOrdersIdsOfSingleTable is not null)
                    {
                        foreach (var orderId in collectionOfOrdersIdsOfSingleTable)
                        {
                            _collectionOfOrderIdsBeUpdated = _collectionOfOrderIdsBeUpdated.Concat(new[] { orderId });
                        }
                    }
                }
            }
            else if (SelectedTable is not null && SelectedTable.OrdersId is not null)
            {
                _collectionOfOrderIdsBeUpdated = SelectedTable.OrdersId;
            }

            RequestClose(new DialogParameters()
            {
                { Constants.DialogParameterKeys.ACCEPT, true },
                { Constants.DialogParameterKeys.EMPLOYEE_ID, SelectedEmployeeToAssignTo?.EmployeeId },
                { Constants.DialogParameterKeys.ORDERS_ID, _collectionOfOrderIdsBeUpdated },
            });
        }

        #endregion
    }
}