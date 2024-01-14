using UnityEngine;


public class TwineLoader {
    public static TwineStory loadStory(TextAsset jsonFile)
    {
        TwineStory story = JsonUtility.FromJson<TwineStory>(jsonFile.text);
        //Debug.Log(JsonUtility.ToJson(story));
        return story;
    }
}

public class TwineStoryWrapper
{
    private TwineStory story;
    private int currentPid;
    public Passage CurrentPassage => GetPassage(currentPid);
    public TwineStory Raw => story;

    public TwineStoryWrapper(TwineStory s)
    {
        story = s;
        currentPid = int.Parse(s.startnode);
    }

    public Passage GetPassage(int pid)
    {
        return story.passages[pid - 1];
    }

    public Passage ChangePassage(Link link)
    {
        currentPid = link.pid;
        return GetPassage(currentPid);
    }

    public void Reset()
    {
        currentPid = int.Parse(story.startnode);
    }

    public bool TryChangePassage(string passageName)
    {
        foreach (Passage p in story.passages)
        {
            if (p.name == passageName)
            {
                ChangePassage(p.pid);
                return true;
            }
        }
        
        return false;
    }

    public Passage ChangePassage(int linkPID)
    {
        bool found = false;
        foreach (Passage p in story.passages)
        {
            if (p.pid == linkPID)
            {
                currentPid = linkPID;
                found = true;
                break;
            }
        }
        if (!found)
            Debug.LogError("Cannot change passage, invalid pid " + linkPID);
        return GetPassage(currentPid);
    }

    public int FindPassageWithName(string passageName)
    {
        foreach (var p in story.passages)
        {
            if (p.name == passageName) return p.pid;
        }
        return -1;
    }
}

[System.Serializable]
public class TwineStory : System.Object
{
    public Passage[] passages;
    public string name;
    public string startnode;
    public string creator;
    public string creator_version;
    public string ifid;
}

[System.Serializable]
public class Passage : System.Object
{
    public string text;
    public Link[] links;
    public string name;
    public int pid;
    public NodePosition position;

}

[System.Serializable]
public class Link : System.Object
{
    public string name;
    public string link;
    public int pid;
}

[System.Serializable]
public class NodePosition : System.Object
{
    public int x;
    public int y;
}


/* Format Example  
 
        Link tempLink = new Link();
        tempLink.name = "linkname";
        tempLink.link = "linklink";
        tempLink.pid = 999;
        Passage temp = new Passage();
        temp.links = new Link[] { tempLink };
        temp.text = "passage text ";
        temp.name = "passage name";
        temp.pid = 999;
        temp.position = new NodePosition();
        temp.position.x = 1;
        temp.position.y = 1;
        TwineStory test = new TwineStory();
        test.creator = "creator";
        test.creator_version = "creator version";
        test.ifid = "0";
        test.name = "name";
        test.passages = new Passage[] { temp };
        test.startnode = "start node";
        //Debug.Log(JsonUtility.ToJson(test));
*/