using UnityEngine;
using System;
using System.Collections;


public class ParticleSystemSound:MonoBehaviour{
	public AudioClip[] _shootSound;
	
	public float _shootPitchMax = 1.25f;
	public float _shootPitchMin = 0.75f;
	public float _shootVolumeMax = 0.75f;
	public float _shootVolumeMin = 0.25f;
	
	public AudioClip[] _explosionSound;
	public float _explosionPitchMax = 1.25f;
	public float _explosionPitchMin = 0.75f;
	public float _explosionVolumeMax = 0.75f;
	public float _explosionVolumeMin = 0.25f;
	
	public AudioClip[] _crackleSound;
	public float _crackleDelay = .25f;
	public int _crackleMultiplier = 3;
	public float _cracklePitchMax = 1.25f;
	public float _cracklePitchMin = 0.75f;
	public float _crackleVolumeMax = 0.75f;
	public float _crackleVolumeMin = 0.25f;
	
	public void LateUpdate() {
		ParticleSystem.Particle[] particles  = new ParticleSystem.Particle[GetComponent<ParticleSystem>().particleCount];
		int length = GetComponent<ParticleSystem>().GetParticles(particles);
		int i = 0;
		while (i < length){
	 		if(_explosionSound.Length > 0 && particles[i].remainingLifetime < Time.deltaTime){
	 			SoundController.instance.Play(_explosionSound[UnityEngine.Random.Range(0, _explosionSound.Length)], UnityEngine.Random.Range(_explosionVolumeMax,_explosionVolumeMin), UnityEngine.Random.Range(_explosionPitchMin,_explosionPitchMax), particles[i].position);
				if(_crackleSound.Length > 0){
				for(int j = 0; j <_crackleMultiplier; j++){
					StartCoroutine(Crackle(particles[i].position, _crackleDelay+j*.1f));
				}
	 		}
	 		}
	 		if(_shootSound.Length > 0 && particles[i].remainingLifetime >= particles[i].startLifetime-Time.deltaTime){
	 			SoundController.instance.Play(_shootSound[UnityEngine.Random.Range(0, _shootSound.Length)], UnityEngine.Random.Range(_shootVolumeMax,_shootVolumeMin), UnityEngine.Random.Range(_shootPitchMin,_shootPitchMax), particles[i].position);
			}
			i++;
		}
	}
	
	public IEnumerator Crackle(Vector3 pos,float delay){
		yield return new WaitForSeconds(delay);
		SoundController.instance.Play(_crackleSound[UnityEngine.Random.Range(0, _crackleSound.Length)], UnityEngine.Random.Range(_crackleVolumeMax,_crackleVolumeMin), UnityEngine.Random.Range(_cracklePitchMax,_cracklePitchMin), pos);
	}
}
