using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpGameExample
{
    public class Item
    {
        public string Name
        {
            get; private set;
        }
        public string Description
        {
            get; private set;
        }

        public void UpdateDescription(string newDescription)
        {
            Description = newDescription;
        }

        public Item(string name, string description)
        {
            Name = name;
            Description = description;
        }

    }
}
