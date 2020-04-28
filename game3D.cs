using System;
using System.Drawing;
using System.Windows.Forms;

// mod: setRoom1 doesn't repeat over and over again
//chrissy




//Kalyn

namespace ACFramework
{ 
	class cCritterDoor : cCritterWall 
	{

	    public cCritterDoor(cVector3 enda, cVector3 endb, float thickness, float height, cGame pownergame ) 
		    : base( enda, endb, thickness, height, pownergame ) 
	    { 
	    }
		
		public override bool collide( cCritter pcritter ) 
		{ 
			bool collided = base.collide( pcritter ); 
			if ( collided && pcritter is cCritter3DPlayer ) 
			{ 
				(( cGame3D ) Game ).setdoorcollision( ); 
				return true; 
			} 
			return false; 
		}

        public override string RuntimeClass
        {
            get
            {
                return "cCritterDoor";
            }
        }
	} 
	
	//==============Critters for the cGame3D: Player, Ball, Treasure ================ 


	class cCritter3DPlayer : cCritterArmedPlayer 
	{ 
        private bool warningGiven = false;
		private char Mode = 'Q';
        public cCritter3DPlayer( cGame pownergame ) 
            : base( pownergame ) 
		{ 
			BulletClass = new cCritter3DPlayerBullet( );
			Sprite = new cSpriteQuake(ModelsMD2.Yoshi);
			Sprite.ModelState = State.Idle;
			Sprite.SpriteAttitude = cMatrix3.scale( 2, 0.8f, 0.4f ); 
			setRadius( cGame3D.PLAYERRADIUS ); //Default cCritter.PLAYERRADIUS is 0.4.  
			setHealth( 10 ); 
			moveTo( _movebox.LoCorner + new cVector3( 0.0f, 0.0f, 2.0f )); 
			WrapFlag = cCritter.CLAMP; //Use CLAMP so you stop dead at edges.
			Armed = true; //Let's use bullets.
			MaxSpeed =  cGame3D.MAXPLAYERSPEED; 
			AbsorberFlag = true; //Keeps player from being buffeted about.
			ListenerAcceleration = 160.0f; //So Hopper can overcome gravity.  Only affects hop.
		
            // YHopper hop strength 12.0
			Listener = new cListenerQuakeScooterYHopper( 0.2f, 12.0f ); 
            // the two arguments are walkspeed and hop strength -- JC
            
            addForce( new cForceGravity( 50.0f )); /* Uses  gravity. Default strength is 25.0.
			Gravity	will affect player using cListenerHopper. */ 
			AttitudeToMotionLock = false; //It looks nicer is you don't turn the player with motion.
			Attitude = new cMatrix3( new cVector3(0.0f, 0.0f, -1.0f), new cVector3( -1.0f, 0.0f, 0.0f ), 
                new cVector3( 0.0f, 1.0f, 0.0f ), Position); 
		}

        public override void update(ACView pactiveview, float dt)
        {
            base.update(pactiveview, dt); //Always call this first
            if (!warningGiven && distanceTo(new cVector3(Game.Border.Lox, Game.Border.Loy,
                Game.Border.Midz)) < 3.0f)
            {
                warningGiven = true;
                MessageBox.Show("You must have a score of 6 to go through this door and then a score of 20 to go to the next one");
				
            }
 
        } 

        public override bool collide( cCritter pcritter ) 
		{ 
			bool playerhigherthancritter = Position.Y - Radius > pcritter.Position.Y; 
		/* If you are "higher" than the pcritter, as in jumping on it, you get a point
	and the critter dies.  If you are lower than it, you lose health and the
	critter also dies. To be higher, let's say your low point has to higher
	than the critter's center. We compute playerhigherthancritter before the collide,
	as collide can change the positions. */
            _baseAccessControl = 1;
			bool collided = base.collide( pcritter );

			//
			//add sound into here!!! YOSHI HITTING SOMETHING


            _baseAccessControl = 0;
            if (!collided) 
				return false;
		/* If you're here, you collided.  We'll treat all the guys the same -- the collision
	 with a Treasure is different, but we let the Treasure contol that collision. */ 
			
			if ( playerhigherthancritter ) 
			{
				// add sound that will make a yoshi noise that will let yoshi make a wierd crushing noises 
				//Framework.snd.play(Sound.Stomp);
                //Framework.snd.play(Sound.Goopy); 
				Framework.snd.play(Sound.Bangbang);
				addScore( 10 ); 
				//adding the coin noise so that when you land it will add a coin noise
				//Framework.snd.play(Sound.Coin)
			} 
			else 
			{ 

				if(pcritter.Sprite.ModelState != State.FallForwardDie && pcritter.Sprite.ModelState != State.FallbackDie)
				{
					damage( 1 );
					Framework.snd.play(Sound.LaserFire); 
				}
			
				//Add sound file here for yoshi in tounge spitting
                //added just need sound file added
				//Framework.snd.play(Sound.laserFireUSing);
				Framework.snd.play(Sound.Pop); 

			} 
			pcritter.die(); 
			return true; 
		}

