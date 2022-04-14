using AutoMapper;
using Next2.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class MembershipEditViewModel : BindableBase
    {
        private readonly IMapper _mapper;

        public MembershipEditViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose,
            IMapper mapper)
        {
            _mapper = mapper;
            SetupParameters(param);

            RequestClose = requestClose;
        }

        public DateTime? SelectedDate { get; set; } = null;

        public DateTime? SelectedEndDate { get; set; } = null;

        public MemberBindableModel Member { get; set; }

        public Action<IDialogParameters> RequestClose;

        public ICommand _CloseCommand;

        public ICommand CloseCommand => _CloseCommand ??= new AsyncCommand(OnCloseCommand, allowsMultipleExecutions: false);

        public ICommand _DisableMembershipCommand;

        public ICommand DisableMembershipCommand => _DisableMembershipCommand ??= new AsyncCommand(OnDisableMembershipCommand, allowsMultipleExecutions: false);

        public ICommand _SaveMembershipCommand;

        public ICommand SaveMembershipCommand => _SaveMembershipCommand ??= new AsyncCommand(OnSaveMembershipCommand, allowsMultipleExecutions: false);

        #region --Private Helpers--

        private Task OnCloseCommand()
        {
            RequestClose(new DialogParameters());

            return Task.CompletedTask;
        }

        private Task OnDisableMembershipCommand()
        {
            if (Member.MembershipEndTime > DateTime.Now)
            {
                Member.MembershipEndTime = DateTime.Now;
            }

            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.UPDATE, Member } };

            RequestClose(dialogParameters);

            return Task.CompletedTask;
        }

        private Task OnSaveMembershipCommand()
        {
            Member.MembershipStartTime = SelectedDate ?? Member.MembershipStartTime;
            Member.MembershipEndTime = SelectedEndDate ?? Member.MembershipEndTime;

            if (Member.MembershipStartTime > Member.MembershipEndTime)
            {
                Member.MembershipEndTime = Member.MembershipStartTime;
            }

            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.UPDATE, Member } };

            RequestClose(dialogParameters);

            return Task.CompletedTask;
        }

        private void SetupParameters(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.MODEL, out MemberBindableModel member))
            {
                Member = _mapper.Map<MemberBindableModel, MemberBindableModel>(member);

                SelectedDate = Member.MembershipStartTime;
                SelectedEndDate = Member.MembershipEndTime;
            }
        }

        #endregion
    }
}
