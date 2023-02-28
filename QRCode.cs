using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using ReedSolomon;

namespace TD2_test
{
    public class QRCode
    {
        #region Attributs

        List<string> alphanum = new List<string>(); // alphabet conversion alphanumérique

        string Message; // message que l'on génère

        List<int> Mode = new List<int>(4) { 0, 0, 1, 0 }; // alphanumérique
        List<int> Length_Message = new List<int>(9); // taille du message sous 9 bits
        List<int> Data = new List<int>(); // message sous 11 bits par paquet de 2 caractères  152 ou 272
        List<int> _tmp = new List<int>(); // attribut pour stocker temporairement des donneés
        List<int> Correction = new List<int>(); // générer par le reedsolomon
        List<int> BoucheTrou1 = new List<int>(8){ 1, 1, 1, 0, 1, 1, 0, 0 }; // 236
        List<int> BoucheTrou2 = new List<int>(8){ 0, 0, 0, 1, 0, 0, 0, 1 }; // 17
        List<int> Masque = new List<int>(15) { 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0 }; // masque 0

        List<int> Total = new List<int>(); // 208 ou 352
        #endregion

        #region Constructeur
        /// <summary>
        /// Selon la taille de la chaine envoi vers un QRCode V1 ou V2
        /// </summary>
        /// <param name="Message"></param>
        public QRCode(string Message)
        {
            Alphanum();
            this.Message = Message.ToUpper();
            int Length_Decimal = Message.Length;
            ConversionDecimalToBinaryInLenght_Message(Length_Decimal);
            while(Length_Message.Count < 9)
            {
                Length_Message.Add(0);
            }
            Length_Message.Reverse();


            if (Length_Decimal <= 23)
            {
                QRCodeV1();
            }
            else if(Length_Decimal <= 47)
            {
                QRCodeV2();
            }
            else
            {
                Console.WriteLine("Trop grand nombre de caractères");
            }
        }
        #endregion