        public override cCritterBullet shoot()
        {
            //Framework.snd.play(Sound.Spit);
            return base.shoot();
        }

        public override string RuntimeClass
        {
            get
            {
                return "cCritter3DPlayer";
            }
        }
		public char Mode1{ get => Mode; set => Mode = value;}
	} 
	
   
	class cCritter3DPlayerBullet : cCritterBullet 
	{

        public cCritter3DPlayerBullet() { }

        public override cCritterBullet Create()
            // has to be a Create function for every type of bullet -- JC
        {
            return new cCritter3DPlayerBullet();
        }
		
		public override void initialize( cCritterArmed pshooter ) //!!!!!!! BULLET TYPE CHANGED HERE
		{ 
            //Sprite.FillColor = Color.Crimson;
			base.initialize( pshooter );  // calls the cCritterBullet initialize 
			if(((cCritter3DPlayer)pshooter).Mode1 == 'Q')
			{
				Sprite = new cSpriteSphere(0.2f);
				Sprite.FillColor = Color.Purple;
				Framework.snd.play(Sound.Crunch);
			}
			else if(((cCritter3DPlayer)pshooter).Mode1 == 'W')
			{
				Framework.snd.play(Sound.Clap);
				Sprite = new cSpriteQuake(ModelsMD2.bunny);
				Radius = 0.2f;
			}
			else //E
			{
				Framework.snd.play(Sound.Goopy);
				Sprite = new cSpriteSphere(0.2f);
				Sprite.FillColor = Color.Green;
			}
            // can use setSprite here too
            setRadius(0.1f);
		} 

		 public override bool collide(cCritter pcritter)
         {
            if(isTarget(pcritter) && touch(pcritter))
            {
                if (((cCritter3DPlayer)Player).Mode1 == 'Q') //1 point
				{

					
					Random rnd = new Random();
					int randomDeath = rnd.Next (1, 3);
					//pcritter.Radius = originalRadius;
					if(randomDeath == 1)
					{
						pcritter.Sprite.ModelState = State.FallbackDie;
					}
					if(randomDeath == 2)
					{
						pcritter.Sprite.ModelState = State.FallForwardDie;
					}
					pcritter.clearForcelist();
					pcritter.addForce(new cForceDrag(50.0f));
					pcritter.addForce(new cForceGravity(25.0f, new cVector3(0, -1, 0)));
					pcritter.Radius = 0;
					Player.addScore(1);
				}
                     
                else if(((cCritter3DPlayer)Player).Mode1 == 'W')//double damage - 3 points
                {

                   //take away twice the amount of health from boss -- how????
				   //add double score for regular chickens
				   Player.addScore(3);

                }
				else if(((cCritter3DPlayer)Player).Mode1 == 'E') //shrink ray - 2 points
				{
					//figure out how to kill them after 2 hits
					if(pcritter.Radius < 0.2)
					{
						/*probably change to just die, not state change
						pcritter.Sprite.ModelState = State.FallbackDie;
						pcritter.clearForcelist();
						pcritter.addForce(new cForceDrag(50.0f));
						pcritter.addForce(new cForceGravity(25.0f, new cVector3(0, -1, 0)));*/
						//add more score for killing
						pcritter.Radius = 0;
						Player.addScore(2);
					}
						
					else
					{
						pcritter.Radius = 0.9f * pcritter.Radius;
						Player.addScore(2); //same touch/radius issue as Q
					}
						
					
				}
               
                return true;
            }
            return false;
        }



        public override string RuntimeClass
        {
            get
            {
                return "cCritter3DPlayerBullet";
            }
        }
	} 
	

	class cCritter3Dcharacter : cCritter  
	{ 
		public bool alive = true;

	    public bool Alive{ get => alive; set => alive = value;}


