using Next2.Enums;
using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class PaidOrderBindableModel : BindableBase, IBaseApiModel
    {
        public Guid Id { get; set; }

        public bool IsUnsavedChangesExist { get; set; }

        public CustomerModelDTO? Customer { get; set; }

        public ObservableCollection<RewardBindabledModel> Rewards { get; set; } = new ();

        public ObservableCollection<SeatWithFreeSetsBindableModel> Seats { get; set; } = new ();

        public EBonusType BonusType { get; set; }

        public BonusBindableModel? Bonus { get; set; }

        public decimal SubtotalWithBonus { get; set; }

        public decimal Subtotal { get; set; }

        public decimal PriceTax { get; set; }

        public decimal TaxCoefficient { get; set; }

        public decimal Tip { get; set; }

        public decimal GiftCard { get; set; }

        private decimal _cash;
        public decimal Cash
        {
            get => _cash;
            set
            {
                Total += _cash;
                Change = 0;

                if (Total > value)
                {
                    _cash = value;
                    Total -= value;
                }
                else
                {
                    Change = value - Total;
                    _cash = Total;
                    Total = 0;
                }
            }
        }

        public decimal Change { get; set; }

        public decimal Total { get; set;  }

        public decimal GiftCardsTotalFunds { get; set; }

        public decimal RemainingGiftCardsTotalFunds { get; set; }
    }
}
