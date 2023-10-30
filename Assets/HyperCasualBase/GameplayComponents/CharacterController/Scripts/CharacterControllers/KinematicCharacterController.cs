using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

public class KinematicCharacterController : CharacterControllerBase
{
    #region Public Methods
    public override void Jump()
    {
		JumpInput = true;
    }

    public override void Move(Vector3 Direction)
    {
        if (!Character.IsControlable)
        {
            MovementDirection = Vector3.zero;
            return;
        }

        if (!IsGrounded())
            JumpInput = false;

        //if (Character.CharacterControlType == CharacterControlType.Player)
        //    Direction = Camera.main.transform.TransformDirection(Direction);

        MovementDirection = Direction;
    }

	//Add momentum to controller;
	public override void AddMomentum(Vector3 _momentum)
	{
		if (Character.CharacterMovementData.useLocalMomentum)
			momentum = tr.localToWorldMatrix * momentum;

		momentum += _momentum;

		if (Character.CharacterMovementData.useLocalMomentum)
			momentum = tr.worldToLocalMatrix * momentum;
	}
    #endregion
    #region Getters
    public override float CurrentSpeed()
	{
		return GetVelocity().magnitude;
	}

	//Get last frame's velocity;
	public override Vector3 GetVelocity()
	{
		return savedVelocity;
	}

	//Get last frame's movement velocity (momentum is ignored);
	public override Vector3 GetMovementVelocity()
	{
		return savedMovementVelocity;
	}

	//Get current momentum;
	public override Vector3 GetMomentum()
	{
		Vector3 _worldMomentum = momentum;
		if (Character.CharacterMovementData.useLocalMomentum)
			_worldMomentum = tr.localToWorldMatrix * momentum;

		return _worldMomentum;
	}

	//Returns 'true' if controller is grounded (or sliding down a slope);
	public override bool IsGrounded()
	{
		return (currentControllerState == ControllerState.Grounded || currentControllerState == ControllerState.Sliding);
	}

	//Returns 'true' if controller is sliding;
	public override bool IsSliding()
	{
		return (currentControllerState == ControllerState.Sliding);
	}
    #endregion


    #region Logic

    //References to attached components;
    protected Transform tr;
	protected Mover mover;
	protected CeilingDetector ceilingDetector;

	//Jump key variables;
	bool jumpKeyWasPressed = false;
	bool jumpKeyWasLetGo = false;
	bool jumpKeyIsPressed = false;

	float currentJumpStartTime = 0f;

	//Current momentum;
	protected Vector3 momentum = Vector3.zero;

	//Saved velocity from last frame;
	Vector3 savedVelocity = Vector3.zero;

	//Saved horizontal movement velocity from last frame;
	Vector3 savedMovementVelocity = Vector3.zero;

	
	//Enum describing basic controller states; 
	public enum ControllerState
	{
		Grounded,
		Sliding,
		Falling,
		Rising,
		Jumping
	}

	ControllerState currentControllerState = ControllerState.Falling;

	public Transform cameraTransform
    {
        get
        {
			if (Character.CharacterData.CharacterControlType == CharacterControlType.Player)
			{
				return Camera.main.transform;
			}
			else return null;
        }
    }


	[HideInInspector]
	public Vector3 MovementDirection;
	[HideInInspector]
	public bool JumpInput;