        public cCritter3Dcharacter( cGame pownergame ) 
            : base( pownergame ) 
		{
            addForce(new cForceGravity(25.0f, new cVector3(0.0f, -1, 0.00f)));
            addForce(new cForceDrag(0.5f));  // default friction strength 0.5 
			Density = 2.0f; 
			MaxSpeed = 30.0f;
            if (pownergame != null) //Just to be safe.
                Sprite = new cSpriteQuake(Framework.models.selectRandomCritter());
            
            if ( Sprite is cSpriteQuake ) //Don't let the figurines tumble.  
			{ 
				AttitudeToMotionLock = false;   
				Attitude = new cMatrix3( new cVector3( 0.0f, 0.0f, 1.0f ), 
                    new cVector3( 1.0f, 0.0f, 0.0f ), 
                    new cVector3( 0.0f, 1.0f, 0.0f ), Position); 
				/* Orient them so they are facing towards positive Z with heads towards Y. */ 
			} 
			Bounciness = 0.0f; //Not 1.0 means it loses a bit of energy with each bounce.
			setRadius( 1.0f );
            MinTwitchThresholdSpeed = 4.0f; //Means sprite doesn't switch direction unless it's moving fast 
		
			randomizePosition(new cRealBox3(new cVector3(_movebox.Lox, _movebox.Loy, _movebox.Loz + 4.0f),
                new cVector3(_movebox.Hix, _movebox.Loy, _movebox.Midz - 1.0f)));
				/* I put them ahead of the player  */ 
			//randomizeVelocity( 0.0f, 30.0f, false ); 

                        
			if ( pownergame != null ) //Then we know we added this to a game so pplayer() is valid 
				addForce( new cForceObjectSeek( Player, 1.0f ));

			
            int begf = Framework.randomOb.random(0, 171);
            int endf = Framework.randomOb.random(0, 171);

            if (begf > endf)
            {
                int temp = begf;
                begf = endf;
                endf = temp;
            }

			Sprite.setstate( State.Other, begf, endf, StateType.Repeat );

            _wrapflag = cCritter.BOUNCE;

		} 

		
		public override void update( ACView pactiveview, float dt ) 
		{ 
			

			base.update( pactiveview, dt ); //Always call this first
			rotateAttitude(Tangent.rotationAngle(AttitudeTangent));
			
			if(alive){
				//draw critters to player to attack (tutorial 2)
				if(distanceTo(Player) >=10){
					clearForcelist();
					Sprite.ModelState = State.Idle;
				//	addForce(new cForceDrag(0.0f));
				//	addForce(new cForceGravity(25.0f, new cVector3(0.0f,-1f,0.00f)));
				}
				else if(distanceTo(Player) <  10){
					clearForcelist();
					addForce(new cForceGravity(25.0f, new cVector3(0.0f, -1f, 0.00f)));
					addForce(new cForceDrag(0.0f));
					addForce(new cForceObjectSeek(Player, 0.5f));
					if(distanceTo(Player) > 8)
					{
						Sprite.ModelState = State.Crouch;
					}
					else
					{
						Sprite.ModelState = State.Run;
					}
				}
			}
			
		

			if ( (_outcode & cRealBox3.BOX_HIZ) != 0 ) /* use bitwise AND to check if a flag is set. */ 
				delete_me(); //tell the game to remove yourself if you fall up to the hiz.
        }

        // do a delete_me if you hit the left end 

       

        public override void die() 
		{ 
			//Player.addScore( Value ); 
			alive = false;
			//MessageBox.Show("in die");
		
			base.die();
		} 

        public override string RuntimeClass
        {
            get
            {
                return "cCritter3Dcharacter";
            }
        }
	} 

	class cCritter3DBoss: cCritter3Dcharacter{
		
		public cCritter3DBoss( cGame pownergame )  : base( pownergame ) 
		{
            addForce(new cForceGravity(25.0f, new cVector3( 0.0f, -1, 0.00f ))); 
			addForce( new cForceDrag( 20.0f ) );  // default friction strength 0.5 
			Density = 2.0f; 
			MaxSpeed = 30.0f;
            if (pownergame != null) //Just to be safe.
                Sprite = new cSpriteQuake(Framework.models.selectRandomCritter());
            
			//draw critters to player to attack (tutorial 2)
		
			if(distanceTo(Player) < 1){
				clearForcelist();
				addForce(new cForceGravity(25.0f, new cVector3(0.0f, -1f, 0.00f)));
				addForce(new cForceDrag(0.0f));
				addForce(new cForceObjectSeek(Player, 0.1f));
				Sprite.ModelState = State.Run;
			}

            // example of setting a specific model
            // setSprite(new cSpriteQuake(ModelsMD2.Knight));
            
            if ( Sprite is cSpriteQuake ) //Don't let the figurines tumble.  
			{ 
				AttitudeToMotionLock = false;   
				Attitude = new cMatrix3( new cVector3( 0.0f, 0.0f, 1.0f ), 
                    new cVector3( 1.0f, 0.0f, 0.0f ), 
                    new cVector3( 0.0f, 1.0f, 0.0f ), Position); 
				/* Orient them so they are facing towards positive Z with heads towards Y. */ 
			} 
			Bounciness = 0.0f; //Not 1.0 means it loses a bit of energy with each bounce.
			setRadius( 5.0f );
            MinTwitchThresholdSpeed = 4.0f; //Means sprite doesn't switch direction unless it's moving fast 
			randomizePosition( new cRealBox3( new cVector3( _movebox.Lox, _movebox.Loy, _movebox.Loz + 4.0f), 
				new cVector3( _movebox.Hix, _movebox.Loy, _movebox.Midz - 1.0f))); 
				/* I put them ahead of the player  */ 
			randomizeVelocity( 0.0f, 30.0f, false ); 

                        
			if ( pownergame != null ) //Then we know we added this to a game so pplayer() is valid 
				addForce( new cForceObjectSeek( Player, 0.5f ));

            int begf = Framework.randomOb.random(0, 171);
            int endf = Framework.randomOb.random(0, 171);

            if (begf > endf)
            {
                int temp = begf;
                begf = endf;
                endf = temp;
            }

			Sprite.setstate( State.Other, begf, endf, StateType.Repeat );

            _wrapflag = cCritter.BOUNCE;

		} 

		
		public override void update( ACView pactiveview, float dt ) 
		{ 
			base.update( pactiveview, dt ); //Always call this first
			rotateAttitude(Tangent.rotationAngle(AttitudeTangent));
			if ( (_outcode & cRealBox3.BOX_HIZ) != 0 ) /* use bitwise AND to check if a flag is set. */ 
				delete_me(); //tell the game to remove yourself if you fall up to the hiz.
        } 

