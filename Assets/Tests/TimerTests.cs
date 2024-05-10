using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

using BugCatcher.Utils;
using UnityEngine.Assertions;

public class TimerTests
{
    [UnityTest]
    public IEnumerator CountUp()
    {
        const float WAIT = 2f;
        var timer = new GameObject().AddComponent<Timer>();
        timer.CountMode = Timer.Count.Up;
        timer.Resume();

        yield return new WaitForSeconds( WAIT );

        Assert.AreApproximatelyEqual( WAIT, timer.Secs, 0.05f );
    }

    [UnityTest]
    public IEnumerator OnSetSecondsInvokes()
    {
        int a = 0, b = 0;
        var timer = new GameObject().AddComponent<Timer>();
        timer.Pause();
        timer.onSetSecs += () => ++a;
        timer.onSetSecs += () => --b;

        yield return new WaitForSeconds( 1E-6f );

        Assert.AreNotEqual( a, 0 );
        Assert.AreNotEqual( b, 0 );
        Assert.AreNotEqual( a, b );
    }
}
