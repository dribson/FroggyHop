using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;

enum TSF_Type
{
    Linear,
    SineEaseIn,
    SineEaseOut,
    SineEaseInOut,
    CubicEaseIn,
    CubicEaseOut,
    CubicEaseInOut,
    QuadraticEaseIn,
    QuadraticEaseOut,
    QuadraticEaseInOut,
    QuinticEaseIn,
    QuinticEaseOut,
    QuinticEaseInOut
}

public class Collectable : MonoBehaviour
{
    [SerializeField] int PointsEarned;
    [SerializeField] bool shouldMove, randomTweenPoints;
    public System.Action<ITween<Vector2>> TweenMoveTo;
    Vector2 tweenStartPos;
    [SerializeField] List<Vector2> tweenMoveList;
    [SerializeField] float tweenStepDuration;
    Vector2Tween v2t = new Vector2Tween();
    [SerializeField] TSF_Type My_TSF_Type;
    System.Func<float, float> TSF;
    string tweenKeyName;
    SpriteRenderer SR;

    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        SR.flipX = (Random.Range(0f, 10f) < 5f);
        if (shouldMove) 
        {
            if (randomTweenPoints)
            {
                RandomizeTweenPoints();
            }
            GenerateTween(); 
        }
    }

    void RandomizeTweenPoints()
    {
        for(int i = 0; i < tweenMoveList.Count; i++)
        {
            //tweenMoveList[i] = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            switch (i % 4)
            {
                case 0:
                    tweenMoveList[i] = new Vector2(Random.Range(0.6f, 1f), Random.Range(0.6f, 1f));
                    break;
                case 1:
                    tweenMoveList[i] = new Vector2(Random.Range(-0.6f, -1f), Random.Range(0.6f, 1f));
                    break;
                case 2:
                    tweenMoveList[i] = new Vector2(Random.Range(-0.6f, -1f), Random.Range(-0.6f, -1f));
                    break;
                case 3:
                    tweenMoveList[i] = new Vector2(Random.Range(0.6f, 1f), Random.Range(-0.6f, -1f));
                    break;
            }
        }
        tweenMoveList.Shuffle();
        
    }

    void GenerateTween()
    {
        {
            //tweenStepDuration += Random.Range(-0.05f, 0.05f); // TODO get good way to add flat random values to slightly change tweenStepDuration to add more variety between collectables
            tweenKeyName = gameObject.name + transform.position.x.ToString() + transform.position.y.ToString();
            TSF = GetTSF(My_TSF_Type);
            for (int i = 0; i < tweenMoveList.Count; i++)
            {
                tweenMoveList[i] += new Vector2(transform.position.x, transform.position.y);
            }
            StartCoroutine(RepeatTween());
        }
    }

    public int GainPoints()
    {
        return PointsEarned;
    }

    void TweenMovement()
    {
        this.TweenMoveTo = (t) =>
        {
            transform.position = t.CurrentValue;
        };
        CreateTween();
    }

    void CreateTween()
    {
        tweenStartPos = transform.position;
        Tween<Vector2> nextTween;
        if (randomTweenPoints) // TODO see about randomizing the end position of the last tween within reason to not make tweek always start/end on same point
        {
            nextTween = v2t = gameObject.Tween(tweenKeyName, tweenMoveList[0], tweenMoveList[1], tweenStepDuration, TSF, this.TweenMoveTo);
            for (int i = 1; i < tweenMoveList.Count - 1; i++)
            {
                nextTween = nextTween.ContinueWith(GenerateTweenStep(tweenMoveList[i], tweenMoveList[i + 1], tweenStepDuration, TSF));
            }
            nextTween.ContinueWith(GenerateTweenStep(tweenMoveList[tweenMoveList.Count - 1], tweenMoveList[0], tweenStepDuration, TSF));
        }
        else
        {
            nextTween = v2t = gameObject.Tween(tweenKeyName, tweenStartPos, tweenMoveList[0], tweenStepDuration, TSF, this.TweenMoveTo);
            for (int i = 0; i < tweenMoveList.Count - 1; i++)
            {
                nextTween = nextTween.ContinueWith(GenerateTweenStep(tweenMoveList[i], tweenMoveList[i + 1], tweenStepDuration, TSF));
            }
            nextTween.ContinueWith(GenerateTweenStep(tweenMoveList[tweenMoveList.Count - 1], tweenStartPos, tweenStepDuration, TSF));
        }
    }

    Tween<Vector2> GenerateTweenStep(Vector2 startPos, Vector2 endPos, float duration, System.Func<float, float> TweenScaleFunc)
    {
        //Debug.Log(startPos + " " + endPos);
        return new Vector2Tween().Setup(startPos, endPos, duration, TweenScaleFunc, this.TweenMoveTo);
    }

    IEnumerator RepeatTween()
    {
        TweenMovement();
        yield return new WaitForSeconds(tweenStepDuration * (randomTweenPoints ? tweenMoveList.Count : (tweenMoveList.Count + 1)));
        StartCoroutine(RepeatTween());
    }

    System.Func<float, float> GetTSF(TSF_Type My_TSF)
    {
        switch (My_TSF)
        {
            case (TSF_Type.Linear):
                return TweenScaleFunctions.Linear;
            case (TSF_Type.SineEaseIn):
                return TweenScaleFunctions.SineEaseIn;
            case (TSF_Type.SineEaseOut):
                return TweenScaleFunctions.SineEaseOut;
            case (TSF_Type.SineEaseInOut):
                return TweenScaleFunctions.SineEaseInOut;
            case (TSF_Type.CubicEaseIn):
                return TweenScaleFunctions.CubicEaseIn;
            case (TSF_Type.CubicEaseOut):
                return TweenScaleFunctions.CubicEaseOut;
            case (TSF_Type.CubicEaseInOut):
                return TweenScaleFunctions.CubicEaseInOut;
            case (TSF_Type.QuadraticEaseIn):
                return TweenScaleFunctions.QuadraticEaseIn;
            case (TSF_Type.QuadraticEaseOut):
                return TweenScaleFunctions.QuadraticEaseOut;
            case (TSF_Type.QuadraticEaseInOut):
                return TweenScaleFunctions.QuadraticEaseInOut;
            case (TSF_Type.QuinticEaseIn):
                return TweenScaleFunctions.QuinticEaseIn;
            case (TSF_Type.QuinticEaseOut):
                return TweenScaleFunctions.QuinticEaseOut;
            case (TSF_Type.QuinticEaseInOut):
                return TweenScaleFunctions.QuinticEaseInOut;
            default:
                Debug.Log("Unknown TSF Value provided: " + My_TSF + " in Object: " + gameObject.name + ", defaulting to 'Linear' TSF!");
                return TweenScaleFunctions.Linear;
        }
    }

    private void OnDestroy()
    {
        TweenFactory.RemoveTweenKey(tweenKeyName, TweenStopBehavior.DoNotModify);
    }

}

// List extention stuff that can be called from anywhere
#region List Extentions
/// <summary>
/// I need this to be able to randomize the order of spawnpoints for spawning
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Shuffle a list of items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        // TODO test to see if removing this random instance is needed for list randomization
        //Random.
        //Random rnd = new Random();
        for (var i = 0; i < list.Count; i++)
            list.Swap(i, Random.Range(i, list.Count));
    }

    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }
}
#endregion