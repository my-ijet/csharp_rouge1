using System.Numerics;

bool game_running = true;

Console.CursorVisible = false;
Console.CancelKeyPress += (sender, e) => { Quit(); };

Player player = new()
{
  Name = "ABOBA"
};

Level level = new()
{
  position = new(1, 1),
  spawn_position = new(4, 3),
  walls = [
    "#############################",
    "#...........................#",
    "#........................... ",
    "#........................... ",
    "#...........................#",
    "#...........................#",
    "#...........................#",
    "#############################"
  ],
  items = {
    {new Vector2(8,3), 'X'},
    {new Vector2(2,5), 'X'},
    {new Vector2(20,6), 'X'},
    {new Vector2(15,2), 'X'}
  }
};

level.SpawnPlayer(ref player);

// Main loop
while (game_running)
{
  logick();
}

Quit();

void logick()
{
  Console.Clear();
  printIventory();
  printLevel();
  printPlayer();
  printControls();

  handleInput();
  handleCollisions();
}

void printControls()
{
  Console.SetCursorPosition(0, level.walls.Length + 1);
  Console.WriteLine(" ╭───────────────────────────╮");
  Console.WriteLine(" Arrows: Move | [Esc, Q]: Quit");
  Console.WriteLine(" ╰───────────────────────────╯");
}

void printPlayer()
{
  Console.SetCursorPosition(
      (int)(player.position.X),
      (int)(player.position.Y)
    );
  Console.ForegroundColor = ConsoleColor.DarkCyan;
  Console.Write(player.image);
  Console.ResetColor();
}

void handleInput()
{
  player.move = new(0, 0);

  ConsoleKey key = Console.ReadKey().Key;
  switch (key)
  {
    case ConsoleKey.LeftArrow: player.move.X--; break;
    case ConsoleKey.RightArrow: player.move.X++; break;
    case ConsoleKey.UpArrow: player.move.Y--; break;
    case ConsoleKey.DownArrow: player.move.Y++; break;

    case ConsoleKey.Escape:
    case ConsoleKey.Q: Quit(); break;
    default: break;
  }
}

void handleCollisions()
{
  Vector2 target_pos = player.position + player.move;
  Vector2 level_player_pos = target_pos - level.position;

  char character = ' ';
  try
  {
    character = level.walls
      [(int)level_player_pos.Y]
      [(int)level_player_pos.X];
  }
  catch (Exception) { }

  if (character != '#')
  {
    player.position = target_pos;
  }

  // Guard for negative position | out of bounds
  player.position.X = Math.Max(player.position.X, 0);
  player.position.Y = Math.Max(player.position.Y, 0);


  handlePickups();
}

void handlePickups()
{
  Vector2 level_player_pos = player.position - level.position;
  if (level.items.ContainsKey(level_player_pos))
  {
    player.AddItem(level.items[level_player_pos]);
    level.items.Remove(level_player_pos);
  }
}

void printIventory()
{
  // Console.SetCursorPosition(0, 0);
  Console.WriteLine($" {player.Name} have: [{player.GetInventory()}]");
}

void printLevel()
{
  // Print walls
  for (int i = 0; i < level.walls.Length; i++)
  {
    Console.SetCursorPosition((int)level.position.X, (int)level.position.Y + i);
    string line = level.walls[i];
    Console.Write(line);
  }

  // Print items
  Console.ForegroundColor = ConsoleColor.DarkYellow;
  foreach ((Vector2 pos, char item) in level.items)
  {
    Console.SetCursorPosition((int)(level.position.X + pos.X), (int)(level.position.Y + pos.Y));
    Console.Write(item);
  }
  Console.ResetColor();
}

void Quit()
{
  game_running = false;
  Console.ResetColor(); Console.Clear();
  Console.CursorVisible = true;
}

class Player
{
  public string Name = "";
  public char image = '@';
  public Vector2 position = new();
  public Vector2 move = new();

  private char[] _inventory = [];
  public string GetInventory()
  {
    return string.Join(", ", _inventory);
  }
  public void AddItem(char item)
  {
    _inventory = [.. _inventory, item];
  }

}
class Level
{
  public Vector2 position = new();
  public Vector2 spawn_position = new();
  public string[] walls = [];
  public Dictionary<Vector2, char> items = [];

  internal void SpawnPlayer(ref Player player)
  {
    player.position = spawn_position;
  }
}
