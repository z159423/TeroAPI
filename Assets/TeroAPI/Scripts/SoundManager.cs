using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [Header("=====BGM=====")]
    public Sound[] Bgm;
    public GameObject bgmPrefab;
    private AudioSource BGMaudioSource;
    private string previousBgmName = "";
    private float previousBgmTime = 0;
    private string nowBgmName = "";
    private bool isFadeOut = false;

    [Space]
    [Header("=====SFX=====")]
    public Sound[] SFX;
    public GameObject SFXPrefab;
    private AudioSource SFXaudioSource;

    [SerializeField] private ObjectPool audioPool;

    public static SoundManager instance;

    private static bool Initialized = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (!Initialized)
        {
            Initialized = true;
        }

        BGMaudioSource = bgmPrefab.AddComponent<AudioSource>();
        SFXaudioSource = SFXPrefab.AddComponent<AudioSource>();
    }

    /// <summary>
    /// BGM 이름으로 찾아서 실행하는 함수
    /// </summary>
    /// <param name="name"> 실행할 BGM 이름 </param>
    /// <param name="isPreviouisBGM"> true면 name과 상관없이 이전에 실행한 BGM을 실행함 </param>
    public void FindBGM(string name = "", bool isPreviousBgm = false)
    {
        Sound s;

        if (isPreviousBgm)
        {
            s = Array.Find(Bgm, sound => sound.name == previousBgmName);
        }
        else
        {
            s = Array.Find(Bgm, sound => sound.name == name);
        }

        PlayBGM(s, isPreviousBgm);
    }

    /// <summary>
    /// 이전 BGM을 실행하는 함수
    /// </summary>
    public void PlayPreviousBGM()
    {
        Sound s = Array.Find(Bgm, sound => sound.name == previousBgmName);

        PlayBGM(s, isPrevious: true);
    }

    public void PlayRandomBossFightBGM()
    {
        string name = "boss" + UnityEngine.Random.Range(2, 4).ToString();

        Sound s = Array.Find(Bgm, sound => sound.name == name);

        PlayBGM(s);
    }

    /// <summary>
    /// 생성되어 있는 단일 AudioSource로 해당 효과음을 실행함, 두개 동시에 실행 안되니 주의
    /// </summary>
    /// <param name="name"> 실행할 효과음 이름 </param>
    public void PlaySFX(string name)
    {

        Sound s = Array.Find(SFX, sound => sound.name == name);

        SFXaudioSource.clip = s.clip;

        SFXaudioSource.volume = s.volume;
        SFXaudioSource.pitch = s.pitch;
        SFXaudioSource.loop = s.loop;
        SFXaudioSource.outputAudioMixerGroup = s.audioMixerGroup;
        SFXaudioSource.time = s.time;

        SFXaudioSource.Play();
    }

    /// <summary>
    /// 매개변수로 받은 AudioSource로 효과음을 실행함
    /// </summary>
    /// <param name="name"> 실행할 효과음 이름 </param>
    /// <param name="audio"> 실행할때 사용할 AudioSource </param>
    public void PlaySFXWithAudioSource(string name, AudioSource audio)
    {
        Sound s = Array.Find(SFX, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogError("SFX Ŭ���� ã�� �� �����ϴ�.");
            return;
        }

        if (audio == null)
        {
            Debug.LogError("Audio Source�� �����ϴ�.");
            return;
        }

        audio.clip = s.clip;

        audio.volume = s.volume;
        audio.pitch = s.pitch;
        audio.loop = s.loop;
        audio.outputAudioMixerGroup = s.audioMixerGroup;
        audio.time = s.time;

        audio.Play();
    }

    /// <summary>
    /// 원하는 위치에 AudioSource를 생성하고 효과음 실행, 오브젝트는 오브젝트 풀링으로 관리됨
    /// </summary>
    /// <param name="name"> 실행할 효과음 이름 </param>
    /// <param name="position"> AudioSource를 생성할 위치 </param>
    public AudioSource GenerateAudioAndPlaySFX(string name, Vector3 position)
    {
        Sound s = Array.Find(SFX, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogError("SFX Ŭ���� ã�� �� �����ϴ�.");
            return null;
        }

        GameObject audio = null;
        AudioSource audioSource = null;

        if (s.useOneAudioSource)
        {
            if (s.useableAudioSource == null)
            {
                audio = audioPool.DequeueObject(position);
                audioSource = audio.GetComponent<AudioSource>();
                s.useableAudioSource = audio.GetComponent<AudioSource>();
            }
            else
            {
                s.useableAudioSource.transform.position = position;
                audioSource = s.useableAudioSource;
            }
        }
        else
        {
            audio = audioPool.DequeueObject(position);
            audioSource = audio.GetComponent<AudioSource>();
        }

        audioSource.clip = s.clip;

        audioSource.volume = s.volume;
        audioSource.pitch = s.pitch;
        audioSource.loop = s.loop;
        audioSource.outputAudioMixerGroup = s.audioMixerGroup;
        audioSource.time = s.time;

        audioSource.Play();

        if (!s.useOneAudioSource)
            StartCoroutine(enqueue());

        IEnumerator enqueue()
        {
            yield return new WaitForSeconds(s.clip.length + 1);
            audioPool.EnqueueObject(audio);
        }

        return audioSource;
    }

    //BGM 실행 함수
    private void PlayBGM(Sound s, bool isPrevious = false)
    {
        if (BGMaudioSource.isPlaying)
        {
            if (s.name == nowBgmName)
                return;

            float previousBgmTimeSave = previousBgmTime;

            previousBgmName = nowBgmName;

            if (!isFadeOut)
                previousBgmTime = BGMaudioSource.time;

            nowBgmName = s.name;

            StopAllCoroutines();
            StartCoroutine(FadeOutBgm(s, isPrevious, previousBgmTime: previousBgmTimeSave));
        }
        else
        {
            nowBgmName = s.name;

            StartCoroutine(FadeInBgm(s, isPrevious));
        }
    }

    //페이드 인 효과 함수
    private IEnumerator FadeInBgm(Sound s, bool isPreviousBgm = false, float previousBgmTime = 0)
    {
        float timeToFade = 1.25f;
        float timeElapsed = 0f;
        float maxVolume = s.volume;

        BGMaudioSource.clip = s.clip;
        BGMaudioSource.pitch = s.pitch;
        BGMaudioSource.loop = s.loop;
        BGMaudioSource.outputAudioMixerGroup = s.audioMixerGroup;

        if (isPreviousBgm)
        {
            //Debug.LogError(previousBgmTime);

            BGMaudioSource.time = previousBgmTime;
        }
        else
        {
            BGMaudioSource.time = s.time;
        }

        BGMaudioSource.Play();

        while (timeElapsed < timeToFade)
        {
            BGMaudioSource.volume = Mathf.Lerp(0, maxVolume, timeElapsed / timeToFade);
            timeElapsed += Time.deltaTime / 3;
            yield return null;
        }
    }

    //페이드 아웃 효과 함수
    private IEnumerator FadeOutBgm(Sound s, bool isPreviousBgm = false, float previousBgmTime = 0)
    {
        float timeToFade = 0.02f;

        isFadeOut = true;
        while (BGMaudioSource.volume > 0.05f)
        {
            //Debug.LogError("FadeOutBgm");
            BGMaudioSource.volume -= timeToFade;
            yield return new WaitForSeconds(0.1f);
        }
        isFadeOut = false;


        StartCoroutine(FadeInBgm(s, isPreviousBgm, previousBgmTime: previousBgmTime));
    }
}

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public float time = 0;

    public bool loop;
    public bool bgm;

    public AudioMixerGroup audioMixerGroup;

    [HideInInspector]
    public AudioSource source;

    [Space]
    public bool useOneAudioSource = false;
    [HideInInspector]
    public AudioSource useableAudioSource;

}