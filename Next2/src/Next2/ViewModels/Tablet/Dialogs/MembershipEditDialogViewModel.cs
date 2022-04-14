using AutoMapper;
using Next2.Models;
using Next2.Services.Membership;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class MembershipEditDialogViewModel : BindableBase
    {
        private readonly IMapper _mapper;

        public MembershipEditDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose,
            IMapper mapper)
        {
            _mapper = mapper;
            SetupParameters(param);
            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(new DialogParameters()));
        }

        #region -- Public properties --

        private MemberBindableModel Member; // { get; set; }

        public DateTime? SelectedStartDate { get; set; }

        public DateTime? SelectedEndDate { get; set; }

        public string CustomerName { get; set; }

        public Action<IDialogParameters> RequestClose;

        public ICommand CloseCommand { get; }

        public ICommand _DisableMembershipCommand;

        public ICommand DisableMembershipCommand => _DisableMembershipCommand ??= new AsyncCommand(OnDisableMembershipCommand, allowsMultipleExecutions: false);

        public ICommand _SaveMembershipCommand;

        public ICommand SaveMembershipCommand => _SaveMembershipCommand ??= new AsyncCommand(OnSaveMembershipCommand, allowsMultipleExecutions: false);

        #endregion

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(SelectedStartDate) or nameof(SelectedEndDate))
            {
                int i = 0;
            }
        }

        #region --Private Helpers--

        private Task OnDisableMembershipCommand()
        {
            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.DISABLE, Member } };

            RequestClose(dialogParameters);

            return Task.CompletedTask;
        }

        private Task OnSaveMembershipCommand()
        {
            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.SAVE, Member } };

            RequestClose(dialogParameters);

            return Task.CompletedTask;
        }

        private void SetupParameters(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.MODEL, out MemberBindableModel member))
            {
                Member = _mapper.Map<MemberBindableModel, MemberBindableModel>(member);

                //SelectedStartDate = Member.MembershipStartTime;
                //SelectedEndDate = Member.MembershipEndTime;
            }
        }

        #endregion
    }
}
