using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{

    // Holds the boolean variables for the menu buttons
    public bool isStart;
    public bool isControls;
    public bool isCredits;
    public bool isQuit;
    public bool isRetry;

    // Holds the bool for story captions
    public bool isCaptions;

    // Holds the list of captions
    public List<GameObject> story;

    // Holds player's level info
    private static readonly string level = "level";

    // Start is called before the first frame update
    void Start()
    {

        // Continue for captions
        if (isCaptions)
        {

            // Continue to first caption
            if (level == null)
            {

                // First caption
                story[0].SetActive(true);
            }

            // Continue for other captions
            else
            {

                // First caption
                story[(int)PlayerPrefs.GetFloat(level)].SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Continue if captions are clicked
        if (isCaptions)
        {

            // Continue if M1 clicked
            if (Input.GetMouseButtonDown(0))
            {

                // Continue to first level
                if (level == null)
                {

                    // Level 1
                    SceneManager.LoadScene(1);
                }

                // Continue to any other level
                else if (level != null && (int)PlayerPrefs.GetFloat(level) != 4)
                {

                    // Move to next scene
                    SceneManager.LoadScene((int)PlayerPrefs.GetFloat(level) + 1);
                }

                // Continue to main menu
                else
                {

                    // Main menu
                    SceneManager.LoadScene(0);
                }
            }
        }

        // Continue for controls and credits
        if (isControls || isCredits)
        {

            // Continue if esc was clicked
            if (Input.GetKey(KeyCode.Escape))
            {

                // Main menu
                SceneManager.LoadScene(0);
            }
        }
    }

    // Method for player collision upon exits
    void OnCollisionEnter2D(Collision2D player)
    {

        // Move to story caption
        SceneManager.LoadScene(5);
    }

    // Method for hovering on buttons
    void OnMouseEnter()
    {

        // Continue if menu button
        if (isRetry || isStart || isControls || isCredits || isQuit)
        {

            // Change text color to white
            GetComponent<TextMesh>().color = Color.white;
        }
    }

    // Method for hovering off buttons
    void OnMouseExit()
    {

        // Continue if menu button
        if (isRetry || isStart || isControls || isCredits || isQuit)
        {

            // Change text color to red
            GetComponent<TextMesh>().color = Color.red;
        }
    }

    // Method for clicking on buttons
    void OnMouseUp()
    {

        // Continue if quit button is clicked
        if (isQuit)
        {

            // Quit application
            Application.Quit();
        }

        // Continue if start button is clicked
        if (isStart)
        {

            // Start game
            SceneManager.LoadScene(1);
        }

        // Continue if retry button is clicked
        if (isRetry)
        {

            // Load scene of player's death
            SceneManager.LoadScene((int) PlayerPrefs.GetFloat(level));
        }

        // Continue if controls button is clicked
        if (isControls)
        {

            // Start game
            SceneManager.LoadScene(7);
        }

        // Continue if credits button is clicked
        if (isCredits)
        {

            // Start game
            SceneManager.LoadScene(8);
        }
    }
}
