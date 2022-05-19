using Next2.Models.Api.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.Api.Results
{
    public class GetGiftCardsListQueryResult
    {
        public IEnumerable<GiftCardModelDTO> GiftCards { get; set; }
    }
}
