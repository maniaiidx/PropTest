using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Listに必要
using UnityEditor;//AssetDatabaseに必要
using System.IO;//Directoryに必要
using RootMotion.FinalIK;//FinalIK

//AssetDatabaseはUnityEditorを使うので、コンパイルできない。
//ので、自動読み込みメソッドを別スクリプト（このスクリプト）にしてEditorに置いて使う（Editorフォルダはコンパイル時無視される）

//このスクリプトでResourceFilesに取得したファイルを、他のスクリプトがResourcesの気分でコピーして使う。
//特にPrefabはコピー（Instantiate）しないと、変更を防ぐために操作ほとんどできない（ペアレントとか）

/*NonResourcesというクラスを使ってロードしている。
http://kan-kikuchi.hatenablog.com/entry/NonResources
//単体ロード(Assets直下にあるimgという画像ファイルをロード)
Object obj = NonResources.Load("Assets/img.png");//Objectで取得
Texture texture = NonResources.Load<Texture>("Assets/img.png");//Textureで取得

//複数ロード(Assets直下にあるImageというディレクトリ内にある全ファイルをロード)
List<Object> objList = NonResources.LoadAll("Assets/Image");//全Objectを取得
List<Texture> textureList = NonResources.LoadAll<Texture>("Assets/Image");//全Textureを取得
*/
public static class NonResources
{
    /// Resourcesディレクトリ以外のオブジェクトにアクセスすることができる。実はResourcesディレクトリのオブジェクトにもアクセスできる

    //=================================================================================
    //単体ロード
    //=================================================================================

    /// ファイルのパス(Assetsから、拡張子も含める)と型を設定し、Objectを読み込む。存在しない場合はNullを返す

    public static T Load<T>(string path) where T : Object
    {
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }

    /// <summary>
    /// ファイルのパス(Assetsから、拡張子も含める)を設定し、Objectを読み込む。存在しない場合はNullを返す
    /// </summary>
    public static Object Load(string path)
    {
        return Load<Object>(path);
    }

    //=================================================================================
    //複数ロード
    //=================================================================================

    /// ディレクトリのパス(Assetsから)と型を設定し、Objectを読み込む。存在しない場合は空のListを返す

    public static List<T> LoadAll<T>(string directoryPath) where T : Object
    {
        List<T> assetList = new List<T>();

        //指定したディレクトリに入っている全ファイルを取得(子ディレクトリも含む)
        //string[] filePathArray = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        //指定したディレクトリに入っている全ファイルを取得(子ディレクトリ含まない)
        string[] filePathArray = Directory.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly);

        //取得したファイルの中からアセットだけリストに追加する
        foreach (string filePath in filePathArray)
        {
            T asset = Load<T>(filePath);
            if (asset != null)
            {
                assetList.Add(asset);
            }
        }

        return assetList;
    }

    /// ディレクトリのパス(Assetsから)を設定し、Objectを読み込む。存在しない場合は空のListを返す

    public static List<Object> LoadAll(string directoryPath)
    {
        return LoadAll<Object>(directoryPath);
    }

}
/*エディタ拡張を使わないと以下のような指定方法（これを簡略化している）
SliderEnmSet = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/_MJ/ResourceFiles/homework/SliderEnmSet.prefab", typeof(GameObject)) as GameObject; 

//指定したディレクトリに入っている全ファイルを取得(子ディレクトリも含まない)
string[] filePathArray = System.IO.Directory.GetFiles("Assets/_MJ/ResourceFiles/homework/drillTextures/", "*", System.IO.SearchOption.TopDirectoryOnly);

//取得したファイルの中からアセットだけリストに追加する
foreach (string filePath in filePathArray)
{
    Texture asset = UnityEditor.AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture)) as Texture;
    if (asset != null)
    {
        drillTextures.Add(asset);
    }
}
*/



