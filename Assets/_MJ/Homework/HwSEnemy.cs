using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HwSEnemy : MonoBehaviour
{

    public DataCounter DC;
    public Renderer thisRenderer;
    public Color thisMatDefColor;//白フラッシュしたあと元の色に戻すため
    public Image thisImage;
    public Color thisImageDefColor;
    public float HP;
    public bool HPSetFlag;//HwHit側で使用するもの。HPがすでにセットされているかのフラグ

    public GameObject HwCanvas, SliderEnmSet, HPSlider;
    public Slider SliderHP;
    public Text SliderHPText;


    IEnumerator Start()
    {
        enabled = false;//コンポーネントOFFにすることでStart終わるまでUpdate開始しない（最後でONにしてる）

        DC = GameObject.Find("Server").GetComponent<DataCounter>();

        #region メッシュとImageあれば取得
        if (this.gameObject.GetComponent<Renderer>() != null)
        {
            thisRenderer = this.gameObject.GetComponent<Renderer>();//ダメージフラッシュさせるために取得しておく
            thisMatDefColor = thisRenderer.material.color;//元の色保持
        }
        if (this.gameObject.GetComponent<Image>() != null)
        {
            thisImage = this.gameObject.GetComponent<Image>();//Imageでも同じく
            thisImageDefColor = thisImage.color;
        }
        #endregion

        #region //ゲージ表示関係
        ////位置設定したあと本体を敵にペアレントして切り離すことでゲージの場所と大きさ統一をする
        //SliderEnmSet = Instantiate(Resources.Load("EventSystem/Homework/SliderEnmSet")) as GameObject;
        //SliderEnmSet.transform.SetParent(transform, false);
        //SliderEnmSet.transform.position = this.transform.position + new Vector3(0, 0, -200);

        ////Rayを使ってオブジェクトの下端を検出しそこへ移動
        //Ray ray = new Ray(SliderEnmSet.transform.position, SliderEnmSet.transform.up);
        //RaycastHit hit;

        //if (gameObject.GetComponent<Collider>().Raycast(ray, out hit, 200f))//このオブジェクトにだけ当たる
        //{
        //    SliderEnmSet.transform.position = hit.point;//Rayの衝突地点に、オブジェクトを移動させる
        //}

        //HPSlider = SliderEnmSet.transform.Find("SliderEnm").gameObject;//子オブジェクトになっているスライダー本体を取得
        //HPSlider.transform.SetParent(this.transform);//ゲージ本体を子に設定(parent ＝ だと黄色エラーが出るので最新のSetParent)
        //Destroy(SliderEnmSet); //SliderEnmSet削除

        ////ゲージに値を渡す
        //SliderHP = HPSlider.GetComponent<Slider>();//まずSlider取得
        //SliderHPText = SliderHP.transform.Find("Text").GetComponent<Text>();//Text取得

        //yield return null;

        //SliderHP.maxValue = SliderHP.value = HP;
        //SliderHP.value = HP;
        //SliderHPText.text = SliderHP.value.ToString("f0");//HP数表示
        #endregion

        enabled = true;//コンポーネントONにすることでUpdate開始（先頭でOFFにしてる）
        yield break;
    }


    void Update()
    {

        //当たっているフラグによる一定リズム構文が働いていて、Listにこのオブジェクトが入っているならHPがパワー分減り白色にして0.04秒後色戻すメソッド実行
        if (DC.HwColDmgList.Contains(this.gameObject))
        {
            HP -= DC.HwPowFloat;
            //SliderHP.value = HP;
            //SliderHPText.text = SliderHP.value.ToString("f0");//HP数表示
            DC.HwColDmgList.Remove(this.gameObject);//ダメージリストから外し
            DC.HwMoneyFloat += DC.HwPowFloat;//ダメージ分お金入手
            StartCoroutine(DamageColorFlash(0.04f));//コルーチンでフラッシュ実行
            DC.SEPlay(DC.UISEObj, "UI_cha");
        }
        //HPが1よりも低く、衝突判定があり、colObjsに入っていたらオブジェクト消滅メソッド実行
        if (HP < 1 && DC.HwColBool == true && DC.HwColObjsList.Contains(this.gameObject))
        {
            #region パワーアップアイテムだった場合
            if (tag == "HwLvUPItem")
            {
                DC.HwMoneyFloat += SliderHP.maxValue;//敵最大体力分お金入手
                DC.HwColObjsList.Remove(this.gameObject);//colObjsリストから外し
                DC.HwColDmgList.Remove(this.gameObject);//リストから外し
                this.gameObject.SetActive(false);
                DC.SEPlay(DC.UISEObj, "ui_magical_open");
                #region リズムアップの場合
                if (name == "HwEnmRhythmUp_1")
                {
                    Instantiate(Resources.Load("EventSystem/Homework/EnemyGimic/RhythmUpText") as GameObject
                        , transform.parent
                        , false);
                    DC.HwAttackRhythmMaxFloat /= 2;
                }
                #endregion
                #region パワーアップの場合
                else if (name == "HwEnmPowerUp_1")
                {
                    Instantiate(Resources.Load("EventSystem/Homework/EnemyGimic/PowerUpText") as GameObject
                        , transform.parent
                        , false);
                    DC.HwPowFloat++;
                }
                #endregion
                #region サイズアップの場合
                else if (name == "HwEnmSizeUp_1")
                {
                    Instantiate(Resources.Load("EventSystem/Homework/EnemyGimic/SizeUpText") as GameObject
                        , transform.parent
                        , false);
                    Vector3 HwPointScl = DC.HwPointTrs.localScale;
                    DC.HwPointTrs.localScale = new Vector3(HwPointScl.x * 1.3f, HwPointScl.y, HwPointScl.z * 1.3f);
                }
                #endregion
            }
            #endregion
            else
            {
                //DC.HwMoneyFloat += SliderHP.maxValue;//敵最大体力分お金入手
                StartCoroutine(DamageColorFlash(0.04f));//コルーチンでフラッシュ実行
                DC.HwColObjsList.Remove(this.gameObject);//colObjsリストから外し
                DC.HwColDmgList.Remove(this.gameObject);//リストから外し
                StartCoroutine(DeActive(0.04f));//コルーチンでウェイトしてメソッド実行
                DC.SEPlay(DC.UISEObj, "UI_cho");
            }

        }


    }

    IEnumerator DeActive(float delay)
    {
        yield return new WaitForSeconds(delay);//引数分待ってから
        //DC.StageClearCheck();//最後の敵だったかチェックして
        this.gameObject.SetActive(false);//削除ではなくオフ（復活させるために）（HwSではいらない？）

        //メソッドにObj送信
        DC.TansakuEnter(transform.parent.gameObject, DataCounter.RayOrColl.HwS);
    }

    IEnumerator DamageColorFlash(float delay)
    {
        //imageとメッシュどちらもあれば白フラッシュ
        if (thisRenderer != null) { thisRenderer.material.color = Color.white; }
        if (thisImage != null) { thisImage.color = Color.white; }

        yield return new WaitForSeconds(delay);//引数分待ってから

        if (thisRenderer != null) { thisRenderer.material.color = thisMatDefColor; }
        if (thisImage != null) { thisImage.color = thisImageDefColor; }
    }


}

