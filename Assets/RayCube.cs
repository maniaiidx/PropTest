using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class RayCube : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] Image image;

    Texture2D tex;
    [SerializeField] MeshRenderer meshRendererT;
    public GameObject tekkin;

    [Header("weightが多いと生成されやすい")]
    //Inspectorに表示される
    [SerializeField]
    public List<Spawntekkin> TekkinObject;
    //プロパティStruct
    [Serializable]
    public class Spawntekkin
    {
        public GameObject
            SpawnPrefab;
        public int
            weight;
        public Spawntekkin()
        {
            SpawnPrefab = null;
            weight = 1;
        }
    }

    private List<Spawntekkin> tmpcopy;

    [Header("鉄筋を刺す角度")]
    public Vector3 EulerQ;
    [Header("次の鉄筋を刺す移動距離")]
    public float DistanceX = 0f;
    public float DistanceY = 0f;
    public float DistanceZ = 0f;
    [Header("移動回数の上限")]
    public int LimitLoop = 5;
    private Vector3 initPos;

    [Header("生成/リセットボタン")]
    //生成ボタン
    [Button(nameof(Generatetekkin), "生成")]
    public bool dummy;//属性でボタンを作るので何かしら宣言が必要。使わない

    //リセットボタン
    [Button(nameof(Resettekkin), "リセット")]
    public bool dummyReset;//属性でボタンを作るので何かしら宣言が必要。使わない

    [Header("※注意　元に戻せなくなる")]
    [Header("通常はリセットするまで生成出来ないが、")]
    [Header("チェックを外せば生成可能")]
    public bool GenerateLock;

    public List<GameObject> GenerateList;


    // Start is called before the first frame update
    void Start()
    {
        //meshRendererT = GetComponent<MeshRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        /* ボタンに移設
        if (testend == true) return;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hitInfo;
        RaycastHit[] hitInfo;
        //if (Physics.Raycast(ray, out hit))
        //if (Physics.Raycast(gameObject.transform.position, transform.TransformDirection(Vector3.forward), out hitInfo))
        //if (Physics.Raycast(ray, out hitInfo))
        for (int k = 0; k < LimitLoop; k++) {
            Ray ray = new Ray(gameObject.transform.position, transform.TransformDirection(Vector3.forward));
            Debug.DrawRay(gameObject.transform.position, transform.TransformDirection(Vector3.forward) * 6, Color.blue, 0.1f);
            hitInfo = Physics.RaycastAll(ray);
            if (hitInfo.Length == 0) break;
            for (int i = 0; i < hitInfo.Length; i++)
            {
                //meshRendererT = hitInfo.collider.gameObject.GetComponent<MeshRenderer>();
                meshRendererT = hitInfo[i].collider.gameObject.GetComponent<MeshRenderer>();
                tex = meshRendererT.materials[1].mainTexture as Texture2D;
                //Vector2 uv = hitInfo.textureCoord;
                Vector2 uv = hitInfo[i].textureCoord;
                try
                {
                    Color[] pix = tex.GetPixels(Mathf.FloorToInt(uv.x * tex.width), Mathf.FloorToInt(uv.y * tex.height), 1, 1);
                    text.text = pix[0].ToString();
                    image.color = pix[0];
                }
                catch
                {
                    Debug.Log("内側に当たっている");
                    //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //GameObject sphere = Instantiate(tekkin, Vector3.zero, Quaternion.Euler(0, 0, 0));
                    GameObject sphere = Instantiate(tekkin, hitInfo[i].point, Quaternion.Euler(EulerQ));
                    sphere.transform.parent = hitInfo[i].transform;
                    Debug.Log("hit: "+i);
                    //Listに追加
                    GenerateList.Add(sphere);
                    //sphere.transform.position = hitInfo.point;
                    //sphere.transform.position = hitInfo[i].point;
                    //testend = true;
                }
            }
            Debug.Log("位置ずらす");
            Vector3 nextpos = transform.position;
            nextpos.x += DistanceX;
            nextpos.y += DistanceY;
            nextpos.z += DistanceZ;
            transform.position = nextpos;
        }
        testend = true;

        */
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Item")
        {
            //Vector3 pos = collision.transform.position;
            //RaycastHit hit;

            // Cubeの中心から衝突した地点へ向かってレイを飛ばす
            //if (Physics.Raycast(pos, collision.contacts[0].point - pos, out hit, Mathf.Infinity))
            //{
            //    Vector2 uv = hit.textureCoord;
            //    Color[] pix = tex.GetPixels(Mathf.FloorToInt(uv.x * tex.width), Mathf.FloorToInt(uv.y * tex.height), 1, 1);
            //    text.text = pix[0].ToString();
            //    image.color = pix[0];
            //}



        }
    }
    void Generatetekkin()
    {
        //#if UNITY_EDITOR
        if (GenerateLock == true) return;
        initPos = transform.position;//初期位置を保存
        RaycastHit[] hitInfo;
        //鉄筋配列をコピー
        tmpcopy = new List<Spawntekkin>(TekkinObject);

        for (int k = 0; k < LimitLoop; k++)
        {
            //Rayを飛ばす
            Ray ray = new Ray(gameObject.transform.position, transform.TransformDirection(Vector3.forward));
            //Rayを表示（デバッグ用）
            Debug.DrawRay(gameObject.transform.position, transform.TransformDirection(Vector3.forward) * 6, Color.blue, 0.1f);
            //Rayが当たったすべてのオブジェクトを取得
            hitInfo = Physics.RaycastAll(ray);
            Debug.Log("hitInfo.Length: " + hitInfo.Length);
            //いっこも当たらなかったらループ破壊。（処理を全て終了）
            if (hitInfo.Length == 0) break;
            //Rayが当たったオブジェクトを全てループ処理
            for (int i = 0; i < hitInfo.Length; i++)
            {
                //tekkintargetのtagだけに鉄筋を刺すので、違うタグなら次のループへ飛ぶ
                if (hitInfo[i].collider.gameObject.tag != "tekkintarget") continue;
                //Rayが当たったオブジェクトのMeshRendererを取得
                meshRendererT = hitInfo[i].collider.gameObject.GetComponent<MeshRenderer>();



                //サブメッシュとhitの三角形座標比較の処理ここから
                Mesh mesh;
                MeshCollider meshCollider = hitInfo[i].collider as MeshCollider;
                mesh = meshCollider.sharedMesh;

                int[] hitTriangle = new int[]
                    {
                        mesh.triangles[hitInfo[i].triangleIndex * 3],
                        mesh.triangles[hitInfo[i].triangleIndex * 3 + 1],
                        mesh.triangles[hitInfo[i].triangleIndex * 3 + 2]
                    };
                //サブメッシュの数だけ繰り返す
                //for (int l = 0; l < mesh.subMeshCount; l++)
                //{
                //2個目のサブメッシュだけ処理
                //サブメッシュの三角形を取得
                int[] subMeshTris = mesh.GetTriangles(1);//固定で1を指定（materialリストの2番目が内壁であるという前提）
                //サブメッシュの三角形の頂点の数だけ繰り返す
                for (int j = 0; j < subMeshTris.Length; j += 3)
                {
                    //サブメッシュの三角形の頂点座標ととRayが当たった三角形の頂点座標が3つとも一致したら
                    if (subMeshTris[j] == hitTriangle[0] &&
                        subMeshTris[j + 1] == hitTriangle[1] &&
                        subMeshTris[j + 2] == hitTriangle[2])
                    {
                        //mat = renderer.materials[l];
                        Debug.Log("内側に当たっている");
                        //GameObject sphere = Instantiate(tekkin, hitInfo[i].point, Quaternion.Euler(EulerQ));
                        //ランダムな鉄筋を取得
                        GameObject tmp = Randomtekkin();
                        //鉄筋をスポーンする
                        GameObject sphere = Instantiate(tmp, hitInfo[i].point, Quaternion.Euler(EulerQ));
                        //鉄筋を瓦礫の子供にする
                        sphere.transform.parent = hitInfo[i].transform;
                        Debug.Log("hit: " + i);
                        //Listに追加
                        GenerateList.Add(sphere);
                    }
                }
                //}
                //ここまで




                /*色取得のエラーでやってた処理ここから
                //MeshRendererの2番目（0の次である1）のテクスチャを取得する
                tex = meshRendererT.materials[1].mainTexture as Texture2D;
                //Rayが当たったUV上の座標を取得する
                Vector2 uv = hitInfo[i].textureCoord;
                try
                {
                    //UV画像から、Rayが当たった個所の座標の色を取得している
                    //エラーになるのは、
                    Color[] pix = tex.GetPixels(Mathf.FloorToInt(uv.x * tex.width), Mathf.FloorToInt(uv.y * tex.height), 1, 1);
                    text.text = pix[0].ToString();
                    image.color = pix[0];
                }
                catch
                {
                    Debug.Log("内側に当たっている");
                    //GameObject sphere = Instantiate(tekkin, hitInfo[i].point, Quaternion.Euler(EulerQ));
                    //ランダムな鉄筋を取得
                    GameObject tmp = Randomtekkin();
                    //鉄筋をスポーンする
                    GameObject sphere = Instantiate(tmp, hitInfo[i].point, Quaternion.Euler(EulerQ));
                    //鉄筋を瓦礫の子供にする
                    sphere.transform.parent = hitInfo[i].transform;
                    Debug.Log("hit: " + i);
                    //Listに追加
                    GenerateList.Add(sphere);
                }
                ここまで*/
            }
            Debug.Log("位置ずらす");
            Vector3 nextpos = transform.position;
            nextpos.x += DistanceX;
            nextpos.y += DistanceY;
            nextpos.z += DistanceZ;
            transform.position = nextpos;
        }
        GenerateLock = true;
        //#else
        //#endif
    }

    void Resettekkin()
    {
        transform.position = initPos;//初期位置に戻る
        GenerateLock = false;//GenerateのLockを解除
        foreach (GameObject Gentekkin in GenerateList)
        {
            //GenerateList.Remove(Gentekkin); Listの中身をforeach中に削除するとエラーになる
            DestroyImmediate(Gentekkin);
        }
        GenerateList.Clear();//foreachでオブジェクトを削除した後、Listを初期化する
        /*
        #if UNITY_EDITOR
        #else
        #endif
        */
    }
    private GameObject Randomtekkin()
    {
        GameObject tekkin;
        int rmdnum = UnityEngine.Random.Range(0, tmpcopy.Count);//ランダム
        Debug.Log("" + rmdnum);
        //ランダムに選ばれた鉄筋を変数に格納
        tekkin = tmpcopy[rmdnum].SpawnPrefab;
        //さらに0～weightの間のランダムな数を算出
        int rmdwei = UnityEngine.Random.Range(0, tmpcopy[rmdnum].weight);
        //0が出たら選ばれた鉄筋をリストから削除
        if (rmdwei <= 0) tmpcopy.RemoveAt(rmdnum);
        //要素が全部無くなったら元のListからコピーしてくる
        if (tmpcopy.Count == 0) tmpcopy = new List<Spawntekkin>(TekkinObject);
        //鉄筋を返す
        return tekkin;
    }
}