	//Get references to all necessary components;
	void Awake()
	{
		mover = GetComponent<Mover>();
		tr = transform;
		ceilingDetector = GetComponent<CeilingDetector>();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		Character character = GetComponent<Character>();
		if (character)
		{
			character.OnCharacterSet.AddListener(Setup);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Character character = GetComponent<Character>();
		if (character)
		{
			character.OnCharacterSet.RemoveListener(Setup);
		}
	}

	//This function is called right after Awake(); It can be overridden by inheriting scripts;
	protected virtual void Setup()
	{
		
	}

	void Update()
	{
		HandleJumpKeyInput();
	}

	//Handle jump booleans for later use in FixedUpdate;
	void HandleJumpKeyInput()
	{
		bool _newJumpKeyPressedState = IsJumpKeyPressed();

		if (jumpKeyIsPressed == false && _newJumpKeyPressedState == true)
			jumpKeyWasPressed = true;

		if (jumpKeyIsPressed == false && _newJumpKeyPressedState == false)
			jumpKeyWasLetGo = true;

		jumpKeyIsPressed = _newJumpKeyPressedState;
	}

	//FixedUpdate;
	void FixedUpdate()
	{
		//Check if mover is grounded;
		mover.CheckForGround();

		//Determine controller state;
		currentControllerState = DetermineControllerState();

		//Apply friction and gravity to 'momentum';
		HandleMomentum();

		//Check if the player has initiated a jump;
		HandleJumping();

		//Calculate movement velocity;
		Vector3 _velocity = CalculateMovementVelocity();

		//If local momentum is used, transform momentum into world space first;
		Vector3 _worldMomentum = momentum;
		if (Character.CharacterMovementData.useLocalMomentum)
			_worldMomentum = tr.localToWorldMatrix * momentum;

		//Add current momentum to velocity;
		_velocity += _worldMomentum;

		//If player is grounded or sliding on a slope, extend mover's sensor range;
		//This enables the player to walk up/down stairs and slopes without losing ground contact;
		mover.SetExtendSensorRange(IsGrounded());

		//Set mover velocity;		
		mover.SetVelocity(_velocity);

		//Store velocity for next frame;
		savedVelocity = _velocity;
		savedMovementVelocity = _velocity - _worldMomentum;

		//Reset jump key booleans;
		jumpKeyWasLetGo = false;
		jumpKeyWasPressed = false;

		//Reset ceiling detector, if one was attached to this gameobject;
		if (ceilingDetector != null)
			ceilingDetector.ResetFlags();
	}

	//Calculate and return movement direction based on player input;
	//This function can be overridden by inheriting scripts to implement different player controls;
	protected virtual Vector3 CalculateMovementDirection()
	{
		//If no character input script is attached to this object, return;

		Vector3 _velocity = Vector3.zero;

		//If no camera transform has been assigned, use the character's transform axes to calculate the movement direction;
		if (cameraTransform == null)
		{
			_velocity += tr.right * MovementDirection.x;
			_velocity += tr.forward * MovementDirection.z;
		}
		else
		{
			//If a camera transform has been assigned, use the assigned transform's axes for movement direction;
			//Project movement direction so movement stays parallel to the ground;
			_velocity += Vector3.ProjectOnPlane(cameraTransform.right, tr.up).normalized * MovementDirection.x;
			_velocity += Vector3.ProjectOnPlane(cameraTransform.forward, tr.up).normalized * MovementDirection.z;
		}

		//If necessary, clamp movement vector to magnitude of 1f;
		if (_velocity.magnitude > 1f)
			_velocity.Normalize();

		return _velocity;
	}

	//Calculate and return movement velocity based on player input, controller state, ground normal [...];
	protected Vector3 CalculateMovementVelocity()
	{
		//Calculate (normalized) movement direction;
		Vector3 _velocity = CalculateMovementDirection();

		//Save movement direction for later;
		Vector3 _velocityDirection = _velocity;

		//Multiply (normalized) velocity with movement speed;
		_velocity *= Character.CharacterMovementData.MoveSpeed;

		//If controller is not grounded, multiply movement velocity with 'airControl';
		if (!(currentControllerState == ControllerState.Grounded))
			_velocity = _velocityDirection * Character.CharacterMovementData.MoveSpeed * Character.CharacterMovementData.airControl;

		return _velocity;
	}

	//Returns 'true' if the player presses the jump key;
	protected virtual bool IsJumpKeyPressed()
	{

		return JumpInput;
	}

	//Determine current controller state based on current momentum and whether the controller is grounded (or not);
	//Handle state transitions;
	ControllerState DetermineControllerState()
	{
		//Check if vertical momentum is pointing upwards;
		bool _isRising = IsRisingOrFalling() && (VectorMath.GetDotProduct(GetMomentum(), tr.up) > 0f);
		//Check if controller is sliding;
		bool _isSliding = mover.IsGrounded() && IsGroundTooSteep();

		//Grounded;
		if (currentControllerState == ControllerState.Grounded)
		{
			if (_isRising)
			{
				OnGroundContactLost();
				return ControllerState.Rising;
			}
			if (!mover.IsGrounded())
			{
				OnGroundContactLost();
				return ControllerState.Falling;
			}
			if (_isSliding)
			{
				return ControllerState.Sliding;
			}
			return ControllerState.Grounded;
		}

		//Falling;
		if (currentControllerState == ControllerState.Falling)
		{
			if (_isRising)
			{
				return ControllerState.Rising;
			}
			if (mover.IsGrounded() && !_isSliding)
			{
				OnGroundContactRegained(momentum);
				return ControllerState.Grounded;
			}
			if (_isSliding)
			{
				OnGroundContactRegained(momentum);
				return ControllerState.Sliding;
			}
			return ControllerState.Falling;
		}

		//Sliding;
		if (currentControllerState == ControllerState.Sliding)
		{
			if (_isRising)
			{
				OnGroundContactLost();
				return ControllerState.Rising;
			}
			if (!mover.IsGrounded())
			{
				return ControllerState.Falling;
			}
			if (mover.IsGrounded() && !_isSliding)
			{
				OnGroundContactRegained(momentum);
				return ControllerState.Grounded;
			}
			return ControllerState.Sliding;
		}

		//Rising;
		if (currentControllerState == ControllerState.Rising)
		{
			if (!_isRising)
			{
				if (mover.IsGrounded() && !_isSliding)
				{
					OnGroundContactRegained(momentum);
					return ControllerState.Grounded;
				}
				if (_isSliding)
				{
					return ControllerState.Sliding;
				}
				if (!mover.IsGrounded())
				{
					return ControllerState.Falling;
				}
			}

			//If a ceiling detector has been attached to this gameobject, check for ceiling hits;
			if (ceilingDetector != null)
			{
				if (ceilingDetector.HitCeiling())
				{
					OnCeilingContact();
					return ControllerState.Falling;
				}
			}
			return ControllerState.Rising;
		}

		//Jumping;
		if (currentControllerState == ControllerState.Jumping)
		{
			//Check for jump timeout;
			if ((Time.time - currentJumpStartTime) > Character.CharacterMovementData.jumpDuration)
				return ControllerState.Rising;

			//Check if jump key was let go;
			if (jumpKeyWasLetGo)
				return ControllerState.Rising;

			//If a ceiling detector has been attached to this gameobject, check for ceiling hits;
			if (ceilingDetector != null)
			{
				if (ceilingDetector.HitCeiling())
				{
					OnCeilingContact();
					return ControllerState.Falling;
				}
			}
			return ControllerState.Jumping;
		}

		return ControllerState.Falling;
	}

	//Check if player has initiated a jump;
	void HandleJumping()
	{
		if (currentControllerState == ControllerState.Grounded)
		{
			if (jumpKeyIsPressed == true || jumpKeyWasPressed)
			{
				//Call events;
				OnGroundContactLost();
				OnJumpStart();

				currentControllerState = ControllerState.Jumping;
			}
		}
	}

	//Apply friction to both vertical and horizontal momentum based on 'friction' and 'gravity';
	//Handle sliding down steep slopes;
	void HandleMomentum()
	{
		//If local momentum is used, transform momentum into world coordinates first;
		if (Character.CharacterMovementData.useLocalMomentum)
			momentum = tr.localToWorldMatrix * momentum;

		Vector3 _verticalMomentum = Vector3.zero;
		Vector3 _horizontalMomentum = Vector3.zero;

		//Split momentum into vertical and horizontal components;
		if (momentum != Vector3.zero)
		{
			_verticalMomentum = VectorMath.ExtractDotVector(momentum, tr.up);
			_horizontalMomentum = momentum - _verticalMomentum;
		}

		//Add gravity to vertical momentum;
		_verticalMomentum -= tr.up * Character.CharacterMovementData.gravity * Time.deltaTime;

		//Remove any downward force if the controller is grounded;
		if (currentControllerState == ControllerState.Grounded)
			_verticalMomentum = Vector3.zero;

		//Apply friction to horizontal momentum based on whether the controller is grounded;
		if (currentControllerState == ControllerState.Grounded)
			_horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(_horizontalMomentum, Character.CharacterMovementData.groundFriction, Time.deltaTime, 0f);
		else
			_horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(_horizontalMomentum, Character.CharacterMovementData.airFriction, Time.deltaTime, 0f);

		//Add horizontal and vertical momentum back together;
		momentum = _horizontalMomentum + _verticalMomentum;

		//Project the current momentum onto the current ground normal if the controller is sliding down a slope;
		if (currentControllerState == ControllerState.Sliding)
		{
			momentum = Vector3.ProjectOnPlane(momentum, mover.GetGroundNormal());
		}

		//Apply slide gravity along ground normal, if controller is sliding;
		if (currentControllerState == ControllerState.Sliding)
		{
			Vector3 _slideDirection = Vector3.ProjectOnPlane(-tr.up, mover.GetGroundNormal()).normalized;
			momentum += _slideDirection * Character.CharacterMovementData.slideGravity * Time.deltaTime;
		}

		//If controller is jumping, override vertical velocity with jumpSpeed;
		if (currentControllerState == ControllerState.Jumping)
		{
			momentum = VectorMath.RemoveDotVector(momentum, tr.up);
			momentum += tr.up * Character.CharacterMovementData.jumpSpeed;
		}

		if (Character.CharacterMovementData.useLocalMomentum)
			momentum = tr.worldToLocalMatrix * momentum;
	}

	//Events;

	//This function is called when the player has initiated a jump;
	void OnJumpStart()
	{
		//If local momentum is used, transform momentum into world coordinates first;
		if (Character.CharacterMovementData.useLocalMomentum)
			momentum = tr.localToWorldMatrix * momentum;

		//Add jump force to momentum;
		momentum += tr.up * Character.CharacterMovementData.jumpSpeed;

		//Set jump start time;
		currentJumpStartTime = Time.time;

		//Call event;
		if (OnJump != null)
			OnJump(momentum);

		if (Character.CharacterMovementData.useLocalMomentum)
			momentum = tr.worldToLocalMatrix * momentum;
	}

	//This function is called when the controller has lost ground contact, i.e. is either falling or rising, or generally in the air;
	void OnGroundContactLost()
	{
		//Calculate current velocity;
		//If velocity would exceed the controller's movement speed, decrease movement velocity appropriately;
		//This prevents unwanted accumulation of velocity;
		float _horizontalMomentumSpeed = VectorMath.RemoveDotVector(GetMomentum(), tr.up).magnitude;
		Vector3 _currentVelocity = GetMomentum() + Vector3.ClampMagnitude(savedMovementVelocity, Mathf.Clamp(Character.CharacterData.CharacterMovementData.MoveSpeed - _horizontalMomentumSpeed, 0f, Character.CharacterData.CharacterMovementData.MoveSpeed));

		//Calculate length and direction from '_currentVelocity';
		float _length = _currentVelocity.magnitude;

		//Calculate velocity direction;
		Vector3 _velocityDirection = Vector3.zero;
		if (_length != 0f)
			_velocityDirection = _currentVelocity / _length;

		//Subtract from '_length', based on 'movementSpeed' and 'airControl', check for overshooting;
		if (_length >= Character.CharacterMovementData.MoveSpeed * Character.CharacterMovementData.airControl)
			_length -= Character.CharacterMovementData.MoveSpeed * Character.CharacterMovementData.airControl;
		else
			_length = 0f;

		//If local momentum is used, transform momentum into world coordinates first;
		if (Character.CharacterMovementData.useLocalMomentum)
			momentum = tr.localToWorldMatrix * momentum;

		momentum = _velocityDirection * _length;

		if (Character.CharacterMovementData.useLocalMomentum)
			momentum = tr.worldToLocalMatrix * momentum;
	}

	//This function is called when the controller has landed on a surface after being in the air;
	void OnGroundContactRegained(Vector3 _collisionVelocity)
	{
		//Call 'OnLand' event;
		if (OnLand != null)
			OnLand(_collisionVelocity);
	}

	//This function is called when the controller has collided with a ceiling while jumping or moving upwards;
	void OnCeilingContact()
	{
		//If local momentum is used, transform momentum into world coordinates first;
		if (Character.CharacterMovementData.useLocalMomentum)
			momentum = tr.localToWorldMatrix * momentum;

		//Remove all vertical parts of momentum;
		momentum = VectorMath.RemoveDotVector(momentum, tr.up);

		if (Character.CharacterMovementData.useLocalMomentum)
			momentum = tr.worldToLocalMatrix * momentum;
	}

	//Helper functions;

	//Returns 'true' if vertical momentum is above a small threshold;
	private bool IsRisingOrFalling()
	{
		//Calculate current vertical momentum;
		Vector3 _verticalMomentum = VectorMath.ExtractDotVector(GetMomentum(), tr.up);

		//Setup threshold to check against;
		//For most applications, a value of '0.001f' is recommended;
		float _limit = 0.001f;

		//Return true if vertical momentum is above '_limit';
		return (_verticalMomentum.magnitude > _limit);
	}

	//Returns true if angle between controller and ground normal is too big (> slope limit), i.e. ground is too steep;
	private bool IsGroundTooSteep()
	{
		if (!mover.IsGrounded())
			return true;

		return (Vector3.Angle(mover.GetGroundNormal(), tr.up) > Character.CharacterData.CharacterMovementData.slopeLimit);
	}
	#endregion

	
}

