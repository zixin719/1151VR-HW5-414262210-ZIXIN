using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pokemonball1 : MonoBehaviour
{
    [SerializeField] private Vector3 initialForce = new Vector3(0f, 200f, 2000f);
    [SerializeField] private Vector3 spinSpeed = new Vector3(720f, 0f, 0f); // 每秒旋轉角度（X 軸常用於往前滾/翻轉）
    [SerializeField] private Transform visualModel; // 選填：若模型是子物件可指定它，不填則轉自己
    [SerializeField] private int targetFrameRate = 60;

    private Rigidbody rb;
    private ParticleSystem ps;
    private bool isSpinning = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();
        if (visualModel == null)
        {
            visualModel = transform;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = targetFrameRate;
        Shoot(initialForce);
    }

    private void Update()
    {
        if (isSpinning && visualModel != null)
        {
            visualModel.Rotate(spinSpeed * Time.deltaTime, Space.Self);
        }
    }

    public void Shoot(Vector3 dir)
    {
        isSpinning = true;
        rb.AddForce(dir);
    }

    private void OnCollisionEnter(Collision other)
    {
        isSpinning = false;
        rb.isKinematic = true;

        if (ps != null)
        {
            ps.Play();
        }
    }
}