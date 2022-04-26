﻿using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Next2.Models
{
    public class PaidOrderBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public bool IsUnsavedChangesExist { get; set; }

        public CustomerModel? Customer { get; set; }

        public ObservableCollection<RewardBindabledModel> Rewards { get; set; } = new ();

        public ObservableCollection<SeatWithFreeSetsBindableModel> Seats { get; set; } = new ();

        public EBonusType BonusType { get; set; }

        public BonusBindableModel? Bonus { get; set; }

        public float SubtotalWithBonus { get; set; } = 0f;

        public float Subtotal { get; set; }

        public float PriceTax { get; set; }

        public TaxModel Tax { get; set; } = new();

        public float Tip { get; set; } = 0;

        public float GiftCards { get; set; }

        private float _cash;
        public float Cash
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

        public float Change { get; set; }

        public float Total { get; set;  }
    }
}
