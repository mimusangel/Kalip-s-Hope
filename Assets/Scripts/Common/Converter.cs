using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Converter {

	public static List<Effect> ParseEffects(string buffer)
    {
        List<Effect> list = new List<Effect>();

        if (buffer == null || buffer == "")
            return (list);

        string[] effects = buffer.Split('|');
        foreach (string effect in effects)
        {
            string[] values = effect.Split(';');
            EffectType type = new EffectType();
            int minValue = 1;
            int maxValue = 1;

            if (values.Length > 0) type = GameManager.instance.GetEffect(Convert.ToInt32(values[0]));
            if (values.Length > 1) minValue = Convert.ToInt32(values[1]);
            if (values.Length > 2) maxValue = Convert.ToInt32(values[2]);
            list.Add(new Effect(type, minValue, maxValue));
        }
        return (list);
    }

    public static string FormatEffects(List<Effect> effects)
    {
        string format = "";

        if (effects.Count > 0)
        {
            foreach (Effect effect in effects)
                format += effect.type.id + ";" + effect.minValue + ";" + effect.maxValue + "|";
            format = format.Remove(format.Length - 1);
        }
        return (format);
    }
}
