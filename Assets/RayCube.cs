using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayCube : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] Image image;

    Texture2D tex;
    [SerializeField] MeshRenderer meshRendererT;
    public bool testend;

    // Start is called before the first frame update
    void Start()
    {
        //meshRenderer = GetComponent<MeshRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        if (testend == true) return;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Ray ray = new Ray(gameObject.transform.position, transform.TransformDirection(Vector3.forward));
        Debug.DrawRay(gameObject.transform.position, transform.TransformDirection(Vector3.forward) * 6, Color.blue, 0.1f);

        RaycastHit hitInfo;
        //if (Physics.Raycast(ray, out hit))
        //if (Physics.Raycast(gameObject.transform.position, transform.TransformDirection(Vector3.forward), out hitInfo))
        if (Physics.Raycast(ray, out hitInfo))
        {
            meshRendererT = hitInfo.collider.gameObject.GetComponent<MeshRenderer>();
            tex = meshRendererT.materials[1].mainTexture as Texture2D;
            Vector2 uv = hitInfo.textureCoord;
            Color[] pix = tex.GetPixels(Mathf.FloorToInt(uv.x * tex.width), Mathf.FloorToInt(uv.y * tex.height), 1, 1);
            text.text = pix[0].ToString();
            image.color = pix[0];

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = hitInfo.point;
            testend = true;
        }
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
}