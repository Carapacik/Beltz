#pragma strict
var _shootSound:AudioClip[];

var _shootPitchMax:float = 1.25;
var _shootPitchMin:float = 0.75;
var _shootVolumeMax:float = 0.75;
var _shootVolumeMin:float = 0.25;

var _explosionSound:AudioClip[];
var _explosionPitchMax:float = 1.25;
var _explosionPitchMin:float = 0.75;
var _explosionVolumeMax:float = 0.75;
var _explosionVolumeMin:float = 0.25;

var _crackleSound:AudioClip[];
var _crackleDelay:float = .25;
var _crackleMultiplier:int = 3;
var _cracklePitchMax:float = 1.25;
var _cracklePitchMin:float = 0.75;
var _crackleVolumeMax:float = 0.75;
var _crackleVolumeMin:float = 0.25;

function LateUpdate() {
	var particles:ParticleSystem.Particle[]  = new ParticleSystem.Particle[particleSystem.particleCount];
	var length:int = particleSystem.GetParticles(particles);
	var i:int = 0;
	while (i < length){
 		if(_explosionSound.Length > 0 && particles[i].lifetime < Time.deltaTime){
 			SoundController.instance.Play(_explosionSound[Random.Range(0, _explosionSound.Length)], Random.Range(_explosionVolumeMax,_explosionVolumeMin), Random.Range(_explosionPitchMin,_explosionPitchMax), particles[i].position);
			if(_crackleSound.Length > 0){
			for(var j:int = 0; j <_crackleMultiplier; j++){
				Crackle(particles[i].position, _crackleDelay+j*.1);
			}
 		}
 		}
 		if(_shootSound.Length > 0 && particles[i].lifetime >= particles[i].startLifetime-Time.deltaTime){
 			SoundController.instance.Play(_shootSound[Random.Range(0, _shootSound.Length)], Random.Range(_shootVolumeMax,_shootVolumeMin), Random.Range(_shootPitchMin,_shootPitchMax), particles[i].position);
		}
		i++;
	}
}

function Crackle(pos:Vector3, delay:float){
	yield WaitForSeconds(delay);
	SoundController.instance.Play(_crackleSound[Random.Range(0, _crackleSound.Length)], Random.Range(_crackleVolumeMax,_crackleVolumeMin), Random.Range(_cracklePitchMax,_cracklePitchMin), pos);
}