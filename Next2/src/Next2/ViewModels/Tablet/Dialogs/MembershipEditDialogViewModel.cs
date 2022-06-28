using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

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

        #region -- Public properties --

        public DateTime? SelectedDate { get; set; } = null;

        public DateTime? SelectedEndDate { get; set; } = null;

        public MemberBindableModel Member { get; set; }

        public Action<IDialogParameters> RequestClose;

        private ICommand _closeCommand;

        public ICommand CloseCommand => _closeCommand ??= new AsyncCommand(OnCloseCommandAsync, allowsMultipleExecutions: false);

        private ICommand _membershipEditCommand;

        public ICommand MembershipEditCommand => _membershipEditCommand ??= new AsyncCommand<EMembershipEditType>(OnMembershipEditCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnCloseCommandAsync()
        {
            RequestClose(new DialogParameters());

            return Task.CompletedTask;
        }

        private Task OnMembershipEditCommandAsync(EMembershipEditType editType)
        {
            switch (editType)
            {
                case EMembershipEditType.Disable:
                    if (Member.EndDate > DateTime.Now)
                    {
                        Member.EndDate = DateTime.Now;
                    }

                    break;

                case EMembershipEditType.Save:
                    TimeSpan time = new(1, 0, 0);
                    Member.StartDate = (SelectedDate ?? Member.StartDate) + time;
                    time = new(23, 0, 0);
                    Member.EndDate = (SelectedEndDate ?? Member.EndDate) + time;

                    if (Member.StartDate > Member.EndDate)
                    {
                        Member.EndDate = Member.StartDate;
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

                SelectedDate = Member.StartDate;
                SelectedEndDate = Member.EndDate;
            }
        }

        #endregion
    }
}
