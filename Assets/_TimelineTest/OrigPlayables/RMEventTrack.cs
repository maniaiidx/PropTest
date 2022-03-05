using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;
using System.Collections.Generic;
using System.IO;//外部ファイル読み込みなど
using System.Text;//UTF-8エンコードに
using Candlelight;//プロパティいじったときのコールバック機能
using System;//array
using DG.Tweening;//DOTween
using System.Reflection;
using UnityEngine.Events;//メソッド指定用

[TrackColor(0.875f, 0.5944853f, 0.1737132f)]
[TrackClipType(typeof(RMEventClip))]

//トラック（これ）で、クリップの数や、ミキサーで扱う変数を設定している。
//コード生成などもここでしている。


public class RMEventTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject obj, int inputCount)
    {
        var mixer = ScriptPlayable<RMEventMixer>.Create(graph, inputCount);

        //ミキサー変数にDC割り当て
        mixer.GetBehaviour().DC = GameObject.Find("Server").GetComponent<DataCounter>();
        mixer.GetBehaviour().DB = GameObject.Find("DataBridging").GetComponent<DataBridging>();
        mixer.GetBehaviour().ResourceFiles = mixer.GetBehaviour().DC.transform.Find("ResourceFiles").GetComponent<ResourceFiles>();

        //ミキサーの変数にDirectorを割り当て（ポーズなどをさせるため）
        mixer.GetBehaviour().m_PlayableDirector = obj.GetComponent<PlayableDirector>();

        //ミキサーの変数にトラックを割り当て
        mixer.GetBehaviour().m_RMEventTrack = this;

        //ミキサーの変数にトラック（This）からクリップリスト入れる
        mixer.GetBehaviour().m_clipsList = GetClips().ToList();

        //ヒエラルキーのGameObjectを取得用（graphのGetResolverが型変換に必要のよう）(ExposedReference<GameObject> としないとヒエラルキーから取得できない)
        mixer.GetBehaviour().graph = graph;

        return mixer;
    }

    #region イベント製作チーム用
    [HeaderAttribute("■イベント製作チーム用")]
    public RuntimeAnimatorController useAnimator;
    public List<String> useIEnumList;

    #endregion

    [SerializeField, PropertyBackingField, Space(40)]//[SerializeField, PropertyBackingField( "spawnObj" )]とすれば　→　PropertyBackingField 属性にプロパティ名を指定することで 変数を好きなプロパティと紐付けることができます
    public List<bool>
    m_flagBoolList;
    public List<bool> flagBoolList //↑のリスト設定時にclipのリストそれぞれ数合わせる
    {
        //getはそのまま
        get { return m_flagBoolList; }
        //名前取得
        set
        {
            //まずはそのままデータ代入
            m_flagBoolList = value;


            List<TimelineClip> clipList = GetClips().ToList();
            //クリップを一個ずつ読んで処理する
            for (int i = 0; i < clipList.Count; i++)
            {
                //まずクリップの変数読み込み
                var cp = (clipList[i].asset as RMEventClip); // クリップが持つBehaviourを参照（パラメータ）

                //フラグ数セット
                Array.Resize(ref cp.flagBoolArray, m_flagBoolList.Count);
                Array.Resize(ref cp.aSentakuflagBoolArray, m_flagBoolList.Count);
                Array.Resize(ref cp.bSentakuflagBoolArray, m_flagBoolList.Count);
                Array.Resize(ref cp.m_behaviour.movePointEnterFlagBoolArray, m_flagBoolList.Count);
            }


        }
    }


    #region ■コード生成 読み取り関係(tmpKey連番付与もこの中)
    [SerializeField, Space(100)]
    [Button(nameof(Build), "コード生成")]
    public bool dummy;//属性でボタンを作るので何かしら宣言が必要。使わない

    [Multiline]
    public string
        readCodeAll;
    string
        tmpReadCode;
    [Multiline]
    public List<string>
        readCodeList = new List<string>();

    #region ■tmpkeyの末尾に連番を付与
    [SerializeField, Space(20)]
    [Button(nameof(tmpKeyNumberSerialized), "tmpKey名の末尾に連番を付与")]
    public bool dummy5;//属性でボタンを作るので何かしら宣言が必要。使わない

    void tmpKeyNumberSerialized()
    {
        //このトラックのクリップをリスト化
        List<TimelineClip> tmpClipList = this.GetClips().ToList();

        //連番付与用Int変数
        int tmpSerialNumberInt = 0;

        //クリップ一つ一つに処理
        for (int i = 0; i < tmpClipList.Count; i++)
        {
            //まずクリップの変数用behaviour読み込み
            var bh = (tmpClipList[i].asset as RMEventClip).m_behaviour; // クリップが持つBehaviourを参照（パラメータ）

            //tmpセリフあったら
            if (!string.IsNullOrWhiteSpace(bh.tmpSerihuKey))
            {
                //連番付与して連番用Int++
                bh.tmpSerihuKey += tmpSerialNumberInt;
                tmpSerialNumberInt++;
            }

            //tmpノベルあったら
            if (!string.IsNullOrWhiteSpace(bh.tmpNovelKey))
            {
                //連番付与して連番用Int++
                bh.tmpNovelKey += tmpSerialNumberInt;
                tmpSerialNumberInt++;
            }

        }

    }

    #endregion

    [SerializeField, Space(100)]
    [Multiline]
    public string
        writeCode;

    public void Build()
    {
#if UNITY_EDITOR

        DataCounter DC = GameObject.Find("Server").GetComponent<DataCounter>();
        DataBridging DB = GameObject.Find("DataBridging").GetComponent<DataBridging>();

        //初期化
        readCodeAll = tmpReadCode = "";
        readCodeList.Clear();

        List<RMEventTrack> outputTrackList = new List<RMEventTrack>();
        #region ■↑書き出すトラックのリストを作成し、Addしていくループ

        //まずこのトラック
        outputTrackList.Add(this);

        #region このトラックのクリップに、別のタイムライン読み込みがあればAddループ（新規Addがなくなったらループ終了）
        var tmpClipList = GetClips().ToList();
        for (int i = 0; i < tmpClipList.Count; i++)
        {
            //クリップ読み込んで
            var cp = tmpClipList[i].asset as RMEventClip;

            //タイムライン再生指定あったら
            if (cp.UnityTimelineObj != null)
            {
                //Prefabのパスを取得
                string path = AssetDatabase.GetAssetPath(cp.UnityTimelineObj);

                //不要部分削除　//Replaceだと、偶然別の消すかもなので、文字数と位置指定削除
                path = path.Remove(0, "Assets/Resources/".Length);//頭の部分
                path = path.Remove(path.Length - ".prefab".Length, ".prefab".Length);//お尻部分


                //■トラックを取得してAdd
                #region timelineObjからPlayableDirector → TimelineAsset →　TrackAssets → Track（RME）と読み込む
                //タイムライン内のトラック一覧を取得
                TimelineAsset tmpTimelineAsset =
                    (Resources.Load(path) as GameObject)
                    .GetComponent<PlayableDirector>()
                    .playableAsset as TimelineAsset;

                IEnumerable<TrackAsset> tmpTracks =
                    tmpTimelineAsset.GetOutputTracks();

                // 指定名称のトラックを抜き出す
                TrackAsset track = tmpTracks.FirstOrDefault(x => x.name == "RM Event Track");

                #endregion

                //それが新規トラックだったら
                if (outputTrackList.Contains(track) == false)
                {
                    //書き出すトラックリストにAdd
                    outputTrackList.Add(track as RMEventTrack);

                    //そのトラックのクリップも検査対象に追加
                    var tmpNewClipList = track.GetClips().ToList();
                    for (int f = 0; f < tmpNewClipList.Count; f++)
                    {
                        tmpClipList.Add(tmpNewClipList[f]);
                    }
                }
            }
        }


        #endregion
        #endregion

        #region ■セリフとノベルのキー被りがないかチェック
        bool isKaburi = false;//あったら処理やめる用

        //被りチェック用リスト
        List<string> tmpSerihuKeyList = new List<string>();
        #region StreamingAssetsSerihu読み込み
        string tmpSerihuStr = DC.SerihuTxtLoad("JP", true);

        //まず「＊改行」区切りで全部配列に読み込み
        var tmpPreSerihuDataArray
            = tmpSerihuStr.Split(new string[] { "＊\r\n" }, System.StringSplitOptions.None);

        //全角の＠がついているものはKey扱い
        foreach (string j in tmpPreSerihuDataArray)
        {
            if (j.IndexOf("＠") == 0)//その文字が何文字目にあるか調べて、0（先頭）だった場合
            {
                tmpSerihuKeyList.Add(j.Remove(0, 1));//先頭1文字（全角＠）を消したうえでAdd
            }
        }
        #endregion
        List<string> tmpNovelKeyList = new List<string>();
        #region StreamingAssetsNovel読み込み
        string tmpNovelStr = DC.NovelTxtLoad("JP", true);

        //まず「＊改行」区切りで全部配列に読み込み
        var tmpPreNovelDataArray
            = tmpNovelStr.Split(new string[] { "＊\r\n" }, System.StringSplitOptions.None);

        //全角の＠がついているものはKey扱い
        foreach (string j in tmpPreNovelDataArray)
        {
            if (j.IndexOf("＠") == 0)//その文字が何文字目にあるか調べて、0（先頭）だった場合
            {
                tmpNovelKeyList.Add(j.Remove(0, 1));//先頭1文字（全角＠）を消したうえでAdd
            }
        }
        #endregion

        #region クリップを一個ずつ読んでリストに追記しながら処理する
        for (int a = 0; a < outputTrackList.Count; a++)
        {
            List<TimelineClip> tmpKaburiCheckClipList = outputTrackList[a].GetClips().ToList();

            for (int i = 0; i < tmpKaburiCheckClipList.Count; i++)
            {
                //displaynameやstartはRMEventClipにしたら読み込めないので没
                ////情報取得用にcpも変数化
                //var cp = (tmpKaburiCheckClipList[i].asset as RMEventClip);

                //まずクリップの変数用behaviour読み込み
                var bh = (tmpKaburiCheckClipList[i].asset as RMEventClip).m_behaviour; // クリップが持つBehaviourを参照（パラメータ）

                //tmpセリフあったら
                if (!string.IsNullOrWhiteSpace(bh.tmpSerihuKey))
                {
                    #region 被りキーがないか確認
                    //■被りがあったら警告してtmpのままにする。
                    if (tmpSerihuKeyList.Contains(bh.tmpSerihuKey))
                    {
                        var info = " clip" + i + " " + tmpKaburiCheckClipList[i].displayName + " " + (tmpKaburiCheckClipList[i].start * 30).ToString("f0");

                        Debug.LogError("■■tmpSerihuキー " + bh.tmpSerihuKey + info + " はキー被りがあったため本テキスト出力処理を停止しました。キー名を変更してください");
                        isKaburi = true;
                    }
                    #endregion
                }
                //tempセリフキーをリストに追加
                tmpSerihuKeyList.Add(bh.tmpSerihuKey);

                //tmpノベルあったら
                if (!string.IsNullOrWhiteSpace(bh.tmpNovelKey))
                {
                    #region 被りキーがないか確認

                    //■被りがあったら警告してtmpのままにする。
                    if (tmpNovelKeyList.Contains(bh.tmpNovelKey))
                    {
                        var info = " clip" + i + " " + tmpKaburiCheckClipList[i].displayName + " " + (tmpKaburiCheckClipList[i].start * 30).ToString("f0");

                        Debug.LogError("■■■tmpNovelキー " + bh.tmpNovelKey + info + " はキー被りがあったため本テキスト出力処理を停止しました。キー名を変更してください");
                        isKaburi = true;
                    }

                    #endregion
                }
                //tempノベルキーをリストに追加
                tmpNovelKeyList.Add(bh.tmpNovelKey);
            }
        }

        #endregion

        //被りあったら処理終了
        if (isKaburi) { return; }
        #endregion


        #region ■トラックリスト分 全部書き出す
        for (int a = 0; a < outputTrackList.Count; a++)
        {
            List<TimelineClip> clipList = outputTrackList[a].GetClips().ToList();

            //デリゲートでクリップをソート（スタートタイム順にする）
            clipList.Sort((o1, o2) =>
            {
                if (o1.start < o2.start) return -1;
                if (o1.start > o2.start) return 1;
                return 0;
            });

            //コード出力用要素追加
            readCodeList.Add("");

            //タイムラインObj名でRegion括る
            tmpReadCode +=
                "#region " + outputTrackList[a].timelineAsset.name + "\n"
                + outputTrackList[a].timelineAsset.name + ":"//ジャンプ用ラベル
                + "\n";

            #region ■フラグ処理用初期設定変数を生成

            tmpReadCode +=
                "#region フラグ処理用初期設定" + "\n"
                + "//トラックのフラグ読み込んで生成" + "\n"
                + "codeFlagBoolList = new List<bool>();" + "\n"
                + "for (int i = 0; i < " + outputTrackList[a].flagBoolList.Count.ToString() + "; i++)" + "\n"
                + "{ codeFlagBoolList.Add(false); }" + "\n"
                + "\n"

                + "//クリップのフラグを取得する用Arrayを生成" + "\n"
                + "clipFlagBoolArray = new bool[" + outputTrackList[a].flagBoolList.Count.ToString() + "];" + "\n"
                + "\n"

                + "//結果判定用のフラグ初期化" + "\n"
                + "isThisCodeFlagJudge = false;" + "\n"
                + "#endregion"
                + "\n";

            #endregion

            #region ■それぞれ書き出し処理
            //クリップを一個ずつ読んで処理する
            for (int i = 0; i < clipList.Count; i++)
            {
                //まずクリップの変数用behaviour読み込み
                var bh = (clipList[i].asset as RMEventClip).m_behaviour; // クリップが持つBehaviourを参照（パラメータ）
                var cp = (clipList[i].asset as RMEventClip); //クリップのみ

                #region クリップ基礎情報書き出し （■ClipStartやclipDurationTimeなど）
                //クリップ何個目やタイム情報
                tmpReadCode +=
                    "\n"
                    + "//■ClipStart" + i
                    + "\n//clipStartTime " + clipList[i].start
                    + "\n//clipDurationTime " + clipList[i].duration
                    + "\n//clipEndTime " + clipList[i].end + "\n";//endは読み取り専用なので使ってないが一応

                //0個目の場合
                if (i == 0)
                {
                    //開始時間までウェイト
                    tmpReadCode += "yield return new WaitForSeconds(" + clipList[i].start + "f);" + "//clipStartTime" + "\n";
                }
                //1～ 個目の場合
                else if (0 < i && i <= clipList.Count - 1)
                {
                    //このクリップの開始時間から一つ前クリップのスタート時間引いた秒ウェイト
                    tmpReadCode += "yield return new WaitForSeconds(" + (clipList[i].start - clipList[i - 1].start) + "f);" + "//clipStartTime - preClipStartTime" + "\n";
                }
                ////最後のクリップの場合
                //今の所することないのでコメントアウト
                //else if(i == clipList.Count - 1)
                //{

                //}
                #endregion

                #region フラグ判定あれば（クリップをifで括るので最後にもある）

                //フラグ判定あるなら(編集中　この段階で テキストでcpのフラグを出力する方法で詰まって保留)
                if (cp.isUseFlagBool)
                {
                    tmpReadCode +=
                        "#region ■フラグ判定 クリップのフラグをコピーして、コードのフラグと照らし合わせて判定を取る（この時点でのリアルタイムな状態を判定）" + "\n"
                        + "//cpのフラグ内容コピー開始" + "\n";
                    //テキストでcpのフラグを出力する（同じ要素数であること前提）
                    for (int f = 0; f < cp.flagBoolArray.Length; f++)
                    {
                        tmpReadCode +=
                            "clipFlagBoolArray[" + f.ToString() + "] = " + cp.flagBoolArray[f].ToString().ToLower() + ";\n";
                    }
                    tmpReadCode +=
                        "//cpのフラグ内容コピー終了" + "\n";

                    tmpReadCode +=
                        "\n"
                        //+ "//結果判定用のフラグ初期化" + "\n"
                        //+ "isThisCodeFlagJudge = false;" + "\n"
                        //+ "\n"

                        + "//コードフラグとクリップフラグ比較して結果代入" + "\n"
                        + "for (int f = 0; f < clipFlagBoolArray.Length; f++)" + "\n"
                        + "{" + "\n"
                        + "//クリップのフラグとトラックのフラグが合ってたらフラグON" + "\n"
                        + "if (clipFlagBoolArray[f] == codeFlagBoolList[f])" + "\n"
                        + "{ isThisCodeFlagJudge = true; }" + "\n"
                        + "else //合ってなければオフして抜け" + "\n"
                        + "{ isThisCodeFlagJudge = false; break; }" + "\n"
                        + "}" + "\n"
                        + "#endregion" + "\n"
                        + "\n";

                    tmpReadCode +=
                        "\n"
                        + "//結果判定でこのクリップの処理を括る" + "\n"
                        + "if(isThisCodeFlagJudge)" + "\n"
                        + "{"
                        + "\n";

                }

                #endregion

                #region 選択肢キーが指定されていたら、コードに追加

                //nullではなく、かつ空文字列でもなく、かつ空白文字列でもない
                if (!string.IsNullOrWhiteSpace(cp.AKeySentakushi)
                    && !string.IsNullOrWhiteSpace(cp.BKeySentakushi))
                {
                    //■選択肢A
                    //追加
                    tmpReadCode +=
                        nameof(DataCounter.Sentakushi) + "(\"■" + cp.AKeySentakushi + "\");";

                    #region 一行目をコメントアウトで追加
                    //改行があれば改行まで（一行だけにする）
                    if (cp.previewAValueSentakushi.IndexOf("\n") > 0)
                    {
                        tmpReadCode += "//" + cp.previewAValueSentakushi.Substring(0, cp.previewAValueSentakushi.IndexOf("\n"));
                    }
                    //なければ最後まで
                    else
                    {
                        tmpReadCode += "//" + cp.previewAValueSentakushi;
                    }
                    #endregion

                    tmpReadCode += "\n";//改行


                    //■選択肢B
                    //追加
                    tmpReadCode +=
                        nameof(DataCounter.Sentakushi) + "(\"■" + cp.BKeySentakushi + "\");";

                    #region 一行目をコメントアウトで追加
                    //改行があれば改行まで（一行だけにする）
                    if (cp.previewBValueSentakushi.IndexOf("\n") > 0)
                    {
                        tmpReadCode += "//" + cp.previewBValueSentakushi.Substring(0, cp.previewBValueSentakushi.IndexOf("\n"));
                    }
                    //なければ最後まで
                    else
                    {
                        tmpReadCode += "//" + cp.previewBValueSentakushi;
                    }
                    #endregion

                    tmpReadCode += "\n\n";//改行
                }
                //tmp選択肢だったら
                else if (!string.IsNullOrWhiteSpace(cp.tmpAKeySentakushi)
                    && !string.IsNullOrWhiteSpace(cp.tmpBKeySentakushi))
                {
                    //■選択肢A
                    //コードにはそのキーで追加
                    tmpReadCode +=
                        nameof(DataCounter.Sentakushi) + "(\"■" + cp.tmpAKeySentakushi + "\");";

                    #region 被りキーがないか確認

                    //StreamingAssets読み込み
                    string tmpStr = DC.SerihuTxtLoad("JP", true);

                    string[]
                        tmpSerihuDataArray = tmpStr.Split(new string[] { "＊\r\n" }, System.StringSplitOptions.None);

                    //全角の＠がついているものはKey扱い
                    List<string> tmpDefKeyList = new List<string>();
                    foreach (string j in tmpSerihuDataArray)
                    {
                        if (j.IndexOf("＠") == 0)//その文字が何文字目にあるか調べて、0（先頭）だった場合
                        {
                            tmpDefKeyList.Add(j.Remove(0, 1));//先頭1文字（全角＠）を消したうえでAdd
                        }
                    }

                    //■被りがあったら警告してtmpのままにする。
                    if (tmpDefKeyList.Contains(cp.tmpAKeySentakushi))
                    {
                        Debug.LogError("■■■tmp選択肢Aキー " + cp.tmpAKeySentakushi + " は、本テキストのキーと被りがあったため本テキスト化しませんでした。キー名を変更してください");
                        #region tmpセリフ一行目をコメントアウトで追加
                        //改行があれば改行まで
                        if (cp.tmpAValueSentakushi.IndexOf("\n") > 0)
                        {
                            tmpReadCode += "//" + cp.tmpAValueSentakushi.Substring(0, cp.tmpAValueSentakushi.IndexOf("\n"));
                        }
                        //なければ最後まで
                        else
                        {
                            tmpReadCode += "//" + cp.tmpAValueSentakushi;
                        }
                        #endregion

                    }
                    #endregion
                    #region 被りがなかったら本テキスト化（UnityTimelineSerihu.txtに追記）
                    else
                    {
                        //改行コードを/r/nに統一（二回置き換えを二回している）
                        cp.tmpAValueSentakushi = cp.tmpAValueSentakushi.Replace("\n", "\r\n").Replace("\r\r", "\r");
                        cp.tmpAValueSentakushi = cp.tmpAValueSentakushi.Replace("\r", "\r\n").Replace("\n\n", "\n");

                        //追記
                        #region 書き込み先テキストが空でなければ、最初に改行を入れる
                        if (File.ReadAllText(Application.streamingAssetsPath + "/Text/UnityTimelineTempSerihu.txt", Encoding.UTF8)
                            != "")
                        {
                            File.AppendAllText(Application.streamingAssetsPath + "/Text/UnityTimelineTempSerihu.txt", "\r\n");
                        }
                        #endregion
                        File.AppendAllText(Application.streamingAssetsPath + "/Text/UnityTimelineTempSerihu.txt"
                            , "＠■" + cp.tmpAKeySentakushi + "＊" + "\r\n"
                            + cp.tmpAValueSentakushi + "＊");
                        //, Encoding.UTF8);//指定するとBOMが先頭に書き込まれるようなので、なし（なしだとBOM無しUTF-8になるらしい）

                        //tmpキーを本キーに
                        cp.AKeySentakushi = cp.tmpAKeySentakushi;
                        //tmpValueをプレビューに
                        cp.previewAValueSentakushi = cp.tmpAValueSentakushi;

                        //tmp削除
                        cp.tmpAKeySentakushi = cp.tmpAValueSentakushi = "";

                        #region セリフ一行目をコメントアウトで追加
                        //改行があれば改行まで（一行だけにする）
                        if (cp.previewAValueSentakushi.IndexOf("\n") > 0)
                        {
                            tmpReadCode += "//" + cp.previewAValueSentakushi.Substring(0, cp.previewAValueSentakushi.IndexOf("\n"));
                        }
                        //なければ最後まで
                        else
                        {
                            tmpReadCode += "//" + cp.previewAValueSentakushi;
                        }
                        #endregion
                    }
                    #endregion

                    tmpReadCode += "\n";//改行


                    //■選択肢B
                    //コードにはそのキーで追加
                    tmpReadCode +=
                        nameof(DataCounter.Sentakushi) + "(\"■" + cp.tmpBKeySentakushi + "\");";

                    #region 被りキーがないか確認

                    //StreamingAssets読み込み
                    tmpStr = DC.SerihuTxtLoad("JP", true);

                    tmpSerihuDataArray = tmpStr.Split(new string[] { "＊\r\n" }, System.StringSplitOptions.None);

                    //全角の＠がついているものはKey扱い
                    tmpDefKeyList = new List<string>();
                    foreach (string j in tmpSerihuDataArray)
                    {
                        if (j.IndexOf("＠") == 0)//その文字が何文字目にあるか調べて、0（先頭）だった場合
                        {
                            tmpDefKeyList.Add(j.Remove(0, 1));//先頭1文字（全角＠）を消したうえでAdd
                        }
                    }

                    //■被りがあったら警告してtmpのままにする。
                    if (tmpDefKeyList.Contains(cp.tmpBKeySentakushi))
                    {
                        Debug.LogError("■■■tmp選択肢Bキー " + cp.tmpBKeySentakushi + " は、本テキストのキーと被りがあったため本テキスト化しませんでした。キー名を変更してください");
                        #region tmpセリフ一行目をコメントアウトで追加
                        //改行があれば改行まで
                        if (cp.tmpBValueSentakushi.IndexOf("\n") > 0)
                        {
                            tmpReadCode += "//" + cp.tmpBValueSentakushi.Substring(0, cp.tmpBValueSentakushi.IndexOf("\n"));
                        }
                        //なければ最後まで
                        else
                        {
                            tmpReadCode += "//" + cp.tmpBValueSentakushi;
                        }
                        #endregion

                    }
                    #endregion
                    #region 被りがなかったら本テキスト化（UnityTimelineSerihu.txtに追記）
                    else
                    {
                        //改行コードを/r/nに統一（二回置き換えを二回している）
                        cp.tmpBValueSentakushi = cp.tmpBValueSentakushi.Replace("\n", "\r\n").Replace("\r\r", "\r");
                        cp.tmpBValueSentakushi = cp.tmpBValueSentakushi.Replace("\r", "\r\n").Replace("\n\n", "\n");

                        //追記
                        #region 書き込み先テキストが空でなければ、最初に改行を入れる
                        if (File.ReadAllText(Application.streamingAssetsPath + "/Text/UnityTimelineTempSerihu.txt", Encoding.UTF8)
                            != "")
                        {
                            File.AppendAllText(Application.streamingAssetsPath + "/Text/UnityTimelineTempSerihu.txt", "\r\n");
                        }
                        #endregion
                        File.AppendAllText(Application.streamingAssetsPath + "/Text/UnityTimelineTempSerihu.txt"
                            , "＠■" + cp.tmpBKeySentakushi + "＊" + "\r\n"
                            + cp.tmpBValueSentakushi + "＊");
                        //, Encoding.UTF8);//指定するとBOMが先頭に書き込まれるようなので、なし（なしだとBOM無しUTF-8になるらしい）

                        //tmpキーを本キーに
                        cp.BKeySentakushi = cp.tmpBKeySentakushi;
                        //tmpValueをプレビューに
                        cp.previewBValueSentakushi = cp.tmpBValueSentakushi;

                        //tmp削除
                        cp.tmpBKeySentakushi = cp.tmpBValueSentakushi = "";

                        #region セリフ一行目をコメントアウトで追加
                        //改行があれば改行まで（一行だけにする）
                        if (cp.previewBValueSentakushi.IndexOf("\n") > 0)
                        {
                            tmpReadCode += "//" + cp.previewBValueSentakushi.Substring(0, cp.previewBValueSentakushi.IndexOf("\n"));
                        }
                        //なければ最後まで
                        else
                        {
                            tmpReadCode += "//" + cp.previewBValueSentakushi;
                        }
                        #endregion
                    }
                    #endregion

                    tmpReadCode += "\n\n";//改行
                }

                //その後、選択肢処理(本キーに追加したのでif取れる)
                if (!string.IsNullOrWhiteSpace(cp.AKeySentakushi)
                    && !string.IsNullOrWhiteSpace(cp.BKeySentakushi))
                {
                    tmpReadCode +=
                        "\n"
                        + "//選択肢選ばれるまでループ" + "\n"
                        + "while (sentakuListNum == 99) { yield return null; }" + "\n"
                        + "switch (sentakuListNum)" + "\n"
                        + "{" + "\n"
                        + "case 0://選択肢a" + "\n"
                        + "//選択肢aのフラグ内容反映" + "\n";

                    //AフラグをcodeFlagBoolArrayに反映する（同じ要素数であること前提）
                    for (int f = 0; f < cp.flagBoolArray.Length; f++)
                    {
                        tmpReadCode +=
                            "codeFlagBoolList[" + f.ToString() + "] = " + cp.aSentakuflagBoolArray[f].ToString().ToLower() + ";\n";
                    }
                    tmpReadCode +=
                        "break;" + "\n"
                        + "case 1://選択肢b" + "\n"
                        + "//選択肢bのフラグ内容反映" + "\n";
                    //BフラグをcodeFlagBoolArrayに反映する（同じ要素数であること前提）
                    for (int f = 0; f < cp.flagBoolArray.Length; f++)
                    {
                        tmpReadCode +=
                            "codeFlagBoolList[" + f.ToString() + "] = " + cp.bSentakuflagBoolArray[f].ToString().ToLower() + ";\n";
                    }
                    tmpReadCode +=
                        "break;" + "\n"
                        + "}" + "\n"
                        + "sentakuListNum = 99;" + "\n"
                        + "\n";
                }

                #endregion
                #region Frac爆発設定
                //初期化なら元に戻す
                if (cp.fracImpact == RMEventClip.FracImpact.初期化)
                {
                    tmpReadCode +=
                        "\n"
                        + "//Frac爆発設定初期化（元に戻す）" + "\n"
                        + "UTLFracImpactSetting(false);" + "\n"
                        + "\n";
                }
                //設定なら設定する
                else if (cp.fracImpact == RMEventClip.FracImpact.設定)
                {
                    //Prefabのパスを取得
                    string path = AssetDatabase.GetAssetPath(cp.impactPosObj);
                    //不要部分削除　//Replaceだと、偶然別の消すかもなので、文字数と位置指定削除
                    path = path.Remove(0, "Assets/Resources/".Length);//頭の部分
                    path = path.Remove(path.Length - ".prefab".Length, ".prefab".Length);//お尻部分

                    //impactPosObj取得
                    tmpReadCode +=
                        "\n"
                        + "#region Frac爆発設定コルーチン始動" + "\n"
                        + "//Frac爆発設定PosObj取得" + "\n"
                        + "GameObject " + cp.impactPosObj.name + i.ToString() + "\n" //変数名被りがあるかもなので、連番付与
                        + "    = Resources.Load(\"" + path + "\") as GameObject;" + "\n";

                    tmpReadCode +=
                        "\n"
                        + "//Frac爆発設定コルーチン始動" + "\n"
                        + "UTLFracImpactSetting(true" + "\n"
                        + "//impactPosObj //コード読み込み用コメント" + "\n"
                        + ", " + cp.impactPosObj.name + i.ToString() + "\n"
                        + "//impactForce //コード読み込み用コメント" + "\n"
                        + ", " + cp.impactForce + "f" + "\n"
                        + "//impactRadius //コード読み込み用コメント" + "\n"
                        + ", " + cp.impactRadius + "f" + "\n"
                        + "//bAlsoImpactFreeChunks //コード読み込み用コメント" + "\n"
                        + ", " + cp.bAlsoImpactFreeChunks.ToString().ToLower() + ");" + "\n"
                        + "#endregion" + "\n"
                        + "\n";
                }
                #endregion
                #region オブジェクト 配置移動削除 関係

                #region 指定オブジェの子オブジェ全削除

                tmpReadCode +=
                    "\n"
                    + "#region 指定オブジェの子オブジェ全削除" + "\n"
                    + "//Nullチェック" + "\n"
                    + "if (GameObject.Find(\"" + cp.childAllDelObjName.ToString() + "\") != null)" + "\n"
                    + "{" + "\n"
                    + "var tmpObj = GameObject.Find(\"" + cp.childAllDelObjName.ToString() + "\"); " + "\n"
                    + "//子オブジェ全削除" + "\n"
                    + "foreach (Transform trs in tmpObj.transform)" + "\n"
                    + "{ Destroy(trs.gameObject); }" + "\n"
                    + "}" + "\n"
                    + "else" + "\n"
                    + "{ Debug.Log(\"■" + cp.childAllDelObjName + "がヒエラルキーにない？\"" + ");}" + "\n"
                    + "#endregion" + "\n"
                    + "\n";

                //if (!string.IsNullOrWhiteSpace(cp.childAllDelObjName))
                //{
                //    //Nullチェック
                //    if (GameObject.Find(cp.childAllDelObjName) != null)
                //    {
                //        var tmpObj = GameObject.Find(cp.childAllDelObjName);
                //        //子オブジェ全削除
                //        foreach (Transform trs in tmpObj.transform)
                //        { Destroy(trs.gameObject); }
                //    }
                //    else
                //    { Debug.Log("■" + cp.childAllDelObjName + "がヒエラルキーにない？" + nameof(cp.childAllDelObjName)); }
                //}
                #endregion


                #endregion


                #region セリフキーが指定されていたら、コードに追加

                //nullではなく、かつ空文字列でもなく、かつ空白文字列でもない
                if (!string.IsNullOrWhiteSpace(bh.serihuKey))
                {
                    //追加
                    tmpReadCode +=
                        nameof(DataCounter.Hukidashi) + "(\"" + bh.serihuKey + "\");";

                    #region セリフ一行目をコメントアウトで追加
                    //改行があれば改行まで（一行だけにする）
                    if (bh.previewSerihuValue.IndexOf("\n") > 0)
                    {
                        tmpReadCode += "//" + bh.previewSerihuValue.Substring(0, bh.previewSerihuValue.IndexOf("\n"));
                    }
                    //なければ最後まで
                    else
                    {
                        tmpReadCode += "//" + bh.previewSerihuValue;
                    }
                    #endregion

                    tmpReadCode += "\n";//改行
                }
                //tmpセリフだったら
                else if (!string.IsNullOrWhiteSpace(bh.tmpSerihuKey))
                {
                    //直接ファイルに書き込むようにしたからいらないか
                    ////テキスト用に書き出してから
                    //readSerihuKeyValue += "＠" + bh.tmpSerihuKey + "＊" + "\r\n";
                    //readSerihuKeyValue += bh.tmpSerihuValue + "＊" + "\r\n";

                    //コードにはそのキーで追加
                    tmpReadCode +=
                        nameof(DataCounter.Hukidashi) + "(\"" + bh.tmpSerihuKey + "\");";

                    #region 被りキーがないか確認

                    //StreamingAssets読み込み
                    string tmpStr = DC.SerihuTxtLoad("JP", true);

                    string[]
                        tmpSerihuDataArray = tmpStr.Split(new string[] { "＊\r\n" }, System.StringSplitOptions.None);

                    //全角の＠がついているものはKey扱い
                    List<string> tmpDefKeyList = new List<string>();
                    foreach (string j in tmpSerihuDataArray)
                    {
                        if (j.IndexOf("＠") == 0)//その文字が何文字目にあるか調べて、0（先頭）だった場合
                        {
                            tmpDefKeyList.Add(j.Remove(0, 1));//先頭1文字（全角＠）を消したうえでAdd
                        }
                    }

                    //■被りがあったら警告してtmpのままにする。
                    if (tmpDefKeyList.Contains(bh.tmpSerihuKey))
                    {
                        Debug.LogError("■■■tmpSerihuキー " + bh.tmpSerihuKey + " は、本テキストのキーと被りがあったため本テキスト化しませんでした。キー名を変更してください");
                        #region tmpセリフ一行目をコメントアウトで追加
                        //改行があれば改行まで
                        if (bh.tmpSerihuValue.IndexOf("\n") > 0)
                        {
                            tmpReadCode += "//" + bh.tmpSerihuValue.Substring(0, bh.tmpSerihuValue.IndexOf("\n"));
                        }
                        //なければ最後まで
                        else
                        {
                            tmpReadCode += "//" + bh.tmpSerihuValue;
                        }
                        #endregion

                    }
                    #endregion
                    #region 被りがなかったら本テキスト化（UnityTimelineSerihu.txtに追記）
                    else
                    {
                        //改行コードを/r/nに統一（二回置き換えを二回している）
                        bh.tmpSerihuValue = bh.tmpSerihuValue.Replace("\n", "\r\n").Replace("\r\r", "\r");
                        bh.tmpSerihuValue = bh.tmpSerihuValue.Replace("\r", "\r\n").Replace("\n\n", "\n");

                        //追記
                        #region 書き込み先テキストが空でなければ、最初に改行を入れる
                        if (File.ReadAllText(Application.streamingAssetsPath + "/Text/UnityTimelineTempSerihu.txt", Encoding.UTF8)
                            != "")
                        {
                            File.AppendAllText(Application.streamingAssetsPath + "/Text/UnityTimelineTempSerihu.txt"
                                , "\r\n");
                        }
                        #endregion
                        File.AppendAllText(Application.streamingAssetsPath + "/Text/UnityTimelineTempSerihu.txt"
                            , "＠" + bh.tmpSerihuKey + "＊" + "\r\n"
                            + bh.tmpSerihuValue + "＊");
                        //, Encoding.UTF8);//指定するとBOMが先頭に書き込まれるようなので、なし（なしだとBOM無しUTF-8になるらしい）

                        //tmpキーを本キーに　//tmpValueをプレビューに
                        bh.serihuKey = bh.tmpSerihuKey;
                        bh.previewSerihuValue = bh.tmpSerihuValue;

                        //tmp削除
                        bh.tmpSerihuKey = bh.tmpSerihuValue = "";

                        #region セリフ一行目をコメントアウトで追加
                        //改行があれば改行まで（一行だけにする）
                        if (bh.previewSerihuValue.IndexOf("\n") > 0)
                        {
                            tmpReadCode += "//" + bh.previewSerihuValue.Substring(0, bh.previewSerihuValue.IndexOf("\n"));
                        }
                        //なければ最後まで
                        else
                        {
                            tmpReadCode += "//" + bh.previewSerihuValue;
                        }
                        #endregion
                    }
                    #endregion


                    tmpReadCode += "\n";//改行
                }
                #endregion
                #region ノベルキーが指定されていたら、コードに追加
                if (!string.IsNullOrWhiteSpace(bh.novelKey))
                {
                    //■追加
                    //オートだったらオートで
                    if (bh.isNovelAuto) { tmpReadCode += nameof(DataCounter.NovelSetVis) + "(\"" + bh.novelKey + "\", true);"; }
                    else { tmpReadCode += nameof(DataCounter.NovelSetVis) + "(\"" + bh.novelKey + "\");"; }

                    #region ノベル一行目をコメントアウトで追加
                    //改行があれば改行まで
                    if (bh.previewNovelValue.IndexOf("\n") > 0)
                    {
                        tmpReadCode += "//" + bh.previewNovelValue.Substring(0, bh.previewNovelValue.IndexOf("\n"));
                    }
                    //なければ最後まで
                    else
                    {
                        tmpReadCode += "//" + bh.previewNovelValue;
                    }
                    #endregion
                    tmpReadCode += "\n\n";//改行
                }
                //tmpノベルだったら
                else if (!string.IsNullOrWhiteSpace(bh.tmpNovelKey))
                {
                    //■コードにそのキーで追加
                    //オートだったらオートで
                    if (bh.isNovelAuto) { tmpReadCode += nameof(DataCounter.NovelSetVis) + "(\"" + bh.tmpNovelKey + "\", true);"; }
                    else { tmpReadCode += nameof(DataCounter.NovelSetVis) + "(\"" + bh.tmpNovelKey + "\");"; }

                    #region 被りキーがないか確認

                    //StreamingAssets
                    string tmpStr = DC.NovelTxtLoad("JP", true);

                    //まず「＊改行」区切りで全部配列に読み込み
                    var tmpNovelDataArray
                        = tmpStr.Split(new string[] { "＊\r\n" }, System.StringSplitOptions.None);

                    //全角の＠がついているものはKey扱い
                    List<string> tmpDefKeyList = new List<string>();
                    foreach (string j in tmpNovelDataArray)
                    {
                        if (j.IndexOf("＠") == 0)//その文字が何文字目にあるか調べて、0（先頭）だった場合
                        {
                            tmpDefKeyList.Add(j.Remove(0, 1));//先頭1文字（全角＠）を消したうえでAdd
                        }
                    }

                    //■被りがあったら警告してtmpのままにする。
                    if (tmpDefKeyList.Contains(bh.tmpNovelKey))
                    {
                        Debug.LogError("■■■tmpNovelキー " + bh.tmpNovelKey + " は、本テキストのキーと被りがあったため本テキスト化しませんでした。キー名を変更してください");
                        #region tmpノベル一行目をコメントアウトで追加
                        //改行があれば改行まで
                        if (bh.tmpNovelValue.IndexOf("\n") > 0)
                        {
                            tmpReadCode += "//" + bh.tmpNovelValue.Substring(0, bh.tmpNovelValue.IndexOf("\n"));
                        }
                        //なければ最後まで
                        else
                        {
                            tmpReadCode += "//" + bh.tmpNovelValue;
                        }
                        #endregion

                    }
                    #endregion
                    #region 被りがなかったら本テキスト化（UnityTimelineNovel.txtに追記）
                    else
                    {
                        //改行コードを/r/nに統一（二回置き換えを二回している）
                        bh.tmpNovelValue = bh.tmpNovelValue.Replace("\n", "\r\n").Replace("\r\r", "\r");
                        bh.tmpNovelValue = bh.tmpNovelValue.Replace("\r", "\r\n").Replace("\n\n", "\n");

                        //追記
                        #region 書き込み先テキストが空でなければ、最初に改行を入れる
                        if (File.ReadAllText(Application.streamingAssetsPath + "/Text/UnityTimelineTempNovel.txt", Encoding.UTF8)
                            != "")
                        {
                            File.AppendAllText(Application.streamingAssetsPath + "/Text/UnityTimelineTempNovel.txt"
                                , "\r\n");
                        }
                        #endregion
                        File.AppendAllText(Application.streamingAssetsPath + "/Text/UnityTimelineTempNovel.txt"
                            , "＠" + bh.tmpNovelKey + "＊" + "\r\n"
                            + bh.tmpNovelValue + "＊");
                        //, Encoding.UTF8);//指定するとBOMが先頭に書き込まれるようなので、なし（なしだとBOM無しUTF-8になるらしい）

                        //tmpキーを本キーに　//tmpValueをプレビューに
                        bh.novelKey = bh.tmpNovelKey;
                        bh.previewNovelValue = bh.tmpNovelValue;

                        //tmp削除
                        bh.tmpNovelKey = bh.tmpNovelValue = "";

                        #region ノベル一行目をコメントアウトで追加
                        //改行があれば改行まで（一行だけにする）
                        if (bh.previewNovelValue.IndexOf("\n") > 0)
                        {
                            tmpReadCode += "//" + bh.previewNovelValue.Substring(0, bh.previewNovelValue.IndexOf("\n"));
                        }
                        //なければ最後まで
                        else
                        {
                            tmpReadCode += "//" + bh.previewNovelValue;
                        }
                        #endregion

                    }
                    #endregion

                    tmpReadCode += "\n\n";//改行
                }
                #endregion
                #region PrefabをGameobjectsに設置があれば spawnObj
                if (bh.spawnObj != null)
                {
                    //Prefabのパスを取得
                    string path = AssetDatabase.GetAssetPath(bh.spawnObj);
                    //不要部分削除　//Replaceだと、偶然別の消すかもなので、文字数と位置指定削除
                    path = path.Remove(0, "Assets/Resources/".Length);//頭の部分
                    path = path.Remove(path.Length - ".prefab".Length, ".prefab".Length);//お尻部分

                    tmpReadCode +=
                        "\n" + "//PrefabをGameobjectsに設置" + "\n"
                        + "GameObject " + bh.spawnObj.name + i.ToString() + "\n" //変数名被りがあるかもなので、連番付与
                        + "    = Instantiate(Resources.Load(\"" + path + "\") as GameObject" + "\n"
                        + "    , GameObjectsTrs);" + "\n"
                        + bh.spawnObj.name + i.ToString() + ".name" + " //ヒエラルキーに置く際のObj名（このコメントはコードから取得用のもの）" + "\n"
                        + "= \"" + bh.spawnObjName + "\";" + "\n"
                        + "\n";

                }
                #endregion
                #region モーションあれば　再生

                //モーションObj名指定があれば、そのObjのAnimaterControllerで
                if (!string.IsNullOrWhiteSpace(bh.motionObjName))
                {
                    if (!string.IsNullOrWhiteSpace(bh.motionStateName))
                    {
                        #region モーションObj名とスポーンObj名が一緒なら1フレ待つフラグ立てる
                        bool is1FrameWait = false;//1フレ待つか判定用

                        //スポーンObj名が指定されていて
                        if (!string.IsNullOrWhiteSpace(bh.spawnObjName))
                        {
                            //モーションObj名とスポーンObj名が一緒だったら
                            if (bh.motionObjName == bh.spawnObjName)
                            {
                                //1フレ待つフラグON
                                is1FrameWait = true;
                            }
                        }
                        #endregion

                        if (is1FrameWait)
                        {
                            tmpReadCode +=
                                "//モーションObj名とスポーンObj名が一緒なので1フレ待つ" + "\n"
                                + "yield return null;" + "\n";
                        }

                        tmpReadCode +=
                            "\n"
                            + "//オブジェ名からAnimator取得" + "\n"
                            + "GameObject.Find(\"" + bh.motionObjName.ToString() + "\").GetComponent<Animator>()" + "\n"
                            + ".CrossFadeInFixedTime(" + "\n"
                            + "//オブジェ指定モーション名取得用コメント" + "\n"
                            + "\"" + bh.motionStateName.ToString() + "\"" + "\n"
                            + "//オブジェ指定モーションタイム取得用コメント" + "\n"
                            + ", " + bh.motionCrossFadeTime.ToString() + "f" + "\n"
                            + ");" + "\n"
                            + "\n";
                    }
                }
                else//なければちえりとして（従来どおり）
                {
                    if (!string.IsNullOrWhiteSpace(bh.motionStateName))
                    {
                        tmpReadCode += nameof(DataCounter.ChieriMotion) + "(\"" + bh.motionStateName + "\", " + bh.motionCrossFadeTime + "f, 0);" + "\n";
                    }
                }
                //表情あれば
                if (!string.IsNullOrWhiteSpace(bh.faceStateName))
                {
                    tmpReadCode += nameof(DataCounter.ChieriMotion) + "(\"" + bh.faceStateName + "\", " + bh.faceCrossFadeTime + "f, 2);" + "\n";
                }


                #endregion
                #region IKLookAtPlayer見る
                if (bh.iKLookAtPlayer == RMEventBehaviour.IKLookAtPlayer.プレイヤー見る)
                {
                    tmpReadCode +=
                        "\n"
                    + "#region IKLookAtプレイヤー見る" + "\n"
                    + nameof(DataCounter.ChieriMotion) + "(\"まばたき\", 0f, 4); " + nameof(DataCounter.blinkTime) + " = 0;" + "\n"
                    + nameof(DataCounter.FollowDOMove) + "(" + nameof(DataCounter.IKLookAtEyeTargetTrs) + ", " + nameof(DataCounter.PlayerEyeTargetTrs) + ", 0f);" + "\n"
                    + nameof(DataCounter.DOTweenToLAIKSEyes) + "(" + nameof(DataCounter.LAIKEyeS) + ", " + nameof(DataCounter.LAIKSEyesDefWeight) + ", 0f);" + "\n"
                    + nameof(DataCounter.FollowDOMove) + "(" + nameof(DataCounter.IKLookAtHeadTargetTrs) + ", " + nameof(DataCounter.PlayerHeadTargetTrs) + ", new Vector3(0, -0.045f, 0));" + "\n"
                    + nameof(DataCounter.DOTweenToLAIKSHead) + "(" + nameof(DataCounter.LAIKHeadS) + ", " + nameof(DataCounter.LAIKSHeadDefWeight) + ", 1f);" + "\n"
                    + "#endregion" + "\n"
                    + "\n";

                    #region //テキスト雛形
                    //#region IKLookAtプレイヤー見る
                    //ChieriMotion("まばたき", 0f, 4); blinkTime = 0;
                    //FollowDOMove(IKLookAtEyeTargetTrs, PlayerEyeTargetTrs,0f);
                    //DOTweenToLAIKSEyes(LAIKEyeS, LAIKSEyesDefWeight, 0f);
                    //FollowDOMove(IKLookAtHeadTargetTrs, PlayerHeadTargetTrs, new Vector3(0, -0.045f, 0));
                    //DOTweenToLAIKSHead(LAIKHeadS, LAIKSHeadDefWeight, 1f);
                    //#endregion
                    #endregion
                }
                else if (bh.iKLookAtPlayer == RMEventBehaviour.IKLookAtPlayer.解除)
                {
                    tmpReadCode +=
                        "\n"
                        + "#region IKLookAt解除" + "\n"
                        + nameof(DataCounter.ChieriMotion) + "(\"まばたき\", 0f, 4); " + nameof(DataCounter.blinkTime) + " = 0;" + "\n"
                        + nameof(DataCounter.DOTweenToLAIKSEyes) + "(" + nameof(DataCounter.LAIKEyeS) + ", 0, 0f);" + "\n"
                        + nameof(DataCounter.DOTweenToLAIKSHead) + "(" + nameof(DataCounter.LAIKHeadS) + ", 0, 1f);" + "\n"
                        + "#endregion" + "\n"
                        + "\n";

                    #region //テキスト雛形
                    //ChieriMotion("まばたき", 0f, 4); blinkTime = 0;
                    //DOTweenToLAIKSEyes(LAIKEyeS, 0, 0f);
                    //DOTweenToLAIKSHead(LAIKHeadS, 0, 1f);
                    #endregion
                }


                #endregion
                #region 削除destroyObjNameあれば （　未　）
                if (!string.IsNullOrWhiteSpace(bh.destroyObjName))
                {
                    tmpReadCode +=
                        "\n"
                        + "#region オブジェ削除" + "\n"
                        + "//Nullチェック" + "\n"
                        + "if (GameObject.Find(\"" + bh.destroyObjName + "\") != null)" + "\n"
                        + "{" + "\n"

                        + "//ヒエラルキーからObj削除（Obj名で直接）" + "\n"
                        + "Destroy(GameObject.Find(\"" + bh.destroyObjName + "\"));" + "\n"

                        + "}" + "\n"
                        + "else" + "\n"
                        + "{ Debug.Log(\"■" + bh.destroyObjName + "がヒエラルキーにない？\"" + ");}" + "\n"
                        + "#endregion" + "\n"
                        + "\n";
                }
                #endregion
                #region 削除destroyObjListあれば //Hideで廃止済み 念の為処理だけはされるように残し
                if (bh.destroyObjList.Count != 0)
                {


                    //ヒエラルキーからObj削除
                    for (int d = 0; d < bh.destroyObjList.Count; d++)
                    {
                        //Nullチェック
                        if (GameObject.Find(bh.destroyObjList[d]) != null)
                        {
                            UnityEngine.Object.Destroy(GameObject.Find(bh.destroyObjList[d]));
                        }
                        else
                        {
                            Debug.Log("■" + bh.destroyObjList[d] + "がヒエラルキーにない？ destroyObjList[" + d + "]");
                        }
                    }
                }
                #endregion
                #region ペアレント設定あれば
                //親と子 両方指定あれば
                if (!string.IsNullOrWhiteSpace(bh.ParentObjName) && !string.IsNullOrWhiteSpace(bh.ChildObjName))
                {
                    tmpReadCode +=
                        "\n"
                        + "//ペアレント（Obj名で直接）" + "\n"
                        + "GameObject.Find(\"" + bh.ChildObjName + "\").transform" + "\n"
                        + ".SetParent(GameObject.Find(\"" + bh.ParentObjName + "\").transform);" + "\n"
                        + "\n";
                }

                #endregion
                #region RigidBodyのisKinematic
                //参照するObj名があれば
                if (!string.IsNullOrWhiteSpace(bh.rigidbodyObjName))
                {
                    tmpReadCode +=
                        "\n"
                        + "//FindしてRigidbodyのisKinematicを設定" + "\n"
                        + "GameObject.Find(\"" + bh.rigidbodyObjName + "\").GetComponent<Rigidbody>()" + "\n"
                        + ".isKinematic = " + bh.isKinematic.ToString().ToLower() + ";" + "\n"
                        + "\n";

                }
                #endregion
                #region プレイヤー位置Prefabあれば
                if (bh.playerLocalPosObj != null)
                {
                    //Prefabのパスを取得
                    string path = AssetDatabase.GetAssetPath(bh.playerLocalPosObj);
                    //不要部分削除　//Replaceだと、偶然別の消すかもなので、文字数と位置指定削除
                    path = path.Remove(0, "Assets/Resources/".Length);//頭の部分
                    path = path.Remove(path.Length - ".prefab".Length, ".prefab".Length);//お尻部分


                    tmpReadCode +=
                        "\n" + "//プレイヤー位置指定" + "\n"
                        + "GameObject " + bh.playerLocalPosObj.name + i.ToString() + "\n" //変数名被りがあるかもなので、連番付与
                        + "    = Resources.Load(\"" + path + "\") as GameObject;" + "\n"
                        + nameof(DataCounter.CameraObjectsTrs) + ".localPosition = " + bh.playerLocalPosObj.name + i.ToString() + ".transform.localPosition;" + "\n"
                        + nameof(DataCounter.CameraObjectsTrs) + ".localEulerAngles = " + bh.playerLocalPosObj.name + i.ToString() + ".transform.localEulerAngles;" + "\n"
                        + "//カメラリセット回転値設定" + "\n"
                        + "DB." + nameof(DataBridging.cameraObjectsResetLocalEul) + " = " + bh.playerLocalPosObj.name + i.ToString() + ".transform.localEulerAngles;" + "\n"
                        + "\n";
                }

                #endregion
                #region プレイヤーAnchor立ち座り倒れるあれば
                if (bh.playerStandSitFall == RMEventBehaviour.PlayerStandSit.立ち)
                {
                    tmpReadCode +=
                        "\n"
                        + "#region プレイヤー立ち（倒れてたら　倒れから復帰するように立つ）" + "\n"
                        + "if (" + nameof(DataCounter.isPlayerFallDownSystem) + ")" + "\n"
                        + "{" + "\n"
                        + nameof(DataCounter.playerFallDownDefCameraAnchorPos) + " = DB." + nameof(DataBridging.cameraStandAnchorDefLocalPos) + ";" + "\n"
                        + nameof(DataCounter.playerFallDownDefCameraAnchorEul) + " = Vector3.zero;" + "\n"
                        + nameof(DataCounter.isPlayerFallDownSystem) + " = false;" + "\n"
                        + "}" + "\n"
                        + "else//倒れてないなら一瞬で" + "\n"
                        + "{" + "\n"
                        + "//立ちでカメラリセット（ユーザーカメラ動かさない）" + "\n"
                        + nameof(DataCounter.CameraReset) + "(null," + "\n"
                        + "DB." + nameof(DataBridging.cameraStandAnchorDefLocalPos) + "//Anchorを立ちに" + "\n"
                        + ", false, null, false, false);" + "\n"
                        + "}" + "\n"
                        + "#endregion" + "\n"
                        + "\n";

                    #region テキスト雛形
                    //#region プレイヤー立ち（倒れてたら　倒れから復帰するように立つ）
                    //if (isPlayerFallDownSystem)
                    //{
                    //    playerFallDownDefCameraAnchorPos = DB.cameraStandAnchorDefLocalPos;
                    //    playerFallDownDefCameraAnchorEul = Vector3.zero;
                    //    isPlayerFallDownSystem = false;
                    //}
                    //else//倒れてないなら一瞬で
                    //{
                    //    //立ちでカメラリセット（ユーザーカメラ動かさない）
                    //    CameraReset(null,
                    //        DB.cameraStandAnchorDefLocalPos//Anchorを立ちに
                    //        , false, null, false, false);
                    //}
                    //#endregion
                    #endregion
                }
                //座りなら
                else if (bh.playerStandSitFall == RMEventBehaviour.PlayerStandSit.座り)
                {
                    tmpReadCode +=
                        "\n"
                        + "#region プレイヤー座り（倒れてたら　倒れから復帰するように座る）" + "\n"
                        + "if (" + nameof(DataCounter.isPlayerFallDownSystem) + ")" + "\n"
                        + "{" + "\n"
                        + nameof(DataCounter.playerFallDownDefCameraAnchorPos) + " = DB." + nameof(DataBridging.cameraSitAnchorDefLocalPos) + ";" + "\n"
                        + nameof(DataCounter.playerFallDownDefCameraAnchorEul) + " = Vector3.zero;" + "\n"
                        + nameof(DataCounter.isPlayerFallDownSystem) + " = false;" + "\n"
                        + "}" + "\n"
                        + "else//倒れてないなら一瞬で" + "\n"
                        + "{" + "\n"
                        + "//座りでカメラリセット（ユーザーカメラ動かさない）" + "\n"
                        + nameof(DataCounter.CameraReset) + "(null," + "\n"
                        + "DB." + nameof(DataBridging.cameraSitAnchorDefLocalPos) + "//Anchorを座りに" + "\n"
                        + ", false, null, false, false);" + "\n"
                        + "}" + "\n"
                        + "#endregion" + "\n"
                        + "\n";

                    #region テキスト雛形
                    //#region プレイヤー座り（倒れてたら　倒れから復帰するように座る）
                    //if (DC.isPlayerFallDownSystem)
                    //{
                    //    DC.playerFallDownDefCameraAnchorPos = DB.cameraSitAnchorDefLocalPos;
                    //    DC.playerFallDownDefCameraAnchorEul = Vector3.zero;
                    //    DC.isPlayerFallDownSystem = false;
                    //}
                    //else//倒れてないなら一瞬で
                    //{
                    //    //座りでカメラリセット（ユーザーカメラ動かさない）
                    //    DC.CameraReset(null,
                    //        DB.cameraSitAnchorDefLocalPos//Anchorを座りに
                    //        , false, null, false, false);
                    //}
                    //#endregion
                    #endregion
                }
                //倒れる
                else if (bh.playerStandSitFall == RMEventBehaviour.PlayerStandSit.倒れる)
                {
                    tmpReadCode +=
                        "\n"
                        + "//倒れコルーチンスタート" + "\n"
                        + "StartCoroutine(" + nameof(DataCounter.PlayerFallDownSystemIEnum) + "());" + "\n"
                        + "\n";
                }
                #endregion
                #region カメラ揺れあれば
                if (bh.isCameraDOShake)
                {

                    tmpReadCode +=
                        "\n"
                        + "#region カメラ揺れコルーチンスタート" + "\n"
                        + "StartCoroutine(UTLDOShakePosition" + "\n"
                        + "//durationDOShake //コード読み込み用コメント" + "\n"
                        + "(" + bh.durationDOShake.ToString() + "f" + "\n"
                        + "//strengthDOShake //コード読み込み用コメント" + "\n"
                        + ", " + bh.strengthDOShake.ToString() + "f" + "\n"
                        + "//vibratoDOShake //コード読み込み用コメント" + "\n"
                        + ", " + bh.vibratoDOShake.ToString() + "\n"
                        + ", 90 //Randomness" + "\n"
                        + ", false //Snaping" + "\n"
                        + "//fadeOutDOShake //コード読み込み用コメント" + "\n"
                        + ", " + bh.fadeOutDOShake.ToString().ToLower() + ")" + "\n"
                        + ");" + "\n"
                        + "#endregion" + "\n"
                        + "\n";


                    //DC.StartCoroutine(DC.UTLDOShakePosition
                    //    //durationDOShake //コード読み込み用コメント
                    //    (bh.durationDOShake
                    //    //strengthDOShake //コード読み込み用コメント
                    //    , bh.strengthDOShake
                    //    //vibratoDOShake //コード読み込み用コメント
                    //    , bh.vibratoDOShake
                    //    , 90 //Randomness
                    //    , false //Snaping
                    //    //fadeOutDOShake //コード読み込み用コメント
                    //    , bh.fadeOutDOShake)
                    //    );
                }

                #endregion

                #region カメラリセットあれば
                if (bh.isCameraReset)
                {
                    tmpReadCode +=
                        "//強制カメラリセット（トラッキングも）" + "\n"
                        + nameof(DataCounter.CameraReset) + "(null, null, true);" + "\n"
                        + "\n";
                }

                #endregion
                #region 黒フェードあれば
                //フェードイン
                if (bh.fadeBlack == RMEventBehaviour.FadeBlack.IN)
                {
                    tmpReadCode += "FadeBlack(1, " + bh.fadeBlackTime + "f);" + "\n";
                }
                //アウト
                else if (bh.fadeBlack == RMEventBehaviour.FadeBlack.OUT)
                {
                    tmpReadCode += "FadeBlack(0, " + bh.fadeBlackTime + "f);" + "\n";
                }

                #endregion
                #region ちえりPosLock指定あれば
                //True
                if (bh.chieriPosLock == RMEventBehaviour.ChieriPosLock.True)
                {
                    tmpReadCode +=
                        "\n"
                        + "//ちえりPosLockTrue" + "\n"
                        + "DB.isChieriPosLock = true;" + "\n"
                        + "\n";
                }
                //False
                else if (bh.chieriPosLock == RMEventBehaviour.ChieriPosLock.False)
                {
                    tmpReadCode +=
                        "\n"
                        + "//ちえりPosLockFalse" + "\n"
                        + "DB.isChieriPosLock = false;" + "\n"
                        + "\n";
                }

                #endregion
                #region Obj移動 回転 拡縮
                //移動Objと移動先PosObjがあれば
                if (!string.IsNullOrWhiteSpace(bh.moveObjName) && bh.movePosObj != null)
                {
                    //Prefabのパスを取得
                    string path = AssetDatabase.GetAssetPath(bh.movePosObj);
                    //不要部分削除　//Replaceだと、偶然別の消すかもなので、文字数と位置指定削除
                    path = path.Remove(0, "Assets/Resources/".Length);//頭の部分
                    path = path.Remove(path.Length - ".prefab".Length, ".prefab".Length);//お尻部分

                    #region //移動するObjがCameraObjectsやGirlだったら変数名で取得テスト(コード復帰大変そうなので保留)
                    string moveObjTrsStr = "GameObject.Find(\"" + bh.moveObjName + "\").transform";
                    //if (bh.moveObjName == "CameraObjects")
                    //{ moveObjTrsStr = nameof(DataCounter.CameraObjectsTrs); }
                    #endregion

                    tmpReadCode +=
                        "\n"
                        + "#region Obj移動 回転 拡縮" + "\n"
                        + "//Nullチェック" + "\n"
                        + "if (GameObject.Find(\"" + bh.moveObjName.ToString() + "\") != null)" + "\n"
                        + "{" + "\n"
                        + "var tmpObj = GameObject.Find(\"" + bh.moveObjName.ToString() + "\");" + "\n"
                        + "\n"
                        + "GameObject tmpPosObj\n"
                        + "    = Resources.Load(\"" + path + "\") as GameObject;" + "\n"
                        + "\n"
                        + "//Obj移動" + "\n"
                        + "tmpObj.transform.DOLocalMove(" + "\n"
                        + "tmpPosObj.transform.localPosition" + "\n"
                        + ", " + bh.moveTime + "f)" + "\n"
                        + ".SetEase(Ease." + bh.moveEase.ToString() + ");"
                        + "\n"
                        + "//Obj回転" + "\n"
                        + "tmpObj.transform.DOLocalRotate(" + "\n"
                        + "tmpPosObj.transform.localEulerAngles" + "\n"
                        + ", " + bh.moveTime + "f)" + "\n"
                        + ".SetEase(Ease." + bh.moveEase.ToString() + ");"
                        + "\n";

                    //スケールはチェックあれば
                    if (bh.isEnableScale)
                    {
                        tmpReadCode +=
                            "\n"
                            + "//Objスケール" + "\n"
                            + "tmpObj.transform.DOScale(" + "\n"
                            + "tmpPosObj.transform.localScale" + "\n"
                            + ", " + bh.moveTime + "f)" + "\n"
                            + ".SetEase(Ease." + bh.moveEase.ToString() + ");"
                            + "\n";
                    }


                    tmpReadCode +=
                        "\n"
                        + "}" + "\n"
                        + "else" + "\n"
                        + "{ Debug.Log(\"■" + bh.moveObjName + "がヒエラルキーにない？\"" + ");}" + "\n"
                        + "#endregion" + "\n"
                        + "\n";


                    ////Nullチェック
                    //if (GameObject.Find(bh.moveObjName) != null)
                    //{
                    //    var tmpObj = GameObject.Find(bh.moveObjName);
                    //    //Obj移動（Obj名で直接）
                    //    tmpObj.transform.DOLocalMove(
                    //        bh.movePosObj.transform.localPosition
                    //        , bh.moveTime)
                    //        .SetEase(bh.moveEase);

                    //    //Obj回転（Obj名で直接）
                    //    tmpObj.transform.DOLocalRotate(
                    //        bh.movePosObj.transform.localEulerAngles
                    //        , bh.moveTime)
                    //        .SetEase(bh.moveEase);

                    //    //Objスケール（Obj名で直接）
                    //    if (bh.isEnableScale)
                    //    {
                    //        tmpObj.transform.DOScale(
                    //            bh.movePosObj.transform.localScale
                    //            , bh.moveTime)
                    //            .SetEase(bh.moveEase);
                    //    }
                    //}
                    //else
                    //{
                    //    Debug.Log("■" + bh.moveObjName + "がヒエラルキーにない？ moveObjName");
                    //}


                    #region //拡大縮小追加する以前の
                    //tmpReadCode +=
                    //    "\n"
                    //    + "//Obj移動先PosObj取得" + "\n"
                    //    + "GameObject " + bh.movePosObj.name + i.ToString() + "\n" //変数名被りがあるかもなので、連番付与
                    //    + "    = Resources.Load(\"" + path + "\") as GameObject;" + "\n"
                    //    + "//Obj移動（Obj名で直接Find）" + "\n"
                    //    //+ moveObjTrsStr + "\n"
                    //    + "GameObject.Find(\"" + bh.moveObjName + "\").transform" + "\n"
                    //    + ".DOLocalMove(" + bh.movePosObj.name + i.ToString() + ".transform.localPosition" + "\n"
                    //    + ", " + bh.moveTime + "f)" + "\n"
                    //    + ".SetEase(Ease." + bh.moveEase.ToString() + ");"
                    //    + "\n"
                    //    + "\n"
                    //    + "//回転（Obj名で直接Find）" + "\n"
                    //    //+ moveObjTrsStr + "\n"
                    //    + "GameObject.Find(\"" + bh.moveObjName + "\").transform" + "\n"
                    //    + ".DOLocalRotate(" + bh.movePosObj.name + i.ToString() + ".transform.localEulerAngles" + "\n"
                    //    + ", " + bh.moveTime + "f)" + "\n"
                    //    + ".SetEase(Ease." + bh.moveEase.ToString() + ");"
                    //    + "\n";
                    #endregion
                }

                #endregion
                #region 移動ポイント設置起動
                //終了boolあれば終了
                if (bh.isSystemOffMovePoint)
                {
                    tmpReadCode +=
                        "\n"
                        + "//シンプル移動システム終了" + "\n"
                        + "isKO_SimplePointObj_Enter = //念のため到着フラグオフ" + "\n"
                        + "isKOSystem = false;//終了処理時にリストのObj削除" + "\n"
                        + "\n";
                }

                //移動ポイントPosObj
                if (bh.movePointPosObj != null)
                {
                    //同時に終了boolあった場合、2フレ待つ（起動までに２フレ必要）
                    if (bh.isSystemOffMovePoint)
                    {
                        tmpReadCode +=
                            "\n"
                            + "//移動ポイント終了命令あるので2フレ待つ" + "\n"
                            + "yield return null; yield return null;" + "\n"
                            + "\n";
                    }


                    //Prefabのパスを取得
                    string path = AssetDatabase.GetAssetPath(bh.movePointPosObj);
                    //不要部分削除　//Replaceだと、偶然別の消すかもなので、文字数と位置指定削除
                    path = path.Remove(0, "Assets/Resources/".Length);//頭の部分
                    path = path.Remove(path.Length - ".prefab".Length, ".prefab".Length);//お尻部分

                    tmpReadCode +=
                        "\n"
                        + "#region シンプル移動ポイント設置起動" + "\n"
                        + "//移動Obj本体" + "\n"
                        + "GameObject tmpMovePointObj" + i.ToString() + "\n"
                        + "    = Instantiate(ResourceFiles.KO_SimplePointObj" + "\n"
                        + "    , GameObjectsTrs);" + "\n"
                        + "//システム終了時削除するようにリストに入れ" + "\n"
                        + "KO_KakurePosObjsList.Add(tmpMovePointObj" + i.ToString() + ");" + "\n"
                        + "\n"
                        + "//移動Objの位置大きさ" + "\n"
                        + "GameObject " + bh.movePointPosObj.name + i.ToString() + "\n"
                        + "    = Resources.Load(\"" + path + "\") as GameObject;" + "\n"
                        + "tmpMovePointObj" + i.ToString() + ".transform.localPosition = " + bh.movePointPosObj.name + i.ToString() + ".transform.localPosition;"
                        + "tmpMovePointObj" + i.ToString() + ".transform.localEulerAngles = " + bh.movePointPosObj.name + i.ToString() + ".transform.localEulerAngles;"
                        + "tmpMovePointObj" + i.ToString() + ".transform.localScale = " + bh.movePointPosObj.name + i.ToString() + ".transform.localScale;"
                        + "\n"
                        + "//シンプル移動システム起動" + "\n"
                        + "StartCoroutine(KakureOniSimpleSystemLoad());" + "\n"
                        + "//ポイント出現 演出" + "\n"
                        + "KO_NewPosPointObjVis(tmpMovePointObj" + i.ToString() + ");" + "\n"
                        + "#endregion" + "\n"
                        + "\n";


                    //※フラグ書き込み命令があれば
                    if (bh.isUseEnterFlagBool)
                    {


                        tmpReadCode +=
                            "\n"
                            + "#region 到着時にフラグ書き込み（移動ポイント到着イベントでやっているT_KO_SimplePointObj_Enter()）" + "\n"
                            + "//フラグをreserveMovePointEnterFlagBoolListに反映する" + "\n"
                            + "isMovePointFlagReserve = true;//書き込み機能ON" + "\n"
                            + "reserveMovePointEnterFlagBoolList.Clear();//クリアしてAddで同じ内容にする"
                            + "\n";
                        for (int f = 0; f < bh.movePointEnterFlagBoolArray.Length; f++)
                        {
                            tmpReadCode +=
                                "reserveMovePointEnterFlagBoolList.Add(" + bh.movePointEnterFlagBoolArray[f].ToString().ToLower() + ");\n";
                        }

                        tmpReadCode +=
                            "#endregion" + "\n"
                            + "\n";//改行
                    }

                }

                #endregion
                #region SE調整（SEのPrefabが設置されていること前提）
                //SEObj指定があれば
                if (!string.IsNullOrWhiteSpace(bh.SEObjName))
                {
                    tmpReadCode +=
                        "\n"
                        + "//SE用Objを取得して調整" + "\n"
                        + "AudioSource tmpAS" + i.ToString() + " =" + "\n" //変数名被りがあるかもなので、連番付与
                        + "//※SE用Obj名を取得用コメント" + "\n"
                        + "GameObject.Find(\"" + bh.SEObjName + "\").GetComponent<AudioSource>();" + "\n"
                        + "\n"
                        + "//フェード処理" + "\n"
                        + "tmpAS" + i.ToString() + ".DOFade( //※seVolume取得用コメント" + "\n"
                        + bh.SEVolume.ToString() + "f //※seFadeTime取得用コメント" + "\n"
                        + ", " + bh.SEFadeTime.ToString() + "f);" + "\n"
                        + "\n";
                }

                #endregion

                #region UnityTimeline再生停止あれば（停止＝イベント移動）
                //UnityTimelineObj指定があれば
                if (cp.UnityTimelineObj != null)
                {
                    //タイムライン名でラベル指定してある前提で、goto処理
                    tmpReadCode +=
                        "\n"
                        + "//タイムライン移動" + "\n"
                        + "goto " + (cp.UnityTimelineObj.GetComponent<PlayableDirector>().playableAsset as TimelineAsset).name + ";" + "\n"
                        + "\n";

                }
                //イベント移動
                if (cp.isEventMove)
                {
                    tmpReadCode +=
                        "\n"
                        + "//イベント移動（タイムライン終了）" + "\n"
                        + "isPDStopped = true;" + "\n"
                        + "isFlowChartEventMove = true;" + "\n"
                        + "EventMove();" + "\n"
                        + "\n";
                }
                #endregion

                #region ウェイト系あれば（ラストに入れる）

                #region モーションウェイトあったら

                //クリップネーム指定あれば
                if (!string.IsNullOrWhiteSpace(cp.waitMotionClipName))
                {
                    //Obj名指定なければちえりモーションウェイト
                    if (string.IsNullOrWhiteSpace(cp.waitMotionObjName))
                    {
                        tmpReadCode +=
                            "\n"
                            + "#region ちえりのモーション待ち" + "\n"
                            + "yield return null;//再生と同時の場合に備えて1フレ待ち" + "\n"
                            + "StartCoroutine(GirlAnimReadSystem());" + "\n"
                            + "while (nowGirlAnimClipName != \"" + cp.waitMotionClipName + "\") { yield return null; }" + "\n"
                            + "while (girlAnimNomTime <= " + cp.waitMotionNormlizedTime.ToString() + "f) { yield return null; }" + "\n"
                            + "#endregion" + "\n"
                            + "\n";
                        //StartCoroutine(GirlAnimReadSystem());
                        //while (nowGirlAnimClipName != cp.waitMotionClipName) { yield return null; }
                        //while (girlAnimNomTime <= cp.waitMotionNormlizedTime) { yield return null; }
                    }
                    else //obj名指定ありなら、取得してウェイト
                    {
                        tmpReadCode +=
                            "\n"
                            + "#region Obj名指定のモーション待ち" + "\n"
                            + "yield return null;//再生と同時の場合に備えて1フレ待ち" + "\n"
                            + "" + "\n"
                            + "//モーションウェイトするObjからAnim取得" + "\n"
                            + "var tmpAnim" + i.ToString() + " = GameObject.Find(\"" + cp.waitMotionObjName + "\").GetComponent<Animator>();" + "\n"
                            + "" + "\n"
                            + "//■まずクリップ名ウェイト" + "\n"
                            + "string tmpNowClipName" + i.ToString() + " = \"dummy\";" + "\n"
                            + "//現在のアニメーションクリップ名取得" + "\n"
                            + "if (tmpAnim" + i.ToString() + ".GetCurrentAnimatorClipInfo(0).Length != 0)//空の時エラーはかないように" + "\n"
                            + "{ tmpNowClipName" + i.ToString() + " = tmpAnim" + i.ToString() + ".GetCurrentAnimatorClipInfo(0)[0].clip.name; }" + "\n"
                            + "" + "\n"
                            + "while (tmpNowClipName" + i.ToString() + " != \"" + cp.waitMotionClipName + "\")" + "\n"
                            + "{" + "\n"
                            + "//現在のアニメーションクリップ名取得" + "\n"
                            + "if (tmpAnim" + i.ToString() + ".GetCurrentAnimatorClipInfo(0).Length != 0)//空の時エラーはかないように" + "\n"
                            + "{ tmpNowClipName" + i.ToString() + " = tmpAnim" + i.ToString() + ".GetCurrentAnimatorClipInfo(0)[0].clip.name; }" + "\n"
                            + "yield return null;" + "\n"
                            + "}" + "\n"
                            + "" + "\n"
                            + "//■次にノーマライズタイムウェイト" + "\n"
                            + "//ノーマライズタイム取得" + "\n"
                            + "float tmpNowAnimNomTime" + i.ToString() + " = tmpAnim" + i.ToString() + ".GetCurrentAnimatorStateInfo(0).normalizedTime;" + "\n"
                            + "while (tmpNowAnimNomTime" + i.ToString() + " <= " + cp.waitMotionNormlizedTime.ToString() + "f)" + "\n"
                            + "{" + "\n"
                            + "//ノーマライズタイム取得" + "\n"
                            + "tmpNowAnimNomTime" + i.ToString() + " = tmpAnim" + i.ToString() + ".GetCurrentAnimatorStateInfo(0).normalizedTime;" + "\n"
                            + "yield return null;" + "\n"
                            + "}" + "\n"
                            + "#endregion" + "\n"
                            + "\n";


                        ////モーションウェイトするObjからAnim取得
                        //var tmpAnim = GameObject.Find(cp.waitMotionObjName).GetComponent<Animator>();

                        ////■まずクリップ名ウェイト
                        //string tmpNowClipName = "dummy";
                        ////現在のアニメーションクリップ名取得
                        //if (tmpAnim.GetCurrentAnimatorClipInfo(0).Length != 0)//空の時エラーはかないように
                        //{ tmpNowClipName = tmpAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name; }

                        //while (tmpNowClipName != cp.waitMotionClipName)
                        //{
                        //    //現在のアニメーションクリップ名取得
                        //    if (tmpAnim.GetCurrentAnimatorClipInfo(0).Length != 0)//空の時エラーはかないように
                        //    { tmpNowClipName = tmpAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name; }
                        //    yield return null;
                        //}

                        ////■次にノーマライズタイムウェイト
                        ////ノーマライズタイム取得
                        //float tmpNowAnimNomTime = tmpAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        //while (tmpNowAnimNomTime <= cp.waitMotionNormlizedTime)
                        //{
                        //    //ノーマライズタイム取得
                        //    tmpNowAnimNomTime = tmpAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        //    yield return null;
                        //}

                    }
                }

                #endregion


                if (bh.isSerihuKeyWait)
                {
                    tmpReadCode += "yield return new WaitForSeconds(1); yield return KeyOrWait(3);" + "\n";
                }
                //ノベル待ちあれば
                if (bh.isNovelEndWait)
                {
                    tmpReadCode += "while (isNovelSetVisIng) { yield return null; }" + "\n";
                }

                #region 移動ポイントウェイト 到着待ち
                if (bh.isEnterWaitMovePoint)
                {
                    tmpReadCode +=
                        "\n"
                        + "//移動ポイント到着待ち" + "\n"
                        + "while (isKO_SimplePointObj_Enter == false) { yield return null; }" + "\n"
                        + "isKO_SimplePointObj_Enter = false;//到着したのでフラグオフ" + "\n"
                        + "\n";
                }
                #endregion

                #endregion

                #region フラグ判定あれば、}で括る
                //フラグ判定あるなら(編集中　この段階で テキストでcpのフラグを出力する方法で詰まって保留)
                if (cp.isUseFlagBool)
                {
                    tmpReadCode +=
                        "}" + "\n"
                        + "\n";
                }
                #endregion

                //次のクリップへの改行
                tmpReadCode += "//■ClipEnd" + i + "\n\n\n";

            }
            #endregion

            //タイムラインObj名でRegion括る
            tmpReadCode +=
                "#endregion □" + outputTrackList[a].timelineAsset.name
                + "\n";

            //コードAllとコードリストにtmpをコピーして初期化
            readCodeAll += tmpReadCode;
            readCodeList[a] += tmpReadCode;
            tmpReadCode = "";

        }
        #endregion

        //それぞれ上書き保存（されてるように見えても、これしないと、保存して終了しないと消える（tmpキーの移動など））
        //■トラックリスト分 全部
        for (int a = 0; a < outputTrackList.Count; a++)
        {
            EditorUtility.SetDirty(outputTrackList[a]);
        }
        AssetDatabase.SaveAssets();

        Debug.Log("コード生成とテキスト出力終了");

        return;
#endif
    }

    [Button(nameof(DeBuild), "コードからトラックを作成（要開きなおし）")]
    public bool dummy2;//属性でボタンを作るので何かしら宣言が必要。使わない


    List<string> clipDataStrList = new List<string>();

    public void DeBuild()
    {
        //現在のタイムラインにトラック生成
        var newTrack = timelineAsset.CreateTrack<RMEventTrack>(null, "RMEvent Track");
        //編集用にコピー
        var tmpCode = writeCode;

        #region フラグ設定読み込み(トラックフラグの要素数を設定)
        //あれば（今Verでは確定であるが、以前のコード読めるように）
        if (tmpCode.Contains("#region フラグ処理用初期設定"))
        {
            //内容の始まり位置と終わり位置取得(forの回数から要素数取得)
            var tmpFlagStartStr = "codeFlagBoolList = new List<bool>();\nfor (int i = 0; i < ";//長いので変数にした
            var flagDataStartInt = tmpCode.IndexOf(tmpFlagStartStr) + tmpFlagStartStr.Length;
            var flagDataEndLength = tmpCode.IndexOf("; i++)", flagDataStartInt) - flagDataStartInt;

            //抜き出した要素数stringをintに
            var tmpIndexInt = int.Parse(tmpCode.Substring(flagDataStartInt, flagDataEndLength));

            //List初期化(これしないとNull扱いでAddできない)
            (newTrack as RMEventTrack).flagBoolList = new List<bool>();
            //要素数分falseをAdd
            for (int f = 0; f < tmpIndexInt; f++)
            {
                (newTrack as RMEventTrack).flagBoolList.Add(false);
            }
        }

        #endregion

        #region Stringからクリップ範囲ごとにList生成
        clipDataStrList.Clear();

        int tmpOverCounter = 0;//※ 処理が無限ループになった時用
        //■ClipStartが含まれる間は繰り返す
        while (tmpCode.Contains("//■ClipStart") && tmpOverCounter < 999)
        {
            //クリップ内容の始まり位置と終わり位置取得
            var clipStartInt = tmpCode.IndexOf("//■ClipStart");
            var clipEndLength = tmpCode.IndexOf("//■ClipEnd", clipStartInt) - clipStartInt;//長さ。スタートより後ろから　指定位置 - スタート位置までの文字数
            clipEndLength += "//■ClipEnd".Length;//一応■ClipEndも含ませる

            //その部分抜き出してAdd（最初の//■ClipStartは含まれる）
            clipDataStrList.Add(tmpCode.Substring(clipStartInt, clipEndLength));//substring（範囲抽出）は、指定位置から長さを指定する

            //削除
            tmpCode = tmpCode.Remove(clipStartInt, clipEndLength);

            tmpOverCounter++;
        }
        if (tmpOverCounter >= 999) { Debug.Log("クリップ数多すぎるか、無限ループしている"); }
        #endregion

        #region ■リストから一個ずつClip生成
        for (int i = 0; i < clipDataStrList.Count; i++)
        {
            //クリップ作成
            var clip = newTrack.CreateDefaultClip();
            //名前
            clip.displayName = nameof(RMEventClip) + " " + i;
            //クリップの変数用behaviour読み込み
            var bh = (clip.asset as RMEventClip).m_behaviour; // クリップが持つBehaviourを参照（パラメータ）
            var cp = (clip.asset as RMEventClip); // クリップを参照（パラメータ）

            #region クリップスタート位置
            //内容の始まり位置と終わり位置取得
            var dataStartInt = clipDataStrList[i].IndexOf("//clipStartTime ") + "//clipStartTime ".Length;
            var dataEndLength = clipDataStrList[i].IndexOf("\n", dataStartInt) - dataStartInt;//長さ。スタートより後ろから　指定位置 - スタート位置までの文字数

            //double型で代入
            clip.start = double.Parse(clipDataStrList[i].Substring(dataStartInt, dataEndLength));//substring（範囲抽出）は、指定位置から長さを指定する
            #endregion

            #region クリップ長さ（Duration　end位置は読み取り専用だった）
            //内容の始まり位置と終わり位置取得
            dataStartInt = clipDataStrList[i].IndexOf("//clipDurationTime ") + "//clipDurationTime ".Length;
            dataEndLength = clipDataStrList[i].IndexOf("\n/", dataStartInt) - dataStartInt;

            //double型で代入
            clip.duration = double.Parse(clipDataStrList[i].Substring(dataStartInt, dataEndLength));
            #endregion

            #region フラグ要素数は判明しているので 要素数セット
            Array.Resize(ref cp.flagBoolArray, flagBoolList.Count);
            Array.Resize(ref cp.aSentakuflagBoolArray, flagBoolList.Count);
            Array.Resize(ref cp.bSentakuflagBoolArray, flagBoolList.Count);
            Array.Resize(ref cp.m_behaviour.movePointEnterFlagBoolArray, flagBoolList.Count);

            #endregion


            #region フラグ設定あれば
            if (clipDataStrList[i].Contains("#region ■フラグ判定"))
            {
                //まずフラグ設定ON
                cp.isUseFlagBool = true;

                //■フラグ要素数分Addが並んでいるので、一行ずつ処理
                //内容の始まり位置と終わり位置取得
                var startInt = clipDataStrList[i].IndexOf("//cpのフラグ内容コピー開始\n") + "//cpのフラグ内容コピー開始\n".Length;
                var endLength = clipDataStrList[i].IndexOf("\n//cpのフラグ内容コピー終了", startInt) - startInt;//長さ。スタートより後ろから　指定位置 - スタート位置までの文字数

                //行ごとに配列化(「clipFlagBoolArray[0] = true;」で1行 という感じ)
                string[]
                    tmpDataArray = clipDataStrList[i].Substring(startInt, endLength)
                    .Split(new string[] { "\n" }, StringSplitOptions.None);

                //代入(trueがあればtrue)
                for (int f = 0; f < cp.flagBoolArray.Length; f++)
                {
                    if (tmpDataArray[f].Contains("true"))
                    {
                        cp.flagBoolArray[f] = true;
                    }
                }

            }

            #endregion

            #region フキダシ
            //まずあるかないか
            if (clipDataStrList[i].Contains(nameof(DataCounter.Hukidashi)))
            {
                //内容の始まり位置と終わり位置取得
                var tmpStartStr = nameof(DataCounter.Hukidashi) + "(\"";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("\");", dataStartInt) - dataStartInt;

                //代入
                bh.serihuKey = clipDataStrList[i].Substring(dataStartInt, dataEndLength);
            }
            //フキダシキーウェイトあるかないか
            if (clipDataStrList[i].Contains("yield return new WaitForSeconds(1); yield return KeyOrWait(3);"))
            {
                //あるならON
                bh.isSerihuKeyWait = true;
            }
            #endregion
            #region ノベル
            //まずあるかないか
            if (clipDataStrList[i].Contains(nameof(DataCounter.NovelSetVis) + "(\"")) //isnovelsetVisIngと被るので + "(\""
            {
                //内容の始まり位置と終わり位置取得
                var tmpStartStr = nameof(DataCounter.NovelSetVis) + "(\"";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("\"", dataStartInt) - dataStartInt;

                //代入
                bh.novelKey = clipDataStrList[i].Substring(dataStartInt, dataEndLength);



                //■オートかどうか
                //内容の始まり位置と終わり位置取得
                tmpStartStr = nameof(DataCounter.NovelSetVis) + "(\"";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf(");", dataStartInt) - dataStartInt;

                if (clipDataStrList[i].Substring(dataStartInt, dataEndLength).Contains("\", true"))
                {
                    //代入
                    bh.isNovelAuto = true;
                }

            }
            //ノベル終了ウェイトあるかないか
            if (clipDataStrList[i].Contains("while (isNovelSetVisIng) { yield return null; }"))
            {
                //あるならON
                bh.isNovelEndWait = true;
            }
            #endregion
            #region モーション（表情含む）

            #region 先にObj名指定でのアニメーションがあれば取得
            //まずあるかないか
            if (clipDataStrList[i].Contains("//オブジェ名からAnimator取得"))
            {
                #region MotionObjName
                //内容の始まり位置と終わり位置取得
                var tmpStartStr = "//オブジェ名からAnimator取得" + "\n" + "GameObject.Find(\"";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("\").GetComponent<Animator>()", dataStartInt) - dataStartInt;

                //代入
                bh.motionObjName = clipDataStrList[i].Substring(dataStartInt, dataEndLength);
                #endregion

                #region motionStateName
                //内容の始まり位置と終わり位置取得
                tmpStartStr = "//オブジェ指定モーション名取得用コメント" + "\n" + "\"";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("\"", dataStartInt) - dataStartInt;

                //代入
                bh.motionStateName = clipDataStrList[i].Substring(dataStartInt, dataEndLength);
                #endregion

                #region motionCrossFadeTime
                //内容の始まり位置と終わり位置取得
                tmpStartStr = "//オブジェ指定モーションタイム取得用コメント" + "\n" + ", ";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("f" + "\n", dataStartInt) - dataStartInt;

                //代入
                bh.motionCrossFadeTime = float.Parse(clipDataStrList[i].Substring(dataStartInt, dataEndLength));
                #endregion
            }

            #endregion

            //■ここから従来のちえりモーションの処理
            //まずコピー（モーションと表情同時にあるかも知れないため、あるだけ消しながら代入する）
            var tmpStr = clipDataStrList[i];

            まだあるかどうか:
            if (tmpStr.Contains(nameof(DataCounter.ChieriMotion)))
            {

                #region ステート名始まり位置と終わり位置取得して 仮ステート名取得
                var tmpStartStr = nameof(DataCounter.ChieriMotion) + "(\"";//長いので変数に
                dataStartInt = tmpStr.IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = tmpStr.IndexOf("\", ", dataStartInt) - dataStartInt;

                //■ステート名仮取得
                var tmpStateName = tmpStr.Substring(dataStartInt, dataEndLength);
                #endregion

                #region CrossFadeTime内容始まり位置と終わり位置取得して 仮CrossDadeTime取得
                tmpStartStr = nameof(DataCounter.ChieriMotion) + "(\"" + tmpStateName + "\", ";//長いので変数に
                dataStartInt = tmpStr.IndexOf(tmpStartStr) + tmpStartStr.Length;

                //fがあるかどうか
                bool isF = false;
                //末尾まで一旦読み込んで
                dataEndLength = tmpStr.IndexOf("\n", dataStartInt) - dataStartInt;
                if (tmpStr.Substring(dataStartInt, dataEndLength).Contains("f"))
                {
                    //fあるなら fあること前提での終わり位置取得
                    dataEndLength = tmpStr.IndexOf("f, ", dataStartInt) - dataStartInt;
                    isF = true;
                }
                else
                {
                    //ないなら ないこと前提での終わり位置取得
                    dataEndLength = tmpStr.IndexOf(", ", dataStartInt) - dataStartInt;
                }

                //■クロスフェードタイム仮取得
                var tmpCrossFadeTime = float.Parse(tmpStr.Substring(dataStartInt, dataEndLength));
                #endregion

                #region モーションのレイヤー番号始まり位置と終わり位置取得して■本代入
                if (isF)
                {
                    tmpStartStr = nameof(DataCounter.ChieriMotion) + "(\"" + tmpStateName + "\", " + tmpCrossFadeTime.ToString()
                        + "f, ";//長いので変数に
                }
                else
                {
                    tmpStartStr = nameof(DataCounter.ChieriMotion) + "(\"" + tmpStateName + "\", " + tmpCrossFadeTime.ToString()
                        + ", ";//長いので変数に
                }

                dataStartInt = tmpStr.IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = tmpStr.IndexOf(");", dataStartInt) - dataStartInt;

                //0ならモーションに代入
                if (int.Parse(tmpStr.Substring(dataStartInt, dataEndLength)) == 0)
                {
                    bh.motionStateName = tmpStateName;
                    bh.motionCrossFadeTime = tmpCrossFadeTime;
                }
                //2なら表情に代入
                else if (int.Parse(tmpStr.Substring(dataStartInt, dataEndLength)) == 2)
                {
                    bh.faceStateName = tmpStateName;
                    bh.faceCrossFadeTime = tmpCrossFadeTime;
                }
                #endregion

                #region その部分削除（ChieriMotion("　　から末尾まで）
                tmpStartStr = nameof(DataCounter.ChieriMotion) + "(\"";//長いので変数に
                dataStartInt = tmpStr.IndexOf(tmpStartStr);
                dataEndLength = tmpStr.IndexOf("\n", dataStartInt) - dataStartInt;

                tmpStr = tmpStr.Remove(dataStartInt, dataEndLength);
                #endregion

                goto まだあるかどうか;
            }
            #endregion
            #region IKLookAtPlayer見る
            //あるかないか
            if (clipDataStrList[i].Contains("#region IKLookAtプレイヤー見る"))
            {
                //あるなら
                bh.iKLookAtPlayer = RMEventBehaviour.IKLookAtPlayer.プレイヤー見る;
            }
            else if (clipDataStrList[i].Contains("#region IKLookAt解除"))
            {
                //あるならON
                bh.iKLookAtPlayer = RMEventBehaviour.IKLookAtPlayer.解除;
            }
            #endregion
            #region PrefabをGameobjectsに設置
            //まずあるかないか
            if (clipDataStrList[i].Contains("//PrefabをGameobjectsに設置"))
            {
                #region spawnObj取得して代入
                //変数に連番をつけているため、パス直前まで確定で同じ文としてスタート位置指定ができない。
                //そのため、範囲で抜き出してそこから("")で囲まれてる場所を抜き出す。（パスにしか("")は使われてない）
                var tmpStartStr = "//PrefabをGameobjectsに設置" + "\n"
                    + "GameObject ";

                //("")で囲まれたパスが含まれる始まり位置と終わり位置取得
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("as GameObject", dataStartInt) - dataStartInt;

                //↑部分抜き出して変数化
                var tmpDataStr = clipDataStrList[i].Substring(dataStartInt, dataEndLength);

                //(" ")で抜き出す
                tmpStartStr = "(\"";
                //パスの始まり位置と終わり位置取得
                dataStartInt = tmpDataStr.IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = tmpDataStr.IndexOf("\")", dataStartInt) - dataStartInt;

                var path = tmpDataStr.Substring(dataStartInt, dataEndLength);

                //代入
                bh.spawnObj = Resources.Load(path) as GameObject;
                #endregion
                #region spawnObjName取得して代入
                //内容の始まり位置と終わり位置取得
                tmpStartStr = " //ヒエラルキーに置く際のObj名（このコメントはコードから取得用のもの）\n= \"";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("\";", dataStartInt) - dataStartInt;

                //代入
                bh.spawnObjName = clipDataStrList[i].Substring(dataStartInt, dataEndLength);

                #endregion
            }
            #endregion
            #region 削除destroyObjNameあれば
            //まずあるかないか
            if (clipDataStrList[i].Contains("//ヒエラルキーからObj削除（Obj名で直接）"))
            {
                //内容の始まり位置と終わり位置取得
                var tmpStartStr = "//ヒエラルキーからObj削除（Obj名で直接）\nDestroy(GameObject.Find(\"";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("\"));", dataStartInt) - dataStartInt;

                //代入
                bh.destroyObjName = clipDataStrList[i].Substring(dataStartInt, dataEndLength);
            }
            #endregion
            #region ペアレント
            //まずあるかないか
            if (clipDataStrList[i].Contains("//ペアレント（Obj名で直接）"))
            {
                //内容の始まり位置と終わり位置取得 (子が先)
                var tmpStartStr = "//ペアレント（Obj名で直接）\nGameObject.Find(\"";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("\").transform", dataStartInt) - dataStartInt;

                //代入
                bh.ChildObjName = clipDataStrList[i].Substring(dataStartInt, dataEndLength);

                //内容の始まり位置と終わり位置取得 (親)
                tmpStartStr = ".SetParent(GameObject.Find(\"";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("\").transform", dataStartInt) - dataStartInt;

                //代入
                bh.ParentObjName = clipDataStrList[i].Substring(dataStartInt, dataEndLength);
            }
            #endregion
            #region RigidBodyのisKinematic
            //あるかどうか
            if (clipDataStrList[i].Contains("//FindしてRigidbodyのisKinematicを設定"))
            {
                //内容の始まり位置と終わり位置取得 Obj名
                var tmpStartStr = "//FindしてRigidbodyのisKinematicを設定" + "\n" + "GameObject.Find(\"";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("\").GetComponent<Rigidbody>()", dataStartInt) - dataStartInt;

                //代入
                bh.rigidbodyObjName = clipDataStrList[i].Substring(dataStartInt, dataEndLength);



                //内容の始まり位置と終わり位置取得 bool
                tmpStartStr = ".isKinematic = ";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf(";", dataStartInt) - dataStartInt;

                //代入(boolどちらも処理する可能性あるので、ifで両対応(falseなら元々falseなのでなにもしない))
                if ("true" == clipDataStrList[i].Substring(dataStartInt, dataEndLength))
                { bh.isKinematic = true; }

            }
            #endregion
            #region プレイヤー位置
            //まずあるかないか（他にもGameObjectを使うかも知れないので、コメントで検索）
            if (clipDataStrList[i].Contains("//プレイヤー位置指定"))
            {
                //変数に連番をつけているため、パス直前まで確定で同じ文としてスタート位置指定ができない。
                //そのため、範囲で抜き出してそこから("")で囲まれてる場所を抜き出す。（パスにしか("")は使われてない）
                var tmpStartStr = "//プレイヤー位置指定" + "\n"
                    + "GameObject ";

                //("")で囲まれたパスが含まれる始まり位置と終わり位置取得
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf(" as GameObject;", dataStartInt) - dataStartInt;

                //↑部分抜き出して変数化
                var tmpDataStr = clipDataStrList[i].Substring(dataStartInt, dataEndLength);

                //(" ")で抜き出す
                tmpStartStr = "(\"";
                //パスの始まり位置と終わり位置取得
                dataStartInt = tmpDataStr.IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = tmpDataStr.IndexOf("\")", dataStartInt) - dataStartInt;

                var path = tmpDataStr.Substring(dataStartInt, dataEndLength);

                //代入
                bh.playerLocalPosObj = Resources.Load(path) as GameObject;
            }

            #endregion
            #region プレイヤー立ち座り
            //プレイヤー立ちあるかないか
            if (clipDataStrList[i].Contains("#region プレイヤー立ち（倒れてたら　倒れから復帰するように立つ）"))
            {
                //あるならON
                bh.playerStandSitFall = RMEventBehaviour.PlayerStandSit.立ち;
            }
            //座りあるか
            else if (clipDataStrList[i].Contains("#region プレイヤー座り（倒れてたら　倒れから復帰するように座る）"))
            {
                //あるならON
                bh.playerStandSitFall = RMEventBehaviour.PlayerStandSit.座り;
            }
            //倒れか
            else if (clipDataStrList[i].Contains("//倒れコルーチンスタート"))
            {
                //あるならON
                bh.playerStandSitFall = RMEventBehaviour.PlayerStandSit.倒れる;
            }

            #endregion
            #region カメラリセット
            //あるか
            if (clipDataStrList[i].Contains("//強制カメラリセット（トラッキングも）"))
            {
                bh.isCameraReset = true;
            }
            #endregion
            #region 黒フェード
            //黒フェードインあるかないか
            if (clipDataStrList[i].Contains(nameof(DataCounter.FadeBlack) + "(1, "))
            {
                //内容の始まり位置と終わり位置取得
                var tmpStartStr = nameof(DataCounter.FadeBlack) + "(1, ";//長いので変数
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf(");", dataStartInt) - dataStartInt;

                //代入 //Floatにしつつ、f入ってたら削除
                bh.fadeBlackTime = float.Parse(clipDataStrList[i].Substring(dataStartInt, dataEndLength).Replace("f", ""));

                //フェードイン
                bh.fadeBlack = RMEventBehaviour.FadeBlack.IN;
            }
            //フェードアウト
            else if (clipDataStrList[i].Contains(nameof(DataCounter.FadeBlack) + "(0, "))
            {
                //内容の始まり位置と終わり位置取得
                var tmpStartStr = nameof(DataCounter.FadeBlack) + "(0, ";//長いので変数
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf(");", dataStartInt) - dataStartInt;

                //代入 //Floatにしつつ、f入ってたら削除
                bh.fadeBlackTime = float.Parse(clipDataStrList[i].Substring(dataStartInt, dataEndLength).Replace("f", ""));

                //フェードアウト
                bh.fadeBlack = RMEventBehaviour.FadeBlack.OUT;
            }

            #endregion
            #region ちえりPosLock
            //あるかないか
            if (clipDataStrList[i].Contains("//ちえりPosLock"))
            {
                //True
                if (clipDataStrList[i].Contains("//ちえりPosLockTrue"))
                {
                    bh.chieriPosLock = RMEventBehaviour.ChieriPosLock.True;
                }
                //False
                else if (clipDataStrList[i].Contains("//ちえりPosLockFalse"))
                {
                    bh.chieriPosLock = RMEventBehaviour.ChieriPosLock.False;
                }
            }

            #endregion
            #region Obj移動 回転 拡縮
            //まずあるかないか
            if (clipDataStrList[i].Contains("//Obj移動先PosObj取得"))
            {
                #region movePosObjのパス取得して代入
                //変数に連番をつけているため、パス直前まで確定で同じ文としてスタート位置指定ができない。
                //そのため、範囲で抜き出してそこから("")で囲まれてる場所を抜き出す。（パスにしか("")は使われてない）
                var tmpStartStr = "//Obj移動先PosObj取得" + "\n"
                    + "GameObject ";

                //("")で囲まれたパスが含まれる始まり位置と終わり位置取得
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf(" as GameObject;", dataStartInt) - dataStartInt;

                //↑部分抜き出して変数化
                var tmpDataStr = clipDataStrList[i].Substring(dataStartInt, dataEndLength);

                //(" ")で抜き出す
                tmpStartStr = "(\"";
                //パスの始まり位置と終わり位置取得
                dataStartInt = tmpDataStr.IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = tmpDataStr.IndexOf("\")", dataStartInt) - dataStartInt;

                var path = tmpDataStr.Substring(dataStartInt, dataEndLength);

                //代入
                bh.movePosObj = Resources.Load(path) as GameObject;
                #endregion

                #region moveObjName取得
                //内容の始まり位置と終わり位置取得
                tmpStartStr = "//Obj移動（Obj名で直接Find）\nGameObject.Find(\"";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("\").transform", dataStartInt) - dataStartInt;

                //代入
                bh.moveObjName = clipDataStrList[i].Substring(dataStartInt, dataEndLength);
                #endregion

                #region moveTime取得
                //内容始まり位置と終わり位置取得して
                tmpStartStr = "transform.localPosition\n, ";//長いので変数に
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf(")\n.SetEase", dataStartInt) - dataStartInt;

                //moveTime代入 //fがあったら削除して代入
                bh.moveTime
                    = float.Parse(clipDataStrList[i].Substring(dataStartInt, dataEndLength)
                    .Replace("f", ""));
                #endregion

                #region Ease取得
                //内容の始まり位置と終わり位置取得 (Ease)
                tmpStartStr = ".SetEase(Ease.";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf(");", dataStartInt) - dataStartInt;

                //stringをenumに変換
                var tmpEaseStr = clipDataStrList[i].Substring(dataStartInt, dataEndLength);
                var enumVal =
                    (DG.Tweening.Ease)System.Enum.Parse(typeof(DG.Tweening.Ease)
                    , tmpEaseStr
                    , true);//大文字小文字区別しないかどうか

                //代入
                bh.moveEase = enumVal;

                #endregion
            }
            #endregion
            #region シンプル移動ポイント設置起動
            //まずあるかないか
            if (clipDataStrList[i].Contains("#region シンプル移動ポイント設置起動"))
            {
                #region movePointPosObjのパス取得して代入
                //変数に連番をつけているため、パス直前まで確定で同じ文としてスタート位置指定ができない。
                //そのため、範囲で抜き出してそこから("")で囲まれてる場所を抜き出す。（パスにしか("")は使われてない）
                var tmpStartStr = "//移動Objの位置大きさ" + "\n"
                    + "GameObject ";

                //("")で囲まれたパスが含まれる始まり位置と終わり位置取得
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("as GameObject;", dataStartInt) - dataStartInt;

                //↑部分抜き出して変数化
                var tmpDataStr = clipDataStrList[i].Substring(dataStartInt, dataEndLength);

                //(" ")で抜き出す
                tmpStartStr = "(\"";
                //パスの始まり位置と終わり位置取得
                dataStartInt = tmpDataStr.IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = tmpDataStr.IndexOf("\")", dataStartInt) - dataStartInt;

                var path = tmpDataStr.Substring(dataStartInt, dataEndLength);

                //代入
                bh.movePointPosObj = Resources.Load(path) as GameObject;
                #endregion
            }
            //到着待ちあれば
            if (clipDataStrList[i].Contains("//移動ポイント到着待ち"))
            {
                bh.isEnterWaitMovePoint = true;
            }
            //終了boolあれば
            if (clipDataStrList[i].Contains("isKOSystem = false;//終了処理時にリストのObj削除"))
            {
                bh.isSystemOffMovePoint = true;
            }
            #endregion
            #region SE調整（SEのPrefabが設置されていること前提）
            //まずあるかないか
            if (clipDataStrList[i].Contains("//SE用Objを取得して調整"))
            {
                #region seObjName取得
                //内容の始まり位置と終わり位置取得
                var tmpStartStr = "//※SE用Obj名を取得用コメント\nGameObject.Find(\"";//長いので変数にした
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf("\")", dataStartInt) - dataStartInt;

                //代入
                bh.SEObjName = clipDataStrList[i].Substring(dataStartInt, dataEndLength);
                #endregion

                #region seVolume取得

                //内容始まり位置と終わり位置取得して
                tmpStartStr = "//※seVolume取得用コメント\n";//長いので変数に
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf(" //", dataStartInt) - dataStartInt;

                //seVolume代入 //fがあったら削除して代入
                bh.SEVolume
                    = float.Parse(clipDataStrList[i].Substring(dataStartInt, dataEndLength)
                    .Replace("f", ""));
                #endregion

                #region seFadeTime取得
                //内容始まり位置と終わり位置取得して
                tmpStartStr = "//※seFadeTime取得用コメント\n, ";//長いので変数に
                dataStartInt = clipDataStrList[i].IndexOf(tmpStartStr) + tmpStartStr.Length;
                dataEndLength = clipDataStrList[i].IndexOf(");", dataStartInt) - dataStartInt;

                //seVolume代入 //fがあったら削除して代入
                bh.SEFadeTime
                    = float.Parse(clipDataStrList[i].Substring(dataStartInt, dataEndLength)
                    .Replace("f", ""));
                #endregion
            }
            #endregion

        }
        #endregion

        Debug.Log("コードからトラックを作成");

        //表示更新されないので試したがだめだった
        //UnityEditor.AssetDatabase.SaveAssets();
        //UnityEditor.AssetDatabase.Refresh();
    }

    #endregion




    #region ■全クリップの内容テキストで一覧出力

    [SerializeField, Space(40)]
    [Button(nameof(TextOutput), "全クリップの内容テキストで一覧出力")]
    public bool dummy3;//属性でボタンを作るので何かしら宣言が必要。使わない

    public void TextOutput()
    {
#if UNITY_EDITOR
        //テキストクリア
        allClipText = nowClipText = "";

        //■比較用にcpとbhのデフォ変数一覧生成
        RMEventClip defCp = new RMEventClip();
        FieldInfo[] defCpInfoArray = defCp.GetType().GetFields();
        RMEventBehaviour defBh = new RMEventBehaviour();
        FieldInfo[] defBhInfoArray = defBh.GetType().GetFields();

        //■このトラックのクリップ全部読み込み
        var tmpClipList = GetClips().ToList();
        //デリゲートでクリップをソート（スタートタイム順にする）
        tmpClipList.Sort((o1, o2) =>
        {
            if (o1.start < o2.start) return -1;
            if (o1.start > o2.start) return 1;
            return 0;
        });


        //■トラックの全クリップで処理
        for (int i = 0; i < tmpClipList.Count; i++)
        {

            //■クリップの変数読み込み
            var nowCp = (tmpClipList[i].asset as RMEventClip); //クリップのみ
            FieldInfo[] nowCpInfoArray = nowCp.GetType().GetFields();

            //■クリップの位置や名前出力
            allClipText += "■＝＝＝＝＝clip" + i + " " + tmpClipList[i].displayName + " " + (tmpClipList[i].start * 30);
            allClipText += "\n";//改行

            #region ■クリップのフィールド変数 1項目ずつ処理

            for (int k = 0; k < defCpInfoArray.Length; k++)
            {
                var tmpDef = defCpInfoArray[k].GetValue(defCp);
                var tmpNow = nowCpInfoArray[k].GetValue(nowCp);

                #region //参考 objectの配列から要素数など抽出
                //if (k == 46)//この時は一括モーションの配列だった
                //{
                //    Debug.Log(defCpInfoArray[k]);
                //    var a = tmpNow;
                //    var type = a.GetType();
                //    var props = type.GetProperties();
                //    var pairs = props.Select(x => x.Name + " = " + x.GetValue(a, null)).ToArray();
                //    var result = string.Join("\n", pairs);
                //    Debug.Log(result);
                //}
                #endregion

                //■比較して、デフォルトと違う（何か変更がある）場合はテキストに出力
                if (!Equals(tmpDef, tmpNow))
                {
                    #region ■無視リスト
                    if (nowCpInfoArray[k].Name == nameof(nowCp.m_behaviour)
                        || nowCpInfoArray[k].Name == nameof(nowCp.autoTextOutput)
                        || nowCpInfoArray[k].Name == nameof(nowCp.flagBoolArray)
                        || nowCpInfoArray[k].Name == nameof(nowCp.aSentakuflagBoolArray)
                        || nowCpInfoArray[k].Name == nameof(nowCp.bSentakuflagBoolArray)
                        || nowCpInfoArray[k].Name == nameof(nowCp.debugFlagBoolArray)
                        )
                    {
                        continue;
                    }

                    #endregion

                    #region ■まず、Nowの内容がArrayで、空じゃなかったらもう書き出して抜ける
                    if (tmpNow.GetType().IsArray)
                    {
                        //配列数1以上だったら出力（コピペでほぼ理解できてないけど、これで配列数が取得できた）
                        var type = tmpNow.GetType(); //型取得
                        var props = type.GetProperties(); //プロパティ取得（この場合取得してるのは配列としてのデータ（モーションの内容とかではない））
                        long tmpLength = (long)props[0].GetValue(tmpNow); //そのプロパティ（0番目はLongLength）が、tmpNowではいくつに設定されているか取得

                        if (tmpLength >= 1)
                        {
                            //変数名出力
                            nowClipText += nowCpInfoArray[k].Name + ":";
                            //要素数出力
                            nowClipText += tmpLength;
                            nowClipText += "\n";//改行
                        }
                        continue;
                    }
                    #endregion

                    #region ■Nowの内容が、Nullじゃない　かつ　空白じゃない　であれば書き出し（Nullと空白以外の何かであれば） 
                    if (tmpNow != null
                        && !Equals(tmpNow, "")
                        && !Equals(tmpNow, "null")
                        )
                    {
                        //更に、型がGameObjectだった場合は
                        if (tmpNow is GameObject)
                        {
                            //nullじゃなければ書き出す
                            if ((GameObject)tmpNow != null)
                            {
                                nowClipText += nowCpInfoArray[k].Name + ":" + nowCpInfoArray[k].GetValue(nowCp);
                                nowClipText += "\n";//改行
                            }
                        }
                        //型がGameObjectじゃないならすぐ書き出す
                        else
                        {
                            //UserMemoはAllにのみ
                            if (nowCpInfoArray[k].Name == nameof(nowCp.userMemo))
                            {
                                allClipText += nowCpInfoArray[k].Name + ":" + nowCpInfoArray[k].GetValue(nowCp);
                                allClipText += "\n";//改行
                            }
                            else
                            {
                                nowClipText += nowCpInfoArray[k].Name + ":" + nowCpInfoArray[k].GetValue(nowCp);
                                nowClipText += "\n";//改行
                            }
                        }
                    }
                    #endregion
                }

            }
            #endregion

            #region ■↑のBh版 (クリップに内包されているのでこの位置)

            //■このクリップのビヘイビアの変数情報読み込み
            var nowBh = (tmpClipList[i].asset as RMEventClip).m_behaviour; // クリップが持つBehaviourを参照（パラメータ）
            FieldInfo[] nowBhInfoArray = nowBh.GetType().GetFields();

            //ビヘイビアのフィールド変数 1項目ずつ処理
            for (int k = 0; k < defBhInfoArray.Length; k++)
            {
                var tmpDef = defBhInfoArray[k].GetValue(defBh);
                var tmpNow = nowBhInfoArray[k].GetValue(nowBh);

                //■比較して、デフォルトと違う（何か変更がある）場合はテキストに出力
                if (!Equals(tmpDef, tmpNow))
                {
                    #region ■無視リスト
                    if (nowBhInfoArray[k].Name == nameof(nowBh.destroyObjList) //Hideで廃止済み 念の為処理だけはされるように残し
                        || nowBhInfoArray[k].Name == nameof(nowBh.isClipEnter)
                        || nowBhInfoArray[k].Name == nameof(nowBh.ReadParentObj)
                        || nowBhInfoArray[k].Name == nameof(nowBh.ReadChildObj)
                        || nowBhInfoArray[k].Name == nameof(nowBh.readMoveObj)
                        || nowBhInfoArray[k].Name == nameof(nowBh.movePointEnterFlagBoolArray)
                        )
                    {
                        continue;
                    }

                    #endregion

                    #region ■まず、Nowの内容がArrayで、空じゃなかったらもう書き出して抜ける
                    if (tmpNow.GetType().IsArray)
                    {
                        //配列数1以上だったら出力（コピペでほぼ理解できてないけど、これで配列数が取得できた）
                        var type = tmpNow.GetType(); //型取得
                        var props = type.GetProperties(); //プロパティ取得（この場合取得してるのは配列としてのデータ（モーションの内容とかではない））
                        long tmpLength = (long)props[0].GetValue(tmpNow); //そのプロパティ（0番目はLength）が、tmpNowではいくつに設定されているか取得

                        if (tmpLength >= 1)
                        {
                            //変数名出力
                            nowClipText += nowBhInfoArray[k].Name + ":";
                            //要素数出力
                            nowClipText += tmpLength;
                            nowClipText += "\n";//改行
                        }
                        continue;
                    }
                    #endregion

                    #region ■Nowの内容が、Nullじゃない　かつ　空白じゃない　であれば書き出し（未入力はそもそも弾く というやり方） 
                    if (tmpNow != null
                        && !Equals(tmpNow, "")
                        && !Equals(tmpNow, "null")
                        )
                    {
                        //更に、型がGameObjectだった場合は
                        if (tmpNow is GameObject)
                        {
                            //nullじゃなければ書き出す
                            if ((GameObject)tmpNow != null)
                            {
                                nowClipText += nowBhInfoArray[k].Name + ":" + nowBhInfoArray[k].GetValue(nowBh);
                                nowClipText += "\n";//改行
                            }
                        }
                        //型がGameObjectじゃないならすぐ
                        else
                        {
                            nowClipText += nowBhInfoArray[k].Name + ":" + nowBhInfoArray[k].GetValue(nowBh);
                            nowClipText += "\n";//改行
                        }
                    }
                    #endregion

                }

            }
            #endregion

            //nowClipTextをAllClipTextへ
            allClipText += nowClipText;

            //クリップにnowClipTextを貼り付け
            nowCp.autoTextOutput = nowClipText;

            //nowClipTextをクリア
            nowClipText = "";

            //次のクリップに移る前に改行
            allClipText += "\n";


        }
#endif
    }

    [Multiline]
    public string
        allClipText;
    string
        nowClipText;
    #endregion

    [SerializeField, Space(40)]
    [Button(nameof(DataMove), "一括Obj削除へ移設")]
    public bool dummy4;//属性でボタンを作るので何かしら宣言が必要。使わない

    void DataMove()
    {
        //■このトラックのクリップ全部読み込み
        var tmpClipList = GetClips().ToList();
        //デリゲートでクリップをソート（スタートタイム順にする）
        tmpClipList.Sort((o1, o2) =>
        {
            if (o1.start < o2.start) return -1;
            if (o1.start > o2.start) return 1;
            return 0;
        });


        //■トラックの全クリップで処理
        for (int i = 0; i < tmpClipList.Count; i++)
        {
            //■クリップの変数読み込み
            var nowCp = (tmpClipList[i].asset as RMEventClip); //クリップのみ

            //■このクリップのビヘイビアの変数情報読み込み
            var nowBh = (tmpClipList[i].asset as RMEventClip).m_behaviour; // クリップが持つBehaviourを参照（パラメータ）


            //実際の処理

            //Hideで廃止したのでコメントアウト
            ////■BhのDestroyObjListの内容を、Cpの一括Obj削除へ
            //if (nowBh.destroyObjList.Count > 0)
            //{
            //    for (int k = 0; k < nowBh.destroyObjList.Count; k++)
            //    {
            //        string[] result = new string[nowCp.destroyObjArray.Length + 1];
            //        Array.Copy(nowCp.destroyObjArray, result, nowCp.destroyObjArray.Length);
            //        result[nowCp.destroyObjArray.Length] = nowBh.destroyObjList[k];
            //        nowCp.destroyObjArray = result;

            //        Debug.Log((k + 1) + "/" + nowBh.destroyObjList.Count
            //            + "■一括Obj削除へデータ移動" + nowBh.destroyObjList[k]);

            //    }

            //    nowBh.destroyObjList.Clear();

            //}

        }

    }

    //宿題ループ時用の変数
    public double RMEHWLoopStartTime;
    //宿題終了時用の変数
    public PlayableAsset RMEHWEndGoPlayableAsset;


    //終了時初期値に戻す命令
    //    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    //    {
    //#if UNITY_EDITOR
    //        Image trackBinding = director.GetGenericBinding(this) as Image;
    //        if (trackBinding == null)
    //            return;

    //        var serializedObject = new UnityEditor.SerializedObject(trackBinding);
    //        var iterator = serializedObject.GetIterator();
    //        while (iterator.NextVisible(true))
    //        {
    //            if (iterator.hasVisibleChildren)
    //                continue;

    //            driver.AddFromName<Image>(trackBinding.gameObject, iterator.propertyPath);
    //        }
    //#endif
    //        base.GatherProperties(director, driver);
    //    }
}