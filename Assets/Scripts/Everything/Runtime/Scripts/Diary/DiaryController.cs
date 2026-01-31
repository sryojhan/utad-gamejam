using UnityEngine;
using UnityEngine.UIElements;
public class DiaryController : MonoBehaviour
{

    public GameObject diary;
    public GameObject grid;
    public GameObject gossip;

    public bool diaryOn = false;

    public Animator diaryAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputController.West.IsJustPressed())
        {
                diaryOn = !diaryOn;
                diaryAnimator.SetBool("diaryOn", diaryOn);
        }

    }



    public void addGossip(GossipData g) 
    {
      GameObject tempGossip = Instantiate(gossip, grid.transform);
      tempGossip.GetComponent<Gossip>().Init(g);
    }


}
