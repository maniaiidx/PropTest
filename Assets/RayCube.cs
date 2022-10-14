using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RayCube : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] Image image;

    Texture2D tex;
    [SerializeField] MeshRenderer meshRendererT;
    public GameObject tekkin;
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
        for (int k = 0; k < LimitLoop; k++)
        {
            Ray ray = new Ray(gameObject.transform.position, transform.TransformDirection(Vector3.forward));
            Debug.DrawRay(gameObject.transform.position, transform.TransformDirection(Vector3.forward) * 6, Color.blue, 0.1f);
            hitInfo = Physics.RaycastAll(ray);
            if (hitInfo.Length == 0) break;
            for (int i = 0; i < hitInfo.Length; i++)
            {
                if (hitInfo[i].collider.gameObject.tag != "tekkintarget") continue;
                meshRendererT = hitInfo[i].collider.gameObject.GetComponent<MeshRenderer>();
                tex = meshRendererT.materials[1].mainTexture as Texture2D;
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
                    GameObject sphere = Instantiate(tekkin, hitInfo[i].point, Quaternion.Euler(EulerQ));
                    sphere.transform.parent = hitInfo[i].transform;
                    Debug.Log("hit: " + i);
                    //Listに追加
                    GenerateList.Add(sphere);
                }
                Debug.Log("てｓｔ");
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
}