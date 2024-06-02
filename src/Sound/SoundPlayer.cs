using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace TinyShopping {

    public class SoundPlayer {
        protected int _musicMasterVolume;
        protected int _effectsMasterVolume;

        protected List<SoundEffectInstance> _currentSoundEffects;
        protected List<SoundEffectInstance> _currentSongs;


        public SoundPlayer(int musicMasterVolume, int effectsMasterVolume) {
            _musicMasterVolume = musicMasterVolume;
            _effectsMasterVolume = effectsMasterVolume;
            _currentSongs = new List<SoundEffectInstance>();
            _currentSoundEffects = new List<SoundEffectInstance>();
        }

        // Use this for fire and forget, one time sound effect
        public bool playSoundEffect(SoundEffect soundEffect, float volumeFactor) {
            try {
                soundEffect.Play(0.01f * (float)_effectsMasterVolume, 0f, 0f);
                return true;
            } catch (NullReferenceException ex) {
                // This case can happen if sound effect has been unloaded but update function is still running
                // This should be fixed in the game logic itself (update should not be called after unloading)
                return false;
            }
        }

        // Use this for looping sound effects, will change sound volume upon 
        public void playSoundEffectInstance(SoundEffectInstance soundEffect, float volumeFactor) {
            soundEffect.Volume = Math.Clamp((0.01f * (float)_effectsMasterVolume) * volumeFactor, 0, 1);
            soundEffect.Play();

            if (!_currentSoundEffects.Contains(soundEffect)) {
                _currentSoundEffects.Add(soundEffect);
            }
        }

        public void playSong(SoundEffectInstance song, float volumeFactor) {
            song.Volume = Math.Clamp((0.01f * (float)_musicMasterVolume) * volumeFactor, 0, 1);
            song.Play();

            if (!_currentSongs.Contains(song)) {
                _currentSongs.Add(song);
            }
        }

        public void RemoveCurrent() {
            foreach (var song in _currentSongs) {
                song.Stop();
                //song.Dispose();
            }
            _currentSongs.Clear();

            foreach (var effect in _currentSoundEffects) {
                effect.Stop();
                //effect.Dispose();
            }
            _currentSoundEffects.Clear();
        }

        public void SetMusicMasterVolume(int volume) {
            _musicMasterVolume = volume;
            foreach (var song in _currentSongs) {
                song.Volume = 0.01f * (float)_musicMasterVolume;
            }
        }

        public void SetEffectsMasterVolume(int volume) {
            _effectsMasterVolume = volume;
            foreach (var effect in _currentSoundEffects) {
                effect.Volume = 0.01f * (float)_effectsMasterVolume ;
            }
        }

    }
}