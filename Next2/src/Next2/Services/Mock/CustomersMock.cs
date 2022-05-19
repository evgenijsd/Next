using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Services.Mock;
using System.Collections.Generic;

namespace Next2.Services
{
    public static class CustomersMock
    {
        public static List<CustomerModelDTO> Create()
        {
            return new List<CustomerModelDTO>()
            {
                new CustomerModelDTO()
                {
                    FullName = "Adam Brody",
                    Email = "adambrody@gmail.com",
                    Phone = "4561125298",
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
                new CustomerModelDTO()
                {
                    FullName = "Abraham Linkoln",
                    Email = "abrahamlinkoln@gmail.com",
                    Phone = "7723031228",
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
                new CustomerModelDTO()
                {
                    FullName = "Aaron Rodgers",
                    Email = "aaronrodgers@gmail.com",
                    Phone = "7139018114",
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
                new CustomerModelDTO()
                {
                    FullName = "Kierra Bergson",
                    Email = "kierrabergson@yaho.com",
                    Phone = "7095025598",
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
                new CustomerModelDTO()
                {
                    FullName = "Angel Dias",
                    Email = "Diazzz@yaho.com",
                    Phone = "7329028298",
                    Points = 22,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                },
                new CustomerModelDTO()
                {
                    FullName = "Kaiya Dorwart",
                    Email = "KaDor@gmail.com",
                    Phone = "4449126298",
                    Points = 34,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Lincoln Lipshutz",
                    Email = "liliputz@yaho.com",
                    Phone = "4529558228",
                    Points = 35,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Randy Mango",
                    Email = "mango_jango@gmail.com",
                    Phone = "3324562363",
                    Points = 123,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Martin Schleifer",
                    Email = "MartSh@gmail.com",
                    Phone = "7329028298",
                    Points = 221,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Carla Dorwart",
                    Email = "Carla_123@gmail.com",
                    Phone = "7323028538",
                    Points = 22,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Davis Septimus",
                    Email = "DavSept@gmail.com",
                    Phone = "7339018244",
                    Points = 108,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Grover Parsons",
                    Email = "groverpars@yaho.com",
                    Phone = "7095025598",
                    Points = 451,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Douglas Moreno",
                    Email = "douglasreno@yaho.com",
                    Phone = "7329028298",
                    Points = 22,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Colin Nichols",
                    Email = "colinic@gmail.com",
                    Phone = "4449126298",
                    Points = 34,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Kenneth Feron",
                    Email = "kenny@yaho.com",
                    Phone = "4529558228",
                    Points = 35,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Geoffrey Kim",
                    Email = "geokim@gmail.com",
                    Phone = "3324562363",
                    Points = 123,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Fiona Cole",
                    Email = "fiona12@gmail.com",
                    Phone = "7329028298",
                    Points = 221,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Yeom Woong",
                    Email = "yeo@gmail.com",
                    Phone = "7323028538",
                    Points = 22,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
                new CustomerModelDTO()
                {
                    FullName = "Do Soon-Bok",
                    Email = "soon-book@gmail.com",
                    Phone = "7339018244",
                    Points = 108,
                    Rewards = 0,
                    GiftCardsCount = 0,
                    GiftCardsTotalFund = 0,
                    GiftCards = new(),
                },
            };
        }
    }
}
