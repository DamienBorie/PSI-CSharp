using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD2_test
{
    public class Pixel
    {
        #region Attributs
        byte red;
        byte green;
        byte blue;
        #endregion

        #region Propriétés
        //propriété "get" qui renvoie la valeur de la couleur demandée (nouvelle syntaxe)
        public byte Red => red;
        public byte Green => green;
        public byte Blue => blue;

        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur naturel qui prend les valeurs des 3 couleurs
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        public Pixel(byte blue, byte green, byte red)
        {
            this.blue = blue;
            this.green = green;
            this.red = red;    
        }
        /// <summary>
        /// constructeur pixel blanc prend en paramètre rien 
        /// renvoi vers le constructeur naturel avec en paramètre (255 255 255)
        /// </summary>
        public Pixel() : this(255, 255, 255) { }
        /// <summary>
        /// inutile
        /// constructeur qui prend un pixel en paramètre pour en recréer un autre tout pareil
        /// renvoi vers le constructeur naturel avec en paramètre les composantes du pixel que l'on veut cloner 
        /// </summary>
        /// <param name="Recopie"></param>
        public Pixel(Pixel clone) : this(clone.blue, clone.green, clone.red) { }
        #endregion

        #region Méthodes d'instances
        /// <summary>
        /// inutile
        /// </summary>
        /// <returns></returns>
        public string toString()
        {
            string a = " ";
            a = "Le pixel est composé de " + blue + " bits de bleu" + red + " bits de red" + green + "bits de vert";
            return a;
        }
        #endregion
        public static bool operator ==(Pixel PixelA,Pixel PixelB)
        {
            if(PixelA.Blue == PixelB.blue && PixelA.Green == PixelB.Green && PixelA.Red == PixelB.Red)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(Pixel PixelA,Pixel PixelB)
        {
            if (PixelA.Blue == PixelB.blue && PixelA.Green == PixelB.Green && PixelA.Red == PixelB.Red)
            {
                return false;
            }
            return true;
        }
    }
}
