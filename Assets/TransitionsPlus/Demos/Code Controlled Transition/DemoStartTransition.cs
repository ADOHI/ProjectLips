using UnityEngine;
using TransitionsPlus;

namespace TransitionsPlusDemos {

    public class DemoStartTransition : MonoBehaviour {

        public TransitionAnimator animator;

        public void Play() {
            animator.profile.invert = false;
            animator.Play();
        }

        public void PlayBackwards() {
            animator.profile.invert = true;
            animator.Play();
        }

        public void PauseOrResume() {
            if (animator.isPlaying) {
                animator.enabled = !animator.enabled;
            }
        }

    }

}