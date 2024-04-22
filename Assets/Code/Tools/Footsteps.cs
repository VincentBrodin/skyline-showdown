using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Tools{
    public class Footsteps : MonoBehaviour{
        public LayerMask groundLayer;
        public AudioClip[] footstepSounds;
        public Vector2 pitchRange;
        [Space] public float footHeight;
        [Header("Left Foot")] public Transform leftFoot;
        public AudioSource leftFootAudioSource;
        [Header("Right Foot")] public Transform rightFoot;
        public AudioSource rightFootAudioSource;

        [Header("Experimental")] public bool useExperimental;
        public ExperimentalFeatures experimentalFeatures = new();

        [Serializable]
        public class ExperimentalFeatures{
            public bool useRbSpeed;
            public Rigidbody rb;
            public float rbSpeedCutoff;
        }

        private int _leftLastIndex, _rightLastIndex;
        private bool _hasLiftLeft, _hasLiftRight;

        private void FixedUpdate(){
            if (Physics.Raycast(leftFoot.position, Vector3.down, footHeight, groundLayer) && !ExperimentalCutoff()){
                if (!_hasLiftLeft) return;
                _hasLiftLeft = false;

                //Random pitch to make footsteps more random
                leftFootAudioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);

                //Make sure the same footstep is not played twice
                int rndSound = Random.Range(0, footstepSounds.Length);
                if (_leftLastIndex == rndSound)
                    _leftLastIndex = (_leftLastIndex + 1) % footstepSounds.Length;
                else
                    _leftLastIndex = rndSound;

                leftFootAudioSource.PlayOneShot(footstepSounds[_leftLastIndex]);
            }
            else{
                _hasLiftLeft = true;
            }

            if (Physics.Raycast(rightFoot.position, Vector3.down, footHeight, groundLayer) && !ExperimentalCutoff()){
                if (!_hasLiftRight) return;
                _hasLiftRight = false;

                //Random pitch to make footsteps more random
                rightFootAudioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);

                //Make sure the same footstep is not played twice
                int rndSound = Random.Range(0, footstepSounds.Length);
                if (_rightLastIndex == rndSound)
                    _rightLastIndex = (_rightLastIndex + 1) % footstepSounds.Length;
                else
                    _rightLastIndex = rndSound;

                rightFootAudioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
            }
            else{
                _hasLiftRight = true;
            }
        }

        private bool ExperimentalCutoff(){
            if (!useExperimental) return false;

            if (experimentalFeatures.useRbSpeed){
                Vector3 velocity = experimentalFeatures.rb.velocity;
                velocity.y = 0;
                if (velocity.magnitude < experimentalFeatures.rbSpeedCutoff) return true;
            }

            return false;
        }

        private void OnDrawGizmos(){
            if (leftFoot)
                Gizmos.DrawRay(leftFoot.position, Vector3.down * footHeight);
            if (rightFoot)
                Gizmos.DrawRay(rightFoot.position, Vector3.down * footHeight);
        }
    }
}