using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Incteractive
{
    public class ParadoxManager : MonoBehaviour
    {
        public GameObject itemParadoxPrefab;
        public GameObject characterParadoxPrefab;

        private List<Character> characters;

        private List<ParadoxEffect> itemParadoxEffectPool = new List<ParadoxEffect>();
        private List<ParadoxEffect> characterParadoxEffectPool = new List<ParadoxEffect>();

        public void Setup(List<Character> characters)
        {
            this.characters = characters;
        }

        public void UpdateParadoxes(float time)
        {
            int activeItemParadoxes = 0;
            int activeCharacterParadoxe = 0;

            for (int i = 0, count = characters.Count; i < count; i++)
            {
                Character character = characters[i];

                for (int s = 0, paradoxCount = character.currentParadoxes.Count; s < paradoxCount; s++) 
                {
                    Paradox paradox = character.currentParadoxes[s];
                    bool warning = paradox.time > time;

                    if (paradox.type == Paradox.ParadoxType.Character)
                    {
                        if (warning == false)
                        {
                            ParadoxEffect characterParadoxEffect = GetCharacterParadox(activeCharacterParadoxe);
                            characterParadoxEffect.transform.position = paradox.character.transform.position;

                            if (warning)
                            {
                                characterParadoxEffect.main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow, Color.black);
                                characterParadoxEffect.main.startSize = 0.1f;
                                characterParadoxEffect.main.maxParticles = 50;
                                characterParadoxEffect.emission.rateOverTime = 50;
                            }
                            else
                            {
                                characterParadoxEffect.main.startColor = new ParticleSystem.MinMaxGradient(Color.red, Color.black);
                                characterParadoxEffect.main.startSize = 0.2f;
                                characterParadoxEffect.main.maxParticles = 300;
                                characterParadoxEffect.emission.rateOverTime = 300;
                            }

                            activeCharacterParadoxe++;
                        }
                    }
                    else if(paradox.type == Paradox.ParadoxType.Item)
                    {
                        ParadoxEffect itemParadoxEffect = GetItemParadox(activeItemParadoxes);
                        itemParadoxEffect.main.startColor = new ParticleSystem.MinMaxGradient((warning ? Color.yellow : Color.red), Color.black);
                        itemParadoxEffect.transform.position = paradox.itemProfile.position;
                        activeItemParadoxes++;
                    }
                }
            }
                
            for (int i = activeItemParadoxes, count = itemParadoxEffectPool.Count; i < count; i++) 
            {
                itemParadoxEffectPool[i].gameObject.SetActive(false);
            }

            for (int i = activeCharacterParadoxe, count = characterParadoxEffectPool.Count; i < count; i++) 
            {
                characterParadoxEffectPool[i].gameObject.SetActive(false);
            }
        }

        private ParadoxEffect GetItemParadox(int index)
        {
            ParadoxEffect paradoxEffect;

            if (itemParadoxEffectPool.Count > index)
            {
                paradoxEffect = itemParadoxEffectPool[index]; 
            }
            else
            {
                GameObject newParadox = Instantiate(itemParadoxPrefab, transform);
                paradoxEffect = newParadox.GetComponent<ParadoxEffect>();

                itemParadoxEffectPool.Add(paradoxEffect);
            }

            paradoxEffect.gameObject.SetActive(true);
            return paradoxEffect;
        }

        private ParadoxEffect GetCharacterParadox(int index)
        {
            ParadoxEffect paradoxEffect;

            if (characterParadoxEffectPool.Count > index)
            {
                paradoxEffect = characterParadoxEffectPool[index]; 
            }
            else
            {
                GameObject newParadox = Instantiate(characterParadoxPrefab, transform);
                paradoxEffect = newParadox.GetComponent<ParadoxEffect>();

                characterParadoxEffectPool.Add(paradoxEffect);
            }

            paradoxEffect.gameObject.SetActive(true);
            return paradoxEffect;
        }

        public bool GetParadoxFound()
        {
            for (int i = 0, count = characters.Count; i < count; i++)
            {
                if (characters[i].currentParadoxes.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}