        #region Methode d'instance
        /// <summary>
        /// Dictionaire alphanum pour avoir la correspondance de chaque caractère avec l'indice de la liste
        /// </summary>
        public void Alphanum()
        {
            alphanum.Add("0");
            alphanum.Add("1");
            alphanum.Add("2");
            alphanum.Add("3");
            alphanum.Add("4");
            alphanum.Add("5");
            alphanum.Add("6");
            alphanum.Add("7");
            alphanum.Add("8");
            alphanum.Add("9");
            alphanum.Add("A");
            alphanum.Add("B");
            alphanum.Add("C");
            alphanum.Add("D");
            alphanum.Add("E");
            alphanum.Add("F");
            alphanum.Add("G");
            alphanum.Add("H");
            alphanum.Add("I");
            alphanum.Add("J");
            alphanum.Add("K");
            alphanum.Add("L");
            alphanum.Add("M");
            alphanum.Add("N");
            alphanum.Add("O");
            alphanum.Add("P");
            alphanum.Add("Q");
            alphanum.Add("R");
            alphanum.Add("S");
            alphanum.Add("T");
            alphanum.Add("U");
            alphanum.Add("V");
            alphanum.Add("W");
            alphanum.Add("X");
            alphanum.Add("Y");
            alphanum.Add("Z");
            alphanum.Add(" ");
            alphanum.Add("$");
            alphanum.Add("%");
            alphanum.Add("*");
            alphanum.Add("+");
            alphanum.Add("-");
            alphanum.Add(".");
            alphanum.Add("/");
            alphanum.Add(":");

        }
        /// <summary>
        /// Conversion decimal vers binaire directement dans la list Length_Message 
        /// </summary>
        /// <param name="nb"></param>
        public void ConversionDecimalToBinaryInLenght_Message(int nb)
        {
            if( nb > 1)
            {
                Length_Message.Add(nb % 2);
                ConversionDecimalToBinaryInLenght_Message(nb / 2);
            }
            else
            {
                Length_Message.Add(nb % 2);
            }
        }
        /// <summary>
        /// Converti la valeur alphanumérique de la lettre en binaire, stock dans la list temporaire en attribut
        /// </summary>
        /// <param name="nb"></param>
        public void ConversionLetter(int nb)
        {
            if (nb > 1)
            {
                _tmp.Add(nb % 2);
                ConversionLetter(nb / 2);
            }
            else
            {
                _tmp.Add(nb % 2);
            }
        }
        /// <summary>
        /// Converti en decimal le tableau de 1 et de 0 pris en paramètre
        /// </summary>
        /// <param name="Conv"></param>
        /// <returns></returns>
        public byte ConversionBinaryToDecimal(int[] Conv)
        {
            double sum = 0;
            for(int i = 0; i < 8; i++)
            {
                sum += Conv[Conv.Length - 1 - i ] * Math.Pow(2, i);
            }
            return Convert.ToByte(sum);
        }
        /// <summary>
        /// Prend les caractères 2 par 2 de la chaine pris en paramètre pour générer le QRCode et rempli la list Data 
        /// </summary>
        public void EncodageTxt()
        {
            for(int i = 0; i < Message.Length; i+=2)
            {
                _tmp.Clear();
                if(Message.Length -1 -i >= 2)
                {  
                    int Letter1 = alphanum.IndexOf(Convert.ToString(Message[i]));
                    int Letter2 = alphanum.IndexOf(Convert.ToString(Message[i + 1]));

                    int sum = 45 * Letter1 + Letter2;
                    ConversionLetter(sum);
                    while (_tmp.Count < 11)
                    {
                        _tmp.Add(0);
                    }
                    _tmp.Reverse();
                    Data.AddRange(_tmp);
                }
                else
                {
                    int sum = alphanum.IndexOf(Convert.ToString(Message[i]));
                    ConversionLetter(sum);
                    while (_tmp.Count < 6)
                    {
                        _tmp.Add(0);
                    }
                    _tmp.Reverse();
                    Data.AddRange(_tmp);
                }
                
            }
        }
        /// <summary>
        /// génère le code erreur grâce à l'algorithme reedsolomon en fonction des paramètres de la v1 ou v2
        /// </summary>
        /// <param name="PourcentageError"></param> 7 ou 10 mot générer pour v1 ou v2
        /// <param name="nbOctetOfData"></param> prend 19 ou 34 octets en paramètre
        public void MethodeReedSolomon(int PourcentageError, int nbOctetOfData)
        {
            List<int> TotalCopy = new List<int>();
            TotalCopy.AddRange(Total);
            byte[] bytesa = new byte[nbOctetOfData]; // nbOctet est à 34 ici pour le v2 et 19 pour la v1
            int[] Conv = new int[8];
            for(int i = 0; i < Total.Count/8; i++) // pour remplir bytesa par paquet de 8 bits à partir de la donnée
            {
                Conv = new int[8];
                for (int j = 0; j < 8;j++)
                {
                    Conv[j] = TotalCopy[0];
                    TotalCopy.RemoveAt(0);
                }
                bytesa[i] = ConversionBinaryToDecimal(Conv);
            }
            byte[] result = ReedSolomonAlgorithm.Encode(bytesa, PourcentageError, ErrorCorrectionCodeType.QRCode);
            foreach (byte element in result)
            {
                _tmp.Clear();
                ConversionLetter(element);
                while (_tmp.Count < 8)
                {
                    _tmp.Add(0);
                }
                _tmp.Reverse();
                Correction.AddRange(_tmp);
            }
            Total.AddRange(Correction);
        }
        /// <summary>
        /// rempli la liste Total avec le Mode la Longueur de la chaine et la Data puis ajoute la terminaison de 4 zéro
        /// puis le nombre de 0 suffisant pour que le reste jusqu'à la taille max soit un multiple de 8 
        /// enfin ajoute les comblages 236 et 17 jusqu'à la taille max
        /// </summary>
        /// <param name="nbbitsOfData"></param>
        public void Terminaison(int nbbitsOfData)
        {
            Total.AddRange(Mode);
            Total.AddRange(Length_Message);
            Total.AddRange(Data);
            Total.Add(0);Total.Add(0);Total.Add(0);Total.Add(0); // terminaison
            int missing = (nbbitsOfData - Total.Count);
            int nb_zero = missing % 8;
            for(int i =0; i < nb_zero; i++)
            {
                Total.Add(0);
            }

            int step = 0;
            while(Total.Count < nbbitsOfData)
            {
                if(step%2 ==0)
                {
                    Total.AddRange(BoucheTrou1);
                }
                else
                {
                    Total.AddRange(BoucheTrou2);
                }
                step++;
            }
        }
        /// <summary>
        /// appelle les fonctions pour créé le QRCode de v1 avec les paramètres :
        /// taille de la chaine en bits : 152 
        /// taille en octet de la chaine : 19 octets
        /// taille de la correction : 7 octets 
        /// </summary>
        public void QRCodeV1()
        {
            Bitmap MonQRCode = new Bitmap(21);
            MonQRCode.Patern();
            EncodageTxt();
            Terminaison(152);
            MethodeReedSolomon(7, 19);
            MonQRCode.Placement_Masque(Masque);
            MonQRCode.Placement_Donnees(Total);
            MonQRCode.Blanc();
            MonQRCode.SaveQRCODE();
        }
        /// <summary>
        /// appelle les fonctions pour créé le QRCode de v2 avec les paramètres :
        /// taille de la chaine en bits : 272 
        /// taille en octet de la chaine : 34 octets
        /// taille de la correction : 10 octets 
        /// </summary>
        public void QRCodeV2()
        {
            Bitmap MonQRCode = new Bitmap(25);
            MonQRCode.Patern();
            MonQRCode.AlignmentPatern();
            EncodageTxt();
            Terminaison(272);
            MethodeReedSolomon(10 , 34);
            MonQRCode.Placement_Masque(Masque);
            MonQRCode.Placement_Donnees(Total);
            MonQRCode.Blanc();
            MonQRCode.SaveQRCODE();
        }
        #endregion
    }
}