		// do a delete_me if you hit the left end 
	
		public override void die() 
		{ 
			Player.addScore( 100 ); 
			base.die(); 
		} 

        public override string RuntimeClass
        {
            get
            {
                return "cCritter3Dcharacter";
            }
        }
	}

	class cCritter3DHealer: cCritter3Dcharacter{
			
        public cCritter3DHealer( cGame pownergame ) 
            : base( pownergame ) 
		{
            addForce(new cForceGravity(25.0f, new cVector3( 0.0f, -1, 0.00f ))); 
			addForce( new cForceDrag( 20.0f ) );  // default friction strength 0.5 
			Density = 2.0f; 
			MaxSpeed = 30.0f;

			
            if (pownergame != null) //Just to be safe.
                Sprite = new cSpriteQuake(ModelsMD2.Squirtle);
			
			Sprite.ModelState = State.Idle;

			//heal player if they get close enough
			if(distanceTo(Player) < 2){
				Player.addHealth(11);
			}
			
            // example of setting a specific model
            // setSprite(new cSpriteQuake(ModelsMD2.Knight));
            
            if ( Sprite is cSpriteQuake ) //Don't let the figurines tumble.  
			{ 
				AttitudeToMotionLock = false;   
				Attitude = new cMatrix3( new cVector3( 0.0f, 0.0f, 1.0f ), 
                    new cVector3( 1.0f, 0.0f, 0.0f ), 
                    new cVector3( 0.0f, 1.0f, 0.0f ), Position); 
				/* Orient them so they are facing towards positive Z with heads towards Y. */ 
			} 
			Bounciness = 0.0f; //Not 1.0 means it loses a bit of energy with each bounce.
			setRadius( 1.0f );
            MinTwitchThresholdSpeed = 4.0f; //Means sprite doesn't switch direction unless it's moving fast 
			randomizePosition( new cRealBox3( new cVector3( _movebox.Lox, _movebox.Loy, _movebox.Loz + 4.0f), 
				new cVector3( _movebox.Hix, _movebox.Loy, _movebox.Midz - 1.0f))); 
				/* I put them ahead of the player  */ 

                        
			if ( pownergame != null ) //Then we know we added this to a game so pplayer() is valid 
				addForce( new cForceObjectSeek( Player, 0.5f ));

            int begf = Framework.randomOb.random(0, 171);
            int endf = Framework.randomOb.random(0, 171);

            if (begf > endf)
            {
                int temp = begf;
                begf = endf;
                endf = temp;
            }

			Sprite.setstate( State.Other, begf, endf, StateType.Repeat );


            _wrapflag = cCritter.BOUNCE;

		} 

		
		public override void update( ACView pactiveview, float dt ) 
		{ 
			base.update( pactiveview, dt ); //Always call this first
			if ( (_outcode & cRealBox3.BOX_HIZ) != 0 ) /* use bitwise AND to check if a flag is set. */ 
				delete_me(); //tell the game to remove yourself if you fall up to the hiz.
        } 

		// do a delete_me if you hit the left end 
	
		public override void die() 
		{ 
			Player.addHealth( 26  ); 
			base.dieOfOldAge();
		} 

        public override string RuntimeClass
        {
            get
            {
                return "cCritter3DHealer";
            }
        }
	}


	
	class cCritterTreasure: cCritter 
	{   // Try jumping through this hoop
		
