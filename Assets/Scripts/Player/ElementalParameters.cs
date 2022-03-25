using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class ElementalParameters
    {
        public Color bulletColour;
    }

    public class NoneParameters : ElementalParameters
    {
        public NoneParameters(Color BulletColour)
        {
            bulletColour = BulletColour;
        }
    }

    public class ElectricParameters : ElementalParameters
    {
        public float damage;
        public int targets;
        public int repeat;
        public float delay;
        public float range;

        public ElectricParameters(Color BulletColour, float Damage, int Targets, int Repeat, float Delay, float Range)
        {
            bulletColour = BulletColour;
            damage = Damage;
            targets = Targets;
            repeat = Repeat;
            delay = Delay;
            range = Range;
        }
    }

    public class FireParameters : ElementalParameters
    {
        public float damage;
        public float duration;
        public float delay;
        public int speedBoost;

        public FireParameters(Color Bulletcolour, float Damage, float Duration, float Delay, int SpeedBoost)
        {
            bulletColour = Bulletcolour;
            damage = Damage;
            duration = Duration;
            delay = Delay;
            speedBoost = SpeedBoost;
        }
    }
    
    public class IceParameters : ElementalParameters
    {
        public float duration;
        public int speedDrop;
        
        public IceParameters(Color Bulletcolour, float Duration, int SpeedDrop)
        {
            bulletColour = Bulletcolour;
            duration = Duration;
            speedDrop = SpeedDrop;
        }
    }
    
    public class PoisonParameters : ElementalParameters
    {
        public float damage;
        public float duration;
        public float delay;
        public float cloudSize;
        public float cloudDuration;
        
        public PoisonParameters(Color Bulletcolour, float Damage, float Duration, float Delay, float CloudSize, float CloudDuration)
        {
            bulletColour = Bulletcolour;
            damage = Damage;
            duration = Duration;
            delay = Delay;
            cloudSize = CloudSize;
            cloudDuration = CloudDuration;
        }
    }
    
    public class EarthParameters : ElementalParameters
    {
        public float damage;
        public float duration;
        public int repeat;
        public float delay;
        public float range;
        
        public EarthParameters(Color Bulletcolour, float Damage, float Duration, int Repeat, float Delay, float Range)
        {
            bulletColour = Bulletcolour;
            damage = Damage;
            duration = Duration;
            repeat = Repeat;
            delay = Delay;
            range = Range;
        }
    }

    public class AirParameters : ElementalParameters
    {
        public float damage;
        public float force;
        public int repeat;
        public float range;

        public AirParameters(Color BulletColour, float Damage, float Force, int Repeat, float Range)
        {
            bulletColour = BulletColour;
            damage = Damage;
            force = Force;
            repeat = Repeat;
            range = Range;
        }
    }
}