using LitMotion;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SE クリップ")]
    [SerializeField] private AudioClip seMove;
    [SerializeField] private AudioClip seShoot;
    [SerializeField] private AudioClip seBounce;
    [SerializeField] private AudioClip seEnemyDead;
    [SerializeField] private AudioClip seModuleOpen;
    [SerializeField] private AudioClip seModuleHover;
    [SerializeField] private AudioClip seModuleSelect;

    [Header("BGM クリップ")]
    [SerializeField] private AudioClip bgmTitle;
    [SerializeField] private AudioClip bgmGame;

    [Header("AudioSource")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSource;
    [SerializeField] private AudioSource bgmSource;

    [Header("BGM 設定")]
    [SerializeField] private float bgmVolume    = 0.6f;
    [SerializeField] private float bgmFadeDuration = 0.8f;

    private MotionHandle _bgmFadeHandle;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void StartMoveLoop()
    {
        if (seMove == null || loopSource == null || loopSource.isPlaying) return;
        loopSource.clip = seMove;
        loopSource.Play();
    }

    public void StopMoveLoop()
    {
        if (loopSource == null) return;
        loopSource.Stop();
    }

    public void PlayShoot()        => PlaySFX(seShoot,        1f,   Random.Range(0.95f, 1.05f));
    public void PlayBounce()       => PlaySFX(seBounce,       0.7f, Random.Range(0.90f, 1.10f));
    public void PlayEnemyDead()    => PlaySFX(seEnemyDead,    1f);
    public void PlayModuleOpen()   => PlaySFX(seModuleOpen,   1f);
    public void PlayModuleHover()  => PlaySFX(seModuleHover,  0.6f);
    public void PlayModuleSelect() => PlaySFX(seModuleSelect, 1f);

    public void PlayTitleBGM() => PlayBGM(bgmTitle);
    public void PlayGameBGM()  => PlayBGM(bgmGame);

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        if (_bgmFadeHandle.IsActive()) _bgmFadeHandle.Cancel();

        if (bgmSource.isPlaying)
        {
            // フェードアウト → クリップ切り替え → フェードイン
            _bgmFadeHandle = LMotion.Create(bgmSource.volume, 0f, bgmFadeDuration * 0.5f)
                .WithEase(Ease.InCubic)
                .WithOnComplete(() =>
                {
                    bgmSource.clip = clip;
                    bgmSource.Play();
                    _bgmFadeHandle = LMotion.Create(0f, bgmVolume, bgmFadeDuration * 0.5f)
                        .WithEase(Ease.OutCubic)
                        .Bind(v => bgmSource.volume = v)
                        .AddTo(this);
                })
                .Bind(v => bgmSource.volume = v)
                .AddTo(this);
        }
        else
        {
            bgmSource.clip   = clip;
            bgmSource.volume = 0f;
            bgmSource.Play();
            _bgmFadeHandle = LMotion.Create(0f, bgmVolume, bgmFadeDuration)
                .WithEase(Ease.OutCubic)
                .Bind(v => bgmSource.volume = v)
                .AddTo(this);
        }
    }

    public void StopBGM()
    {
        if (bgmSource == null || !bgmSource.isPlaying) return;
        if (_bgmFadeHandle.IsActive()) _bgmFadeHandle.Cancel();
        _bgmFadeHandle = LMotion.Create(bgmSource.volume, 0f, bgmFadeDuration)
            .WithEase(Ease.InCubic)
            .WithOnComplete(() => bgmSource.Stop())
            .Bind(v => bgmSource.volume = v)
            .AddTo(this);
    }
}
