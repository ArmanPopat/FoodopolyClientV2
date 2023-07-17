using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.ViewModels.Popups;

public partial class PropertyPopupViewModel:ObservableObject
{
    public string Name { get; }
    public int Rent {
        get
        {
            if (RentDoubled)
            {
                return _rent * 2;
            }
            return _rent;
        }
        }
    private int _rent;
    public bool RentVisible
    {
        get
        {
            if (Rent == 0)
            {
                return false;
            }
            return true;
        }
    }
    public int RentL1 { get; }
    public bool RentL1Visible
    {
        get
        {
            if (RentL1 == 0)
            {
                return false;
            }
            return true;
        }
    }
    public int RentL2 { get; }
    public bool RentL2Visible
    {
        get
        {
            if (RentL2 == 0)
            {
                return false;
            }
            return true;
        }
    }
    public int RentL3 { get; }
    public bool RentL3Visible
    {
        get
        {
            if (RentL3 == 0)
            {
                return false;
            }
            return true;
        }
    }
    public int RentL4 { get; }
    public bool RentL4Visible
    {
        get
        {
            if (RentL4 == 0)
            {
                return false;
            }
            return true;
        }
    }
    public int RentL5 { get; }
    public bool RentL5Visible
    {
        get
        {
            if (RentL5 == 0)
            {
                return false;
            }
            return true;
        }
    }
    
    public string TypeOrSet { get; }
    
    public bool UpgradablePotential { get; }
    public bool UpgradeEnabled { get; }

    public bool MortgagePotential { get; }
    public bool Mortgaged { get; }

    public GameViewModel ThisGameViewModel { get; }
    public int BoardPos { get; }
    public string OwnerName { get; }

    [ObservableProperty]
    //[NotifyPropertyChangedFor(nameof(RentBold))]
    //[NotifyPropertyChangedFor(nameof(RentL1Bold))]
    //[NotifyPropertyChangedFor(nameof(RentL2Bold))]
    //[NotifyPropertyChangedFor(nameof(RentL3Bold))]
    //[NotifyPropertyChangedFor(nameof(RentL4Bold))]
    //[NotifyPropertyChangedFor(nameof(RentL5Bold))]
    [NotifyPropertyChangedFor(nameof(RentDoubled))]
    public int rentLevel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RentDoubled))]
    public bool setExclusivelyOwned;

    public bool RentDoubled
    {
        get 
        {
            if (SetExclusivelyOwned && TypeOrSet != "Stations" && TypeOrSet != "Utilities")
            {
                return true;
            }
            return false;
        }
    }

    //public bool RentBold
    //{
    //    get
    //    {
    //        if (RentLevel == 0)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //}
    //public bool RentL1Bold
    //{
    //    get
    //    {
    //        if (RentLevel == 1)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //}
    //public bool RentL2Bold
    //{
    //    get
    //    {
    //        if (RentLevel == 2)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //}
    //public bool RentL3Bold
    //{
    //    get
    //    {
    //        if (RentLevel == 3)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //}
    //public bool RentL4Bold
    //{
    //    get
    //    {
    //        if (RentLevel == 4)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //}
    //public bool RentL5Bold
    //{
    //    get
    //    {
    //        if (RentLevel == 5)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //}



    public PropertyPopupViewModel(GameViewModel gameViewModel,string name, int boardPos, int rent, int rentL1, int rentL2, int rentL3,
        int rentL4, int rentL5, string typeOfSet, bool upgradablePotential, bool upgradeEnabled, string ownerName, int rentLevel, bool setExclusivelyOwned, bool mortgagePotential, bool mortgaged) 
    {
        Name = name;
        _rent = rent;
        RentL1 = rentL1;
        RentL2 = rentL2;
        RentL3 = rentL3;
        RentL4 = rentL4;
        RentL5 = rentL5;
        TypeOrSet = typeOfSet;
        UpgradablePotential = upgradablePotential;
        UpgradeEnabled = upgradeEnabled;
        ThisGameViewModel = gameViewModel;
        BoardPos = boardPos;
        OwnerName = ownerName;
        RentLevel = rentLevel;
        SetExclusivelyOwned = setExclusivelyOwned;
        MortgagePotential = mortgagePotential;
        Mortgaged = mortgaged;
    }
}