public class ResourceFilesEditor : EditorWindow
{
    //    [ContextMenu("AssetReload")] //Inspectorの歯車アイコンにAssetReloadという項目を追加、押すと下のメソッドが実行される
    [MenuItem("_MJ/リソースファイル自動ロード")]//Unityのタブに追加できる
    public static void AssetReload()
    {
        DataCounter DC = GameObject.Find("Server").GetComponent<DataCounter>();
        ResourceFiles ResourceFiles = GameObject.Find("ResourceFiles").GetComponent<ResourceFiles>();

        #region SE_BGM
        #region//一生懸命Dictionaryで読み込んだが、インスペクタに表示されない=事前にデータを持っておけないっぽい（シリアライズ？できない）ので一旦ListにしておいてからAwakeでDictionaryに入れる。
        /*
                #region//SE_BGM(ファイル名をキー名に指定して取得するのでNonResources使わず手動で)
        SE.Clear();
        BGM.Clear();
        SEView.Clear();
        BGMView.Clear();
                #region//SE
                //指定したディレクトリに入っている全ファイルを取得(子ディレクトリ含む)
                string[] filePathArraySE = Directory.GetFiles("Assets/_MJ/ResourceFiles/SE/", "*", SearchOption.AllDirectories);

                //取得したファイルの中からアセットだけ追加する
                foreach (string filePath in filePathArraySE)
                {
                    //キー名用にファイル名だけ取得（拡張子も抜く）
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    //実データ
                    AudioClip asset = AssetDatabase.LoadAssetAtPath(filePath, typeof(AudioClip)) as AudioClip;

                    if (asset != null)//AudioClipとして読み込めたらDictionaryに追加
                    {
                        SE.Add(fileName, asset);
                    }
                }
                //インスペクタで見えるようにString化してList化
                foreach (KeyValuePair<string, AudioClip> pair in SE)
                {
                    SEView.Add(pair.Key + ":" + pair.Value);
                }
                #endregion
                #region//BGM
                string[] filePathArrayBGM = Directory.GetFiles("Assets/_MJ/ResourceFiles/BGM/", "*", SearchOption.AllDirectories);
                foreach (string filePath in filePathArrayBGM)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    AudioClip asset = AssetDatabase.LoadAssetAtPath(filePath, typeof(AudioClip)) as AudioClip;
                    if (asset != null)
                    {
                        BGM.Add(fileName,asset);
                    }
                }
                foreach (KeyValuePair<string, AudioClip> pair in BGM)
                {
                    BGMView.Add(pair.Key + ":" + pair.Value);
                }
                #endregion

        #endregion
                    */


        #endregion

        ResourceFiles.SEAudioClipList.Clear();
        ResourceFiles.BGMAudioClipList.Clear();
        ResourceFiles.SEKeyNameList.Clear();
        ResourceFiles.BGMKeyNameList.Clear();

        //SE
        //キー名用にファイル名取得するため、NonResources使わずに手動。
        //まずファイルパス読み込み
        string[] filePathArraySE = Directory.GetFiles("Assets/_MJ/ResourceFiles/SE/", "*", SearchOption.AllDirectories);
        //取得したファイルの中からアセットにだけ追加する
        foreach (string filePath in filePathArraySE)
        {
            //キー名用にファイル名だけ取得（拡張子も抜く）
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            //実データ
            AudioClip asset = AssetDatabase.LoadAssetAtPath(filePath, typeof(AudioClip)) as AudioClip;

            if (asset != null)//AudioClipとして読み込めたらStringを追加
            {
                ResourceFiles.SEAudioClipList.Add(asset);//実ファイル
                ResourceFiles.SEKeyNameList.Add(fileName);//ファイル名
            }
        }

        //BGM（SEと同じやり方）
        string[] filePathArrayBGM = Directory.GetFiles("Assets/_MJ/ResourceFiles/BGM/", "*", SearchOption.AllDirectories);
        foreach (string filePath in filePathArrayBGM)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            AudioClip asset = AssetDatabase.LoadAssetAtPath(filePath, typeof(AudioClip)) as AudioClip;
            if (asset != null)
            {
                ResourceFiles.BGMAudioClipList.Add(asset);
                ResourceFiles.BGMKeyNameList.Add(fileName);
            }
        }

        #endregion

        #region UI
        ResourceFiles.TobecontinuedObj = NonResources.Load<GameObject>("Assets/_MJ/ResourceFiles/UI/Tobecontinued/Tobecontinued.FBX");
        #endregion

