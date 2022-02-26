//using追加も自由です（もし追加したら、後で教えてほしいかも）
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//クラスは↓の通りにしてください。
public partial class DataCounter
{
    public GameObject
        //Assetbundleテスト用
        Gobj_skirt,
        //着替えメニューカテゴリオンオフ用のCheckmark場所
        GObj_menu_HeadToggle_y,
        GObj_menu_ClothToggle_y,
        GObj_menu_ShoesToggle_y,
        GObj_menu_ALLToggle_y,
        //着替えメニューの各カテゴリ場所
        GObj_menu_HeadCat_y,
        GObj_menu_ClothCat_y,
        GObj_menu_ShoesCat_y,
        GObj_menu_ALLCat_y,
        //着替えメニューの各服のCheckmark場所
        GObj_menu_Head_Normal_y,
        GObj_menu_Head_A_y,
        GObj_menu_Head_B_y,
        GObj_menu_Cloth_School_y,
        GObj_menu_Cloth_Tanktop_y,
        GObj_menu_Cloth_A_y,
        GObj_menu_Shoes_Socks_y,
        GObj_menu_Shoes_Barefoot_y,
        GObj_menu_Shoes_A_y,
        GObj_menu_ALL_None_y,
        GObj_menu_ALL_Bikini_y;

    public GameObject
        Gobj_ydloadMenu;

    const string
        //カテゴリメニュー表示用のサムネ名
        catHead = "Toggle_Head_y",
        catCloth = "Toggle_Cloth_y",
        catShoes = "Toggle_Shoes_y",
        catALL = "Toggle_ALL_y",
        //カテゴリ先の着替え用メニューのサムネ名
        cfShoes_Socks = "Toggle_Socks_y",
        cfShoes_Barefoot = "Toggle_Barefoot_y",
        cfCloth_School = "Toggle_School_y",
        cfCloth_Tanktop = "Toggle_Tanktop_y",
        cfAll_None = "Toggle_None_y",
        cfAll_Bikini = "Toggle_Bikini_y",
        ToggleCM = "ToggleCheckmark";

    //■メニュー起動時に行われるメソッド
    public void MenuStart_ydload()
    {
        //旧→新変換
        OldconversionNew();
        #region ■Y■ここで初期設定など
        //Yキー押した初回だけ動く処理
        //初回検索_各カテゴリの一覧
        InitialFindMenuToggle();
        //クリア_各カテゴリの一覧全て
        ClothMenuALLClear();
        //オンになっている服の赤枠を表示
        ClothMenuSetRedframe();
        #endregion
    }

    //■ここからコルーチンのメソッドを追加していきます
    
