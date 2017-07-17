using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
    public struct ItemProfile
    {
        public int id;
        public bool showContent;
        public ItemProfile[] itemsInsideProfiles;
        public Vector3 position;

        public static bool operator == (ItemProfile p1, ItemProfile p2) 
        {
            return p1.Equals(p2);
        }

        public static bool operator != (ItemProfile p1, ItemProfile p2) 
        {
            return !p1.Equals(p2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            
            ItemProfile profile = (ItemProfile)obj;

            if (profile == null || profile.id != id || profile.showContent != showContent)
                return false;
            
            if (showContent)
            {
                int length = itemsInsideProfiles.Length;

                if (length != profile.itemsInsideProfiles.Length)
                    return false;
                
                for (int i = 0; i < length; i++)
                {
                    if (itemsInsideProfiles[i] != profile.itemsInsideProfiles[i])
                        return false;
                }
            }

            return true;
        }
        
        public bool CompareSurface(ItemProfile profile)
        {
            if (profile == null || profile.id != id || profile.showContent != showContent)
                return false;

            return true;           
        }

        public override string ToString()
        {
            string s = " " + id;

            if (showContent)
            {
                s += " [";   

                for (int i = 0, length = itemsInsideProfiles.Length; i < length; i++)
                {
                    s += itemsInsideProfiles[i].ToString();
                }

                s += "] ";
            }

            return s;
        }
    }
}
