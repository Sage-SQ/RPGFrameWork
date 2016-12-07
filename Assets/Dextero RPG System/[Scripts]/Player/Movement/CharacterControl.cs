using System.Collections.Generic;

using UnityEngine;

using System.Collections;


public class CharacterControl : MonoBehaviour

{
	private PlayerNew player;
	
	//Collision
	public float floatRay = 1.0f;
	public float timeInAir = 0;
	public float timeTillJump = 0.15f;
	public float timeTillFall = 1.8f;
    public GlobalValues.Seperate MovementSpeed;

    //moving forwards and backwards

	public float baseWalkSpeed = 1.5f;
	public float WalkSpeed = 3;
	public float RunSpeed = 4;
    public float BackwardSpeed = 5;

    public float AerialLerpModifier = 2;

    public float MovementLerpSpeed = 0.5f;

    private bool _backPedaling;
	
	
	//Move to click
	private Transform myTransform;				// this transform
	public Vector3 destinationPosition;		// The destination Point
	
	private float destinationDistance;			// The distance between myTransform and destinationPosition
 	private GameObject myCamera;
	public float moveSpeed;						// The Speed the character will move
 	
	public static bool useNewPosition;
	public static Vector3 newPosition;		// The destination Point
	public static bool arrivedAtDestination = true;
	
	// ************************ //
	
	public CharacterState State;
 	public enum CharacterState {
		Idle,
		Running,
		SkillAnimation,
		Harvesting,
		Dead
	}

    public GlobalValues.Seperate RotatingSpeed;

    //moving left and right turning left and right

    public float RotationSpeed = 0.05f;

 

    public GlobalValues.Seperate Gravity;

    public Vector3 GravityDrop = new Vector3(0, -0.3f, 0);

    public float MaximumGravity = 20;

    public bool IsGrounded;

 

    public GlobalValues.Seperate Jump;

    public float JumpSpeed = 10;

    public float ConsideredFloor = 0.5f;

    public float JumpLerpSpeed = 0.1f; 

    public Vector3 Velocity;

 

    //on the off hand we need to retain movment (air)

    private Vector3 _currentMovement;

    private Vector3 _currentGravity;

    private bool _hasJumped;

 

    void Start()

    {

		if(player == null){
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		}
		
        GlobalValues.CharacterTransform = transform;

        GlobalValues.CharacterRigid = GetComponent<Rigidbody>();

        GlobalValues.CharacterRigid.freezeRotation = true;

        GlobalValues.CharacterRigid.useGravity = false;
		
		myTransform = transform;							// sets myTransform to this GameObject.transform
		destinationPosition = myTransform.position;			// prevents myTransform reset
		if(myCamera == null){
			myCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}

    }

    void OnCollisionStay(Collision other)

    {

        IsGrounded = false;

        foreach (ContactPoint contact in other.contacts)

        {

            if (contact.normal.y < ConsideredFloor) continue;
			
            	IsGrounded = true;

        }

    }

    void OnCollisionExit(Collision other)

    {
        	IsGrounded = false;

    }
	

 
    void Update()

    {		
		WalkSpeed = baseWalkSpeed * player.MoveSpeed;
		RunSpeed = WalkSpeed * 2;
			
		State = CharacterState.Idle;
 		
		if(player.Alive)
			MoveToMouseClick();
		else
			State = CharacterState.Dead;
		
        //if jumping

        if (GlobalValues.JumpAxis > 0)

        {

            if (IsGrounded)

            {
                _hasJumped = true;
				

            }

        }

        if (IsGrounded)

        {

            _currentGravity = Vector3.zero;

        }

        else

        {
            _currentGravity += GravityDrop;

        }
				
		if(player.playerState == PlayerState.Harvesting){
			State = CharacterState.Harvesting;
		}		
    }
	