		public cCritterTreasure( cGame pownergame ) : 
		base( pownergame ) 
		{ 
			/* The sprites look nice from afar, but bitmap speed is really slow
		when you get close to them, so don't use this. */ 
			cShape ppoly = new cShape( 24 ); 
			ppoly.Filled = false;
            ppoly.LineColor = Color.LightGray;
			ppoly.LineWidthWeight = 0.5f;
			Sprite = ppoly; 
			_collidepriority = cCollider.CP_PLAYER + 1; /* Let this guy call collide on the
			player, as his method is overloaded in a special way. */ 
			rotate( new cSpin( (float) Math.PI / 2.0f, new cVector3(0.0f, 0.0f, 1.0f) )); /* Trial and error shows this
			rotation works to make it face the z diretion. */ 
			setRadius( cGame3D.TREASURERADIUS ); 
			FixedFlag = true;
            moveTo(new cVector3(_movebox.Midx, _movebox.Midy - 2.0f,
                _movebox.Loz - 1.5f * cGame3D.TREASURERADIUS));
		} 

		
		public override bool collide( cCritter pcritter ) 
		{ 
			if ( contains( pcritter )) //disk of pcritter is wholly inside my disk 
			{
                Framework.snd.play(Sound.Clap); 
				//pcritter.addScore( 100 ); 
				pcritter.addHealth( 1 ); 
				pcritter.moveTo( new cVector3( _movebox.Midx, _movebox.Loy + 1.0f,
                    _movebox.Hiz - 3.0f )); 
				return true; 
			} 
			else 
				return false; 
		} 

		//Checks if pcritter inside.
	
		public override int collidesWith( cCritter pothercritter ) 
		{ 
			if ( pothercritter is cCritter3DPlayer ) 
				return cCollider.COLLIDEASCALLER; 
			else 
				return cCollider.DONTCOLLIDE; 
		} 

		/* Only collide
			with cCritter3DPlayer. */ 

        public override string RuntimeClass
        {
            get
            {
                return "cCritterTreasure";
            }
        }
	} 

    //======================cGame3D========================== 

    class cGame3D : cGame 
	{ 
		public static readonly float TREASURERADIUS = 1.2f; 
		public static readonly float WALLTHICKNESS = 0.5f; 
		public static readonly float PLAYERRADIUS = 0.4f; 
		public static readonly float BOSSRADIUS = 0.9f;
		public static readonly float MAXPLAYERSPEED = 30.0f; 
		//private cCritterTreasure _ptreasure;
        private cCritterShape shape;
        private bool doorcollision;
        private bool wentThrough = false;
        private float startNewRoom;
		public  cCritter3DHealer critterHealer;

