using System;
using UnityEngine;

namespace BugCatcher.Utils
{
    public class Timer : MonoBehaviour
    {
        public enum Count { Down = -1, Up = 1 }

        public Count CountMode = Count.Up;
        [Min(0)] public float startSecs = 0f;
        public bool PlayOnStart = true;
        public bool PauseOnDisable = false;
        public bool ResumeOnEnable = false;

        public delegate void OnResume();
        public event OnResume onResume;

        public delegate void OnPause();
        public event OnPause onPause;

        public delegate void OnSetSecs();
        public event OnSetSecs onSetSecs;

        float _secs = 0f;
        [HideInInspector]
        public float Secs
        {
            get => _secs;
            set
            {
                _secs = value;
                if ( onSetSecs is not null )
                    onSetSecs.Invoke();
            }
        }

        [HideInInspector] public bool IsRunning { get; private set; }

        void Start()
        {
            Secs = startSecs;
            IsRunning = PlayOnStart;
        }

        void Update()
        {
            if ( IsRunning )
                Secs += Time.deltaTime * (float)CountMode;
        }

        public void Resume()
        {
            IsRunning = true;
            if ( onResume is not null )
                onResume();
        }

        public void Pause()
        {
            IsRunning = false;
            if ( onPause is not null )
                onPause();
        }

        void OnDisable()
        {
            IsRunning = !PauseOnDisable || IsRunning;
            if ( PauseOnDisable ) onPause.Invoke();
        }

        void OnEnable()
        {
            IsRunning = ResumeOnEnable || IsRunning;
            if ( ResumeOnEnable ) onResume.Invoke();
        }

        public void ResetToZero()  => Secs = 0;
        public void ResetToStart() => Secs = startSecs;

        public string FmtMinutes()
            => $"{Mathf.FloorToInt( Secs / 60 ):00}:{Mathf.FloorToInt( Secs % 60 ):00}";

        public ReadOnlySpan<char> SpanMinutes()
            => $"{Mathf.FloorToInt( Secs / 60 ):00}:{Mathf.FloorToInt( Secs % 60 ):00}".AsSpan();

        public bool TryFormatMinutes(Span<char> dest, int start)
        {
            // Not enough space for formatting
            if ( start + 5 > dest.Length )
                return false;

            var m = (short)Mathf.Min(Mathf.FloorToInt( Secs / 60), 99 );
            var s = (short)Mathf.FloorToInt( Secs % 60 );

            // MM:SS, Max 99 minutes
            dest[start + 0] = (char)( m / 10 + '0' );
            dest[start + 1] = (char)( m % 10 + '0' );
            dest[start + 2] = ':';
            dest[start + 3] = (char)( s / 10 + '0' );
            dest[start + 4] = (char)( s % 10 + '0' );
            return true;
        }
    }
}
