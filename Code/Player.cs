using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class Player : Node2D
{
	[Export]
	private float _rotationSpeed;
	[Export]
    private float quantization;
	[Export]
    private float distanceSpeed;
	[Export]
    private float minDistance;
	[Export]
    private float maxDistance;
	[Export]
	private float _maxSpeed;
	[Export]
	private float _minSpeed;
	[Export]
	private Vector2 _gravity;
	[Export]
	private float _floorFriction;
	[Export]
	private float slipInfluence;
	[Export]
	private float gravityFriction;
	[Export]
    private Node2D sun;
	[Export]
    private ShapeCast2D sunCast;
	[Export]
    private Node2D moon;
    [Export]
    private ShapeCast2D moonCast;
    [Export]
    private Node2D shifter;
	/// <summary>
	/// represents radius not diameter, each body is _bodyDistance pixels away from the center
	/// </summary>
	private float _bodyDistance = 16;
	private bool _locked;
    private bool _lockedBody;
    private Vector2 _moveDirection;
    private float _moveSpeed;
	private Vector2 frictionMovement;
	private float bounceTimer;
	private bool alive = true;

	//properties
	public float RotationSpeed { get => _rotationSpeed; set => _rotationSpeed = value; }
    /// <summary>
    /// represents radius not diameter, each body is _bodyDistance pixels away from the center
    /// </summary>
    public float BodyDistance { get => _bodyDistance; set => _bodyDistance = Mathf.Clamp(value, minDistance, maxDistance); }
	/// <summary>
	/// body distance as a 0-1 float relative to the min and max
	/// </summary>
	public float BodyDistanceNormalized { get => (_bodyDistance - minDistance) / (maxDistance - minDistance); }
	public Vector2 MoveDirection { get => _moveDirection; set => _moveDirection = value.Normalized(); }
	public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
	public float MoveSpeedNormalized { get => (_moveSpeed - _minSpeed) / (_maxSpeed - _minSpeed); }
	/// <summary>
	/// movement speed and direction combined into one vector
	/// </summary>
	public Vector2 Velocity 
	{ 
		get => _moveDirection * _moveSpeed;
		set
		{
			_moveDirection = value.Normalized();
			_moveSpeed = value.Length();
		}
	}
	public bool Locked { get => _locked; set => _locked = value; }
    /// <summary>
    /// true for sun false for moon
    /// </summary>
    public bool LockedBody { get => _lockedBody; set => _lockedBody = value; }
	public Vector2 Gravity { get => _gravity; set => _gravity = value; }
	public float FloorFriction { get => _floorFriction; set => _floorFriction = value; }

	//input stuff
	private bool upHeld;
	private bool downHeld;

	[Signal]
	public delegate void SunLockedEventHandler();
	[Signal]
	public delegate void MoonLockedEventHandler();
	[Signal]
	public delegate void UnlockEventHandler();
	[Signal]
	public delegate void PlayerDeathEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if (!alive) return;
		//always be rotating
		this.Rotation += _rotationSpeed * (float)delta;
        //set distance between bodies
        PositionBodies((float)delta);
		//locked behaviors
		if(_locked)
		{
			SetShifter();
			CalculateLaunchVector();
			FrictionMovement((float)delta);
        }
		//unlocked behaviors
		else
		{
            FreeMovement((float)delta);
        }
		if(bounceTimer > 0) bounceTimer -= (float)delta;
    }

	public void CalculateGravity()
	{
        //calculate gravity
        Vector2 newGravity = Vector2.Zero;
        bool splitGravity = false;
        if (((PlayerPlanet)sun).Gravity != Vector2.Zero)
        {
            newGravity += ((PlayerPlanet)sun).Gravity;
            splitGravity = true;
        }
        if (((PlayerPlanet)moon).Gravity != Vector2.Zero)
        {
            newGravity += ((PlayerPlanet)moon).Gravity;
        }
        else
        {
            splitGravity = false;
        }
        if (splitGravity) newGravity /= 2;
        _gravity = newGravity;
    }

	private void PositionBodies(float delta)
    {
        //do distancing
        if (upHeld && bounceTimer <= 0)
        {
            _bodyDistance = Mathf.MoveToward(_bodyDistance, maxDistance, distanceSpeed * delta);
        }
        if (downHeld && bounceTimer <= 0)
        {
            _bodyDistance = Mathf.MoveToward(_bodyDistance, minDistance, distanceSpeed * delta);
        }
		//position elements
        sun.Position = new Vector2(_bodyDistance, 0);
        moon.Position = new Vector2(-_bodyDistance, 0);
    }

	private void CalculateLaunchVector()
	{
        //set shifter position
        if (_lockedBody) shifter.Position = new Vector2(-_bodyDistance, 0);
        else shifter.Position = new Vector2(_bodyDistance, 0);
        //quantize launch vector
		_moveDirection = QuantizeVector(this.Transform.Y) * (_lockedBody ? -1 : 1) * (_rotationSpeed > 0 ? 1 : -1);
		//quantize launch speed?
        _moveSpeed = Mathf.Abs(_rotationSpeed * (_bodyDistance));
		if(_moveSpeed < _minSpeed) _moveSpeed = _minSpeed;
    }

	private void SetShifter()
	{
        //set shifter position
		if(_locked)
		{
            if (_lockedBody) shifter.Position = new Vector2(-_bodyDistance, 0);
            else shifter.Position = new Vector2(_bodyDistance, 0);
        }
		else
		{
			shifter.Position = Vector2.Zero;
		}
    }

	private void FrictionMovement(float delta)
	{
		//don't do slippery movement if friction is turned off
		if (_floorFriction <= 0) return;
		//do slipping
		frictionMovement += _moveDirection * slipInfluence * (_bodyDistance / maxDistance);
		frictionMovement += _gravity * gravityFriction * delta;
		frictionMovement = frictionMovement.MoveToward(Vector2.Zero, _floorFriction * delta);
		this.Position += frictionMovement * delta;
        //do ground check
		if (!CheckFloor(_lockedBody))
		{
			UnlockBody();
			Velocity = QuantizeVector(frictionMovement) * frictionMovement.Length();
		}
	}

	private void FreeMovement(float delta)
	{
        //apply gravity to movement vector
        this.Velocity += _gravity * delta;
		//maintain max speed
		if(_moveSpeed > _maxSpeed) _moveSpeed = _maxSpeed;
        //move player launchways
        this.Position += _moveDirection * (_moveSpeed * delta);
    }

	private void Bounce(KinematicCollision2D coll, bool sun)
	{
		//prevent reverse bounces
		if ((GetPlanetMotion(sun).Normalized() + _moveDirection).Normalized().Dot(coll.GetNormal()) > 0.1f) return;
        //do bounce
        if (_locked)
		{
            //calculate friction rebound angle
            frictionMovement = -frictionMovement.Reflect(coll.GetNormal());
            _rotationSpeed *= -1;
		}
		else
		{
			if (bounceTimer > 0.15f) return;
            //calculate rebound angle
			if(_moveDirection.Dot(coll.GetNormal()) < 0.1f)
			{
                _moveDirection = -_moveDirection.Reflect(coll.GetNormal());
            }
            //parallel edge case
			if(Mathf.Abs(_moveDirection.Dot(coll.GetNormal())) <= 0.1f)
			{
				_moveDirection = (_moveDirection + coll.GetNormal()).Normalized();
			}
            //quantize angle
            _moveDirection = QuantizeVector(_moveDirection);
            _rotationSpeed *= -1;
        }
        //rotate out of wall
        this.Rotation -= (coll.GetDepth() / _bodyDistance) * Mathf.Sign(_rotationSpeed) * 2;
        //move out of wall
        this.Position += coll.GetNormal() * coll.GetDepth();
        //play sound
        AudioSystem.PlaySFX("Bounce");
        bounceTimer = 0.2f;
    }

    public override void _Input(InputEvent @event)
    {
		if (!alive) return;
		//get lock actions
		if(@event.IsActionPressed("Btn_A"))
		{
			if(!_locked || (_locked && !_lockedBody))
			{
				LockSun();
			}
		}
		if(@event.IsActionReleased("Btn_A"))
		{
			if(_locked && _lockedBody)
			{
				UnlockBody();
				AudioSystem.PlaySFX("Launch");
			}
		}
        if (@event.IsActionPressed("Btn_B"))
        {
            if (!_locked || (_locked && _lockedBody))
            {
                LockMoon();
            }
        }
        if (@event.IsActionReleased("Btn_B"))
        {
            if (_locked && !_lockedBody)
            {
                UnlockBody();
                AudioSystem.PlaySFX("Launch");
            }
        }
        //get distance input state
        if (@event.IsActionPressed("Dpad_Up"))
        {
            upHeld = true;
			downHeld = false;
        }
		else if (@event.IsActionReleased("Dpad_Up"))
		{
			upHeld = false;
		}
        if (@event.IsActionPressed("Dpad_Down"))
        {
            downHeld = true;
            upHeld = false;
        }
        else if (@event.IsActionReleased("Dpad_Down"))
        {
            downHeld = false;
        }
    }

	private void UnlockBody()
	{
        this.Position = sun.GlobalPosition.Lerp(moon.GlobalPosition, 0.5f);
        shifter.Position = Vector2.Zero;
		_locked = false;
		RemoveIce();
		EmitSignal(SignalName.Unlock);
	}

	private void LockSun()
	{
        //do lockability check here
        if (!CheckFloor(true)) return;
		if(!alive) return;

        if (_locked)
		{
			this.Position = sun.GlobalPosition;
			shifter.Position = new Vector2(-_bodyDistance, 0);
		}
		_locked = true;
		_lockedBody = true;
		this.Position = sun.GlobalPosition;
        frictionMovement = Velocity;
        //do floor type check
        CheckFloor(true);
        EmitSignal(SignalName.SunLocked);
		AudioSystem.PlaySFX("Lock");
	}

    private void LockMoon()
    {
        //do lockability check here
        if (!CheckFloor(false)) return;
        if (!alive) return;

        if (_locked)
        {
            this.Position = moon.GlobalPosition;
            shifter.Position = new Vector2(_bodyDistance, 0);
        }
        _locked = true;
        _lockedBody = false;
        this.Position = moon.GlobalPosition;
		frictionMovement = Velocity;
		//do floor type check
		CheckFloor(false);
        EmitSignal(SignalName.MoonLocked);
        AudioSystem.PlaySFX("Lock");
    }

	private void Die(bool sun)
	{
		GD.Print("Fuck!");
		AudioSystem.PlaySFX("Explode");
        alive = false;
		EmitSignal(SignalName.PlayerDeath);
		_moveDirection = Vector2.Zero;
		_rotationSpeed = 0;
		if (sun) ((PlayerPlanet)this.sun).Explode();
		else ((PlayerPlanet)this.moon).Explode();
    }

	public void OnCollision(KinematicCollision2D coll, bool sun)
	{
		//check wall type
        if (coll.GetCollider() is TileMap)
        {
			TileMap tileMap = coll.GetCollider() as TileMap;
			Vector2I tilePoint = tileMap.LocalToMap(tileMap.ToLocal(coll.GetPosition() - (coll.GetNormal() * 4)));
			TileData hitTile = tileMap.GetCellTileData(0, tilePoint);
			if(hitTile != null)
			{
                Variant data = hitTile.GetCustomData("Type");
                if (data.VariantType is Variant.Type.String && ((string)data).Equals("Bounce"))
                {
					//for bounce walls
                    GD.Print("Bounce!");
                    Bounce(coll, sun);
                }
				else
				{
					//for normal walls
					Die(sun);
				}
            }
        }
    }

	//TODO: WE ALWAYS DO TRUE FOR SUN FALSE FOR MOON WHEN WE DO BOOLS TO DISTINGUISH BODIES
	private bool CheckFloor(bool sun)
	{
        ShapeCast2D shape = sun ? sunCast : moonCast;
		Node2D res = null;

		shape.ForceShapecastUpdate();
		int ct = shape.GetCollisionCount();

		for (int i = 0; i < ct; i++)
		{
            Node2D body = shape.GetCollider(i) as Node2D;
            if (body != null && body.IsInGroup("Floor"))
			{
				if(body is TileMap)
				{
                    TileMap map = body as TileMap;
                    Vector2I tilePos = map.LocalToMap(sun ? this.sun.GlobalPosition : this.moon.GlobalPosition);
                    TileData hitTile = map.GetCellTileData(0, tilePos);
                    if (hitTile == null) continue;
                    Variant data = hitTile.GetCustomData("Type");
					ResolveFloorType((string)data, sun);
					return true;
                }
				//if body is movingplatform
				//return true;
            }
        }
		return false;
	}

	public Vector2 GetPlanetMotion(bool sun)
	{
		Vector2 result = Vector2.Zero;
		if (_locked && sun == _lockedBody) return result;
		result = (this.Transform.Y * (_rotationSpeed * _bodyDistance)) * (sun ? 1 : -1);
		if(upHeld && _bodyDistance < maxDistance)
		{
			result += this.Transform.X * distanceSpeed * (sun ? 1 : -1);
		}
		if(downHeld && _bodyDistance > minDistance)
		{
            result -= this.Transform.X * distanceSpeed * (sun ? 1 : -1);
        }
		return result;
	}

	private void ResolveFloorType(String type, bool sun)
	{
		switch(type)
		{
			case "Floor":
				RemoveIce();
				break;
			case "Ice":
				ApplyIce();
				break;
			case "SunFloor":
				if (!sun) Die(sun);
				break;
			case "MoonFloor":
				if (sun) Die(sun);
				break;
			default:
				break;
		}
	}

	private void ApplyIce()
	{
		_floorFriction = 25;
	}

	private void RemoveIce()
	{
		_floorFriction = -10;
	}

	private Vector2 QuantizeVector(Vector2 value)
	{
		return Vector2.FromAngle(Mathf.DegToRad(Mathf.Round(Mathf.RadToDeg(value.Angle()) / quantization) * quantization));
    }
}
