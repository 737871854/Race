using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Need.Mx;

public class UIWelcome : MonoBehaviour {

    private RawImage image_Movie;
    //public Image image_BackGround;
    private MovieTexture movTexture;
    protected bool isPlaying;
	// Use this for initialization
	void Start () 
    {
        Cursor.visible = true;
        //Screen.fullScreen = true;
        //Screen.SetResolution(1920, 1080, true);
        movTexture = ResManager.Instance.LoadInstance(MoviePathDefine.GetMoviePathByType(EnumMovieType.Welcome)) as MovieTexture;
        image_Movie = transform.Find("RawImage_Movie").GetComponent<RawImage>();
        //image_BackGround = transform.GetComponent<Image>();
        //image_Movie.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        //image_BackGround.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        StartCoroutine(OnCloseWelcome());
	}
	
	// Update is called once per frame
	void Update () {

	}

    IEnumerator OnCloseWelcome()
    {
        yield return new WaitForSeconds(1);
        isPlaying = true;
        movTexture.Play();
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = movTexture.audioClip;
        audio.Play();
        image_Movie.texture = movTexture;
        image_Movie.gameObject.SetActive(true);
        //image_BackGround.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.6f);
        movTexture.Pause();
        Application.LoadLevel("StartGame");
        movTexture.Stop();
        audio.Stop();
        //image_Movie.gameObject.SetActive(false);
        //image_BackGround.gameObject.SetActive(false);
    }
}
