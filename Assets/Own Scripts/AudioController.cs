using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour {
    public AudioClip winClip;
    public AudioClip loseClip;
    public AudioClip loopClip;

    void Start() {
        StartCoroutine(playSound());
    }

    IEnumerator playSound() {
        if (GameDetails.GameMode == GameDetails.Mode.Singleplayer && GameDetails.Winner == GameDetails.Team.White) {
            GetComponent<AudioSource>().clip = winClip;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(winClip.length / 2);
        }
        if (GameDetails.GameMode == GameDetails.Mode.Singleplayer && GameDetails.Winner == GameDetails.Team.Black) {
            GetComponent<AudioSource>().clip = loseClip;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(loseClip.length / 2);
        }
        GetComponent<AudioSource>().clip = loopClip;
        GetComponent<AudioSource>().Play();
        GetComponent<AudioSource>().loop = true;
    }
}
