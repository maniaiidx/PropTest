using UnityEngine;
using UnityEngine.Playables;

public class PlayableDirectorCallbackExample : MonoBehaviour
{
    public PlayableDirector director;

    void OnEnable()
    {
        director.stopped += OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        Debug.Log("aaaaaa");
        if (director == aDirector)
            Debug.Log("PlayableDirector named " + aDirector.name + " is now stopped.");
    }

    void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
    }
}