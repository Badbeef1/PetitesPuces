using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public static class Extentions
    {
        //https://codereview.stackexchange.com/questions/90195/generic-method-to-split-provided-collection-into-smaller-collections
        /// <summary>
        /// Permet de transformer un liste a une dimension en une liste a deux dimensions, selen la dimemsion passer en parametre.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public static List<List<T>> Separe<T>(this IEnumerable<T> collection, int dimension)
        {
            var chunks = new List<List<T>>();
            var count = 0;
            var temp = new List<T>();

            foreach (var element in collection)
            {
                if (count++ == dimension)
                {
                    chunks.Add(temp);
                    temp = new List<T>();
                    count = 1;
                }
                temp.Add(element);
            }
            chunks.Add(temp);

            return chunks;
        }
    }
}