using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyOnParticleFinish : MonoBehaviour {

    ParticleSystem _particleSystem;
    public void Awake(){
        _particleSystem = GetComponent<ParticleSystem>();
    }
    public void Update(){
        if(_particleSystem.isPlaying) return;

        Destroy(gameObject);
    }
}