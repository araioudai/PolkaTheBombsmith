using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    PlayerAction controls;
    [SerializeField] private Transform target;                         //追従対象（プレイヤーのTransform）
    [SerializeField] private Vector2 deadZone = new Vector2(2f, 1.5f); //デッドゾーンのX・Y範囲（中心からの距離）
    [SerializeField] private float followSpeed = 5f;                   //追従速度（Lerpの係数）
    private Camera maincam;                                            //カメラ拡大縮小用

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

    void Start()
    {
        Init();
    }
    void Init()
    {
        maincam = GetComponent<Camera>();
    }

    void CameraExpansion(InputAction.CallbackContext context)
    {
        if (maincam.orthographicSize > 5f)
        {
            maincam.orthographicSize -= 5f;
            /*float target = maincam.orthographicSize - 5f;
            while (target < maincam.orthographicSize)
            {
                maincam.orthographicSize -= Time.deltaTime;
            }*/
        }
        //cameraScale.transform.localScale = Vector3.one / 2;
        //Debug.Log("縮小");
    }

    void CameraReduction(InputAction.CallbackContext context)
    {
        if (maincam.orthographicSize < 20f)
        {
            maincam.orthographicSize += 5f;
            /* float target = maincam.orthographicSize + 5f;
             while(target > maincam.orthographicSize)
             {
                 maincam.orthographicSize += Time.deltaTime;
             }*/
        }
        //cameraScale.transform.localScale = Vector3.one * 2;
        //Debug.Log("拡大");
    }

    private void Awake()
    {
        controls = new PlayerAction();
    }

    private void OnEnable()
    {
        //「Camera」アクションのアクションマップを有効
        controls.Camera.CameraExpansion.Enable();
        controls.Camera.CameraReduction.Enable();

        //カメラ拡大
        controls.Camera.CameraExpansion.performed += CameraExpansion;

        //カメラ縮小
        controls.Camera.CameraReduction.performed += CameraReduction;
    }
    private void OnDisable()
    {
        //カメラ拡大
        controls.Camera.CameraExpansion.performed -= CameraExpansion;

        //カメラ縮小
        controls.Camera.CameraReduction.performed -= CameraReduction;

        //「Camera」アクションのアクションマップを無効
        controls.Camera.CameraExpansion.Disable();
        controls.Camera.CameraReduction.Disable();
    }
}