		public cGame3D() 
		{
			doorcollision = false; 
			_menuflags &= ~ cGame.MENU_BOUNCEWRAP; 
			_menuflags |= cGame.MENU_HOPPER; //Turn on hopper listener option.
			_spritetype = cGame.ST_MESHSKIN; 
			setBorder( 50.0f, 14.0f, 50.0f ); // size of the world
			
			setBorder( 64.0f, 16.0f, 64.0f ); // size of the world
			
			cRealBox3 skeleton = new cRealBox3();
            skeleton.copy(_border);
			setSkyBox( skeleton );
		/* In this world the coordinates are screwed up to match the screwed up
		listener that I use.  I should fix the listener and the coords.
		Meanwhile...
		I am flying into the screen from HIZ towards LOZ, and
		LOX below and HIX above and
		LOY on the right and HIY on the left. */ 
			SkyBox.setSideTexture( cRealBox3.HIZ, BitmapRes.Wall4 ); //back
			SkyBox.setSideTexture( cRealBox3.LOZ, BitmapRes.Wall4 ); //Far
			SkyBox.setSideTexture( cRealBox3.HIX, BitmapRes.Wall42 ); //Left
			SkyBox.setSideTexture( cRealBox3.LOX, BitmapRes.Wall42 ); //Right
			
			
			//SkyBox.setSideSolidColor( cRealBox3.HIZ, Color.ForestGreen ); //Make the near HIZ transparent 
			//SkyBox.setSideSolidColor( cRealBox3.LOZ, Color.ForestGreen ); //Far wall 
			//SkyBox.setSideSolidColor( cRealBox3.LOX, Color.ForestGreen ); //left wall 
            //SkyBox.setSideSolidColor( cRealBox3.HIX, Color.ForestGreen ); //right wall 
			SkyBox.setSideTexture( cRealBox3.LOY, BitmapRes.Wood2,16 ); //floor 
			SkyBox.setSideTexture( cRealBox3.HIY, BitmapRes.Sky,2 ); //ceiling 
			


			WrapFlag = cCritter.BOUNCE; 
			_seedcount = 5; 
			setPlayer( new cCritter3DPlayer( this )); 
			critterHealer = new cCritter3DHealer(this);

			//_ptreasure = new cCritterTreasure( this );
            shape = new cCritterShape(this);
            shape.moveTo(new cVector3( Border.Midx, Border.Hiy, Border.Midz ));

			/* In this world the x and y go left and up respectively, while z comes out of the screen.
		A wall views its "thickness" as in the y direction, which is up here, and its
		"height" as in the z direction, which is into the screen. */ 
			//First draw a wall with dy height resting on the bottom of the world.
			float zpos = 0.0f; /* Point on the z axis where we set down the wall.  0 would be center,
			halfway down the hall, but we can offset it if we like. */ 
			float height = 0.5f * _border.YSize; 
			float ycenter = -_border.YRadius + height / 2.0f; 
			float wallthickness = cGame3D.WALLTHICKNESS;
		
			//Then draw a ramp to the top of the wall.  Scoot it over against the right wall.
			float planckwidth = 8.5f * height; 

			//make a bunch of walls to create room layout
            cCritterWall pwall = new cCritterWall(
                new cVector3(_border.Midx + 0.0f, ycenter, zpos),
                new cVector3(_border.Hix, ycenter, zpos),
                height, 
                wallthickness, 
                this);

            cCritterWall pwall2 = new cCritterWall(
                new cVector3(_border.Midx - 2.0f, ycenter, zpos + 20.0f),
                new cVector3(_border.Hix - 32.0f, ycenter, zpos),
                height, 
                wallthickness, 
                this);
				
            cCritterWall pwall3 = new cCritterWall(
                new cVector3(_border.Midx - 80.0f, ycenter, zpos + 15.0f),
                new cVector3(_border.Hix - 45.0f, ycenter, zpos + 4.0f),
                height,
                wallthickness,                 this);

            cCritterWall pwall4 = new cCritterWall(
                new cVector3(_border.Midx - 2.0f, ycenter, zpos + 10.0f),
                new cVector3(_border.Hix - 2.0f, ycenter, zpos),
                height, 
                wallthickness, 
                this);

            cCritterWall pwall5 = new cCritterWall(
                new cVector3(_border.Midx - 20, ycenter, zpos - 5.0f),
                new cVector3(_border.Hix - 20, ycenter, zpos - 35.0f),
                height,
                wallthickness,
                this);

			cSpriteTextureBox pspritebox1 = new cSpriteTextureBox(pwall.Skeleton, BitmapRes.Wall3, 16); 
            cSpriteTextureBox pspritebox2 = new cSpriteTextureBox(pwall2.Skeleton, BitmapRes.Wall3, 16); 
            cSpriteTextureBox pspritebox3 = new cSpriteTextureBox(pwall3.Skeleton, BitmapRes.Wall3, 16);            
			cSpriteTextureBox pspritebox4 = new cSpriteTextureBox(pwall4.Skeleton, BitmapRes.Wall3, 16); 
            cSpriteTextureBox pspritebox5 = new cSpriteTextureBox(pwall5.Skeleton, BitmapRes.Wall3, 16); 

            pwall.Sprite = pspritebox1;
            pwall2.Sprite = pspritebox2;
            pwall3.Sprite = pspritebox3;
            pwall4.Sprite = pspritebox4;
            pwall5.Sprite = pspritebox5;
		
			cCritterDoor pdwall = new cCritterDoor( 
				new cVector3( _border.Lox, _border.Loy, _border.Midz ), 
				new cVector3( _border.Lox, _border.Midy - 3, _border.Midz ), 
				0.1f, 2, this ); 

			cSpriteTextureBox pspritedoor = 
				new cSpriteTextureBox( pdwall.Skeleton, BitmapRes.Door ); 
			pdwall.Sprite = pspritedoor;
			
		} 

