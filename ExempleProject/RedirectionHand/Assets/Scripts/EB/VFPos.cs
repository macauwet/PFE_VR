using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFPos : MonoBehaviour
{
	public GameObject palm, index, thumb;
    // Start is called before the first frame update
    void Start()
    {
        palm = GameObject.Find("Hands/AvatarHands/TrackingSpace/RightHandAnchor/WarpedRightHand/Warped_OculusHand_R/b_r_wrist/r_palm_center_marker");
    	index = GameObject.Find("Hands/AvatarHands/TrackingSpace/RightHandAnchor/WarpedRightHand/Warped_OculusHand_R/b_r_wrist/b_r_index1/b_r_index2/b_r_index3/r_index_finger_tip_marker");
    	thumb = GameObject.Find("Hands/AvatarHands/TrackingSpace/RightHandAnchor/WarpedRightHand/Warped_OculusHand_R/b_r_wrist/b_r_thumb0/b_r_thumb1/b_r_thumb2/b_r_thumb3/r_thumb_finger_tip_marker");

    }

    // Update is called once per frame
    void Update()
    {
    	StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        this.transform.position = thumb.transform.position + (thumb.transform.position - index.transform.position)/2;
        yield return new WaitForSeconds(0.3f);
    }
}
