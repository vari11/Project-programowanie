class Game
{
    private List<Map> wagons;
    private int currentWagonIndex;
    private Player player;
    private List<NPC> npcs;
    private List<Item> items;

    private Random random = new Random();
    private bool doorUnlocked = false;
    private bool gameRunning = true;
    private bool quizStarted = false;

    public Game()
    {
        wagons = new List<Map>();
        currentWagonIndex = 0;
        player = new Player(0, 0);
        npcs = new List<NPC>();
        items = new List<Item>();
    }

    public void Start()
    {
        wagons = new List<Map>
        {
            new Map(30, 10, "Wagon 1", true),
            new Map(30, 10, "Wagon 2", false, isDifferent: true)
        };
        currentWagonIndex = 0;
        player = new Player(1, 1);

        items = new List<Item>
        {
            CreateRandomItem("Ticket"),
            CreateRandomItem("Powerbank")
        };

        npcs = new List<NPC>
        {
            CreateRandomNPC("Grandma"),
            CreateRandomNPC("Teenager", true),
            new NPC("Conductor", 28, 8),
        };

        while (gameRunning)
        {
            Console.Clear();
            CurrentMap().Render(player, npcs, items);
            Console.WriteLine("Inventory: " + string.Join(", ", player.Inventory.ConvertAll(i => i.Name)));
            Console.WriteLine($"Health: {player.Health} | Steps Made: {player.Hunger} | {CurrentMap().Name}");

            if (player.Health <= 0 || player.Hunger >= 200)
            {
                Console.WriteLine("You have collapsed from exhaustion. Game Over.");
                Console.ReadKey();
                break;
            }

            ConsoleKeyInfo key = Console.ReadKey();
            HandleInput(key);

            player.Hunger++;

            MoveActiveNPCs();
        }
    }

    private void MoveActiveNPCs()
    {
        foreach (var npc in npcs.ToArray())
        {
            if (npc.Name == "Conductor" && currentWagonIndex == 0)
            {
                int dx = Math.Sign(player.X - npc.X);
                int dy = Math.Sign(player.Y - npc.Y);
                int newX = npc.X + dx;
                int newY = npc.Y + dy;
                if (!CurrentMap().IsWall(newX, newY))
                    npc.Move(newX, newY);

                if (npc.X == player.X && npc.Y == player.Y)
                {
                    Console.WriteLine("Conductor has caught you!");
                    if (player.HasItem("Ticket"))
                    {
                        Console.WriteLine("You showed your ticket. Conductor nods and leaves.");
                        player.RemoveItem("Ticket");
                    }
                    else
                    {
                        Console.WriteLine("You have no ticket! Conductor hits you hard!");
                        player.Health -= 5;
                    }
                    npcs.Remove(npc);
                    Console.ReadKey();
                    return;
                }
            }
            else if (npc.CanMove)
            {
                int dx = random.Next(-1, 2);
                int dy = random.Next(-1, 2);
                int newX = npc.X + dx;
                int newY = npc.Y + dy;
                if (!CurrentMap().IsWall(newX, newY))
                    npc.Move(newX, newY);
            }
        }
    }

    private Item CreateRandomItem(string name)
    {
        int x, y;
        do
        {
            x = random.Next(1, 29);
            y = random.Next(1, 9);
        } while (CurrentMap().IsWall(x, y));
        return new Item(name, x, y);
    }

    private NPC CreateRandomNPC(string name, bool canMove = false)
    {
        int x, y;
        do
        {
            x = random.Next(1, 29);
            y = random.Next(1, 9);
        } while (CurrentMap().IsWall(x, y));
        return new NPC(name, x, y, canMove);
    }

    private void HandleInput(ConsoleKeyInfo key)
    {
        int newX = player.X;
        int newY = player.Y;

        switch (key.Key)
        {
            case ConsoleKey.W: newY--; break;
            case ConsoleKey.S: newY++; break;
            case ConsoleKey.A: newX--; break;
            case ConsoleKey.D: newX++; break;

            case ConsoleKey.E:
                if (player.X == 28 && player.Y == 5 && currentWagonIndex == 0)
                {
                    if (doorUnlocked)
                    {
                        currentWagonIndex = 1;
                        player.Move(1, 5);
                        if (!npcs.Exists(n => n.Name == "Conductor" && currentWagonIndex == 1))
                        {
                            npcs.Add(new NPC("Conductor", 10, 5));
                        }
                    }
                    else
                    {
                        Console.WriteLine("The door is locked. You need a key from Grandma.");
                        Console.ReadKey();
                    }
                    return;
                }
                else if (player.X == 1 && player.Y == 5 && currentWagonIndex == 1)
                {
                    currentWagonIndex = 0;
                    player.Move(28, 5);
                    return;
                }
                else
                {
                    foreach (var npc in npcs)
                    {
                        if (Math.Abs(npc.X - player.X) <= 1 && Math.Abs(npc.Y - player.Y) <= 1)
                        {
                            if (npc.Name == "Conductor" && currentWagonIndex == 1 && !quizStarted)
                            {
                                StartQuiz();
                                return;
                            }

                            if (npc.Name == "Grandma")
                            {
                                if (!player.HasItem("Key"))
                                {
                                    Console.WriteLine("Grandma: Here, take this key to unlock the door to the next wagon.");
                                    player.Inventory.Add(new Item("Key", 0, 0));
                                    doorUnlocked = true;
                                }
                                else
                                {
                                    Console.WriteLine("Grandma: You already have the key, dear.");
                                }
                                Console.ReadKey();
                                return;
                            }

                            if (npc.Name == "Teenager" && player.HasItem("Powerbank"))
                            {
                                Console.WriteLine("Teenager challenges you to rock-paper-scissors to win a mystery item!");
                                PlayMinigame();
                                Console.ReadKey();
                                return;
                            }
                        }
                    }
                }
                break;
        }

        if (!CurrentMap().IsWall(newX, newY))
        {
            player.Move(newX, newY);

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].X == player.X && items[i].Y == player.Y)
                {
                    Console.WriteLine("You picked up: " + items[i].Name);
                    player.Inventory.Add(items[i]);
                    items.RemoveAt(i);
                    Console.ReadKey();
                    break;
                }
            }
        }
    }

    private void StartQuiz()
    {
        quizStarted = true;

        Console.Clear();
        Console.WriteLine("Conductor: Ah, you again! Well-well-well, trying to change the wagon so I won't check your ticket? Okay, I'll let you pass if you prove you are a true Krakowianin! To pass, answer 3 questions about Kraków!");

        int correctAnswers = 0;

        correctAnswers += AskQuestion("What river flows through Kraków?", "vistula");
        correctAnswers += AskQuestion("What famous animal is associated with Kraków?", "dragon");
        correctAnswers += AskQuestion("What is the most famous castle in Kraków?", "wawel");

        if (correctAnswers == 3)
        {
            Console.WriteLine("You Win!");
        }
        else if (correctAnswers == 2 && player.HasItem("Ściąga"))
        {
            Console.WriteLine("Ah, you answered one question incorrectly... But since you have Sciaga... You Win!");
        }
        else
        {
            Console.WriteLine("Wrong answers... Game Over.");
        }

        gameRunning = false;
        Console.WriteLine("Press any key to quit");
        Console.ReadKey();
    }

    private int AskQuestion(string question, string correctAnswer)
    {
        Console.WriteLine(question);
        Console.Write("> ");
        string input = (Console.ReadLine() ?? "").Trim().ToLower();
        return input.Contains(correctAnswer.ToLower()) ? 1 : 0;

        
    }

    private void PlayMinigame()
    {
        string[] options = { "Rock", "Paper", "Scissors" };
        string playerChoice = "";
        string npcChoice = "";

        while (true)
        {
            Console.WriteLine("Choose [R]ock, [P]aper, or [S]cissors:");
            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.R: playerChoice = "Rock"; break;
                case ConsoleKey.P: playerChoice = "Paper"; break;
                case ConsoleKey.S: playerChoice = "Scissors"; break;
                default:
                    Console.WriteLine("Invalid input. Please press R, P, or S.");
                    continue;
            }

            npcChoice = options[random.Next(3)];
            Console.WriteLine($"\nYou chose {playerChoice}. Teenager chose {npcChoice}.");

            if (playerChoice == npcChoice)
            {
                Console.WriteLine("It's a tie! Try again.");
                continue;
            }

            break;
        }

        bool win =
            (playerChoice == "Rock" && npcChoice == "Scissors") ||
            (playerChoice == "Paper" && npcChoice == "Rock") ||
            (playerChoice == "Scissors" && npcChoice == "Paper");

        if (win)
        {
            Console.WriteLine("You win! Teenager gives you a secret item 'Ściąga'... Hm, what is it?...");
            player.Inventory.Add(new Item("Ściąga", 0, 0));
            player.Health += 2;
        }
        else
        {
            Console.WriteLine("You lose. Teenager laughs and walks away.");
            player.Health -= 2;
        }
        Console.ReadKey();
    }

    private Map CurrentMap()
    {
        return wagons[currentWagonIndex];
    }
}
