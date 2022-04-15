using AutoMapper;
using Next2.Enums;
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
    public class MembershipEditDialogViewModel : BindableBase
    {
        private readonly IMapper _mapper;

        public MembershipEditDialogViewModel(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose,
            IMapper mapper)
        {
            _mapper = mapper;
            SetupParameters(parameters);

            RequestClose = requestClose;
        }

        public DateTime? SelectedDate { get; set; } = null;

        public DateTime? SelectedEndDate { get; set; } = null;

        public MemberBindableModel Member { get; set; }

        public Action<IDialogParameters> RequestClose;

        private ICommand _CloseCommand;

        public ICommand CloseCommand => _CloseCommand ??= new AsyncCommand(OnCloseCommand, allowsMultipleExecutions: false);

        private ICommand _MembershipEditCommand;

        public ICommand MembershipEditCommand => _MembershipEditCommand ??= new AsyncCommand<EMembershipEditType>(OnMembershipEditCommand, allowsMultipleExecutions: false);

        #region --Private Helpers--

        private Task OnCloseCommand()
        {
            RequestClose(new DialogParameters());

            return Task.CompletedTask;
        }

        private Task OnMembershipEditCommand(EMembershipEditType editType)
        {
            switch (editType)
            {
                case EMembershipEditType.Disable:
                    if (Member.MembershipEndTime > DateTime.Now)
                    {
                        Member.MembershipEndTime = DateTime.Now;
                    }

                    break;

                case EMembershipEditType.Save:
                    Member.MembershipStartTime = SelectedDate ?? Member.MembershipStartTime;
                    Member.MembershipEndTime = SelectedEndDate ?? Member.MembershipEndTime;

                    if (Member.MembershipStartTime > Member.MembershipEndTime)
                    {
                        Member.MembershipEndTime = Member.MembershipStartTime;
                    }

                    break;
                default:
                    break;
            }

            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.UPDATE, Member } };

            RequestClose(dialogParameters);

            return Task.CompletedTask;
        }

        private void SetupParameters(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.MODEL, out MemberBindableModel member))
            {
                Member = _mapper.Map<MemberBindableModel, MemberBindableModel>(member);

                SelectedDate = Member.MembershipStartTime;
                SelectedEndDate = Member.MembershipEndTime;
            }
        }

        #endregion
    }
}
