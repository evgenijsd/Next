using Next2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.MockService
{
    public static class CustomersMock
    {
        public static void Create(out List<CustomerModel> result)
        {
            var customers = new List<CustomerModel>()
            {
                new CustomerModel()
                {
                    Id = 1,
                    Name = "Martin Schleifer",
                    Email = "MartSh@gmail.com",
                    Phone = "732-902-8298",
                    Points = 221,
                    Rewards = 13,
                    GiftCardCount = 8,
                    GiftCardTotal = 30.02,
                },
                new CustomerModel()
                {
                    Id = 2,
                    Name = "Carla Dorwart",
                    Email = "Carla_123@gmail.com",
                    Phone = "732-302-8538",
                    Points = 22,
                    Rewards = 1,
                    GiftCardCount = 1,
                    GiftCardTotal = 1,
                },
                new CustomerModel()
                {
                    Id = 3,
                    Name = "Davis Septimus",
                    Email = "Davbich@gmail.com",
                    Phone = "733-901-8244",
                    Points = 108,
                    Rewards = 23,
                    GiftCardCount = 5,
                    GiftCardTotal = 12.5,
                },
                new CustomerModel()
                {
                    Id = 4,
                    Name = "Kierra Bergson",
                    Email = "Kia@yaho.com",
                    Phone = "709-502-5598",
                    Points = 451,
                    Rewards = 33,
                    GiftCardCount = 17,
                    GiftCardTotal = 50.02,
                },
                new CustomerModel()
                {
                    Id = 5,
                    Name = "Angel Dias",
                    Email = "Diazzz@yaho.com",
                    Phone = "732-902-8298",
                    Points = 22,
                    Rewards = 3,
                    GiftCardCount = 4,
                    GiftCardTotal = 5,
                },
                new CustomerModel()
                {
                    Id = 6,
                    Name = "Kaiya Dorwart",
                    Email = "KaDor@gmail.com",
                    Phone = "444-912-6298",
                    Points = 34,
                    Rewards = 4,
                    GiftCardCount = 2,
                    GiftCardTotal = 10.02,
                },
                new CustomerModel()
                {
                    Id = 7,
                    Name = "Lincoln Lipshutz",
                    Email = "liliputz@yaho.com",
                    Phone = "452-955-8228",
                    Points = 35,
                    Rewards = 8,
                    GiftCardCount = 2,
                    GiftCardTotal = 9,
                },
                new CustomerModel()
                {
                    Id = 8,
                    Name = "Randy Mango",
                    Email = "mango_jango@gmail.com",
                    Phone = "332-456-2363",
                    Points = 123,
                    Rewards = 4,
                    GiftCardCount = 3,
                    GiftCardTotal = 12.99,
                },
            };
            result = customers;
        }
    }
}
