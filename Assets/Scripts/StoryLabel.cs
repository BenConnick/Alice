﻿using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Text.RegularExpressions;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StoryLabel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    const string storyFile = "twine"; // twine.json

    public Color normalColor;
    public Color hoverColor;

    private int highlightedLink = -1;
    private TextMeshProUGUI label;
    private TwineStoryWrapper _innerTwineStory;
    private TwineStoryWrapper Story
    {
        get
        {
            if (_innerTwineStory == null)
            {
                _innerTwineStory = new TwineStoryWrapper(TwineLoader.loadStory(Resources.Load<TextAsset>(storyFile)));
            }
            return _innerTwineStory;
        }
    }

    private void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        label.text = Format(Story.CurrentPassage);
        //Debug.Log(Story.CurrentPassage.links);
        //foreach (var item in Story.CurrentPassage.links)
        //{
        //    Debug.Log(item);
        //    Debug.Log(item.name);
        //}
    }

    private string Format(Passage passage)
    {
        string output = "";
        Regex twineLinkPattern = new Regex("\\[\\[.*\\]\\]+");
        var splits = twineLinkPattern.Split(passage.text);

        var links = passage.links;

        // no links
        if (links == null) return passage.text;

        //Debug.Log("length compare " + splits.Length + " " + links.Length);

        // replace the Twine links with TMP links
        for (int i = 0; i < splits.Length; i++)
        {
            output += splits[i];
            if (i < links.Length)
            {
                // link has two formatting, handled differently
                // bug in json exporter means that when link text and link path are not an exact match, pid is missing
                // to fix, we search for pid by matching the link path against all passage names
                string link;
                if (links[i].name.Contains("|"))
                {
                    var split = links[i].name.Split('|'); ;
                    int pidLookup = Story.FindPassageWithName(split[1]);
                    link = $"<link=\"{pidLookup}\">{split[0]}</link>";
                }
                else
                {
                    link = $"<link=\"{links[i].pid}\">{links[i].name}</link>";
                }
                // underline all links
                link = $"<u>{link}</u>";
                // color for hover
                link = Util.ColorMarkup(link, i == highlightedLink ? hoverColor : normalColor);
                output += link;
            }
        }

        // return the new combined string
        return output;
    }

    public void OnLinkClicked()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var linkIndex = TMPro.TMP_TextUtilities.FindIntersectingLink(label, eventData.pointerPressRaycast.screenPosition, GM.FindSingle<GameplayCameraBehavior>().GetComponent<Camera>());
        Debug.Log("link clicked with local index " + linkIndex);
        if (linkIndex >= 0)
        {
            //Debug.Log(label.textInfo.linkInfo.Length);
            //Debug.Log(label.textInfo.linkInfo[0]);
            //Debug.Log(label.textInfo.linkInfo[linkIndex]);
            //Debug.Log(label.textInfo.linkInfo[linkIndex].GetLinkID());
            //Debug.Log(label.textInfo.linkInfo[linkIndex].GetLinkText());
            string linkValue = label.textInfo.linkInfo[linkIndex].GetLinkID();
            Debug.Log("On Link Clicked: " + linkValue);
            if (int.TryParse(linkValue, out int linkPID))
            {
                Story.ChangePassage(linkPID);
                UpdateUI();
            }
            else
            {
                Debug.LogError("Invalid link: " + linkValue);
            }
        }
    }

    private void Update()
    {
        int linkIndex = TMPro.TMP_TextUtilities.FindIntersectingLink(label, Input.mousePosition, GM.FindSingle<GameplayCameraBehavior>().GetComponent<Camera>());
        PerFrameVariableWatches.SetDebugQuantity("OverLink", linkIndex.ToString());
        if (highlightedLink != linkIndex)
        {
            highlightedLink = linkIndex;
            UpdateUI();
        }
    }
}
