using UnityEngine;

public class TextBasedAdventure : MonoBehaviour
{
    public enum TileType
    {
        Invalid,
        Empty,
        Item,
        Enemy,
        Teleporter,
        Blockade,
        Exit
    }

    private string[,] tileNames = { { "Dark Cave",  "Mossy Tunnel", "Crystal Room", "Science Lab"},
                                    { "Bone Chamber", "Flooded Hall", "Iron Gate", "Evil Lair of Doom"},
                                    { "Goblin Den", "Armory", "Throne Room", "Abandoned Classroom"},
                                    { "Heavenly Top", "Lava Pit", "Convenience Store", "Backroom"}
                                  };

    private TileType[,] tileTypes = { { TileType.Empty, TileType.Empty, TileType.Teleporter, TileType.Teleporter },
                                      { TileType.Enemy, TileType.Empty, TileType.Blockade, TileType.Enemy },
                                      { TileType.Enemy, TileType.Item, TileType.Teleporter, TileType.Enemy },
                                      { TileType.Teleporter, TileType.Blockade, TileType.Item, TileType.Exit }
                                    };

    private string[,] tileDescriptions = { { "So dark and cavey... What a start.",
                                             "Wet and mossy in the pipes.",
                                             "I can see myself in the crystals!",
                                             "This science lab has a cozy fresh air." },
                                           { "So many spooky skeletons!",
                                             "Great. My feet are wet in this hall.",
                                             "I don't have a key to get through the gates.",
                                             "EVIL. LAIR. OF. DOOM!!!" },
                                           { "Must. Slay. All. Goblins.",
                                             "Some weapons and armors here could be useful.",
                                             "Does a king live here...?",
                                             "I feel chilly and uncomfortable in this abandoned classroom..."},
                                           { "God Almighty blessing upon the top",
                                             "The floor is literally lava. I don't wanna go.",
                                             "Convenience store here of all place? I'm hungry.",
                                             "Oh no. Not the backrooms. It seems like it's the only way out..."
                                           }
                                         };

    private bool[,] tileEntered = new bool[4, 4]; // All falses

    private int playerRow = 0;
    private int playerCol = 0;
    private int playerHealth = 10;
    private int enemyDamage = 1;
    private int itemHealAmount = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OutputTileInformation();
    }

    // Update is called once per frame
    void Update()
    {
        bool wasKeyPressed = HandleInput(out int newRow, out int newCol);
        if (!wasKeyPressed)
        {
            return;
        }
        SetPlayerPosition(newRow, newCol);
        OutputTileInformation();
    }

    private void OutputTileInformation()
    {
        Debug.Log("You are in: " + tileNames[playerRow, playerCol]);

        if (tileEntered[playerRow, playerCol] == false)
        {
            Look();
            tileEntered[playerRow, playerCol] = true;
        }

        switch (tileTypes[playerRow, playerCol])
        {
            case TileType.Empty:
                Debug.Log("There is nothing here.");
                break;
            case TileType.Enemy:
                Debug.Log("A wild enemy appeared!");
                EncounterEnemy();
                break;
            case TileType.Item:
                Debug.Log("You see a health potion.");
                ItemPickup();
                break;
            case TileType.Teleporter:
                Debug.Log("You see a teleporter here.");
                break;
            case TileType.Blockade:
                Debug.Log("Something is blocking your way.");
                break;
            case TileType.Exit:
                Debug.Log("You see a way out.");
                break;
            default:
                Debug.LogError("Invalid TileType");
                break;
        }
    }

    private void Look()
    {
        Debug.Log(tileDescriptions[playerRow, playerCol]);
    }

    private void EncounterEnemy()
    {
        PlayerTakeDamage(enemyDamage);
    }
    
    private void ItemPickup()
    {
        PlayerHeal(itemHealAmount);
    }

    private void PlayerHeal(int heal)
    {
        playerHealth += heal;
        Debug.Log("You get healed. Your health is now " + playerHealth);
    }

    private void PlayerTakeDamage(int damage)
    {
        playerHealth -= damage;
        Debug.Log("You get hit. Your health is now " + playerHealth);
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("You are dead");
        }
    }

    /// <summary>
    /// Sets the player position to a new row and column position
    /// </summary>
    /// <param name="newRow"></param>
    /// <param name="newCol"></param>
    private void SetPlayerPosition(int newRow, int newCol)
    {
        if (CheckIfNewPositionInTileBounds(newRow, newCol))
        {
            if (!CheckIfNewPositionInBlockade(newRow, newCol))
            {
                playerRow = newRow;
                playerCol = newCol;
            }
            else
            {
                Debug.Log("Can't go that way. Something is blocking your way.");
            }
        }
        else
        {
            Debug.Log("Can't go that way. Out of bounds.");
        }
    }

    /// <summary>
    /// Determine if the new row and column position are within the bounds of the tiles
    /// </summary>
    /// <param name="newRow"></param>
    /// <param name="newCol"></param>
    /// <returns>True if it is within the bounds, false if not</returns>
    private bool CheckIfNewPositionInTileBounds(int newRow, int newCol)
    {
        return (newRow >= 0 && newRow < tileNames.GetLength(0)) && (newCol >= 0 && newCol < tileNames.GetLength(1));
    }

    /// <summary>
    /// Determine if the new row and column position is not in blockade
    /// </summary>
    /// <param name="newRow"></param>
    /// <param name="newCol"></param>
    /// <returns>True if it is in blockade, false if not</returns>
    private bool CheckIfNewPositionInBlockade(int newRow, int newCol)
    {
        return (tileTypes[newRow,newCol] == TileType.Blockade);
    }

    /// <summary>
    /// Handles the player's input and sets potential new position in the tileNames array
    /// </summary>
    /// <param name="newRow">new row position</param>
    /// <param name="newCol">new column position</param>
    /// <returns>True if an input was pressed, false if not</returns>
    private bool HandleInput(out int newRow, out int newCol)
    {
        bool hasPressedKey = true;
        newRow = playerRow;
        newCol = playerCol;
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("You pressed " + KeyCode.D);
            newCol++;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("You pressed " + KeyCode.A);
            newCol--;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("You pressed " + KeyCode.W);
            newRow--;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("You pressed " + KeyCode.S);
            newRow++;
        }else if (Input.GetKeyDown(KeyCode.Space))
        {
            Look();
        }
        else
        {
            hasPressedKey = false;
        }
        return hasPressedKey;
    }

}
