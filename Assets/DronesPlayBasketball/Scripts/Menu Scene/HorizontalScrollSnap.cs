using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


[RequireComponent(typeof(ScrollRect))]
public class HorizontalScrollSnap : MonoBehaviour
{
    private Transform _screensContainer;

    private int _screens = 1;

    private List<Vector3> _positions;
    private ScrollRect _scroll_rect;
    private Vector3 _lerp_target;
    private bool _lerp;

    [Tooltip("Button to go to the next page.")]
    public GameObject NextButton;
    [Tooltip("Button to go to the previous page.")]
    public GameObject PrevButton;
    [Tooltip("Transition speed between pages.")]
    public float transitionSpeed = 7.5f;

    [Tooltip("The currently active page")]
    private int _currentScreen;

    [Tooltip("The screen / page to start the control on")]
    [SerializeField]
    public int StartingScreen = 1;

    [Tooltip("The distance between two pages based on page height, by default pages are next to each other")]
    [SerializeField]
    [Range(1, 8)]
    public float PageStep = 1;

    public int CurrentPage
    {
        get
        {
            return _currentScreen;
        }
    }

    public Button selectionButton;
    public SelectionManager_Multiplayer selectionManager;


    void Awake()
    {
        _scroll_rect = gameObject.GetComponent<ScrollRect>();

        _screensContainer = _scroll_rect.content;

        DistributePages();

        if (NextButton)
            NextButton.GetComponent<Button>().onClick.AddListener(() => { NextScreen(); });

        if (PrevButton)
            PrevButton.GetComponent<Button>().onClick.AddListener(() => { PreviousScreen(); });
    }

    void Start()
    {
        UpdateChildPositions();
        _lerp = false;
        _currentScreen = StartingScreen - 1;
        _scroll_rect.horizontalNormalizedPosition = (float)(_currentScreen) / (_screens - 1);
        OnValidate();
        ManageSelectionButton();
    }

    void Update()
    {
        if (_lerp)
        {
            _screensContainer.localPosition = Vector3.Lerp(_screensContainer.localPosition, _lerp_target, transitionSpeed * Time.deltaTime);
            if (Vector3.Distance(_screensContainer.localPosition, _lerp_target) < 0.1f)
            {
                _lerp = false;
            }
        }
        ManageSelectionButton();
    }

    //Function for switching screens with buttons
    public void NextScreen()
    {
        //if (_currentScreen < _screens - 1)
        //{
        //    _currentScreen++;
        //    _lerp = true;
        //    _lerp_target = _positions[_currentScreen];
        //}

        //New
        if (_currentScreen < _screens - 1)
        {
            _currentScreen++;
        }
        else
        {
            _currentScreen = 0;
        }
        _screensContainer.GetChild(0).transform.SetAsLastSibling();
    }

    //Function for switching screens with buttons
    public void PreviousScreen()
    {
        //if (_currentScreen > 0)
        //{
        //    _currentScreen--;
        //    _lerp = true;
        //    _lerp_target = _positions[_currentScreen];
        //}

        //New
        if (_currentScreen > 0)
        {
            _currentScreen--;
        }
        else
        {
            _currentScreen = _screens - 1;
        }
        _screensContainer.GetChild(_screens - 1).transform.SetAsFirstSibling();
    }

    /// <summary>
    /// Function for switching to a specific screen
    /// *Note, this is based on a 0 starting index - 0 to x
    /// </summary>
    /// <param name="screenIndex">0 starting index of page to jump to</param>
    public void GoToScreen(int screenIndex)
    {
        if (screenIndex <= _screens - 1 && screenIndex >= 0)
        {
            _lerp = true;
            _currentScreen = screenIndex;
            _lerp_target = _positions[_currentScreen];
        }
    }

    //find the closest registered point to the releasing point
    private Vector3 FindClosestFrom(Vector3 start, List<Vector3> positions)
    {
        Vector3 closest = Vector3.zero;
        float distance = Mathf.Infinity;

        foreach (Vector3 position in _positions)
        {
            if (Vector3.Distance(start, position) < distance)
            {
                distance = Vector3.Distance(start, position);
                closest = position;
            }
        }

        return closest;
    }

    //returns the current screen that the is seeing
    public int CurrentScreen()
    {
        var pos = FindClosestFrom(_screensContainer.localPosition, _positions);
        return _currentScreen = GetPageforPosition(pos);
    }

    //used for changing between screen resolutions
    private void DistributePages()
    {
        int _offset = 0;
        float _dimension = 0;
        Rect panelDimensions = gameObject.GetComponent<RectTransform>().rect;
        float currentXPosition = 0;
        var pageStepValue = (int)panelDimensions.width * ((PageStep == 0) ? 3 : PageStep);


        for (int i = 0; i < _screensContainer.transform.childCount; i++)
        {
            RectTransform child = _screensContainer.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
            currentXPosition = _offset + (int)(i * pageStepValue);
            child.sizeDelta = new Vector2(panelDimensions.width, panelDimensions.height);
            child.anchoredPosition = new Vector2(currentXPosition, 0f);
            child.anchorMin = new Vector2(0f, child.anchorMin.y);
            child.anchorMax = new Vector2(0f, child.anchorMax.y);
            child.pivot = new Vector2(0f, child.pivot.y);
        }

        _dimension = currentXPosition + _offset * -1;

        _screensContainer.GetComponent<RectTransform>().offsetMax = new Vector2(_dimension, 0f);
    }

    void UpdateChildPositions()
    {
        _screens = _screensContainer.childCount;

        _positions = new List<Vector3>();

        if (_screens > 0)
        {
            for (float i = 0; i < _screens; ++i)
            {
                _scroll_rect.horizontalNormalizedPosition = i / (_screens - 1);
                _positions.Add(_screensContainer.localPosition);
            }
        }
    }

    int GetPageforPosition(Vector3 pos)
    {
        for (int i = 0; i < _positions.Count; i++)
        {
            if (_positions[i] == pos)
            {
                return i;
            }
        }
        return 0;
    }

    void OnValidate()
    {
        var childCount = gameObject.GetComponent<ScrollRect>().content.childCount;
        if (StartingScreen > childCount - 1)
        {
            StartingScreen = childCount - 1;
        }
        if (StartingScreen < 0)
        {
            StartingScreen = 0;
        }
    }

    void ManageSelectionButton()
    {
        if (selectionManager.DronesButtons[_currentScreen].interactable)
            selectionButton.interactable = true;
        else
            selectionButton.interactable = false;
    }
}