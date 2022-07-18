using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.Messages
{
    public interface IMessagesService
    {
        Task ShowInfoDialogAsync(string titleText, string descriptionText, string okText);

        Task ResponseToBadRequestAsync(string statusCode);
    }
}
