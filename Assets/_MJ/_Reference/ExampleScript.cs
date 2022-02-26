using UnityEngine;

[ExecuteInEditMode] //SendMessageでエラーが出ないように
public class ExampleScript : MonoBehaviour
{

    //表示したログ、SerializeFieldを付ける事でInspectorに表示されるように
    [SerializeField]
    private string _log = "";

    /// <summary>
    /// _logを"ぷらいべーと！"に変更してConsoleに表示
    /// </summary>
    private void PrivateMethod()
    {
        _log = "ぷらいべーと！";
        Debug.Log(_log);
    }

    /// <summary>
    /// _logを"ぷぅあぶりっく！"に変更してConsoleに表示
    /// </summary>
    public void PublicMethod()
    {
        _log = "ぷぅあぶりっく！";
        Debug.Log(_log);
    }



    DataCounter DC;

    public void AnimationTest()
    {
        DC = GameObject.Find("Server").GetComponent<DataCounter>();

        DC.EdTest();

    }


    //void Update()
    //{
    //    Debug.Log("a");
    //}

}