using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GJFramework
{
    [Serializable]
    public class ParticleInfo
    {
        public string effectName;
        public ParticleSystem particleSystem;
        public GameObject rootGameObject;
        public Coroutine autoDestroyCoroutine;

        public ParticleInfo(string name, ParticleSystem system, GameObject root)
        {
            effectName = name;
            particleSystem = system;
            rootGameObject = root;
        }
    }

    public class ParticleMgr : SingletonPersistent<ParticleMgr>
    {
        public List<ParticleInfo> activeParticleEffects;

        private GameObject _particleRootGO;
        private Dictionary<string, GameObject> _particlePrefabCache;

        // 粒子资源路径配置
        private const string PARTICLE_PATH_PREFIX = "Particle/";

        protected override void Awake()
        {
            base.Awake();

            _particleRootGO = new GameObject("PARTICLE_ROOT");
            _particleRootGO.transform.SetParent(transform);

            _particlePrefabCache = new Dictionary<string, GameObject>();
            activeParticleEffects = new List<ParticleInfo>();
        }

        private void OnDestroy()
        {
            // 清理所有协程和资源
            ClearAllEffects();
        }

        #region 主要播放方法

        /// <summary>
        /// 播放粒子特效
        /// </summary>
        public ParticleSystem PlayEffect(string effectName,
            Vector3 position,
            Quaternion rotation = default,
            Transform parent = null,
            bool autoDestroy = true,
            float destroyDelay = -1f,
            Vector3? scale = null)
        {
            // 加载粒子预制体
            GameObject prefab = GetParticlePrefab(effectName);
            if (prefab == null)
            {
                Debug.LogWarning($"粒子特效预制体加载失败：{effectName}");
                return null;
            }

            // 创建粒子实例
            Transform parentTransform = parent != null ? parent : _particleRootGO.transform;
            GameObject instance = Instantiate(prefab, position, rotation == default ? Quaternion.identity : rotation, parentTransform);

            if (scale.HasValue)
            {
                instance.transform.localScale = scale.Value;
            }

            ParticleSystem particleSystem = instance.GetComponent<ParticleSystem>();
            if (particleSystem == null)
            {
                particleSystem = instance.GetComponentInChildren<ParticleSystem>();
                if (particleSystem == null)
                {
                    Debug.LogWarning($"粒子预制体上没有ParticleSystem组件：{effectName}");
                    Destroy(instance);
                    return null;
                }
            }

            // 创建粒子信息
            ParticleInfo info = new ParticleInfo(effectName, particleSystem, instance);

            // 播放粒子
            particleSystem.Play();

            // 自动销毁处理
            if (autoDestroy)
            {
                float delay = destroyDelay >= 0 ? destroyDelay : GetParticleDuration(particleSystem);
                info.autoDestroyCoroutine = StartCoroutine(AutoDestroyEffect(info, delay));
            }

            activeParticleEffects.Add(info);
            return particleSystem;
        }

        #endregion

        #region 简化重载方法

        // 重载1：仅需特效名称和位置
        public ParticleSystem PlayEffect(string effectName, Vector3 position)
        {
            return PlayEffect(effectName, position, default, null, true, -1f, null);
        }

        // 重载2：指定位置和旋转
        public ParticleSystem PlayEffect(string effectName, Vector3 position, Quaternion rotation)
        {
            return PlayEffect(effectName, position, rotation, null, true, -1f, null);
        }

        // 重载3：指定父物体
        public ParticleSystem PlayEffect(string effectName, Vector3 position, Transform parent)
        {
            return PlayEffect(effectName, position, default, parent, true, -1f, null);
        }

        // 重载4：完全自定义参数
        public ParticleSystem PlayEffect(string effectName,
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            bool autoDestroy,
            float destroyDelay)
        {
            return PlayEffect(effectName, position, rotation, parent, autoDestroy, destroyDelay, null);
        }

        // 重载5：自定义缩放
        public ParticleSystem PlayEffect(string effectName, Vector3 position, Vector3 scale)
        {
            return PlayEffect(effectName, position, default, null, true, -1f, scale);
        }

        // 重载6：跟随父物体（位置和旋转随父物体）
        public ParticleSystem PlayEffectOnTransform(string effectName, Transform parent, Vector3? localPosition = null)
        {
            Vector3 position = localPosition.HasValue ? parent.TransformPoint(localPosition.Value) : parent.position;
            return PlayEffect(effectName, position, parent.rotation, parent, true, -1f, null);
        }

        // 重载7：世界坐标播放，不自动销毁（用于持续效果）
        public ParticleSystem PlayEffectPersistent(string effectName, Vector3 position)
        {
            return PlayEffect(effectName, position, default, null, false, -1f, null);
        }

        #endregion

        #region 特效控制

        /// <summary>
        /// 停止指定名称的粒子特效
        /// </summary>
        public void StopEffect(string effectName, bool immediate = false)
        {
            List<ParticleInfo> effectsToRemove = new List<ParticleInfo>();

            for (int i = activeParticleEffects.Count - 1; i >= 0; i--)
            {
                ParticleInfo info = activeParticleEffects[i];
                if (info.effectName == effectName)
                {
                    if (immediate)
                    {
                        DestroyParticleEffect(info);
                        effectsToRemove.Add(info);
                    }
                    else
                    {
                        StopParticleEffect(info);
                    }
                }
            }

            // 移除已销毁的特效
            foreach (var effect in effectsToRemove)
            {
                activeParticleEffects.Remove(effect);
            }
        }

        /// <summary>
        /// 停止所有指定名称的粒子特效
        /// </summary>
        public void StopAllEffectsByName(string effectName, bool immediate = false)
        {
            StopEffect(effectName, immediate);
        }

        /// <summary>
        /// 暂停粒子特效
        /// </summary>
        public void PauseEffect(string effectName)
        {
            foreach (var info in activeParticleEffects)
            {
                if (info.effectName == effectName && info.particleSystem != null)
                {
                    info.particleSystem.Pause();
                }
            }
        }

        /// <summary>
        /// 暂停所有粒子特效
        /// </summary>
        public void PauseAllEffects()
        {
            foreach (var info in activeParticleEffects)
            {
                if (info.particleSystem != null)
                {
                    info.particleSystem.Pause();
                }
            }
        }

        /// <summary>
        /// 恢复播放粒子特效
        /// </summary>
        public void ResumeEffect(string effectName)
        {
            foreach (var info in activeParticleEffects)
            {
                if (info.effectName == effectName && info.particleSystem != null)
                {
                    info.particleSystem.Play();
                }
            }
        }

        /// <summary>
        /// 恢复所有粒子特效
        /// </summary>
        public void ResumeAllEffects()
        {
            foreach (var info in activeParticleEffects)
            {
                if (info.particleSystem != null)
                {
                    info.particleSystem.Play();
                }
            }
        }

        /// <summary>
        /// 停止所有粒子特效
        /// </summary>
        public void StopAllEffects(bool immediate = false)
        {
            for (int i = activeParticleEffects.Count - 1; i >= 0; i--)
            {
                ParticleInfo info = activeParticleEffects[i];
                if (immediate)
                {
                    DestroyParticleEffect(info);
                }
                else
                {
                    StopParticleEffect(info);
                }
            }

            if (immediate)
            {
                activeParticleEffects.Clear();
            }
        }

        /// <summary>
        /// 清理所有粒子特效
        /// </summary>
        public void ClearAllEffects()
        {
            StopAllEffects(true);
        }

        /// <summary>
        /// 清理所有已完成的粒子特效
        /// </summary>
        public void CleanupFinishedEffects()
        {
            for (int i = activeParticleEffects.Count - 1; i >= 0; i--)
            {
                ParticleInfo info = activeParticleEffects[i];
                if (info.particleSystem == null || !info.particleSystem.IsAlive())
                {
                    DestroyParticleEffect(info);
                    activeParticleEffects.RemoveAt(i);
                }
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取粒子预制体（带缓存）
        /// </summary>
        private GameObject GetParticlePrefab(string effectName)
        {
            if (_particlePrefabCache.TryGetValue(effectName, out GameObject prefab))
            {
                return prefab;
            }

            string path = PARTICLE_PATH_PREFIX + effectName;
            prefab = Resources.Load<GameObject>(path);
            if (prefab != null)
            {
                _particlePrefabCache[effectName] = prefab;
            }
            else
            {
                Debug.LogWarning($"粒子资源加载失败，路径：{path}");
            }

            return prefab;
        }

        /// <summary>
        /// 获取粒子系统持续时间
        /// </summary>
        private float GetParticleDuration(ParticleSystem ps)
        {
            if (ps == null) return 0f;

            ParticleSystem.MainModule main = ps.main;
            return main.duration + main.startLifetime.constantMax;
        }

        /// <summary>
        /// 停止粒子效果（优雅停止）
        /// </summary>
        private void StopParticleEffect(ParticleInfo info)
        {
            if (info.particleSystem != null)
            {
                info.particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                // 重新设置自动销毁
                if (info.autoDestroyCoroutine != null)
                {
                    StopCoroutine(info.autoDestroyCoroutine);
                }
                info.autoDestroyCoroutine = StartCoroutine(AutoDestroyEffect(info, GetParticleDuration(info.particleSystem)));
            }
        }

        /// <summary>
        /// 立即销毁粒子效果
        /// </summary>
        private void DestroyParticleEffect(ParticleInfo info)
        {
            if (info.autoDestroyCoroutine != null)
            {
                StopCoroutine(info.autoDestroyCoroutine);
            }

            if (info.rootGameObject != null)
            {
                Destroy(info.rootGameObject);
            }
        }

        /// <summary>
        /// 自动销毁粒子特效
        /// </summary>
        private IEnumerator AutoDestroyEffect(ParticleInfo info, float delay)
        {
            yield return new WaitForSeconds(delay);

            // 等待粒子完全消失
            if (info.particleSystem != null)
            {
                yield return new WaitWhile(() => info.particleSystem.IsAlive());
            }

            // 从列表中移除并销毁对象
            if (activeParticleEffects.Contains(info))
            {
                activeParticleEffects.Remove(info);
            }

            if (info.rootGameObject != null)
            {
                Destroy(info.rootGameObject);
            }
        }

        /// <summary>
        /// 检查特效是否正在播放
        /// </summary>
        public bool IsEffectPlaying(string effectName)
        {
            foreach (var info in activeParticleEffects)
            {
                if (info.effectName == effectName && info.particleSystem != null && info.particleSystem.IsAlive())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取指定名称的所有活跃粒子特效
        /// </summary>
        public List<ParticleSystem> GetEffectsByName(string effectName)
        {
            List<ParticleSystem> results = new List<ParticleSystem>();
            foreach (var info in activeParticleEffects)
            {
                if (info.effectName == effectName && info.particleSystem != null)
                {
                    results.Add(info.particleSystem);
                }
            }
            return results;
        }

        /// <summary>
        /// 预加载粒子资源（可选，用于提前加载常用特效）
        /// </summary>
        public void PreloadEffect(string effectName)
        {
            GetParticlePrefab(effectName);
        }

        /// <summary>
        /// 清理资源缓存（在内存紧张时调用）
        /// </summary>
        public void ClearCache()
        {
            _particlePrefabCache.Clear();
            Resources.UnloadUnusedAssets();
        }

        #endregion
    }
}