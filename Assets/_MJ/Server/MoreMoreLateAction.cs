using System;//FuncやActionを使うために必要
using System.Collections.Generic; //Listに必要
using UnityEngine;


public class MoreMoreLateAction : MonoBehaviour
{
    public bool isLateAction = false;
    public List<Action> lateActionList = new List<Action>();
    public void LateAction(Action action)
    {
        //ListにAddしたのを実行して消す。 （以前はただ実行するだけだったが、同時に2つ以上来たら駄目なのでList化）
        lateActionList.Add(action);
        isLateAction = true;
    }

    void LateUpdate()//フレーム最後に処理。アニメ処理後にすべき処理用・モーフ連動操作用に(アニメーション中マスクかけてもなぜかロックされてる)
    {
        #region LateUpdateでActionするシステム用
        if (isLateAction)
        {
            //ListにAddしたのを実行して消す。 （以前はただ実行するだけだったが、同時に2つ以上来たら駄目なのでList化）
            for (int i = 0; i < lateActionList.Count; i++)
            {
                lateActionList[i]();
                lateActionList.Remove(lateActionList[i]);
            }
            //Debug.Log(lateActionList.Count);

            if (lateActionList.Count == 0)
            { isLateAction = false; }

        }
        #endregion
    }
}
