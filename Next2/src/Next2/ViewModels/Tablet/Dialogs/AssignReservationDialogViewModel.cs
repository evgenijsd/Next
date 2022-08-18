using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Tablet.Dialogs
{
    public class AssignReservationDialogViewModel : BindableBase
    {
        public AssignReservationDialogViewModel(Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;

            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters()));

            Tables = new(Enumerable.Range(1, 10));
            Servers = new(Enumerable.Range(1, 25));
        }

        #region -- Public properties --

        public int SelectedTable { get; set; }

        public ObservableCollection<int> Tables { get; set; } = new();

        public int SelectedServer { get; set; }

        public ObservableCollection<int> Servers { get; set; } = new();

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
                or nameof(SelectedServer))
            {
                CanAssingReservation = SelectedTable > 0 && SelectedServer > 0;
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnAssignReservationCommandAsync()
        {
            var param = new DialogParameters();

            if (CanAssingReservation)
            {
            }

            RequestClose(param);

            return Task.CompletedTask;
        }

        #endregion
    }
}
