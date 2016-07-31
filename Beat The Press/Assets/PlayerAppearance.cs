using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PlayerAppearance : MonoBehaviour {

    public PlayerAppearanceSet defaultSet;

    private PlayerAppearanceSet currentSet;

    public PlayerAppearanceSet[] sets;

	// Use this for initialization
	void Awake () {

        var usedSets = new List<PlayerAppearanceSet>();

        foreach (var item in FindObjectsOfType<PlayerAppearance>()) {
            if (item == this)
                continue;

            usedSets.Add(item.currentSet);
        }

        var availableSets = new List<PlayerAppearanceSet>(sets);

        foreach (var used in usedSets) {
            availableSets.Remove(used);
        }

        defaultSet = availableSets[Random.Range(0, availableSets.Count)];

        UseAppearanceSet(defaultSet);


	}

    void Update() {
        if (Application.isEditor && Application.isPlaying == false) {
            UseAppearanceSet(defaultSet);
        }

        if (armUp != lastArmUp) {
            switch (armUp) {
            case true:
                SpriteRendererNamed("Character/Body/Arm").sprite = currentSet.armUpSprite;
                break;
            case false:
                SpriteRendererNamed("Character/Body/Arm").sprite = currentSet.armDownSprite;
                break;
            }
            lastArmUp = armUp;
        } 

        if (isSmiling != lastSmiling) {
            if (isSmiling) {
                SpriteRendererNamed("Character/Head/Mouth").sprite = currentSet.mouthSmileSprite;
            } else {
                SpriteRendererNamed("Character/Head/Mouth").sprite = currentSet.mouthFrownSprite;
            }
            lastSmiling = isSmiling;
        }


    }

    public bool armUp;
    private bool lastArmUp;

    public bool isSmiling;
    private bool lastSmiling;

    public Sprite teamSprite {
        get {
            return currentSet.teamSprite;
        }
    }
	
    public void UseAppearanceSet(PlayerAppearanceSet set) {

        currentSet = set;

        SpriteRendererNamed("Character/Body").sprite = set.bodySprite;
        SpriteRendererNamed("Character/Head").sprite = set.faceSprite;

        SpriteRendererNamed("Character/Head/Mouth").sprite = currentSet.mouthFrownSprite;

        SpriteRendererNamed("Desk/Team Indicator").sprite = currentSet.teamSprite;

        lastArmUp = !armUp;
        lastSmiling = !isSmiling;

    }

    public SpriteRenderer SpriteRendererNamed(string name) {
        return transform.Find(name).GetComponent<SpriteRenderer>();
    }


}