        public void setRoom1( )
        {
			//bool gotToSecondRoom = true
            Biota.purgeCritters<cCritterWall>();
            Biota.purgeCritters<cCritter3Dcharacter>();
            Biota.purgeCritters<cCritterShape>();
            setBorder(80.0f, 15.0f, 50.0f); 
	        cRealBox3 skeleton = new cRealBox3();
            skeleton.copy( _border );
	        setSkyBox(skeleton);


	        SkyBox.setAllSidesTexture( BitmapRes.Y2Ground, 2 );
	        SkyBox.setSideTexture( cRealBox3.LOY, BitmapRes.YGround );
	        SkyBox.setSideSolidColor( cRealBox3.HIY, Color.Blue );
	        _seedcount = 20;
	        Player.setMoveBox( new cRealBox3( 80.0f, 15.0f, 50.0f ) );
            float zpos = 0.0f; /* Point on the z axis where we set down the wall.  0 would be center,
			halfway down the hall, but we can offset it if we like. */
            float height = 3.0f * _border.YSize;
			//this code makes the wall go up or down
            float ycenter = -_border.YRadius + height / 2.0f;
            float wallthickness = cGame3D.WALLTHICKNESS;
           

				//make a bunch of walls to create room layout
            cCritterWall pwall = new cCritterWall(
                new cVector3(_border.Midx + 0.0f, ycenter, zpos),
                new cVector3(_border.Hix, ycenter, zpos),
                height, 
                wallthickness, 
                this);

            cCritterWall pwall2 = new cCritterWall(
                new cVector3(_border.Midx - 2.0f, ycenter, zpos + 20.0f),
                new cVector3(_border.Hix - 32.0f, ycenter, zpos),
                height, 
                wallthickness, 
                this);
				
            cCritterWall pwall3 = new cCritterWall(
                new cVector3(_border.Midx - 80.0f, ycenter, zpos + 15.0f),
                new cVector3(_border.Hix - 45.0f, ycenter, zpos + 4.0f),
                height,
                wallthickness,                 this);

            cCritterWall pwall4 = new cCritterWall(
                new cVector3(_border.Midx - 2.0f, ycenter, zpos + 10.0f),
                new cVector3(_border.Hix - 2.0f, ycenter, zpos),
                height, 
                wallthickness, 
                this);

            /*cCritterWall pwall5 = new cCritterWall(
                new cVector3(_border.Midx - 20, ycenter, zpos - 5.0f),
                new cVector3(_border.Hix - 20, ycenter, zpos - 35.0f),
                height,
                wallthickness,
                this);
			*/
			cSpriteTextureBox pspritebox1 = new cSpriteTextureBox(pwall.Skeleton, BitmapRes.Wall3, 16); 
            cSpriteTextureBox pspritebox2 = new cSpriteTextureBox(pwall2.Skeleton, BitmapRes.Wall3, 16); 
            cSpriteTextureBox pspritebox3 = new cSpriteTextureBox(pwall3.Skeleton, BitmapRes.Wall3, 16);            
			cSpriteTextureBox pspritebox4 = new cSpriteTextureBox(pwall4.Skeleton, BitmapRes.Wall3, 16); 
         //   cSpriteTextureBox pspritebox5 = new cSpriteTextureBox(pwall5.Skeleton, BitmapRes.Wall3, 16); 

            pwall.Sprite = pspritebox1;
            pwall2.Sprite = pspritebox2;
            pwall3.Sprite = pspritebox3;
            pwall4.Sprite = pspritebox4;
           // pwall5.Sprite = pspritebox5;


            /* We'll tile our sprites three times along the long sides, and on the
        short ends, we'll only tile them once, so we reset these two. */
			//MADE DOOR HERE put in room one
			//------------------------------------------------------------------------------------------------
			cCritterDoor pdwall = new cCritterDoor( 
				new cVector3( _border.Lox, _border.Loy, _border.Midz ), 
				new cVector3( _border.Lox, _border.Midy - 3, _border.Midz ), 
				0.1f, 15, this ); 
			cSpriteTextureBox pspritedoor = 
				new cSpriteTextureBox( pdwall.Skeleton, BitmapRes.Door ); 
			pdwall.Sprite = pspritedoor;
			//---------------------------------------------------------------------------------------------------
            wentThrough = true;
            startNewRoom = Age;
			
		}
		
		public void setRoom2( )
        {
			_seedcount = 0;
            Biota.purgeCritters<cCritterWall>();
            Biota.purgeCritters<cCritter3Dcharacter>();
            Biota.purgeCritters<cCritterShape>();
            setBorder(80.0f, 15.0f, 50.0f); 
	        cRealBox3 skeleton = new cRealBox3();
            skeleton.copy( _border );
	        setSkyBox(skeleton);
	        SkyBox.setAllSidesTexture( BitmapRes.Graphics3, 2 );
	        SkyBox.setSideTexture( cRealBox3.LOY, BitmapRes.Wood2,16 );
	        SkyBox.setSideSolidColor( cRealBox3.HIY, Color.Red );
	        //create boss here
			//add healer here too
			new cCritter3DHealer(this);
			new cCritter3DBoss(this);
			//_seedcount = 1;

	        Player.setMoveBox( new cRealBox3( 80.0f, 15.0f, 50.0f ) );
            float zpos = 0.0f; /* Point on the z axis where we set down the wall.  0 would be center,
			halfway down the hall, but we can offset it if we like. */
            float height = 3.0f * _border.YSize;
			//this code makes the wall go up or down
            float ycenter = -_border.YRadius + height / 2.0f;
            float wallthickness = cGame3D.WALLTHICKNESS;
            
			//////////////////////////////////////////////////WALL NEEDS TO MOVE
			
			/*cCritterMovingWall pwall = new cCritterMovingWall(
                           new cVector3(-_border.Midx, -ycenter, -zpos),
                            new cVector3(-_border.Hix, -ycenter, -zpos),
                            height, //thickness param for wall's dy which goes perpendicular to the 
                            wallthickness, //height argument for this wall's dz  goes into the screen 
                            this);
			
            cSpriteTextureBox pspritebox =
                new cSpriteTextureBox(pwall.Skeleton, BitmapRes.Door, 16); //Sets all sides 
            /* We'll tile our sprites three times along the long sides, and on the
        short ends, we'll only tile them once, so we reset these two. */
          //  pwall.Sprite = pspritebox;
            wentThrough = true;
            startNewRoom = Age;
			
		}
		public override void seedCritters() 
		{
			Biota.purgeCritters<cCritterBullet>(); 
			Biota.purgeCritters<cCritter3Dcharacter>();
            for (int i = 0; i < _seedcount; i++) 
				new cCritter3Dcharacter( this );

            Player.moveTo(new cVector3(0.0f, Border.Loy, Border.Hiz - 3.0f)); 
				/* We start at hiz and move towards	loz */ 
		} 

		
		public void setdoorcollision( ) { doorcollision = true; } 
		
