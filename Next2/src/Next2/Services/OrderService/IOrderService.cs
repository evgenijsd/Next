﻿using Next2.Helpers;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services
{
    public interface IOrderService
    {
        Task<List<OrderModel>> GetOrdersAsync();
    }
}
