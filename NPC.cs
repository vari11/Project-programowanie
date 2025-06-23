class NPC
    {
        public string Name { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool CanMove { get; private set; }

        public NPC(string name, int x, int y, bool canMove = false)
        {
            Name = name;
            X = x;
            Y = y;
            CanMove = canMove;
        }

        public void Move(int x, int y)
        {
            X = x;
            Y = y;
        }
    }