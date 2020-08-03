using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpGameExample
{
    public class Room
    {
        public string Label
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public List<Item> Items
        {
            get; set;
        }

        public string[] Exits
        {
            get; set;
        }

        public List<UniqueAction> UniqueActions
        {
            get; set;
        }

        public void UpdateDescription(string newDescription)
        {
            Description = newDescription;
        }

        public void AddItem()
        {

        }

        public void RemoveItem()
        {

        }
    }
}
