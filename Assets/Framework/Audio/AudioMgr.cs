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

        internal IEnumerable<object> ToArray()
        {
            throw new NotImplementedException();
        }
    }
    public class AudioMgr : SingletonMono<AudioMgr>
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
        protected override void Awake()
        {
            base.Awake();

            // 创建BGM和SFX的AudioSource根节点
            _bgmSourcesRootGO = new GameObject("BGM_ROOT");
            _sfxSourcesRootGO = new GameObject("SFX_ROOT");

            _bgmSourcesRootGO.transform.SetParent(transform);
            _sfxSourcesRootGO.transform.SetParent(transform);

            // 没有存储的设定就使用默认的音量
            mainVolumeFactor = PlayerPrefs.HasKey("MainVolume") ? PlayerPrefs.GetFloat("MainVolume") : 1;
            bgmVolumeFactor = PlayerPrefs.HasKey("BgmVolumeFactor") ? PlayerPrefs.GetFloat("BgmVolumeFactor") : 1;
            sfxVolumeFactor = PlayerPrefs.HasKey("SfxVolumeFactor") ? PlayerPrefs.GetFloat("SfxVolumeFactor") : 1;

            _tweenContainer = new TweenContainer();
        }

        private void Start()
        {
            ChangeMainVolume(mainVolumeFactor);
            ChangeBgmVolume(bgmVolumeFactor);
            ChangeSfxVolume(sfxVolumeFactor);
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

            if(bgmAudioInfo == null)
            {
                AudioSource source = _bgmSourcesRootGO.AddComponent<AudioSource>();
                source.clip = Resources.Load<AudioClip>("Audio/Bgm/"+ fadeInMusicName);
                source.loop = loop;
                source.volume = fadeInDuration > 0 ? 0 : mainVolumeFactor * bgmVolumeFactor;
            }
            else
            {

                if (bgmAudioInfo.audioName == fadeInMusicName)
                {
                    Debug.LogWarning("当前已经在播放" + fadeInMusicName + "，无法再次播放！！！");
                    return;
                }

                Sequence seq = DOTween.Sequence();
                if(fadeOutDuration > 0)
                {
                    seq.Append(bgmAudioInfo.audioSource.DOFade(0, fadeOutDuration)).
                        OnComplete(() =>
                        {
                            bgmAudioInfo.audioSource.clip = Resources.Load<AudioClip>("Audio/Bgm/" + fadeInMusicName);
                            bgmAudioInfo.audioSource.loop = loop;
                            bgmAudioInfo.audioSource.volume = fadeInDuration > 0 ? 0 : mainVolumeFactor * bgmVolumeFactor;
                        });
                }
                else
                {
                    bgmAudioInfo.audioSource.clip = Resources.Load<AudioClip>("Audio/Bgm/" + fadeInMusicName);
                    bgmAudioInfo.audioSource.loop = loop;
                    bgmAudioInfo.audioSource.volume = fadeInDuration > 0 ? 0 : mainVolumeFactor * bgmVolumeFactor;
                }
                if (fadeInDuration > 0)
                {
                    seq.Append(bgmAudioInfo.audioSource.DOFade(mainVolumeFactor * bgmVolumeFactor, fadeInDuration));
                }
                bgmAudioInfo.audioSource.Play();
                bgmAudioInfo.audioName = fadeInMusicName;
                _tweenContainer.RegDoTween(seq);
            }
        }

        /// <summary>
        /// 暂停BGM
        /// </summary>
        /// <param name="pauseBgmName">要暂停的片段名称</param>
        /// <param name="fadeOutDuration">淡出间隔</param>
        public void PauseBgm(string pauseBgmName, float fadeOutDuration = 0.5f)
        { 
        }


        /// <summary>
        /// 停止BGM
        /// </summary>
        /// <param name="stopBgmName">要停止的片段名称</param>
        /// <param name="fadeOutDuration">淡出间隔</param>
        public void StopBgm(string stopBgmName, float fadeOutDuration = 0.5f)
        {
            //AudioInfo audioInfo = bgmAudioInfoList.Find(x => x.audioName == stopBgmName);

            //if (audioInfo == null)
            //{
            //    Debug.LogWarning("未找到BGM：" + stopBgmName);
            //    return;
            //}

            //Sequence s = DOTween.Sequence();

            //s.Append(audioInfo.audioSource.DOFade(0, fadeOutDuration).OnComplete(() =>
            //{
            //    audioInfo.audioSource.Stop();

            //    Destroy(audioInfo.audioSource.gameObject);
            //}));

            //bgmAudioInfoList.Remove(audioInfo);
        }

        #endregion

        #region Sfx

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="sfxName">要播放的音效片段名称</param>
        /// <param name="loop">是否循环</param>
        public void PlaySfx(string sfxName, bool loop = false)
        {
            //Sequence s = DOTween.Sequence();

            //// 从音频列表中寻找
            //AudioData sfxData = audioDatas.audioDataList.Find(x => x.audioName == sfxName);

            //if (sfxData == null)
            //{
            //    Debug.LogWarning("未找到sfx：" + sfxName);
            //    return;
            //}

            //// 创建音频播放器
            //GameObject sfxAudioGO = new GameObject(sfxName);
            //sfxAudioGO.transform.SetParent(_sfxSourcesRootGO.transform);

            //AudioSource sfxAudioSource = sfxAudioGO.AddComponent<AudioSource>();
            //sfxAudioSource.clip = Resources.Load<AudioClip>(sfxData.audioPath);
            //sfxAudioSource.loop = loop;

            //sfxAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[2]; // 设置为音频混合器的 "Master" 组，确保应用音量控制

            //sfxAudioSource.Play();

            //AudioInfo info = new AudioInfo();
            //info.audioName = sfxName;
            //info.audioSource = sfxAudioSource;
            //sfxAudioInfoList.Add(info);

            //StartCoroutine(DetectingAudioPlayState(info, false));
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
        /// 停止音效
        /// </summary>
        /// <param name="stopSfxName">要停止的音效名称</param>
        public void StopSfx(string stopSfxName)
        {
            //AudioInfo audioInfo = bgmAudioInfo.Find(x => x.audioName == stopSfxName);

            //if (audioInfo == null)
            //{
            //    Debug.LogWarning("未找到sfx：" + stopSfxName);
            //    return;
            //}

            //audioInfo.audioSource.Stop();

            //bgmAudioInfo.Remove(audioInfo);

            //Destroy(audioInfo.audioSource.gameObject);
        }

        #endregion

        #region 音量

        /// <summary>
        /// 修改全局音量，并保存到PlayerPrefs
        /// </summary>
        /// <param name="factor">新的全局音量</param>
        public void ChangeMainVolume(float factor)
        {
            mainVolumeFactor = factor;

            mainVolumeFactor = Mathf.Clamp(mainVolumeFactor, 0, 1);

            PlayerPrefs.SetFloat("MainVolume", mainVolumeFactor);
        }

        /// <summary>
        /// 修改BGM音量并使用AudioMixer控制音量
        /// </summary>
        public void ChangeBgmVolume(float factor)
        {
            bgmVolumeFactor = factor;

            bgmVolumeFactor = Mathf.Clamp(bgmVolumeFactor, 0f, 1f);

            PlayerPrefs.SetFloat("BgmVolumeFactor", bgmVolumeFactor);
        }

        /// <summary>
        /// 修改音效音量并使用AudioMixer控制音量
        /// </summary>
        public void ChangeSfxVolume(float factor)
        {
            sfxVolumeFactor = factor;

            sfxVolumeFactor = Mathf.Clamp(sfxVolumeFactor, 0f, 1f);

            PlayerPrefs.SetFloat("SfxVolumeFactor", sfxVolumeFactor);

        }

        #endregion

        /// <summary>
        /// 检测音频播放状态并清理结束播放的音频资源
        /// </summary>
        IEnumerator DetectingSfxPlayState(AudioInfo info)
        {
            AudioSource audioSource = info.audioSource;
            while (audioSource.isPlaying)
            {
                yield return null;
            }
            sfxAudioInfoList.Remove(info);

            Destroy(info.audioSource.gameObject);
        }
    }
}
