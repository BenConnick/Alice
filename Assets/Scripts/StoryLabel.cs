using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Text.RegularExpressions;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StoryLabel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    const string storyFile = "twine"; // twine.json

    // inspector
    public CanvasGroup canvasGroup;
    public Color normalColor;
    public Color hoverColor;
    public bool fadeBetweenPassages;

    private float fadeProgress = 1;
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
        // check for command
        if (Story.CurrentPassage.text.Length > 0 && Story.CurrentPassage.text[0] == '$')
        {
            ExecuteSpecialCommand(Story.CurrentPassage.text);
        }
        else if (fadeBetweenPassages && fadeProgress < 1)
        {
            label.alpha = Mathf.Abs(2 * (fadeProgress-.5f));
            fadeProgress += Time.deltaTime;
            if (fadeProgress > 0.5f)
                label.text = Format(Story.CurrentPassage);
        }
        else
        {
            label.alpha = 1;
            label.text = Format(Story.CurrentPassage);
        }
    }

    private void ExecuteSpecialCommand(string command)
    {
        switch (command.ToLowerInvariant())
        {
            case "$play":
                Story.Reset();
                GM.OnGameEvent(GM.NavigationEvent.FallFromMonologue);
                break;
            default:
                throw new System.Exception("command not implemented: " + command);
        }
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

    public void OnPointerDown(PointerEventData eventData)
    {
        // required empty block
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // required empty block
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (fadeProgress > 0 && fadeProgress < 1) return; // no click during fade
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
                fadeProgress = 0;
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
        // PerFrameVariableWatches.SetDebugQuantity("OverLink", linkIndex.ToString());
        if (highlightedLink != linkIndex)
        {
            highlightedLink = linkIndex;
        }
        UpdateUI();
    }

    public void ShowStory(string passage)
    {
        Story.TryChangePassage(passage);
    }
}
