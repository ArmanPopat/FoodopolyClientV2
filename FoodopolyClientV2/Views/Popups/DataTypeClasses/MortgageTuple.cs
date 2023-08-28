using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.Views.Popups.DataTypeClasses;

public class MortgageTuple
{
    public string Name { get; }
    public int UnmortgageFee { get; }
    public int BoardPosition { get; }

    public MortgageTuple(string name, int unmortgageFee, int boardPosition)
    {
        Name = name;
        UnmortgageFee = unmortgageFee;
        BoardPosition = boardPosition;
    }
}