    //旧→新変換（メニュー開始時）
    void OldconversionNew()
    {
        //既存命令全部書き換えることにしたので一旦コメントオフ MJ0711

        ////旧素足オン→新足（1）
        //if (DB.isUserClothsBarefoot) 
        //{
        //    DB.intCurrentShoes = 1;
        //}
        ////旧タンクトップオン→新服（1）
        //if (DB.isUserClothsTankTop)
        //{
        //    DB.intCurrentCloth = 1;
        //}
        ////旧ビキニオン→新ALL（1）
        //if (DB.isUserClothsBikini)
        //{
        //    DB.intCurrentALL = 1;
        //}
    }
    //新→旧変換（新着替えメニュークリック時）
    void NewconversionOld()
    {
        //旧状態全部書き換えることにしたので一旦コメントアウト ■MJ0711

        ////服装の新旧変換
        //switch (DB.intCurrentCloth)
        //{
        //    //制服が選択されている
        //    case 0:
        //        DB.isUserClothsTankTop = false;
        //        break;
        //    //タンクトップが選択されている
        //    case 1:
        //        DB.isUserClothsTankTop = true;
        //        break;
        //}
        ////靴の新旧変換
        //switch (DB.intCurrentShoes)
        //{
        //    //靴下が選択されている
        //    case 0:
        //        DB.isUserClothsBarefoot = false;
        //        break;
        //    //素足が選択されている
        //    case 1:
        //        DB.isUserClothsBarefoot = true;
        //        break;
        //}
        ////ALの新旧変換
        //switch (DB.intCurrentALL)
        //{
        //    //なしが選択されている
        //    case 0:
        //        DB.isUserClothsBikini = false;
        //        break;
        //    //ビキニが選択されている
        //    case 1:
        //        DB.isUserClothsBikini = true;
        //        break;
        //}

    }
    //赤枠オンオフ
    void TogglleChange_ydload(GameObject ToggleObj, GameObject Checkmark)
    {
        if (Checkmark.activeSelf)
        {
            Checkmark.SetActive(false);
        }
        else
        {
            Checkmark.SetActive(true);
        }
    }
    //初回検索_各カテゴリの一覧
    public void InitialFindMenuToggle()
    {
        // StreamingAssetsからAssetBundleをロードする
        //var assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/testbundle");
        //if (assetBundle != null)
        //{
            // AssetBundle内のアセットにはビルド時のアセットのパス、またはファイル名、ファイル名＋拡張子でアクセスできる
        //    var m_MainTexture = assetBundle.LoadAsset<Texture2D>("Assets/_ResizeMeEventTeam/yd_load/test/skirt2.png");
        //    Gobj_skirt = GirlMeshTrs.Find("Skirt").gameObject;

            //マテリアル
        //    Gobj_skirt.GetComponent<Renderer>().material.SetTexture("_MainTex", m_MainTexture);

            // 不要になったAssetBundleのメタ情報をアンロードする
        //    assetBundle.Unload(false);

        //}


        GObj_menu_HeadToggle_y = Gobj_ydloadMenu.transform.Find(catHead).Find(ToggleCM).gameObject;
        GObj_menu_ClothToggle_y = Gobj_ydloadMenu.transform.Find(catCloth).Find(ToggleCM).gameObject;
        GObj_menu_ShoesToggle_y = Gobj_ydloadMenu.transform.Find(catShoes).Find(ToggleCM).gameObject;
        GObj_menu_ALLToggle_y = Gobj_ydloadMenu.transform.Find(catALL).Find(ToggleCM).gameObject;

        GObj_menu_HeadCat_y = GObj_menu_HeadToggle_y.transform.Find("HeadCatMenu").gameObject;
        GObj_menu_ClothCat_y = GObj_menu_ClothToggle_y.transform.Find("ClothCatMenu").gameObject;
        GObj_menu_ShoesCat_y = GObj_menu_ShoesToggle_y.transform.Find("ShoesCatMenu").gameObject;
        GObj_menu_ALLCat_y = GObj_menu_ALLToggle_y.transform.Find("ALLCatMenu").gameObject;

        GObj_menu_Head_Normal_y = GObj_menu_HeadCat_y.transform.Find("Toggle_Normal_y").Find(ToggleCM).gameObject;
        GObj_menu_Cloth_School_y = GObj_menu_ClothCat_y.transform.Find(cfCloth_School).Find(ToggleCM).gameObject;
        GObj_menu_Cloth_Tanktop_y = GObj_menu_ClothCat_y.transform.Find(cfCloth_Tanktop).Find(ToggleCM).gameObject;
        GObj_menu_Shoes_Socks_y = GObj_menu_ShoesCat_y.transform.Find(cfShoes_Socks).Find(ToggleCM).gameObject;
        GObj_menu_Shoes_Barefoot_y = GObj_menu_ShoesCat_y.transform.Find(cfShoes_Barefoot).Find(ToggleCM).gameObject;
        GObj_menu_ALL_None_y = GObj_menu_ALLCat_y.transform.Find(cfAll_None).Find(ToggleCM).gameObject;
        GObj_menu_ALL_Bikini_y = GObj_menu_ALLCat_y.transform.Find(cfAll_Bikini).Find(ToggleCM).gameObject;

    }
    //クリア_各カテゴリの一覧全て
    public void ClothMenuALLClear()
    {
        GObj_menu_Head_Normal_y.SetActive(false);
        GObj_menu_Cloth_School_y.SetActive(false);
        GObj_menu_Cloth_Tanktop_y.SetActive(false);
        GObj_menu_Shoes_Socks_y.SetActive(false);
        GObj_menu_Shoes_Barefoot_y.SetActive(false);
        GObj_menu_ALL_None_y.SetActive(false);
        GObj_menu_ALL_Bikini_y.SetActive(false);
    }
    //オンになっている服の赤枠を表示
    public void ClothMenuSetRedframe()
    {
        //設定維持のチェックボックス表示
        TogglleChange(Gobj_ydloadMenu.transform.Find("Toggle_FixityOutfit_y").gameObject, DB.isUserFixityOutfit);
        //服装の赤枠表示
        switch (DB.intCurrentCloth)
        {
            //制服が選択されている
            case 0:
                GObj_menu_Cloth_School_y.SetActive(true);
                break;
            //タンクトップが選択されている
            case 1:
                GObj_menu_Cloth_Tanktop_y.SetActive(true);
                break;
        }
        //靴の赤枠表示
        switch (DB.intCurrentShoes)
        {
            //靴下が選択されている
            case 0:
                GObj_menu_Shoes_Socks_y.SetActive(true);
                break;
            //素足が選択されている
            case 1:
                GObj_menu_Shoes_Barefoot_y.SetActive(true);
                break;
        }
        //ALLの赤枠表示
        switch (DB.intCurrentALL)
        {
            //なしが選択されている
            case 0:
                GObj_menu_ALL_None_y.SetActive(true);
                break;
            //ビキニが選択されている
            case 1:
                GObj_menu_ALL_Bikini_y.SetActive(true);
                break;
        }
    }

