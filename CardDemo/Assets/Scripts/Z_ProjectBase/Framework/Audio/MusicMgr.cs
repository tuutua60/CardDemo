using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 音效管理模块
/// 使用对象池管理AudioSource，避免频繁创建和销毁，减少GC。
/// 使用协程回收音效，取代Update轮询，提高性能。
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    public void Initialize()
    {
        PlayBkMusic("BKMusic");
        ChangeBKValue(0.2f);
    }

    #region 成员变量
    // 背景音乐组件
    private AudioSource bkMusic;
    private float bkValue = 1f;

    // 音效挂载的父对象
    private GameObject soundObj;
    // 音效池
    private List<AudioSource> soundPool = new List<AudioSource>();
    // 音效音量
    private float soundValue = 1f;

    // 音频剪辑缓存，避免重复加载
    private Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();

    #endregion

    #region 背景音乐
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name">音乐文件名</param>
    public void PlayBkMusic(string name)
    {
        if (bkMusic == null)
        {
            GameObject obj = new GameObject("BkMusic");
            // 确保背景音乐对象在切换场景时不被销毁
            GameObject.DontDestroyOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }

        // 异步加载背景音乐并播放
        LoadAudioClip("Music/BkMusic/" + name, (clip) =>
        {
            bkMusic.clip = clip;
            bkMusic.loop = true;
            bkMusic.volume = bkValue;
            bkMusic.Play();
        });
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBKMusic()
    {
        bkMusic?.Pause();
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBKMusic()
    {
        bkMusic?.Stop();
    }

    /// <summary>
    /// 改变背景音乐音量大小
    /// </summary>
    public void ChangeBKValue(float value)
    {
        bkValue = Mathf.Clamp01(value);
        if (bkMusic != null)
        {
            bkMusic.volume = bkValue;
        }
    }
    #endregion

    #region 音效
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效文件名</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="callBack">播放开始时的回调</param>
    public void PlaySound(string name, bool isLoop = false, UnityAction<AudioSource> callBack = null)
    {
        if (soundObj == null)
        {
            soundObj = new GameObject("Sound");
            GameObject.DontDestroyOnLoad(soundObj);
        }
        // 加载音效资源
        LoadAudioClip("Music/Sound/" + name, (clip) =>
        {
            // 从池中获取一个AudioSource
            AudioSource source = GetSoundFromPool();
            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();

            callBack?.Invoke(source);

            // 如果不循环，则在播放结束后自动回收
            if (!isLoop)
            {
                MonoMgr.Instance.StartCoroutine(RecycleSoundAfterPlaying(source));
            }
        });
    }

    /// <summary>
    /// 改变所有音效的音量
    /// </summary>
    public void ChangeSoundValue(float value)
    {
        soundValue = Mathf.Clamp01(value);
        // 仅修改未来要播放的音效和池中已有的音效音量
        foreach (var source in soundPool)
        {
            source.volume = soundValue;
        }
    }

    /// <summary>
    /// 停止并回收一个指定的音效
    /// </summary>
    public void StopSound(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            RecycleSound(source);
        }
    }
    #endregion

    #region 资源与对象池管理
    /// <summary>
    /// 统一的音频加载方法，带缓存功能
    /// </summary>
    private void LoadAudioClip(string path, UnityAction<AudioClip> callback)
    {
        if (audioClipCache.TryGetValue(path, out AudioClip clip))
        {
            callback(clip);
            return;
        }

        ResourcesMgr.Instance.LoadAsync<AudioClip>(path, (loadedClip) =>
        {
            if (loadedClip != null)
            {
                audioClipCache[path] = loadedClip;
                callback(loadedClip);
            }
        });
    }

    /// <summary>
    /// 从池中获取一个可用的AudioSource
    /// </summary>
    private AudioSource GetSoundFromPool()
    {
        foreach (var source in soundPool)
        {
            if (!source.isPlaying)
            {
                source.enabled = true;
                return source;
            }
        }

        // 如果池中没有可用的，则创建一个新的
        AudioSource newSource = soundObj.AddComponent<AudioSource>();
        soundPool.Add(newSource);
        return newSource;
    }

    /// <summary>
    /// 播放结束后自动回收AudioSource的协程
    /// </summary>
    private IEnumerator RecycleSoundAfterPlaying(AudioSource source)
    {
        // 等待音效播放完成
        yield return new WaitWhile(() => source.isPlaying);
        RecycleSound(source);
    }

    /// <summary>
    /// 回收一个AudioSource到池中（通过禁用它来标记为可用）
    /// </summary>
    private void RecycleSound(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.enabled = false; // 禁用组件比销毁组件性能高得多
    }

    /// <summary>
    /// 清空缓存
    /// </summary>
    public void ClearCache()
    {
        audioClipCache.Clear();
    }
    #endregion
}
