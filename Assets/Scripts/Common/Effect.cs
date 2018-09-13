using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect {

    public EffectType type = new EffectType();
    public int minValue = 0;
    public int maxValue = 0;

    public Effect(Effect effect)
    {
        this.type = effect.type;
        this.minValue = effect.minValue;
        this.maxValue = effect.maxValue;
    }

    public Effect(EffectType type, int minValue, int maxValue)
    {
        this.type = type;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}
