using UnityEngine;

public class DiaryController : MonoBehaviour
{

    public GameObject diary;
    public GameObject grid;
    public GameObject gossip;

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
            if (diary.activeSelf)
            {
                diaryAnimator.SetTrigger("leave");
            }
            else
            {
                diary.SetActive(true);
                diaryAnimator.SetTrigger("enter");
            }
        }
    }



    public void addGossip(GossipData g) 
    {
      GameObject tempGossip = Instantiate(gossip, grid.transform);
      tempGossip.GetComponent<Gossip>().Init(g);
    }


}
