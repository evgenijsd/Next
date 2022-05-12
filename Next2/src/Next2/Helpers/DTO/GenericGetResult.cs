using Newtonsoft.Json;
using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers.DTO
{
    public class GenericGetResult<T>
    {
        public T? Result { get; set; }
    }
}
