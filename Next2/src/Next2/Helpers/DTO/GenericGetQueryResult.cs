using Newtonsoft.Json;
using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO
{
    public class GenericGetQueryResult<T>
    {
        [JsonProperty(PropertyName = "memberships")]
        public T? Result { get; set; }
    }
}
