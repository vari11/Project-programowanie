
class Map
{
    private readonly char[,] grid;
    public string Name { get; }

    public Map(int width, int height, string name, bool hasDoor, bool isDifferent = false)
    {
        Name = name;
        grid = new char[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[y, x] = (x == 0 || x == width - 1 || y == 0 || y == height - 1) ? '#' : '.';
            }
        }

        if (hasDoor)
            grid[5, width - 2] = 'D';
        else
            grid[5, 1] = 'D';

        if (isDifferent)
        {
            for (int y = 2; y < height - 2; y++)
            {
                grid[y, 10] = '#';
                grid[y, 20] = '#';
            }
        }
    }

    public void Render(Player player, List<NPC> npcs, List<Item> items)
    {
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                if (player.X == x && player.Y == y)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write('@');
                    Console.ResetColor();
                    continue;
                }

                var npc = npcs.Find(n => n.X == x && n.Y == y);
                if (npc != null)
                {
                    ConsoleColor npcColor;
                    char symbol;

                    switch (npc.Name)
                    {
                        case "Grandma":
                            symbol = 'G';
                            npcColor = ConsoleColor.Magenta;
                            break;
                        case "Conductor":
                            symbol = 'C';
                            npcColor = ConsoleColor.Blue;
                            break;
                        case "Teenager":
                            symbol = 'T';
                            npcColor = ConsoleColor.Yellow;
                            break;
                        default:
                            symbol = 'N';
                            npcColor = ConsoleColor.Gray;
                            break;
                    }

                    Console.ForegroundColor = npcColor;
                    Console.Write(symbol);
                    Console.ResetColor();
                    continue;
                }

                if (items.Exists(i => i.X == x && i.Y == y))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write('I');
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(grid[y, x]);
                }
            }
            Console.WriteLine();
        }
    }

    public bool IsWall(int x, int y) => grid[y, x] == '#';
}