	void MoveToMouseClick(){
		if(GlobalValues.MouseOnGUI || player.playerState == PlayerState.Harvesting || player.playerState == PlayerState.Cutscene ) return;
			// keep track of the distance between this gameObject and destinationPosition
		
		// Moves the Player if the Left Mouse Button was clicked
		if (Input.GetMouseButtonDown(0)&& GUIUtility.hotControl ==0) {
 			MovePlayer();
		}
 
		// Moves the player if the mouse button is hold down
		else if (Input.GetMouseButton(0)&& GUIUtility.hotControl ==0) {
 			MovePlayer();
			myTransform.position = Vector3.MoveTowards(myTransform.position, destinationPosition, RunSpeed * Time.deltaTime);
			State = CharacterState.Running;
		}
 		
 		if(useNewPosition) {
			destinationPosition = newPosition;
		}
		
		destinationDistance = Vector3.Distance(destinationPosition, myTransform.position);
		if(destinationDistance < .5f){		// To prevent shakin behavior when near destination
			RunSpeed = 0;
			destinationPosition = myTransform.position;
			//arrivedAtDestination = true;
		}
		else if(destinationDistance > .5f){			// To Reset Speed to default
			RunSpeed = WalkSpeed * 2;
		}
 
 
		
		
		
 
		// To prevent code from running if not needed
		
		if(!arrivedAtDestination){
			if(destinationDistance > 0.9f){
				float angleToTarget = Mathf.Atan2((destinationPosition.x - transform.position.x), (destinationPosition.z - transform.position.z)) * Mathf.Rad2Deg;
				myTransform.eulerAngles = new Vector3(0,angleToTarget, 0);
				arrivedAtDestination = false;
				if(!useNewPosition){
					arrivedAtDestination = false;
					myTransform.position = Vector3.MoveTowards(myTransform.position, destinationPosition, RunSpeed * Time.deltaTime);
					State = CharacterState.Running;
				}
				else if(destinationDistance >= player.AttackRange - 0.5f){
					arrivedAtDestination = false;
					myTransform.position = Vector3.MoveTowards(myTransform.position, destinationPosition, RunSpeed * Time.deltaTime);
					State = CharacterState.Running;
				}
			}
			else {
				arrivedAtDestination = true;
				destinationPosition = transform.position;
				useNewPosition = false;
			}
		}
		else {
			State = CharacterState.Idle;
		}
	}
	
	void MovePlayer(){
		if(!shouldMove()){
				destinationPosition = myTransform.position;
				return;
		} 
		
		arrivedAtDestination = false;
		Plane playerPlane = new Plane(Vector3.up, myTransform.position);
		Ray ray = myCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		float hitdist = 0.0f;
		if (playerPlane.Raycast(ray, out hitdist)) {
			Vector3 targetPoint = ray.GetPoint(hitdist);
			destinationPosition = ray.GetPoint(hitdist);
			float angleToTarget = Mathf.Atan2((targetPoint.x - transform.position.x), (targetPoint.z - transform.position.z)) * Mathf.Rad2Deg;
			myTransform.eulerAngles = new Vector3(0,angleToTarget, 0);
		}
	}
	
	bool shouldMove(){
		
		Ray ray = myCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		RaycastHit hitinfo = new RaycastHit();

		if(Physics.Raycast (ray,out hitinfo)){
			float angleToTarget = Mathf.Atan2((hitinfo.transform.position.x - transform.position.x), (hitinfo.transform.position.z - transform.position.z)) * Mathf.Rad2Deg;
			myTransform.eulerAngles = new Vector3(0,angleToTarget, 0);
			if(hitinfo.collider.tag == "Enemy" || hitinfo.collider.tag == "NPC" || hitinfo.collider.tag == "QuestNPC"){
				//float angleToTarget = Mathf.Atan2((hitinfo.transform.position.x - transform.position.x), (hitinfo.transform.position.z - transform.position.z)) * Mathf.Rad2Deg;
				//myTransform.eulerAngles = new Vector3(0,angleToTarget, 0);
				float dist = Vector3.Distance (myTransform.position,hitinfo.transform.position);
				if(dist < 2 + 0.5f){
					return false;
				}
				else {
					newPosition = hitinfo.transform.position;
					useNewPosition = true;
					return true;
				}
			}
		}
		
		
		useNewPosition = false;
		return true;
	}
	
	
	public float airVelocity;
    void FixedUpdate()

    {

        airVelocity = Mathf.Lerp(GlobalValues.CharacterRigid.velocity.y, _currentGravity.y, JumpLerpSpeed);
		
        if (_hasJumped)

        {
			
		    _hasJumped = false;
			
            airVelocity += JumpSpeed;

        }

        if (airVelocity < -MaximumGravity) airVelocity = -MaximumGravity;

        GlobalValues.CharacterRigid.velocity = new Vector3(_currentMovement.x, airVelocity, _currentMovement.z);

        Velocity = GlobalValues.CharacterRigid.velocity;

    }

}