using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetUVScript : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] Image image;

    Texture2D tex;
    MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        tex = meshRenderer.materials[1].mainTexture as Texture2D;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector2 uv = hit.textureCoord;
            Color[] pix = tex.GetPixels(Mathf.FloorToInt(uv.x * tex.width), Mathf.FloorToInt(uv.y * tex.height), 1, 1);
            text.text = pix[0].ToString();
            image.color = pix[0];
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