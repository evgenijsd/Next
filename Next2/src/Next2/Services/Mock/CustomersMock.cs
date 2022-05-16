using Next2.Models;
using Next2.Services.Mock;
using System.Collections.Generic;

namespace Next2.Services
{
    public static class CustomersMock
    {
        public static List<CustomerModel> Create()
        {
            return new List<CustomerModel>()
            {
                new CustomerModel()
                {
                    Name = "Adam Brody",
                    Email = "adambrody@gmail.com",
                    Phone = "456-112-5298",
                    Points = 221,
                    Rewards = 4,
                    GiftCardsCount = 2,
                    GiftCardsTotalFund = 60.50f,
                    GiftCards = new List<GiftCardModel>
                    {
                        new GiftCardModel()
                        {
                            Id = 1,
                            GiftCardFunds = 20f,
                            GiftCardNumber = 100,
                            IsRegistered = true,
                        },
                        new GiftCardModel()
                        {
                            Id = 2,
                            GiftCardFunds = 40.50f,
                            GiftCardNumber = 101,
                            IsRegistered = true,
                        },
                    },
                },
                new CustomerModel()
                {
                    Name = "Abraham Linkoln",
                    Email = "abrahamlinkoln@gmail.com",
                    Phone = "772-303-1228",
                    Points = 22,
                    Rewards = 3,
                    GiftCardsCount = 2,
                    GiftCardsTotalFund = 66f,
                    GiftCards = new List<GiftCardModel>
                    {
                        new GiftCardModel()
                        {
                            Id = 3,
                            GiftCardFunds = 25.50f,
                            GiftCardNumber = 103,
                            IsRegistered = true,
                        },
                        new GiftCardModel()
                        {
                            Id = 4,
                            GiftCardFunds = 40.50f,
                            GiftCardNumber = 104,
                            IsRegistered = true,
                        },
                    },
                },
                new CustomerModel()
                {
                    Name = "Aaron Rodgers",
                    Email = "aaronrodgers@gmail.com",
                    Phone = "713-901-8114",
                    Points = 108,
                    Rewards = 2,
                    GiftCardsCount = 2,
                    GiftCardsTotalFund = 106f,
                    GiftCards = new List<GiftCardModel>
                    {
                        new GiftCardModel()
                        {
                            Id = 5,
                            GiftCardFunds = 65.50f,
                            GiftCardNumber = 105,
                            IsRegistered = true,
                        },
                        new GiftCardModel()
                        {
                            Id = 6,
                            GiftCardFunds = 40.50f,
                            GiftCardNumber = 106,
                            IsRegistered = true,
                        },
                    },
                },
                new CustomerModel()
                {
                    Name = "Kierra Bergson",
                    Email = "kierrabergson@yaho.com",
                    Phone = "709-502-5598",
                    Points = 451,
                    Rewards = 0,
                    GiftCardsCount = 17,
                    GiftCardsTotalFund = 50.02f,
                    GiftCards = new List<GiftCardModel>
                    {
                        new GiftCardModel()
                        {
                            Id = 7,
                            GiftCardFunds = 30f,
                            GiftCardNumber = 107,
                            IsRegistered = true,
                        },
                    },
                },
                new CustomerModel()
                {
                    Name = "Angel Dias",
                    Email = "Diazzz@yaho.com",
                    Phone = "732-902-8298",
                    Points = 22,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                },
                new CustomerModel()
                {
                    Name = "Kaiya Dorwart",
                    Email = "KaDor@gmail.com",
                    Phone = "444-912-6298",
                    Points = 34,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Lincoln Lipshutz",
                    Email = "liliputz@yaho.com",
                    Phone = "452-955-8228",
                    Points = 35,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Randy Mango",
                    Email = "mango_jango@gmail.com",
                    Phone = "332-456-2363",
                    Points = 123,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Martin Schleifer",
                    Email = "MartSh@gmail.com",
                    Phone = "732-902-8298",
                    Points = 221,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Carla Dorwart",
                    Email = "Carla_123@gmail.com",
                    Phone = "732-302-8538",
                    Points = 22,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Davis Septimus",
                    Email = "DavSept@gmail.com",
                    Phone = "733-901-8244",
                    Points = 108,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Grover Parsons",
                    Email = "groverpars@yaho.com",
                    Phone = "709-502-5598",
                    Points = 451,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Douglas Moreno",
                    Email = "douglasreno@yaho.com",
                    Phone = "732-902-8298",
                    Points = 22,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Colin Nichols",
                    Email = "colinic@gmail.com",
                    Phone = "444-912-6298",
                    Points = 34,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Kenneth Feron",
                    Email = "kenny@yaho.com",
                    Phone = "452-955-8228",
                    Points = 35,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Geoffrey Kim",
                    Email = "geokim@gmail.com",
                    Phone = "332-456-2363",
                    Points = 123,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Fiona Cole",
                    Email = "fiona12@gmail.com",
                    Phone = "732-902-8298",
                    Points = 221,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Yeom Woong",
                    Email = "yeo@gmail.com",
                    Phone = "732-302-8538",
                    Points = 22,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Do Soon-Bok",
                    Email = "soon-book@gmail.com",
                    Phone = "733-901-8244",
                    Points = 108,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Gwan Jee",
                    Email = "gwanone@yaho.com",
                    Phone = "709-502-5598",
                    Points = 451,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Son Hae-Won",
                    Email = "sonwon98@yaho.com",
                    Phone = "732-902-8298",
                    Points = 22,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Wesley Chavez",
                    Email = "whiskey@gmail.com",
                    Phone = "444-912-6298",
                    Points = 34,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Harlan Sharp",
                    Email = "orlan@yaho.com",
                    Phone = "452-955-8228",
                    Points = 35,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModel()
                {
                    Name = "Audrey Bishop",
                    Email = "andrewBishop@gmail.com",
                    Phone = "332-456-2363",
                    Points = 123,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
            };
        }
    }
}
