using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GJFramework
{
    [Serializable]
    public class AudioInfo
    {
        public string audioName;
        public AudioSource audioSource;

        public AudioInfo(string name, AudioSource source)
        {
            audioName = name;
            audioSource = source;
        }
    }

    public class AudioMgr : SingletonPersistent<AudioMgr>
    {
        // BGM的音频信息
        public AudioInfo bgmAudioInfo;

        // 所有SFX的音频信息
        public List<AudioInfo> sfxAudioInfoList;

        // 音量控制全局音量
        public float mainVolumeFactor;
        public float bgmVolumeFactor;
        public float sfxVolumeFactor;

        private GameObject _bgmSourcesRootGO;
        private GameObject _sfxSourcesRootGO;

        private TweenContainer _tweenContainer;

        // 音频资源路径配置
        private const string BGM_PATH_PREFIX = "Audio/Bgm/";
        private const string SFX_PATH_PREFIX = "Audio/Sfx/";

        protected override void Awake()
        {
            base.Awake();

            // 创建BGM和SFX的AudioSource根节点
            _bgmSourcesRootGO = new GameObject("BGM_ROOT");
            _sfxSourcesRootGO = new GameObject("SFX_ROOT");

            _bgmSourcesRootGO.transform.SetParent(transform);
            _sfxSourcesRootGO.transform.SetParent(transform);

            // 初始化列表
            sfxAudioInfoList = new List<AudioInfo>();

            // 没有存储的设定就使用默认的音量
            //mainVolumeFactor = PlayerPrefs.HasKey("MainVolume") ? PlayerPrefs.GetFloat("MainVolume") : 1;
            //bgmVolumeFactor = PlayerPrefs.HasKey("BgmVolumeFactor") ? PlayerPrefs.GetFloat("BgmVolumeFactor") : 1;
            //sfxVolumeFactor = PlayerPrefs.HasKey("SfxVolumeFactor") ? PlayerPrefs.GetFloat("SfxVolumeFactor") : 1;

            _tweenContainer = new TweenContainer();
        }


        private void OnDestroy()
        {
            _tweenContainer?.KillAllDoTween();
            _tweenContainer = null;
        }

        #region BGM

        /// <summary>
        /// 播放BGM
        /// </summary>
        public void PlayBgm(string fadeInMusicName, float fadeInDuration = 0.5f,
            float fadeOutDuration = 0.5f, bool loop = true)
        {
            if (bgmAudioInfo.audioSource == null)
            {
                // 创建新的BGM AudioSource
                GameObject bgmGO = new GameObject(fadeInMusicName + "_BGM");
                bgmGO.transform.SetParent(_bgmSourcesRootGO.transform);

                AudioSource source = bgmGO.AddComponent<AudioSource>();
                AudioClip clip = Resources.Load<AudioClip>(BGM_PATH_PREFIX + fadeInMusicName);

                if (clip == null)
                {
                    Debug.LogError($"BGM资源加载失败: {BGM_PATH_PREFIX + fadeInMusicName}");
                    Destroy(bgmGO);
                    return;
                }

                source.clip = clip;
                source.loop = loop;
                source.volume = fadeInDuration > 0 ? 0 : mainVolumeFactor * bgmVolumeFactor;

                bgmAudioInfo = new AudioInfo(fadeInMusicName, source);
                source.Play();

                if (fadeInDuration > 0)
                {
                    var tween = source.DOFade(mainVolumeFactor * bgmVolumeFactor, fadeInDuration);
                    _tweenContainer.RegDoTween(tween);
                }
            }
            else
            {
                if (bgmAudioInfo.audioName == fadeInMusicName)
                {
                    Debug.LogWarning("当前已经在播放" + fadeInMusicName + "，无法再次播放！！！");
                    return;
                }

                Sequence seq = DOTween.Sequence();

                // 淡出当前BGM
                if (fadeOutDuration > 0 && bgmAudioInfo.audioSource.isPlaying)
                {
                    seq.Append(bgmAudioInfo.audioSource.DOFade(0, fadeOutDuration));
                }

                seq.OnComplete(() =>
                {
                    // 停止并销毁旧的BGM
                    if (bgmAudioInfo != null && bgmAudioInfo.audioSource != null)
                    {
                        bgmAudioInfo.audioSource.Stop();
                        Destroy(bgmAudioInfo.audioSource.gameObject);
                    }

                    // 创建新的BGM
                    GameObject bgmGO = new GameObject(fadeInMusicName + "_BGM");
                    bgmGO.transform.SetParent(_bgmSourcesRootGO.transform);

                    AudioSource newSource = bgmGO.AddComponent<AudioSource>();
                    AudioClip newClip = Resources.Load<AudioClip>(BGM_PATH_PREFIX + fadeInMusicName);

                    if (newClip == null)
                    {
                        Debug.LogError($"BGM资源加载失败: {BGM_PATH_PREFIX + fadeInMusicName}");
                        Destroy(bgmGO);
                        bgmAudioInfo = null;
                        return;
                    }

                    newSource.clip = newClip;
                    newSource.loop = loop;
                    newSource.volume = fadeInDuration > 0 ? 0 : mainVolumeFactor * bgmVolumeFactor;
                    newSource.Play();

                    bgmAudioInfo = new AudioInfo(fadeInMusicName, newSource);

                    // 淡入新BGM
                    if (fadeInDuration > 0)
                    {
                        var fadeInTween = newSource.DOFade(mainVolumeFactor * bgmVolumeFactor, fadeInDuration);
                        _tweenContainer.RegDoTween(fadeInTween);
                    }
                });

                _tweenContainer.RegDoTween(seq);
            }
        }

        /// <summary>
        /// 暂停BGM
        /// </summary>
        /// <param name="fadeOutDuration">淡出间隔</param>
        public void PauseBgm(float fadeOutDuration = 0.5f)
        {
            if (bgmAudioInfo.audioSource == null)
            {
                Debug.LogWarning("当前bgmAudioInfo为空，没有播放任何音乐！！！");
                return;
            }

            if (fadeOutDuration > 0)
            {
                Sequence seq = DOTween.Sequence();
                seq.Append(bgmAudioInfo.audioSource.DOFade(0, fadeOutDuration));
                seq.OnComplete(() => bgmAudioInfo.audioSource.Pause());
                _tweenContainer.RegDoTween(seq);
            }
            else
            {
                bgmAudioInfo.audioSource.Pause();
            }
        }

        /// <summary>
        /// 恢复播放BGM
        /// </summary>
        /// <param name="fadeInDuration">淡入间隔</param>
        public void ResumeBgm(float fadeInDuration = 0.5f)
        {
            if (bgmAudioInfo.audioSource == null)
            {
                Debug.LogWarning("当前bgmAudioInfo为空，没有可恢复的音乐！！！");
                return;
            }

            bgmAudioInfo.audioSource.UnPause();

            if (fadeInDuration > 0)
            {
                float targetVolume = mainVolumeFactor * bgmVolumeFactor;
                var tween = bgmAudioInfo.audioSource.DOFade(targetVolume, fadeInDuration);
                _tweenContainer.RegDoTween(tween);
            }
        }

        /// <summary>
        /// 停止BGM
        /// </summary>
        /// <param name="fadeOutDuration">淡出间隔</param>
        public void StopBgm(float fadeOutDuration = 0.5f)
        {
            if (bgmAudioInfo.audioSource == null)
            {
                Debug.LogWarning("当前没有播放BGM！！！");
                return;
            }

            Sequence seq = DOTween.Sequence();

            if (fadeOutDuration > 0)
            {
                seq.Append(bgmAudioInfo.audioSource.DOFade(0, fadeOutDuration));
            }

            seq.OnComplete(() =>
            {
                bgmAudioInfo.audioSource.Stop();
                Destroy(bgmAudioInfo.audioSource.gameObject);
                bgmAudioInfo = null;
            });

            _tweenContainer.RegDoTween(seq);
        }

        #endregion

        #region SFX

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="sfxName">要播放的音效片段名称</param>
        /// <param name="loop">是否循环</param>
        public void PlaySfx(string sfxName, bool loop = false)
        {
            AudioClip clip = Resources.Load<AudioClip>(SFX_PATH_PREFIX + sfxName);

            if (clip == null)
            {
                Debug.LogError($"SFX资源加载失败: {SFX_PATH_PREFIX + sfxName}");
                return;
            }

            // 创建音效GameObject
            GameObject sfxGO = new GameObject(sfxName + "_SFX");
            sfxGO.transform.SetParent(_sfxSourcesRootGO.transform);

            AudioSource source = sfxGO.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = loop;
            float dymagicFactor = 1;
            switch (sfxName) 
            {
                case "low":
                    dymagicFactor = 0.7f;
                    break;
                case "黑胶":
                    dymagicFactor = 1.2f;
                    break;
                case "小提琴":
                    dymagicFactor = 0.7f;
                    break;
                case "手风琴":
                    dymagicFactor = 0.85f;
                    break;
                case "小号":
                    dymagicFactor = 0.85f;
                    break;
                case "单簧管":
                    dymagicFactor = 0.75f;
                    break;
                case "怪物攻击cut":
                    dymagicFactor = 0.6f;
                    break;
                case "鼓掌 胜利音cut":
                    dymagicFactor = 0.2f;
                    break;


            }

            source.volume = mainVolumeFactor * sfxVolumeFactor * dymagicFactor;
            source.Play();

            AudioInfo info = new AudioInfo(sfxName, source);
            sfxAudioInfoList.Add(info);

            // 如果不是循环音效，自动清理
            if (!loop)
            {
                StartCoroutine(DetectingSfxPlayState(info));
            }
        }

        /// <summary>
        /// 暂停音效
        /// </summary>
        /// <param name="pauseSfxName">要暂停的音效名称</param>
        public void PauseSfx(string pauseSfxName)
        {
            AudioInfo audioInfo = sfxAudioInfoList.Find(x => x.audioName == pauseSfxName);

            if (audioInfo == null)
            {
                Debug.LogWarning("未找到sfx：" + pauseSfxName);
                return;
            }

            audioInfo.audioSource.Pause();
        }

        /// <summary>
        /// 恢复播放音效
        /// </summary>
        /// <param name="sfxName">要恢复的音效名称</param>
        public void ResumeSfx(string sfxName)
        {
            AudioInfo audioInfo = sfxAudioInfoList.Find(x => x.audioName == sfxName);

            if (audioInfo == null)
            {
                Debug.LogWarning("未找到sfx：" + sfxName);
                return;
            }

            audioInfo.audioSource.UnPause();
        }

        /// <summary>
        /// 停止音效
        /// </summary>
        /// <param name="stopSfxName">要停止的音效名称</param>
        public void StopSfx(string stopSfxName)
        {
            AudioInfo audioInfo = sfxAudioInfoList.Find(x => x.audioName == stopSfxName);

            if (audioInfo == null)
            {
                Debug.LogWarning("未找到sfx：" + stopSfxName);
                return;
            }

            audioInfo.audioSource.Stop();
            sfxAudioInfoList.Remove(audioInfo);
            Destroy(audioInfo.audioSource.gameObject);
        }

        /// <summary>
        /// 停止所有音效
        /// </summary>
        public void StopAllSfx()
        {
            for (int i = sfxAudioInfoList.Count - 1; i >= 0; i--)
            {
                AudioInfo info = sfxAudioInfoList[i];
                info.audioSource.Stop();
                Destroy(info.audioSource.gameObject);
            }
            sfxAudioInfoList.Clear();
        }

        #endregion

        #region 音量控制

        /// <summary>
        /// 更新所有音频的音量
        /// </summary>
        private void UpdateAllVolumes()
        {
            // 更新BGM音量
            if (bgmAudioInfo != null && bgmAudioInfo.audioSource != null)
            {
                bgmAudioInfo.audioSource.volume = mainVolumeFactor * bgmVolumeFactor;
            }

            // 更新所有SFX音量
            foreach (var sfxInfo in sfxAudioInfoList)
            {
                if (sfxInfo.audioSource != null)
                {
                    sfxInfo.audioSource.volume = mainVolumeFactor * sfxVolumeFactor;
                }
            }
        }

        /// <summary>
        /// 修改全局音量，并保存到PlayerPrefs
        /// </summary>
        /// <param name="factor">新的全局音量</param>
        public void ChangeMainVolume(float factor)
        {
            mainVolumeFactor = Mathf.Clamp(factor, 0, 1);
            PlayerPrefs.SetFloat("MainVolume", mainVolumeFactor);
            UpdateAllVolumes();
        }

        /// <summary>
        /// 修改BGM音量
        /// </summary>
        public void ChangeBgmVolume(float factor)
        {
            bgmVolumeFactor = Mathf.Clamp(factor, 0f, 1f);
            PlayerPrefs.SetFloat("BgmVolumeFactor", bgmVolumeFactor);

            if (bgmAudioInfo != null && bgmAudioInfo.audioSource != null)
            {
                bgmAudioInfo.audioSource.volume = mainVolumeFactor * bgmVolumeFactor;
            }
        }

        /// <summary>
        /// 修改音效音量
        /// </summary>
        public void ChangeSfxVolume(float factor)
        {
            sfxVolumeFactor = Mathf.Clamp(factor, 0f, 1f);
            PlayerPrefs.SetFloat("SfxVolumeFactor", sfxVolumeFactor);

            foreach (var sfxInfo in sfxAudioInfoList)
            {
                if (sfxInfo.audioSource != null)
                {
                    sfxInfo.audioSource.volume = mainVolumeFactor * sfxVolumeFactor;
                }
            }
        }

        #endregion

        /// <summary>
        /// 检测音频播放状态并清理结束播放的音频资源
        /// </summary>
        private IEnumerator DetectingSfxPlayState(AudioInfo info)
        {
            AudioSource audioSource = info.audioSource;

            // 等待音频播放完成
            yield return new WaitWhile(() => audioSource.isPlaying);

            // 清理资源
            if (sfxAudioInfoList.Contains(info))
            {
                sfxAudioInfoList.Remove(info);
            }

            if (audioSource != null && audioSource.gameObject != null)
            {
                Destroy(audioSource.gameObject);
            }
        }

        /// <summary>
        /// 清理所有音频资源
        /// </summary>
        public void ClearAll()
        {
            StopBgm(0.2f);
            StopAllSfx();
        }
    }
}