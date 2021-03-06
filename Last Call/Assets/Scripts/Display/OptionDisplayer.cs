﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OptionDisplayer : MonoBehaviour
{
    public GameObject optionPrefab;
    public GameObject panelPrefab;
    public PCDeciderImp DeciderImp;
    
    private int lockedIndex = -1;
    private bool locked = false;
    private int page = 0;
    
    private static int pageN = 9;
    private List<GameObject> panels = new List<GameObject>();
    private List<Option> options = new List<Option>();
    private List<Thought> thoughts = new List<Thought>();
    private List<Thought> unUpdatedThoughts = new List<Thought>();
    private bool update = false;

    private Dictionary<KeyCode, int> numberKeys = new Dictionary<KeyCode, int>()
    {
        {KeyCode.Alpha1, 0},
        {KeyCode.Alpha2, 1},
        {KeyCode.Alpha3, 2},
        {KeyCode.Alpha4, 3},
        {KeyCode.Alpha5, 4},
        {KeyCode.Alpha6, 5},
        {KeyCode.Alpha7, 6},
        {KeyCode.Alpha8, 7},
        {KeyCode.Alpha9, 8},
        {KeyCode.Alpha0, 9},
        {KeyCode.Keypad1, 0},
        {KeyCode.Keypad2, 1},
        {KeyCode.Keypad3, 2},
        {KeyCode.Keypad4, 3},
        {KeyCode.Keypad5, 4},
        {KeyCode.Keypad6, 5},
        {KeyCode.Keypad7, 6},
        {KeyCode.Keypad8, 7},
        {KeyCode.Keypad9, 8},
        {KeyCode.Keypad0, 9}
    };

    private Option hovered = null;
    private Option selected = null;
    private KeyCode pressed = KeyCode.None; 

    private void Awake()
    {
        for (int j = 0; j < pageN; j++)
        {
            panels.Add(Instantiate(panelPrefab, transform));
            panels[j].GetComponent<RectTransform>().SetAsLastSibling();
        }
    }

    private void LateUpdate()
    {
        if (update) UpdateThoughts();
    }

    private void Update()
    {
        UpdateHover();

        UpdateSelection();
    }

    private void UpdateHover()
    {
        foreach (Option option in options)
        {
            if (option == null) continue;
            
            if (IsHovered(option.rectTransform))
            {
                option.Hovered(true);
                hovered = option;
            }
            else
            {
                option.Hovered(false);
            }
        }    
    }

    private void UpdateSelection()
    {
        if (selected != null && Input.GetKeyUp(pressed))
        {
            ReleaseThought(selected.thought);
            pressed = KeyCode.None;
        }

        if (selected == null)
        {
            if (hovered != null && Input.GetKeyDown(KeyCode.Mouse0))
            {
                SelectThought(hovered.thought);
                pressed = KeyCode.Mouse0;
                return;
            }

            foreach (KeyCode keyCode in numberKeys.Keys)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    Option option = panels[numberKeys[keyCode]].GetComponentInChildren<Option>();
                    if (option == null) continue;
                    
                    SelectThought(option.thought);
                    pressed = keyCode;
                    return;
                }
            }
        }
    }

    public void UpdateThoughts()
    {
        update = false;
        Queue<Thought> delete = new Queue<Thought>();
        if (locked)
        {
            Thought lockedThought = panels[lockedIndex].GetComponentInChildren<Option>().thought;
            foreach (Thought newThought in unUpdatedThoughts)
            { if (newThought.Text.Equals(lockedThought.Text)) delete.Enqueue(newThought); }
        }

        while (delete.Count > 0) unUpdatedThoughts.Remove(delete.Dequeue());
        
        thoughts = new List<Thought>(unUpdatedThoughts);
        UpdateOptions();
    }
    
    public void QueueUpdateThoughts(List<Thought> newThoughts)
    {
        HashSet<Thought> set1 = new HashSet<Thought>(thoughts);
        HashSet<Thought> set2 = new HashSet<Thought>(newThoughts);

        if (!set1.SetEquals(set2))
        {
            unUpdatedThoughts = new List<Thought>(newThoughts);
            update = true;
        }
    }

    public void SelectThought(Thought thought)
    {
        selected = FindOptionOfThought(thought);
        LockOption(selected);
        selected.Selected(true);

        DeciderImp.SelectThought(thought);
    }

    public void ReleaseThought(Thought thought)
    {
        if (thought == null) return;
        
        if (selected == null || !selected.thought.Equals(thought)) return;
        
        selected.Selected(false);
        if (locked) UnlockOption(FindOptionOfThought(thought));
        selected = null;
        
        DeciderImp.ReleaseThought(thought);
    }

    public void FinishThought(Thought thought)
    {
        if (thought == null) return;
        
        thoughts.Remove(thought);
        ReleaseThought(thought);
        UpdateOptions();
    }
    
    private void UpdateOptions()
    {
        ShowOptions(page * pageN);
    }

    private void ShowOptions(int first)
    {
        int j = 0;
        
        for (int i = 0; i < pageN; i++)
        {
            if (options.Count - 1 < i) { options.Add(null); }
            if (locked && i == lockedIndex) continue;
            if (first + i > thoughts.Count - 1)
            {
                if (options[i] != null)
                {
                    options[i].Destroy();
                }
            } 
            else 
            {
                if (options[i] == null)
                {
                    options[i] = CreateOption(thoughts[first + j], i);
                } else if (!options[i].thought.Equals(thoughts[first + j]))
                {
                    options[i].Destroy();
                    options[i] = CreateOption(thoughts[first + j], i);
                }
            }
            
            j++;
        }
    }

    private Option CreateOption(Thought thought, int place)
    {
        if (thought == null) return null;
        
        Option option = Instantiate(optionPrefab, panels[place].transform).GetComponent<Option>();
        option.SetUp(thought, place + 1);
        
        return option;
    }

    private void LockOption(Option lockedOption)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            Option option = panels[i].GetComponentInChildren<Option>();
            if (lockedOption.Equals(option))
            {
                lockedIndex = i;
                locked = true;
            }
        }
    }

    private void UnlockOption(Option lockedOption)
    {
        lockedIndex = -1;
        locked = false;
        UpdateOptions();
    }

    private Option FindOptionOfThought(Thought thought)
    {
        return options.Where(option => option != null).FirstOrDefault(option => option.thought.Equals(thought));
    }

    private bool IsHovered(RectTransform rectTransform)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, Camera.main);
    }
}