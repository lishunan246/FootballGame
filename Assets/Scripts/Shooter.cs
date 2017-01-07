using UnityEngine;
using UnityEngine.UI;

public class Shooter : MonoBehaviour
{
    public Slider ForceBarSlider;
    private float _kickForce;
    private GameObject _Football;

    private readonly float MaxKickForce = 1.0f;
    private readonly float baseKickForce = 0.2f;

    public float force=300.0f;
    public float up = 1.0f;
    public float power = 10.0f;

    // dribble
    public float dribbleRange = 2.0f;
    public float dribbleForce = 2000.0f;
    public Vector3 preForward = new Vector3(0.0f,0.0f,0.0f);

    //stop ball
    public float stopHeight = 0.28f;

    // Reference to projectile prefab to shoot
    public GameObject projectile;

    // Reference to AudioClip to play
    public AudioClip shootSFX;

    private void Start(){
        _Football = GameObject.FindWithTag("Football");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Football")
        { 
            var ball = collision.gameObject;
            var f = gameObject.transform.forward;

            f[1] = up;

            ball.GetComponent<Rigidbody>().AddForce(f*(_kickForce+baseKickForce)*force);
            GameManager.gm.LastBallTouch=GameManager.Side.Player;
            _kickForce = 0;
        }
    }

    // Update is called once per frame
    private void Update()
    {

        if(Input.GetButton("Fire1")){
            float dist = Vector3.Distance (gameObject.transform.position, _Football.transform.position);
            if(dist<=dribbleRange){
                Debug.Log(string.Format("Distance:{0},Dribble range {1}",dist,dribbleRange));
                Vector3 pos = _Football.transform.position;
                if(pos.y <= stopHeight){
                    // spin the ball
                    Vector3 playPos = gameObject.transform.position;
                    Vector3 forward = gameObject.transform.forward;
                    Vector3 newBallPos = playPos + dist*forward;
                    newBallPos.y = 0.25f;
                    // _Football.GetComponent<Rigidbody>().Sleep();
                    _Football.GetComponent<Rigidbody>().MovePosition(newBallPos);
                    // _Football.GetComponent<Rigidbody>().AddForce(dribbleForce*(forward-preForward));
                    // Debug.Log(string.Format("Dribble direction:{0}",forward));
                    // preForward = forward;
                // Debug.Log(string.Format("Pos:{0}",pos));
                }
            }
        }
        // GameObject ball = GameObject.FindWithTag("Football");
        
        // move the ball in horizon
        // if(dist<=dribbleRange){
        //     Vector3 forward = gameObject.transform.forward;
        //     _Football.GetComponent<Rigidbody>().AddForce((forward-preForward)*baseKickForce);
        //     Debug.Log(string.Format("Applied force....{0}",(forward-preForward)*baseKickForce));
        //     // sleep the ball
        //     if(Input.GetButton("Fire1")){
        //         // stop the ball
        //         _Football.GetComponent<Rigidbody>().Sleep();     
        //     }
        // }
        // else{
        //     preForward = new Vector3(0.0f,0.0f,0.0f);
        // }
        

        if (Input.GetButton("Jump"))
        {
            Debug.Log(string.Format("force..."));
            _kickForce += 1.0f*Time.deltaTime;
            _kickForce = _kickForce > MaxKickForce ? MaxKickForce : _kickForce;
        }
        else
        {
            _kickForce -= 0.5f*Time.deltaTime;
            _kickForce = _kickForce < 0 ? 0 : _kickForce;
        }

        ForceBarSlider.value = _kickForce;
    }
}