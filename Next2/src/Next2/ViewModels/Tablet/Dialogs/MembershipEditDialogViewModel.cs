using AutoMapper;
using Next2.Models;
using Next2.Services.Membership;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
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
            DisableMembershipCommand = new Command(OnDisableMembershipCommand);
            SaveMembershipCommand = new Command(OnSaveMembershipCommand);
        }

        #region -- Public properties --

        public MemberBindableModel Member { get; set; }

        public Action<IDialogParameters> RequestClose;

        public ICommand CloseCommand { get; }

        public ICommand DisableMembershipCommand { get; }

        public ICommand SaveMembershipCommand { get; }

        #endregion

        #region --Private Helpers--

        private void OnDisableMembershipCommand()
        {
            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.DISABLE, Member } };

            RequestClose(dialogParameters);
        }

        private void OnSaveMembershipCommand()
        {
            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.SAVE, Member } };

            RequestClose(dialogParameters);
        }

        private void SetupParameters(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.MODEL, out MemberBindableModel member))
            {
                Member = _mapper.Map<MemberBindableModel, MemberBindableModel>(member);
            }
        }

        #endregion
    }
}
