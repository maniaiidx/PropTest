#pragma strict

//private var a:float = 1.087451;
private var a:float = 1.15;
private var b:float;
var hi:float = 0.01;
var sizechange:float = 1.01;


function Update () {
	//大きさ調節

	if (Input.GetKey("3")) {
		b = transform.localPosition.y - transform.localScale.y * a;
		transform.localScale.x *= sizechange;
		transform.localScale.y *= sizechange;
		transform.localScale.z *= sizechange;
		transform.localPosition.y = transform.localScale.y * a + b;
	}
	if (Input.GetKey("4")) {
		b = transform.localPosition.y - transform.localScale.y * a;
		transform.localScale.x /= sizechange;
		transform.localScale.y /= sizechange;
		transform.localScale.z /= sizechange;
		transform.localPosition.y = transform.localScale.y * a + b;
	}

	//子オブジェクトのOVRCameraControllerの高さを調節
	if (Input.GetKey("5")) {
//		transform.localPosition.y += (transform.localScale.y * hi);
		transform.localPosition.y += 0.1;
	}
	if (Input.GetKey("6")) {
//		transform.localPosition.y -= (transform.localScale.y * hi);
		transform.localPosition.y -= 0.1;
	}

	//子オブジェクトのOVRCameraControllerの高さを調節 SHIFTでスピードアップ
	if (Input.GetKey("left shift") && Input.GetKey("5")) {
//		transform.localPosition.y += (transform.localScale.y * hi);
		transform.localPosition.y += 0.9;
	}
	if (Input.GetKey("left shift") && Input.GetKey("6")) {
//		transform.localPosition.y -= (transform.localScale.y * hi);
		transform.localPosition.y -= 0.9;
	}


	//高さリセット
	if (Input.GetKey("9")) {
		transform.localPosition.y = transform.localScale.y * a;
	}
	//大きさリセット
	if (Input.GetKey("0")) {
		transform.localScale = new Vector3(1.0, 1.0, 1.0);
//		transform.localPosition.y = transform.localScale.y * a;
	}
}