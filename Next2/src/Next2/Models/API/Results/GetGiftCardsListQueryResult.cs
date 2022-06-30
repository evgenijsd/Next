using Next2.Models.API.DTO;
using System.Collections.Generic;

namespace Next2.Models.API.Results
{
    public class GetGiftCardsListQueryResult
    {
        public IEnumerable<GiftCardModelDTO> GiftCards { get; set; }
    }
}
