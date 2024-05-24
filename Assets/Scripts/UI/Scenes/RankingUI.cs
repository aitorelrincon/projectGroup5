using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BugCatcher.Utils;
using BugCatcher.Extensions.Functional;

public class RankingUI : MonoShared<RankingUI>
{
    public const string HEADER = "Ranking\n#, NAME, SECS, SCORE\n";
    [SerializeField] TMPro.TMP_Text       _rankingTmp;
    [SerializeField] TMPro.TMP_InputField _inputField;

    void Start()
    {
        SetRanking();

        _inputField.gameObject.transform.parent.gameObject.SetActive( Ranking.lastGame.i >= 0 && Ranking.lastGame.i < Ranking.ranking.Count );
    }

    public bool SaveEntry()
    {
        if ( Ranking.SaveLastScore( _inputField.name ) )
        {
            _inputField.gameObject.transform.parent.gameObject.SetActive( false );
            _inputField.text = "";
            return true;
        }

        return false;
    }

    void SetRanking()
    {
        StringBuilder result = new();
        for ( int i = 0 ; i < Ranking.ranking.Count ; ++i )
        {
            var e = Ranking.ranking[i];
            result
                .AppendFormat("{0}, {1}, {2}, {3}\n",
                              i+1, e.name, Timer.FmtMinutes( e.secs ), e.score);
        }

        _rankingTmp.text = HEADER + result.ToString();
    }
}
