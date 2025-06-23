class Player
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Health { get; set; } = 10;
    public int Hunger { get; set; } = 0;
    public List<Item> Inventory { get; private set; }

    public Player(int x, int y)
    {
        X = x;
        Y = y;
        Inventory = new List<Item>();
    }

    public void Move(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool HasItem(string name)
    {
        return Inventory.Exists(i => i.Name == name);
    }
        public void RemoveItem(string name)
{
    var item = Inventory.Find(i => i.Name == name);
    if (item != null)
    {
        Inventory.Remove(item);
    }
}
    }