    //Rayの当たり判定処理
    public void RayhitCheck()
    {
        //RayがあたってるObjを変数化
        GameObject rayHitObj = mouseOnMenuRayHit.collider.gameObject;

        //カテゴリオンオフ
        ClothMenuCatChange(rayHitObj);
        //着替え設定フラグ変更
        ClothFlagChange(rayHitObj);
        //新→旧変換
        NewconversionOld();

    }

    public void ClothMenuCatClear()
    {
        //カテゴリすべてオフ
        GObj_menu_HeadToggle_y.SetActive(false);
        GObj_menu_ClothToggle_y.SetActive(false);
        GObj_menu_ShoesToggle_y.SetActive(false);
        GObj_menu_ALLToggle_y.SetActive(false);
    }

    public void ClothMenuCatChange(GameObject rayHitObj)
    {
        //カテゴリオンオフ
        switch (rayHitObj.name)
        {
            case catHead:
                //カテゴリすべてオフ
                ClothMenuCatClear();
                //boolとトグル表示切替
                TogglleChange_ydload(rayHitObj, GObj_menu_HeadToggle_y);
                break;
            case catCloth:
                //カテゴリすべてオフ
                ClothMenuCatClear();
                //boolとトグル表示切替
                TogglleChange_ydload(rayHitObj, GObj_menu_ClothToggle_y);
                break;
            case catShoes:
                //カテゴリすべてオフ
                ClothMenuCatClear();
                //boolとトグル表示切替
                TogglleChange_ydload(rayHitObj, GObj_menu_ShoesToggle_y);
                break;
            case catALL:
                //カテゴリすべてオフ
                ClothMenuCatClear();
                //boolとトグル表示切替
                TogglleChange_ydload(rayHitObj, GObj_menu_ALLToggle_y);
                break;
        }
    }
    public void ClothFlagChange(GameObject rayHitObj)
    {
        switch (rayHitObj.name)
        {
            case "Toggle_FixityOutfit_y":
                //着替え設定維持bool切り替え
                if (DB.isUserFixityOutfit)
                {
                    DB.isUserFixityOutfit = false;
                    TogglleChange(rayHitObj, false);
                }
                else
                {
                    DB.isUserFixityOutfit = true;
                    TogglleChange(rayHitObj, true);
                }
                break;
            #region 服装の切り替え処理
            case cfCloth_School:
                //制服に変更
                DB.intCurrentCloth = 0;
                //クリア_各カテゴリの一覧全て
                ClothMenuALLClear();
                //オンになっている服の赤枠を表示
                ClothMenuSetRedframe();
                //最後にこのメソッドでbool内容を読み取って着替え処理される
                ClothsApply_ydload();
                break;
            case cfCloth_Tanktop:
                //タンクトップに変更
                DB.intCurrentCloth = 1;
                //クリア_各カテゴリの一覧全て
                ClothMenuALLClear();
                //オンになっている服の赤枠を表示
                ClothMenuSetRedframe();
                //最後にこのメソッドでbool内容を読み取って着替え処理される
                ClothsApply_ydload();
                break;
            #endregion
            #region 靴の切り替え処理
            case cfShoes_Socks:
                //靴下に変更
                DB.intCurrentShoes = 0;
                //クリア_各カテゴリの一覧全て
                ClothMenuALLClear();
                //オンになっている服の赤枠を表示
                ClothMenuSetRedframe();
                //最後にこのメソッドでbool内容を読み取って着替え処理される
                ClothsApply_ydload();
                break;
            case cfShoes_Barefoot:
                //素足に変更
                DB.intCurrentShoes = 1;
                //クリア_各カテゴリの一覧全て
                ClothMenuALLClear();
                //オンになっている服の赤枠を表示
                ClothMenuSetRedframe();
                //最後にこのメソッドでbool内容を読み取って着替え処理される
                ClothsApply_ydload();
                break;
            #endregion
            #region 全身の切り替え処理
            case cfAll_None:
                //なしに変更
                DB.intCurrentALL = 0;
                //クリア_各カテゴリの一覧全て
                ClothMenuALLClear();
                //オンになっている服の赤枠を表示
                ClothMenuSetRedframe();
                //最後にこのメソッドでbool内容を読み取って着替え処理される
                ClothsApply_ydload();
                break;
            case cfAll_Bikini:
                //ビキニに変更
                DB.intCurrentALL = 1;
                //クリア_各カテゴリの一覧全て
                ClothMenuALLClear();
                //オンになっている服の赤枠を表示
                ClothMenuSetRedframe();
                //最後にこのメソッドでbool内容を読み取って着替え処理される
                ClothsApply_ydload();
                break;
                #endregion
        }
    }
    //着替え処理
    public void ClothsApply_ydload()
    {
        //初期化
        //制服OFF
        GirlMeshTrs.Find("NeckTai").gameObject.SetActive(false);
        GirlMeshTrs.Find("Pants").gameObject.SetActive(false);
        GirlMeshTrs.Find("Sailor").gameObject.SetActive(false);
        GirlMeshTrs.Find("Skirt").gameObject.SetActive(false);
        //タンクトップOFF
        GirlMeshTrs.Find("Spats").gameObject.SetActive(false);
        GirlMeshTrs.Find("TankTop").gameObject.SetActive(false);
        //ビキニOFF
        GirlMeshTrs.Find("Bikini").gameObject.SetActive(false);
        //靴下OFF
        SocksObj.SetActive(false);

        //服装の変更
        //【服装】0:制服,1:タンクトップ
        switch (DB.intCurrentCloth)
        {
            case 0:
                //制服ON
                GirlMeshTrs.Find("NeckTai").gameObject.SetActive(true);
                GirlMeshTrs.Find("Pants").gameObject.SetActive(true);
                GirlMeshTrs.Find("Sailor").gameObject.SetActive(true);
                GirlMeshTrs.Find("Skirt").gameObject.SetActive(true);
                break;
            case 1:
                //タンクトップON
                GirlMeshTrs.Find("Spats").gameObject.SetActive(true);
                GirlMeshTrs.Find("TankTop").gameObject.SetActive(true);
                break;
        }
        //靴の変更
        //【靴】0:靴下,1:素足
        switch (DB.intCurrentShoes)
        {
            case 0:
                #region 靴下に切り替え処理
                //制服の靴下
                if (DB.intCurrentCloth == 0)
                {
                    BodyObj.GetComponent<Renderer>().material = mat_body;
                }
                //タンクトップの靴下
                else if (DB.intCurrentCloth == 1)
                {
                    BodyObj.GetComponent<Renderer>().material = mat_body_TankTop;
                }
                SocksObj.SetActive(true);
                #endregion
                break;
            case 1:
                #region 素足に切り替え処理
                //制服の素足
                if (DB.intCurrentCloth == 0)
                {
                    BodyObj.GetComponent<Renderer>().material = mat_body_Barefoot;
                }
                //タンクトップの素足
                else if (DB.intCurrentCloth == 1)
                {
                    BodyObj.GetComponent<Renderer>().material = mat_body_TankTop_Barefoot;
                }
                SocksObj.SetActive(false);
                #endregion
                break;
        }
        //全身の変更
        //【全身】0:なし,1:ビキニ
        switch (DB.intCurrentALL)
        {
            case 0:
                //なし
                break;
            case 1:
                #region ビキニに切り替え処理
                //制服OFF
                GirlMeshTrs.Find("NeckTai").gameObject.SetActive(false);
                GirlMeshTrs.Find("Pants").gameObject.SetActive(false);
                GirlMeshTrs.Find("Sailor").gameObject.SetActive(false);
                GirlMeshTrs.Find("Skirt").gameObject.SetActive(false);

                //タンクトップOFF
                GirlMeshTrs.Find("Spats").gameObject.SetActive(false);
                GirlMeshTrs.Find("TankTop").gameObject.SetActive(false);

                //靴下OFF
                SocksObj.SetActive(false);


                //ビキニON
                GirlMeshTrs.Find("Bikini").gameObject.SetActive(true);

                //マテリアル
                BodyObj.GetComponent<Renderer>().material = mat_body_Bikini;
                #endregion
                break;
        }
    }
}