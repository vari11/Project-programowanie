 class Item
    {
        public string Name { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public Item(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }
    }
