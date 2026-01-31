using UnityEngine;
using UnityEngine.UI;

public class Gossip : MonoBehaviour
{

    public Text title;
    public Text body;
    
    public void Init(GossipData g)
    {
        title.text = g.title;
        body.text = g.body;
    }

}
