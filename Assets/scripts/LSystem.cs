﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dictates the grammar of the LSystem strings
/// </summary>
public class LSystem : MonoBehaviour {

    public static int DIMS = 5;

    public List<LRule> rules;
    public List<LSymbol> definedSymbols;
    public List<LPatch> definedPatches;
    public LSymbol[][,] systemString;

	// Use this for initialization
	void Reset () {
        rules = new List<LRule>();
        definedSymbols = new List<LSymbol>();
        definedPatches = new List<LPatch>();

        LSymbol plains = new LSymbol('p', "plains");
        LSymbol hills = new LSymbol('h', "hills");
        LSymbol ocean = new LSymbol('o', "ocean");

        definedSymbols.Add(plains);
        definedSymbols.Add(hills);
        definedSymbols.Add(ocean);
        
        LSymbol grass = new LSymbol('g', "grass");
        LSymbol sand = new LSymbol('s', "sand");
        LSymbol beach = new LSymbol('b', "beach");

        definedSymbols.Add(grass);
        definedSymbols.Add(beach);
        definedSymbols.Add(sand);

        systemString = new LSymbol[1][,];
        systemString[0] = new LSymbol[3,3]{
            {ocean,  beach,  hills },
            {ocean,  beach,  hills },
            {ocean,  beach,  hills }
        };


        LRule plainsToGrass = LRule.CreatePropegateRule(plains, grass);
        rules.Add(plainsToGrass);
        plainsToGrass.name = "Pprgate Plains > Grass";
        
        LSymbol[,] beachRepl = new LSymbol[5, 5]{
            { ocean, sand, sand, grass, grass },
            { ocean, sand, sand, grass, grass },
            { ocean, ocean, sand, grass, grass },
            { ocean, ocean, sand, sand, grass },
            { ocean, sand, sand, grass, grass }
        };
        LRule detailBeach = LRule.CreateRule(beach, beachRepl);
        detailBeach.name = "Detail Beach";
        rules.Add(detailBeach);
    }

    public LSymbol[,] IterateLString(LSymbol[,] source)
    {
        int idim = source.GetLength(0);
        int jdim = source.GetLength(1);
        LSymbol[,] newSystemString = new LSymbol[DIMS * idim, DIMS * jdim];

        for (int i = 0; i < idim; ++i)
        {
            for (int j = 0; j < jdim; ++j)
            {
                LRule matchingRule = GetLRuleMatch(source[i, j]);

                for (int i2 = 0; i2 < DIMS; ++i2)
                {
                    for (int j2 = 0; j2 < DIMS; ++j2)
                    {
                        newSystemString[i*DIMS+i2,j*DIMS+j2] = matchingRule.replacementVals[i2,j2];
                    }
                }

            }
        }

        return newSystemString;
    }
	
	public LRule GetLRuleMatch(LSymbol toMatch)
    {
        foreach (LRule rule in rules)
        {
            if (toMatch == rule.matchVal)
                return rule;
        }

        //if no match found, create a rule to propegate toMatch (default behavior)
        return LRule.CreatePropegateRule(toMatch, toMatch);
    }

    public LPatch GetLPatchMatch(LSymbol toMatch)
    {
        foreach (LPatch patch in definedPatches)
        {
            if (toMatch == patch.matchVal)
                return patch;
        }

        //if no match found, create a new patch
        return new LPatch();
    }
}

public class LSymbol
{
    public LSymbol(char symbol, string name)
    {
        this.symbol = symbol;
        this.name = name;
    }

    public char symbol; //a single char to represent the LSymbol type ex 'g'
    public string name; //longer form name, ex. 'grass'
    public Texture tex; //image icon for symbol ex. picture of grass
}

public class LRule
{
    public static LRule CreateRule(LSymbol matchVal, LSymbol[,] replacementVals)
    {
        if (replacementVals.GetLength(0) != LSystem.DIMS || replacementVals.GetLength(0) != replacementVals.GetLength(1))
            return null;
        else
            return new LRule(matchVal, replacementVals);
    }

    public static LRule CreatePropegateRule(LSymbol matchVal, LSymbol propegateVal)
    {
        LSymbol[,] replacementVals = new LSymbol[LSystem.DIMS, LSystem.DIMS];
        for (int i = 0; i < LSystem.DIMS; ++i)
        {
            for (int j = 0; j < LSystem.DIMS; ++j)
            {
                replacementVals[i, j] = propegateVal;
            }
        }
        return CreateRule(matchVal, replacementVals);
    }

    private LRule(LSymbol matchVal, LSymbol[,] replacementVals)
    {
        this.matchVal = matchVal;
        this.replacementVals = replacementVals;
    }

    public string name;
    public LSymbol matchVal;
    public LSymbol[,] replacementVals;
}

public class LPatch
{
    public string name;
    public LSymbol matchVal;
    public float minHeight;
    public float maxHeight;
    public GameObject prefab; //TODO: change into list of procedural prefab parameters
    public Texture tex;
}