		public override ACView View 
		{
            set
            {
                base.View = value; //You MUST call the base class method here.
                value.setUseBackground(ACView.FULL_BACKGROUND); /* The background type can be
			    ACView.NO_BACKGROUND, ACView.SIMPLIFIED_BACKGROUND, or 
			    ACView.FULL_BACKGROUND, which often means: nothing, lines, or
			    planes&bitmaps, depending on how the skybox is defined. */
                value.pviewpointcritter().Listener = new cListenerViewerRide();
            }
		} 

		
		public override cCritterViewer Viewpoint 
		{ 
            set
            {
			    if ( value.Listener.RuntimeClass == "cListenerViewerRide" ) 
			    { 
				    value.setViewpoint( new cVector3( 0.0f, 0.3f, -1.0f ), _border.Center); 
					//Always make some setViewpoint call simply to put in a default zoom.
				    value.zoom( 0.35f ); //Wideangle 
				    cListenerViewerRide prider = ( cListenerViewerRide )( value.Listener); 
				    prider.Offset = (new cVector3( -1.5f, 0.0f, 1.0f)); /* This offset is in the coordinate
				    system of the player, where the negative X axis is the negative of the
				    player's tangent direction, which means stand right behind the player. */ 
			    } 
			    else //Not riding the player.
			    { 
				    value.zoom( 1.0f ); 
				    /* The two args to setViewpoint are (directiontoviewer, lookatpoint).
				    Note that directiontoviewer points FROM the origin TOWARDS the viewer. */ 
				    value.setViewpoint( new cVector3( 0.0f, 0.3f, 1.0f ), _border.Center); 
			    }
            }
		} 

		/* Move over to be above the
			lower left corner where the player is.  In 3D, use a low viewpoint low looking up. */ 
	
		public override void adjustGameParameters() 
		{
			bool room1Cleared = false;
		// (1) End the game if the player is dead 
			if ( (Health == 0) && !_gameover ) //Player's been killed and game's not over.
			{ 
				_gameover = true; 
			//	Player.addScore( _scorecorrection ); // So user can reach _maxscore  
                Framework.snd.play(Sound.Hallelujah);
                return ; 
			} 
		// (2) Also don't let the the model count diminish.
					//(need to recheck propcount in case we just called seedCritters).
			int modelcount = Biota.count<cCritter3Dcharacter>(); 
			int modelstoadd = _seedcount - modelcount; 
			for ( int i = 0; i < modelstoadd; i++) 
				new cCritter3Dcharacter( this ); 
		// (3) Maybe check some other conditions.
		int rmcnt =1;
            if (wentThrough && (Age - startNewRoom) > 1.0f)
            {
            
                wentThrough = false;
            }
			

      
			if (rmcnt ==1&& Score>=1&& doorcollision==true){
				rmcnt=rmcnt+1;
                setRoom1();
                doorcollision = false;
				room1Cleared = true;
			}
				//_____________________________________
			if (rmcnt ==2&& Score>=15 && room1Cleared == true){
				rmcnt=rmcnt+1;
                setRoom2();
                doorcollision = false;

			}
			if (Score >=10){
				
					//Framework.snd.play(Sound.Clap); 
					//Environment.Exit;
						//Application.Exit;
						//should be working but it doesnt seem to work
						//MessageBox.Show("Congratulations You beat Macho Chicken!!!");
						//obstacles
			}
				///////////////////////////////////////
				
            //}
				
			
		} 
	}
}

		
	
	





// MOVE WALL ADD SOUND FILES
// move wall is the next objective
// critter boss will be just like the healer class
// so chrissy can take care of that 
// move the score up as you continue into the game

	//CHECK RESPAWN PROBLEM IN ROOM 2 AND 3