        #region ChieriKomono
        ResourceFiles.SkirtBoneQuad = NonResources.Load<GameObject>("Assets/_MJ/ResourceFiles/ChieriKomono/SkirtBoneQuad.prefab");
        ResourceFiles.SharpenObj = NonResources.Load<GameObject>("Assets/_MJ/ResourceFiles/ChieriKomono/Sharpen.prefab");
        ResourceFiles.KeshigomuObj = NonResources.Load<GameObject>("Assets/_MJ/ResourceFiles/ChieriKomono/Keshigomu.prefab");
        ResourceFiles.ChieriSumahoObj = NonResources.Load<GameObject>("Assets/_MJ/ResourceFiles/ChieriKomono/ChieriSumaho.prefab");

        ResourceFiles.keshigomuPMat = NonResources.Load<PhysicMaterial>("Assets/_MJ/ResourceFiles/ChieriKomono/Keshigomu_pMat.physicMaterial");

        #endregion

        #region Collider

        //事前クリア
        ResourceFiles.colliderObjGameobjectList.Clear();
        ResourceFiles.colliderObjKeyNameList.Clear();

        //キー名用にファイル名取得するため、NonResources使わずに手動。
        //まずファイルパス読み込み
        string[] filePathArrayColliderObj = Directory.GetFiles("Assets/_MJ/ResourceFiles/ColliderObj/", "*", SearchOption.AllDirectories);
        //取得したファイルの中からアセットにだけ追加する
        foreach (string filePath in filePathArrayColliderObj)
        {
            //キー名用にファイル名だけ取得（拡張子も抜く）
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            //実データ
            GameObject asset = AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject)) as GameObject;

            if (asset != null)//GameObjectとして読み込めたらStringを追加
            {
                ResourceFiles.colliderObjGameobjectList.Add(asset);//実ファイル
                ResourceFiles.colliderObjKeyNameList.Add(fileName);//ファイル名
            }
        }

        #endregion Collider

        //DC
        #region ChieriKomono_TransformParameters
        DC.Sharpen_table_Room_ParameterTrs = NonResources.Load<GameObject>("Assets/_MJ/ResourceFiles/ChieriKomono/TransformParameters/Sharpen_table_Room.prefab").transform;
        DC.Sharpen_syukudai_GirlRhitosashi02_ParameterTrs = NonResources.Load<GameObject>("Assets/_MJ/ResourceFiles/ChieriKomono/TransformParameters/Sharpen_syukudai_GirlRhitosashi02.prefab").transform;
        DC.Keshigomu_table_Room_ParameterTrs = NonResources.Load<GameObject>("Assets/_MJ/ResourceFiles/ChieriKomono/TransformParameters/Keshigomu_table_Room.prefab").transform;
        DC.Keshigomu_syukudai_GirlRhitosashi02_ParameterTrs = NonResources.Load<GameObject>("Assets/_MJ/ResourceFiles/ChieriKomono/TransformParameters/Keshigomu_syukudai_GirlRhitosashi02.prefab").transform;
        DC.Keshigomu_C_Modositehosii_table_Room_ParameterTrs = NonResources.Load<GameObject>("Assets/_MJ/ResourceFiles/ChieriKomono/TransformParameters/Keshigomu_C_Modositehosii_table_Room.prefab").transform;
        #endregion

        //■ヒエラルキーObjロードメソッド実行
        if (GameObject.Find("Server") == null) { Debug.Log("Serverオブジェクトが無いかオフ"); }
        else { GameObject.Find("Server").GetComponent<DataCounter>().HierarchyObjLoad(); }
    }

    //アセットバンドルビルド（http://kconcon3.hatenablog.com/entry/2018/10/11/233000）
    [MenuItem("_MJ/Build AssetBundleData")]
    public static void Build()
    {
        string assetBundleDirectory = "./AssetBundleData";      // 出力先ディレクトリ

        // 出力先ディレクトリが無かったら作る
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        // AssetBundleのビルド(ターゲット(プラットフォーム)毎に3つ目の引数が違うので注意)
        BuildPipeline.BuildAssetBundles(assetBundleDirectory
            , BuildAssetBundleOptions.None
            , BuildTarget.StandaloneWindows);

        // ビルド終了を表示
        EditorUtility.DisplayDialog("アセットバンドルビルド終了", "アセットバンドルビルドが終わりました", "OK");
    }


}