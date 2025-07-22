using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform target;           // 追従対象（プレイヤーのTransform）
    public Vector2 deadZone = new Vector2(2f, 1.5f); // デッドゾーンのX・Y範囲（中心からの距離）
    public float followSpeed = 5f;     // 追従速度（Lerpの係数）

    void LateUpdate()
    {
        if (target == null) return;   // 追従対象がセットされていなければ処理終了

        Vector3 pos = transform.position;
        Vector3 tpos = target.position;

        // ------ X 軸の処理 ------
        float dx = tpos.x - pos.x;
        if (Mathf.Abs(dx) > deadZone.x)
        {
            // デッドゾーン外に出ている場合のみLerpで滑らかに追従
            pos.x = Mathf.Lerp(pos.x,
                               tpos.x - Mathf.Sign(dx) * deadZone.x,
                               followSpeed * Time.deltaTime);
        }

        // ------ Y 軸の処理 ------
        float dy = tpos.y - pos.y;
        if (Mathf.Abs(dy) > deadZone.y)
        {
            pos.y = Mathf.Lerp(pos.y,
                               tpos.y - Mathf.Sign(dy) * deadZone.y,
                               followSpeed * Time.deltaTime);
        }
        // カメラ位置を更新（Z軸は維持）
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}