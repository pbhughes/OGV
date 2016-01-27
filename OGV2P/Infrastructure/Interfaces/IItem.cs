using System;
using System.Collections.Generic;
using System.ComponentModel;
using Infrastructure.Models;

namespace Infrastructure.Interfaces
{
    public delegate void ItemChangedEventHandler(Item item);
    public interface IItem
    {
        string Description { get; set; }
        string ID { get; set; }
        List<Item> Items { get; set; }
        Item Parent { get; set; }
        TimeSpan TimeStamp { get; set; }
        string Title { get; set; }
       
        event ItemChangedEventHandler ItemChangedEvent;
        event PropertyChangedEventHandler PropertyChanged;
        
    }
}