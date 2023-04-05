using System;
using UnityEngine;
using UnityEngine.Pool;


/// <summary>
/// 오브젝트 풀을 사용해 <see cref="AudioSource">AudioSource</see>의 생성, 소멸을 제어합니다.
/// </summary>
public interface IAudioPool
{
    /// <summary>
    /// 모든 위치에서 동일한 음량으로 들을 수 있는 오디오를 재생합니다.
    /// </summary>
    /// <param name="onEnd"><see cref="AudioSource.isPlaying">AudioSource.isPlaying</see>이 false로 변할 때 호출됩니다.</param>
    /// <returns>Pool로 반환하는 Dispose 함수를 구현하는 Disposable</returns>
    IDisposable PlayGlobalAudio(AudioClip clip, Action onEnd = null);


    /// <summary>
    /// 위치에 따라 음량이 달라지는 오디오를 목표 위치에 생성하고 재생합니다.
    /// </summary>
    /// <param name="onEnd"><see cref="AudioSource.isPlaying">AudioSource.isPlaying</see>이 false로 변할 때 호출됩니다.</param>
    /// <returns>Pool로 반환하는 Dispose 함수를 구현하는 Disposable</returns>
    IDisposable PlayLocalAudio(AudioClip clip, Vector3 position, Action onEnd = null);


    /// <summary>
    /// 위치에 따라 음량이 달라지는 오디오를 목표 대상을 부모로 선택하고 재생합니다.
    /// </summary>
    /// <param name="onEnd"><see cref="AudioSource.isPlaying">AudioSource.isPlaying</see>이 false로 변할 때 호출됩니다.</param>
    /// <returns>Pool로 반환하는 Dispose 함수를 구현하는 Disposable</returns>
    IDisposable PlayLocalAudio(AudioClip clip, Transform parent, Action onEnd = null);
}


/// <summary>
/// 오브젝트 풀을 사용해 <see cref="AudioSource">AudioSource</see>의 생성, 소멸을 제어합니다.
/// </summary>
public class AudioPool : IAudioPool
{
    #region Nested Class, Struct

    public struct Settings
    {
        public int globalAudioPoolDefaultSize, globalAudioPoolMaxSize;
        public int localAudioPoolDefaultSize, localAudioPoolMaxSize;

        public float globalAudioReverbZoneMix, localAudioReverbZoneMix;
    }

    private class AudioElement : IDisposable
    {
        public readonly AudioSource source;
        private readonly IObjectPool<AudioElement> pool;


        public AudioElement(AudioSource source, IObjectPool<AudioElement> pool)
        {
            this.source = source;
            this.pool = pool;
        }

        public void Dispose()
        {
            pool.Release(this);
        }
    }

    #endregion


    #region Members

    private readonly Settings settings;
    private readonly Transform globalAudioPoolRoot, localAudioPoolRoot;

    private readonly ObjectPool<AudioElement> globalAudioPool, localAudioPool;

    #endregion


    public AudioPool(Settings settings)
    {
        this.settings = settings;
        this.globalAudioPoolRoot = new GameObject("GlobalAudioPool Root").transform;

        this.globalAudioPool = new ObjectPool<AudioElement>(createFunc: CreateGlobalAudioElement,
                                                       actionOnGet: OnTakeFromPool,
                                                       actionOnRelease: OnReturnedToPool,
                                                       actionOnDestroy: OnDestroyObject,
                                                       collectionCheck: true,
                                                       defaultCapacity: settings.globalAudioPoolDefaultSize,
                                                       maxSize: settings.globalAudioPoolMaxSize);

        this.localAudioPool = new ObjectPool<AudioElement>(createFunc: CreateLocalAudioElement,
                                               actionOnGet: OnTakeFromPool,
                                               actionOnRelease: OnReturnedToPool,
                                               actionOnDestroy: OnDestroyObject,
                                               collectionCheck: true,
                                               defaultCapacity: settings.localAudioPoolDefaultSize,
                                               maxSize: settings.localAudioPoolMaxSize);
    }


    public IDisposable PlayGlobalAudio(AudioClip clip, Action onEnd = null)
    {
        var newElement = CreateGlobalAudioElement();
        newElement.source.PlayOneShot(clip);

        return newElement;
    }


    public IDisposable PlayLocalAudio(AudioClip clip, Vector3 position, Action onEnd = null)
    {
        var newElement = PlayLocalAudio(clip, onEnd);
        newElement.source.transform.position = position;

        return newElement;
    }


    public IDisposable PlayLocalAudio(AudioClip clip, Transform parent, Action onEnd = null)
    {
        var newElement = PlayLocalAudio(clip, onEnd);
        newElement.source.transform.SetParent(parent);
        newElement.source.transform.localPosition = Vector3.zero;

        return newElement;
    }


    /// <summary>
    /// 위치에 따라 음량이 달라지는 오디오를 재생합니다.
    /// </summary>
    /// <param name="onEnd"><see cref="AudioSource.isPlaying">AudioSource.isPlaying</see>이 false로 변할 때 호출됩니다.</param>
    private AudioElement PlayLocalAudio(AudioClip clip, Action onEnd = null)
    {
        var newElement = CreateLocalAudioElement();
        newElement.source.PlayOneShot(clip);

        return newElement;
    }


    #region Pool Manage Functions

    private void OnDestroyObject(AudioElement element)
    {
        UnityEngine.Object.Destroy(element.source.gameObject);
    }


    private void OnReturnedToPool(AudioElement element)
    {
        element.source.gameObject.SetActive(false);
        element.source.transform.SetParent(globalAudioPoolRoot);
    }


    private void OnTakeFromPool(AudioElement element)
    {
        element.source.gameObject.SetActive(true);
    }


    private AudioElement CreateGlobalAudioElement()
    {
        GameObject newObject = new GameObject("GlobalAudioPool Element");
        AudioSource source = newObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.reverbZoneMix = settings.globalAudioReverbZoneMix;
        newObject.transform.SetParent(globalAudioPoolRoot);

        AudioElement newElement = new AudioElement(source, globalAudioPool);

        return newElement;
    }


    private AudioElement CreateLocalAudioElement()
    {
        GameObject newObject = new GameObject("GlobalAudioPool Element");
        AudioSource source = newObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.reverbZoneMix = settings.localAudioReverbZoneMix;
        newObject.transform.SetParent(localAudioPoolRoot);

        AudioElement newElement = new AudioElement(source, localAudioPool);

        return newElement;
    }

    #endregion

}