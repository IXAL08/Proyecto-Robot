using System.Collections;
using TerrorConsole;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Menu")]
    public MenuOption[] menuOptions;
    
    [Header("Visual")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public Color selectedColor = Color.green;
    public float fadeDuration = 0.2f;
    
    private int currentSelection = 0;
    private bool canInteract = true;
    public GameObject opciones;
    public GameObject menuInicial;
    
    void Start()
    {
        InitializeMenuOptions();
    }
    
    void Update()
    {
        HandleKeyboardNavigation();
    }

    void InitializeMenuOptions()
    {
        for (int i = 0; i < menuOptions.Length; i++)
        {
            int index = i;

            if (menuOptions[i].textMesh != null)
            {
                // Configurar color inicial
                menuOptions[i].textMesh.color = normalColor;

                // Añadir Event Trigger si no existe
                EventTrigger trigger = menuOptions[i].textMesh.gameObject.GetComponent<EventTrigger>();
                if (trigger == null)
                {
                    trigger = menuOptions[i].textMesh.gameObject.AddComponent<EventTrigger>();
                }

                // Limpiar triggers existentes
                trigger.triggers.Clear();

                // Configurar evento Pointer Enter
                EventTrigger.Entry enterEntry = new EventTrigger.Entry();
                enterEntry.eventID = EventTriggerType.PointerEnter;
                enterEntry.callback.AddListener((data) => { OnOptionHover(index); });
                trigger.triggers.Add(enterEntry);

                // Configurar evento Pointer Exit
                EventTrigger.Entry exitEntry = new EventTrigger.Entry();
                exitEntry.eventID = EventTriggerType.PointerExit;
                exitEntry.callback.AddListener((data) => { OnOptionExit(index); });
                trigger.triggers.Add(exitEntry);

                // Configurar evento Pointer Click
                EventTrigger.Entry clickEntry = new EventTrigger.Entry();
                clickEntry.eventID = EventTriggerType.PointerClick;
                clickEntry.callback.AddListener((data) => { OnOptionClick(index); });
                trigger.triggers.Add(clickEntry);
            }
        }
    }

    void HandleKeyboardNavigation()
    {
        if(!canInteract) return;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentSelection = (currentSelection + 1) % menuOptions.Length;
            UpdateVisualSelection();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentSelection = (currentSelection - 1 + menuOptions.Length) % menuOptions.Length;
            UpdateVisualSelection();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            SelectOption(currentSelection);
        }
    }
    
    public void OnOptionHover(int index)
    {
        if(!canInteract) return;
        
        StartCoroutine(ChangeColorSmooth(menuOptions[index].textMesh, hoverColor));
    }

    public void OnOptionExit(int index)
    {
        if(!canInteract) return;

        StartCoroutine(ChangeColorSmooth(menuOptions[index].textMesh, normalColor));
    }

    public void OnOptionClick(int index)
    {
        if(!canInteract) return;
        
        SelectOption(index);
    }

    void UpdateVisualSelection()
    {
        for (int i = 0; i < menuOptions.Length; i++)
        {
            if (menuOptions[i].textMesh != null)
            {
                if (i == currentSelection)
                {
                    StartCoroutine(ChangeColorSmooth(menuOptions[i].textMesh, hoverColor));
                }
                else
                {
                    StartCoroutine(ChangeColorSmooth(menuOptions[i].textMesh, normalColor));
                }
            }
        }
    }

    void SelectOption(int index)
    {
        if (!canInteract) return;

        if (menuOptions[index].textMesh != null)
        {
            StartCoroutine(FlashColor(menuOptions[index].textMesh));
        }
        
        canInteract = false;

        ExecuteOption(index);
        
        Invoke("ReenableInteraction", 0.5f);
    }

    void ExecuteOption(int index)
    {
        switch (menuOptions[index].optionType)
        {
            case MenuOptionType.StartGame:
                StartGame();
                break;
            case MenuOptionType.Continue:
                ContinueGame();
                break;
            case MenuOptionType.Options:
                ShowOptions();
                break;
            case MenuOptionType.Exit:
                ExitGame();
                break;
            case MenuOptionType.Volver:
                Volver();
                break;
        }
    }

    void StartGame()
    {
        SaveSystemManager.Source.ResetSaveSlot(SaveSystemManager.Source.GetCurrentSaveSlot());
        SaveSystemManager.Source.LoadGame();
        GoToNextScene();
    }

    private static void GoToNextScene()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        string nextSceneName = next < SceneManager.sceneCountInBuildSettings
            ? System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(next))
            : null;
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError($"There is no scene in the index {next}");
            return;
        }

        ScreenTransitionManager.Source.TransitionToScene(nextSceneName);
    }

    void ContinueGame()
    {
        GoToNextScene();
    }

    void ShowOptions()
    {
        
       menuInicial.SetActive(false);
        
       opciones.SetActive(true);
        
    }
    
    void ExitGame()
    {
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void Volver()
    {
        
        opciones.SetActive(false);
        menuInicial.SetActive(true);
    }
    
    void ReenableInteraction()
    {
        canInteract = true;
        for (int i = 0; i < menuOptions.Length; i++)
        {
            if (menuOptions[i].textMesh != null && i != currentSelection)
            {
                menuOptions[i].textMesh.color = normalColor;
            }
        }
    }
    
    IEnumerator ChangeColorSmooth(TextMeshProUGUI text, Color targetColor)
    {
        if (text == null) yield break;

        Color startColor = text.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            yield return null;
        }

        text.color = targetColor;
    }
    
    IEnumerator FlashColor(TextMeshProUGUI text)
    {
        if (text == null) yield break;

        Color originalColor = text.color;
        
        text.color = selectedColor;
        yield return new WaitForSeconds(0.1f);
        
        text.color = originalColor;
    }

    
}

[System.Serializable]
public class MenuOption
{
    public string optionName;
    public TextMeshProUGUI textMesh;
    public MenuOptionType optionType;
}

public enum MenuOptionType
{
    StartGame,
    Continue,
    Options,
    Exit,
    Volver
}
