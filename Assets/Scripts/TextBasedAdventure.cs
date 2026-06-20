using Unity.VisualScripting;
using UnityEngine;
/**
 * In-Class Assignment 01 - TextBasedAdventure.cs
 * Name: Ka Bo Cheung
 * Date: 06/19/2026
 * Course: GAME-1377-001
 *
 * Script for the text-based adventure on the tilemap-bounds with such representations
 * README: SPACE for Look function & R for Teleporter function
 */
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
                                             "Convenience store here of all place? What a convenience.",
                                             "Oh no. Not the backrooms. It seems like it's the only way out..."
                                           }
                                         };

    private bool[,] tileEntered = new bool[4,4]; // All defaulted to false

    private int[][][] teleporterPairs = new int[][][]{
                                                       new int[][] {new int[] { 0, 3 }, new int[] { 2, 2 }},
                                                       new int[][] {new int[] { 0, 2 }, new int[] { 3, 0 }}
                                                     };

    private int playerRow = 0;
    private int playerCol = 0;
    private int playerHealth = 10;
    private int enemyDamage = 1;
    private int itemHealAmount = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckForEvenNumberOfTeleporters();
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

    /// <summary>
    /// Displays the tile information on the player location
    /// </summary>
    private void OutputTileInformation()
    {
        Debug.Log("You are in: " + tileNames[playerRow, playerCol]);

        // When the player enters the tile for the first time, the tile description will be displayed
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

    /// <summary>
    /// Player encounters Enemy
    /// </summary>
    private void EncounterEnemy()
    {
        PlayerTakeDamage(enemyDamage);
    }
    
    /// <summary>
    /// Player picks up the item
    /// </summary>
    private void ItemPickup()
    {
        PlayerHeal(itemHealAmount);
    }

    /// <summary>
    /// Heals the player by specific amount of health points
    /// </summary>
    /// <param name="heal">Restoring health points</param>
    private void PlayerHeal(int heal)
    {
        playerHealth += heal;
        Debug.Log("You get healed. Your health is now " + playerHealth);
    }

    /// <summary>
    /// Damages the player by specific amount of health points
    /// </summary>
    /// <param name="damage">damage points</param>
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
    /// Displays the tile description message
    /// </summary>
    private void Look()
    {
        Debug.Log(tileDescriptions[playerRow, playerCol]);
    }

    /// <summary>
    /// Uses the teleporter on the player location if there is one available
    /// </summary>
    /// <param name="newRow">Predetermined new row</param>
    /// <param name="newCol">Predetermined new column</param>
    private void UseTeleporter(ref int newRow, ref int newCol)
    {
        if (tileTypes[playerRow,playerCol] != TileType.Teleporter)
        {
            Debug.Log("There's no teleporter here.");
            return;
        }

        SearchForOtherTeleporter(ref newRow, ref newCol);
        Debug.Log("Teleporting to the other room...");
    }

    /// <summary>
    /// Searchs for the other connected teleporter and get its location 
    /// </summary>
    /// <param name="teleportRow">Predetermined new row from other teleporter</param>
    /// <param name="teleportCol">Predetermined new column from other teleporter</param>
    private void SearchForOtherTeleporter(ref int teleportRow, ref int teleportCol)
    {
        for (int pair = 0; pair < teleporterPairs.Length; pair++)
        {
            for (int teleporter = 0; teleporter < teleporterPairs[pair].Length; teleporter++)
            {
                if (teleporterPairs[pair][teleporter][0] == playerRow && teleporterPairs[pair][teleporter][1] == playerCol)
                {
                    if (teleporter == 0)
                    {
                        teleportRow = teleporterPairs[pair][1][0];
                        teleportCol = teleporterPairs[pair][1][1];
                        return;
                    }
                    else
                    {
                        teleportRow = teleporterPairs[pair][0][0];
                        teleportCol = teleporterPairs[pair][0][1];
                        return;
                    }
                }
            }
        }
        // There always should be even number of teleporters, and each one has a pair-connection 
        Debug.LogError("Unable to find other teleporter.");
    }

    /// <summary>
    /// Checks if there is even number of teleporters on the map.
    /// Otherwise, the error message will be displayed.
    /// </summary>
    private void CheckForEvenNumberOfTeleporters()
    {
        int numOfTeleporters = 0;
        for (int x = 0; x < tileTypes.GetLength(0); x++)
        {
            for (int y = 0; y < tileTypes.GetLength(1); y++)
            {
                if (tileTypes[x, y] == TileType.Teleporter)
                {
                    numOfTeleporters++;
                }
            }
        }

        if (numOfTeleporters % 2 != 0)
        {
            Debug.LogError("The number of available teleporters is not even. Please install one more.");
        } 
    }

    /// <summary>
    /// Sets the player position to a new row and column position
    /// </summary>
    /// <param name="newRow">Predetermined new row</param>
    /// <param name="newCol">Predetermined new column</param>
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
    /// <param name="newRow">Predetermined new row</param>
    /// <param name="newCol">Predetermined new column</param>
    /// <returns>True if it is within the bounds, false if not</returns>
    private bool CheckIfNewPositionInTileBounds(int newRow, int newCol)
    {
        return (newRow >= 0 && newRow < tileNames.GetLength(0)) && (newCol >= 0 && newCol < tileNames.GetLength(1));
    }

    /// <summary>
    /// Determine if the new row and column position is not in blockade
    /// </summary>
    /// <param name="newRow">Predetermined new row</param>
    /// <param name="newCol">Predetermined new column</param>
    /// <returns>True if it is in blockade, false if not</returns>
    private bool CheckIfNewPositionInBlockade(int newRow, int newCol)
    {
        return (tileTypes[newRow,newCol] == TileType.Blockade);
    }

    /// <summary>
    /// Handles the player's input and sets potential new position in the tileNames array
    /// </summary>
    /// <param name="newRow">Predetermined new row</param>
    /// <param name="newCol">Predetermined new column</param>
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
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Look();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            UseTeleporter(ref newRow, ref newCol);
        }
        else
        {
            hasPressedKey = false;
        }
        return hasPressedKey;
    }

}
