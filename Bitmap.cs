using System;
using System.IO;
using System.Collections.Generic;

namespace TD2_test
{
    public class Bitmap
    {
        #region Constante d'offset
        //décallage en nombre de bit pour lire les infos que l'on veut sur les tableaux de bytes 

        // sur le header
        public static int fileSize_Off = 2; // d'après wikipédia on sait que la taille du fichier est décaller de 2 bits sur l'entête
        public static int imageOffset_Off = 10; // etc...

        // sur le header info
        public static int width_Off = 4; // décallé mais sur le header info (2ème entête) 
        public static int height_Off = 8;
        public static int colordepth_Off = 14;
        #endregion

        #region Attributs
        string imageType = "BM"; // toujours BM car l'objet d'étude est la Bitmap
        int fileSize; 
        int imageOffset = 54; // ou démarre la matrice de pixel (taille total de l'entête)
        int width; 
        int height;
        int colordepth = 24; // bits par pixel soit 3 octets : 1 par couleur (RGB)
        Pixel[,] image; // matrice de Pixel 

        //tableaux de bytes dans le lequels on va ranger les bytes du header en lisant le fichier en FileStream afin de retrouver les infos avec les offsets
        byte[] bmp_Header;
        byte[] bmp_HeaderInfo;
        #endregion

        #region Propriétés
        /// <summary>
        /// propriété pour get avec la nouvelle syntaxe
        /// </summary>
        public string ImageType => imageType;
        public int FileSize => fileSize;
        public int ImageOffset => imageOffset;
        public int Width => width;
        public int Height => height;
        public int Colordepth => colordepth;

        #endregion

        #region Constructeur
        /// <summary>
        /// lit un fichier le transforme en instance de la classe Bitmap
        /// </summary>
        /// <param name="path"></param> chemin de l'image
        public Bitmap(string path) 
        {
            bmp_Header = new byte[14]; //on initialise la taille du tableau qui va contenir le header à 14 car il y aura toujours 14 bits dans une BMP
            bmp_HeaderInfo = new byte[40]; //toujours 40 car toujours 54 la taille totale de l'entête
            
            using (FileStream fs = File.OpenRead(path)) //using pour ouvrir le fichier et le ferme à la fin des crochets
            {
                fs.Read(bmp_Header, 0, 14);
                fs.Read(bmp_HeaderInfo, 0, 40);

                //on va remplir les attributs
                //attention Int32 pour convertir les bits en nombre décimal(prend en compte little endian)
                // et 32 car la taille du fichier est sur 4 bytes donc 32 octets
                // pour les informations sur 2 bytes comme la colordepth, 2 bytes soit 16 octets donc Int16
                fileSize = BitConverter.ToInt32(bmp_Header, fileSize_Off);
                imageOffset = BitConverter.ToInt32(bmp_Header, imageOffset_Off);
                width = BitConverter.ToInt32(bmp_HeaderInfo, width_Off);
                height = BitConverter.ToInt32(bmp_HeaderInfo, height_Off);
                colordepth = BitConverter.ToInt16(bmp_HeaderInfo, colordepth_Off); //colordepth dans le fichier BMP est écrit sous 2 bytes

                image = new Pixel[width, height];

                for(int y = 0; y < height; y++)
                {
                    for(int x = 0; x < width; x++)
                    {
                        byte[] RGB = new byte[3]; //crée un tableau de bytes pour stocker les 3 prochains bits pour le pixel 
                        fs.Read(RGB, 0, 3);         //stock les 3 prochains bytes dans le tableau RGB
                        image[x, y] = new Pixel(RGB[0],RGB[1],RGB[2]);  //crée un pixel avec les 3 bytes de couleurs en paramètre pour le mettre dans la case
                    }
                }
            }            
        }
        /// <summary>
        /// Constructeur pour créer une instance bitmap mais à partir de caractéristiques et non en lisant un fichier
        /// Cette fonction sert à création une bitmap mais la matrice de pixel est vide on utilisera des méthodes d'instance
        /// pour remplir la matrice de pixel par exemple en appliquant une rotation par rapport à une autre bitmap par exemple
        /// </summary>
        /// <param name="width"></param> nombre de pixel en largeur
        /// <param name="height"></param> nombre de pixel en longueur
        public Bitmap(int width, int height)
        {
            this.fileSize = width * height * 3 + 54;
            this.width = width;
            this.height = height;
            this.image = new Pixel[width, height]; // on initialise la matrice de pixel de notre image
                                                   // on va mettre des pixels blanc partout par défaut

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    image[x, y] = new Pixel(); // constructeur pour créer un pixel blanc
                }
            }
        }
        /// <summary>
        /// Constructeur pour QRCode, prend qu'un seul paramètre comme c'est un carre
        /// Crée l'image avoir un pixel Violet, différent de blanc ou noir pour différentier les pixels vide et le Patterne de base
        /// </summary>
        /// <param name="V1ouV2"></param>
        public Bitmap(int Length_Version)
        {
            this.width = Length_Version + 3; // pour avoir un multiple de 4 padding
            this.height = Length_Version;
            this.fileSize = height * width * 3  + 54;
            this.image = new Pixel[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    image[x, y] = new Pixel(254,6,125); // Pixel violet
                }
            }
        }
        #endregion

        #region Méthodes d'instances
        /// <summary>
        /// inutile 
        /// </summary>
        /// <returns></returns>
        public string toString()
        {
            string a = " ";
            a = "Le type est fichier est " + imageType
                + "\nLa taille du fichier est de  : " + fileSize + " octets"
                + "\nLa taille de l'offset est de  : " + imageOffset
                + "\nLa hauteur de l'image est de " + height
                + "\nLa largeur de l'image est de : " + width
                + "\nImage : aller l'ouvrir dans repertoire :) ";

            return a;
        }
        /// <summary>
        /// méthode pour enregistrer dans le répertoire le fichier binaire en recréant donc le header, header info etc 
        /// pour que l'on puisse l'ouvrir sous windows
        /// </summary>
        /// <param name="name"></param> nom que l'on veut donner à l'image en enregistrant
        public void Save(string name)
        {
            string path = "Repertoire/" + name + ".bmp";

            byte[] MonImageEnBits = new byte[this.fileSize]; // tableau de bits que l'on va enregistrer 

            //BM
            MonImageEnBits[0] = 66;
            MonImageEnBits[1] = 77;

            //taille du fichier
            byte[] temp = new byte[4];
            temp = BitConverter.GetBytes(fileSize);

            MonImageEnBits[2] = temp[0];
            MonImageEnBits[3] = temp[1];
            MonImageEnBits[4] = temp[2];
            MonImageEnBits[5] = temp[3];

            //largeur de l'image
            temp = BitConverter.GetBytes(width);

            MonImageEnBits[18] = temp[0];
            MonImageEnBits[19] = temp[1];
            MonImageEnBits[20] = temp[2];
            MonImageEnBits[21] = temp[3];

            //longueur de l'image
            temp = BitConverter.GetBytes(height);

            MonImageEnBits[22] = temp[0];
            MonImageEnBits[23] = temp[1];
            MonImageEnBits[24] = temp[2];
            MonImageEnBits[25] = temp[3];

            // paramètre toujours présent pour une bitmap
            MonImageEnBits[10] = 54;
            MonImageEnBits[14] = 40;
            MonImageEnBits[26] = 1;
            MonImageEnBits[28] = 24;


            // de 53 jusqu'à la fin de l'image on va recopier la matrice de pixel
            int i = 53;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    i += 1;
                    MonImageEnBits[i] = this.image[x, y].Blue;
                    i += 1;
                    MonImageEnBits[i] = this.image[x, y].Green;
                    i += 1;
                    MonImageEnBits[i] = this.image[x, y].Red;
                    
                }
            }
        
            File.WriteAllBytes("ImageDeSortie.bmp", MonImageEnBits);
            
        }
        /// <summary>
        /// Convertir un format little endian en int
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public int Convertir_Endian_To_Int(byte[] tab)
        {
            double sum = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                sum += tab[i] * Math.Pow(256, i);
            }
            return Convert.ToInt32(sum);
        }
        /// <summary>
        /// convertir un format int en little endian
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public byte[] Convertir_Int_To_Endian(int val)
        {
            byte[] result = new byte[4];
            result[0] = (byte)val;
            result[1] = (byte)(((uint)val >> 8) & 0xFF);
            result[2] = (byte)(((uint)val >> 16) & 0xFF);
            result[3] = (byte)(((uint)val >> 24) & 0xFF);
            return result;
        }

        #region Nuance de gris
        /// <summary>
        /// Parcours la matrice de pixel pour lui appliquer la fonction qui transforme les pixels en nuance de gris
        /// </summary>
        public void ToGreyScale()
        {
            Bitmap modified = new Bitmap(this.width, this.height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    modified.image[x, y] = ToPixelGrey(this.image[x, y]);
                }
            }

            modified.Save("NuanceDeGris");

        }
        /// <summary>
        /// Transforme le pixel en pixel de gris
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public Pixel ToPixelGrey(Pixel pixel)
        {
            int somme = (pixel.Red + pixel.Blue + pixel.Green) / 3;
            byte octetDeGris = Convert.ToByte(somme);
            return new Pixel(octetDeGris, octetDeGris, octetDeGris);
        }
        #endregion

        #region Noir et blanc
        /// <summary>
        /// Parcours la matrice de pixel pour lui appliquer la fonction qui transforme les pixels en noir et blanc
        /// </summary>
        public void ToBlackAndWhite()
        {
            Bitmap modified = new Bitmap(this.width, this.height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    modified.image[x, y] = ToPixelBlackAndWhite(this.image[x, y]); 
                }
            }

            modified.Save("NoirEtBlanc");

        }
        /// <summary>
        /// retourne un pixel noir ou blanc selon ses composantes
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public Pixel ToPixelBlackAndWhite(Pixel pixel)
        {
            int somme = (pixel.Red + pixel.Blue + pixel.Green) / 3;
            if(somme >127)
            {
                return new Pixel();
            }
            return new Pixel(0, 0, 0);
            
        }
        #endregion

        #region Miroir
        /// <summary>
        /// Réécrit la matrice de pixel en partant de droite vers la gauche
        /// </summary>
        public void ToMirrorHorizontal()
        {
            Bitmap modified = new Bitmap(this.width, this.height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    modified.image[width - 1 - x, y] = this.image[x, y];
                }
            }

            modified.Save("MiroirHorizontal");
        }
        /// <summary>
        /// réécrit la matrice de pixel de bas en haut
        /// </summary>
        public void ToMirrorVertical()
        {
            Bitmap modified = new Bitmap(this.width, this.height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    modified.image[x, height - 1 - y] = this.image[x, y];
                }
            }

            modified.Save("Image Vertical");
            
        }

  
        #endregion

        #region Rotation
        /// <summary>
        /// Rotation selon un angle quelconque
        /// 1ère étape on applique la rotation à partir de coin en bas à gauche de l'image en la posant au centre d'une grande image
        /// 2ème étape on vient chercher les pixels voisin lorsqu'un pixel est resté blanc à cause des arrondis de la formule avec cos et sin
        /// 3ème étape on vient recadrage proprement l'image
        /// </summary>
        /// <param name="angle"></param>
        public void ToRotate(double angle)
        {

            #region Rotation 
            angle *= Math.PI / 180;
            int NewLength = 2 * (int)(Math.Sqrt(width * width + height * height));
            NewLength += NewLength % 4;

            Bitmap modified = new Bitmap(NewLength, NewLength);
            
            for (int j = 0 ; j < height; j++)
            {
                for (int i = 0 ; i < width; i++)
                {
                    double x = (int)(j * Math.Cos(angle)) + (int)(i *Math.Sin(angle)) ;
                    double y = - (int)(j *Math.Sin(angle)) + (int)(i *Math.Cos(angle)) ;
                    modified.image[(int)y + NewLength/2 , (int)x + NewLength/2  ] = this.image[i, j];
                }
            }
            #endregion
            #region Comblage des trous avec les pixels voisins
            Bitmap modified2 = new Bitmap(modified.width, modified.height);
            
            for (int j = 0; j < modified.height; j++)
            {
                for (int i = 1; i < modified.width; i++)
                {
                    if((modified.image[i,j].Red == 255) && (modified.image[i, j].Blue == 255) && modified.image[i, j].Green == 255)
                    {
                        modified2.image[i, j] = modified.image[i-1,j];
                    }
                    else
                    {
                        modified2.image[i, j] = modified.image[i, j];
                    }
                }
            }
            #endregion
            #region Recadrage

            int StartHeight = 0;
            int EndHeight = 0;
            int StartWidth = 0;
            int EndWidth = 0;
            bool FirstFind = true;
            for (int j = 0; j < modified2.height; j++)
            {
                for (int i = 0; i < modified2.width; i++)
                {
                    if(modified2.image[i,j].Red != 255 && modified2.image[i,j].Blue !=255 && modified2.image[i,j].Green != 255 && FirstFind == true)
                    {
                        StartHeight = j;
                        FirstFind = false;
                    }
                }
            }
            FirstFind = true;
            for (int j = modified2.height - 1; j >= 0; j--)
            {
                for (int i = 0; i < modified2.width; i++)
                {
                    if (modified2.image[i, j].Red != 255 && modified2.image[i, j].Blue != 255 && modified2.image[i, j].Green != 255 && FirstFind == true)
                    {
                        EndHeight = j;
                        FirstFind = false;
                    }
                }
            }
            FirstFind = true;
            for (int i = 0; i < modified2.width; i++)
            {
                for (int j = 0; j < modified2.height; j++)
                {
                    if (modified2.image[i, j].Red != 255 && modified2.image[i, j].Blue != 255 && modified2.image[i, j].Green != 255 && FirstFind== true)
                    {
                        StartWidth = i;
                        FirstFind = false;
                    }
                }
            }
            FirstFind = true;
            for (int i = modified2.width - 1; i >=0 ; i--)
            {
                for (int j = 0; j < modified2.height; j++)
                {
                    if (modified2.image[i, j].Red != 255 && modified2.image[i, j].Blue != 255 && modified2.image[i, j].Green != 255 && FirstFind == true)
                    {
                        EndWidth = i;
                        FirstFind = false;
                    }
                }
            }
            int FinalWidth = EndWidth - StartWidth;
            FinalWidth = FinalWidth - FinalWidth%4 +4;
            int FinalHeight = EndHeight - StartHeight;
            FinalHeight = FinalHeight - FinalHeight%4 + 4;
            Bitmap modified3 = new Bitmap(FinalWidth,FinalHeight);
            
            for (int j = 0; j < FinalHeight; j++)
            {
                for (int i = 0; i < FinalWidth; i++)
                {
                    modified3.image[i, j] = modified2.image[i+StartWidth, j+StartHeight];
                }
            }

            #endregion

            string name = "TournéDe" + angle * 180 / Math.PI + "deg";
            modified3.Save(name);
        }
        #endregion

        #region Réduire/agrandir
        /// <summary>
        /// Recrée l'image en l'agrandissant ou en la rétrécissant
        /// </summary>
        /// <param name="coef"></param>
        public void ToScale(double coef)
        {

            int NewWidth = (int)(width * coef) + (int)(width * coef) % 4;
            int NewHeight = (int)(height * coef) + (int)(height * coef) % 4;
            Bitmap modified = new Bitmap(NewWidth, NewHeight);

            for (int y = 0; y < modified.height; y++)
            {
                for (int x = 0; x < modified.width; x++)
                {
                    try
                    {
                        modified.image[x, y] = this.image[(int)(x / coef), (int)(y / coef)];
                    }
                    catch (IndexOutOfRangeException)
                    {

                    }
                }
                    
            }

            modified.Save("ImageGrande");
        }
        #endregion
        
        #region Flou
        /// <summary>
        /// Toutes les fonctions à base de convolution aurait pu etre simplifier avec une méthode qui prend en paramètre le
        /// filtre à passer dans la matrice et aurait pu être découper en plusieurs méthode
        /// Code à améliorer...
        /// </summary>
        public void ToBlur()
        {
            Bitmap modified = new Bitmap(this.width, this.height);

            // on ne traite pas le contour 
            for (int y = 0; y < height; y++) // recopie première et dernière colonne
            {
                modified.image[0, y] = this.image[0, y];
                modified.image[width-1, y] = this.image[width-1, y];
            }

            for (int x = 0; x < width; x++) // recopie première et dernière ligne
            {
                modified.image[x, 0] = this.image[x, 0];
                modified.image[x, height-1] = this.image[x, height-1];
            }

            for (int y = 1; y < height-1; y++)
            {
                for (int x = 1; x < width-1; x++)
                {
                    modified.image[x, y] = ToPixelBlur( x, y, image);
                }
            }

            modified.Save("Flou");
        }
        /// <summary>
        /// Passe le filtre de kernel sur le pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public Pixel ToPixelBlur(int x, int y, Pixel[,] image)
        {
            // composante rouge
            byte[,] temp = { {image[x-1,y-1].Red , image[x-1,y  ].Red , image[x-1,y+1].Red },
                             {image[x  ,y-1].Red , image[x  ,y  ].Red , image[x  ,y+1].Red },
                             {image[x+1,y-1].Red , image[x+1,y  ].Red , image[x+1,y+1].Red } };

            int[,] kernel = { { 1,1,1 },
                                   { 1,0,1 },
                                   { 1,1,1 } };

            int NewRed = 0;
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j< 3; j++)
                {
                    NewRed += temp[i, j] * kernel[i, j];
                }
            }

            // composante bleu
            byte[,] temp2 = { {image[x-1,y-1].Blue , image[x-1,y  ].Blue , image[x-1,y+1].Blue },
                              {image[x  ,y-1].Blue , image[x  ,y  ].Blue , image[x  ,y+1].Blue },
                              {image[x+1,y-1].Blue , image[x+1,y  ].Blue , image[x+1,y+1].Blue } };

            int NewBlue = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewBlue += temp2[i, j] * kernel[i, j];
                }
            }

            // composante Verte
            byte[,] temp3 = { {image[x-1,y-1].Green , image[x-1,y  ].Green , image[x-1,y+1].Green },
                              {image[x  ,y-1].Green , image[x  ,y  ].Green , image[x  ,y+1].Green },
                              {image[x+1,y-1].Green , image[x+1,y  ].Green , image[x+1,y+1].Green } };

            int NewGreen = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewGreen += temp3[i, j] * kernel[i, j];
                }
            }
            // divise par 8 car somme des coefs 8
            byte Blue = Convert.ToByte(NewBlue/8);
            byte Green = Convert.ToByte(NewGreen/8);
            byte Red = Convert.ToByte(NewRed/8);

            return new Pixel(Blue, Green, Red);
        }
        #endregion

        #region Détection des contours
        /// <summary>
        /// Toutes les fonctions à base de convolution aurait pu etre simplifier avec une méthode qui prend en paramètre le
        /// filtre à passer dans la matrice et aurait pu être découper en plusieurs méthode
        /// Code à améliorer...
        /// </summary>
        public void ToEdgeDetection()
        {
            Bitmap modified = new Bitmap(this.width, this.height);

            // on ne traite pas le contour 
            for (int y = 0; y < height; y++) // recopie première et dernière colonne
            {
                modified.image[0, y] = this.image[0, y];
                modified.image[width - 1, y] = this.image[width - 1, y];
            }

            for (int x = 0; x < width; x++) // recopie première et dernière ligne
            {
                modified.image[x, 0] = this.image[x, 0];
                modified.image[x, height - 1] = this.image[x, height - 1];
            }

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    modified.image[x, y] = ToPixelEdgeDetection(x, y, image);
                }
            }

            modified.Save("DétectionDesContours");
        }
        /// <summary>
        /// méthode qui passe le filtre de kernel au pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public Pixel ToPixelEdgeDetection(int x, int y, Pixel[,] image)
        {
            // composante rouge
            byte[,] temp = { {image[x-1,y-1].Red , image[x-1,y  ].Red , image[x-1,y+1].Red },
                             {image[x  ,y-1].Red , image[x  ,y  ].Red , image[x  ,y+1].Red },
                             {image[x+1,y-1].Red , image[x+1,y  ].Red , image[x+1,y+1].Red } };

            int[,] kernel = { { 0,1,0 },
                                   { 1,-4,1 },
                                   { 0,1,0 } };

            int NewRed = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewRed += temp[i, j] * kernel[i, j];
                }
            }

            // composante bleu
            byte[,] temp2 = { {image[x-1,y-1].Blue , image[x-1,y  ].Blue , image[x-1,y+1].Blue },
                              {image[x  ,y-1].Blue , image[x  ,y  ].Blue , image[x  ,y+1].Blue },
                              {image[x+1,y-1].Blue , image[x+1,y  ].Blue , image[x+1,y+1].Blue } };

            int NewBlue = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewBlue += temp2[i, j] * kernel[i, j];
                }
            }

            // composante Verte
            byte[,] temp3 = { {image[x-1,y-1].Green , image[x-1,y  ].Green , image[x-1,y+1].Green },
                              {image[x  ,y-1].Green , image[x  ,y  ].Green , image[x  ,y+1].Green },
                              {image[x+1,y-1].Green , image[x+1,y  ].Green , image[x+1,y+1].Green } };

            int NewGreen = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewGreen += temp3[i, j] * kernel[i, j];
                }
            }

            //normalise les valeurs
            if (NewBlue > 255)
            {
                NewBlue = 255;
            }
            if (NewBlue < 0)
            {
                NewBlue = 0;
            }

            if (NewRed > 255)
            {
                NewRed = 255;
            }
            if (NewRed < 0)
            {
                NewRed = 0;
            }

            if (NewGreen > 255)
            {
                NewGreen = 255;
            }
            if (NewGreen < 0)
            {
                NewGreen = 0;
            }

            byte Blue = Convert.ToByte(NewBlue);
            byte Green = Convert.ToByte(NewGreen);
            byte Red = Convert.ToByte(NewRed);

            return new Pixel(Blue, Green, Red);
        }
        #endregion

        #region Repoussage
        /// <summary>
        /// Toutes les fonctions à base de convolution aurait pu etre simplifier avec une méthode qui prend en paramètre le
        /// filtre à passer dans la matrice et aurait pu être découper en plusieurs méthode
        /// Code à améliorer...
        /// </summary>
        public void ToSharpen()
        {
            Bitmap modified = new Bitmap(this.width, this.height);

            // on ne traite pas le contour 
            for (int y = 0; y < height; y++) // recopie première et dernière colonne
            {
                modified.image[0, y] = this.image[0, y];
                modified.image[width - 1, y] = this.image[width - 1, y];
            }

            for (int x = 0; x < width; x++) // recopie première et dernière ligne
            {
                modified.image[x, 0] = this.image[x, 0];
                modified.image[x, height - 1] = this.image[x, height - 1];
            }

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    modified.image[x, y] = ToPixelSharpen(x, y, image);
                }
            }

            modified.Save("Repoussage");
        }
        /// <summary>
        /// méthode qui passe le filtre de kernel au pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public Pixel ToPixelSharpen(int x, int y, Pixel[,] image)
        {
            // composante rouge
            byte[,] temp = { {image[x-1,y-1].Red , image[x-1,y  ].Red , image[x-1,y+1].Red },
                             {image[x  ,y-1].Red , image[x  ,y  ].Red , image[x  ,y+1].Red },
                             {image[x+1,y-1].Red , image[x+1,y  ].Red , image[x+1,y+1].Red } };

            int[,] kernel = { { -2,-1,0 },
                                   { -1,1,1 },
                                   { 0,1,2 } };

            int NewRed = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewRed += temp[i, j] * kernel[i, j];
                }
            }

            // composante bleu
            byte[,] temp2 = { {image[x-1,y-1].Blue , image[x-1,y  ].Blue , image[x-1,y+1].Blue },
                              {image[x  ,y-1].Blue , image[x  ,y  ].Blue , image[x  ,y+1].Blue },
                              {image[x+1,y-1].Blue , image[x+1,y  ].Blue , image[x+1,y+1].Blue } };

            int NewBlue = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewBlue += temp2[i, j] * kernel[i, j];
                }
            }

            // composante Verte
            byte[,] temp3 = { {image[x-1,y-1].Green , image[x-1,y  ].Green , image[x-1,y+1].Green },
                              {image[x  ,y-1].Green , image[x  ,y  ].Green , image[x  ,y+1].Green },
                              {image[x+1,y-1].Green , image[x+1,y  ].Green , image[x+1,y+1].Green } };
            
            int NewGreen = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewGreen += temp3[i, j] * kernel[i, j];
                }
            }

            // normalise les valeurs
            if (NewBlue > 255)
            {
                NewBlue = 255;
            }
            if (NewBlue < 0)
            {
                NewBlue = 0;
            }

            if (NewRed > 255)
            {
                NewRed = 255;
            }
            if (NewRed < 0)
            {
                NewRed = 0;
            }

            if (NewGreen > 255)
            {
                NewGreen = 255;
            }
            if (NewGreen < 0)
            {
                NewGreen = 0;
            }
            
            byte Blue = Convert.ToByte(NewBlue);
            byte Green = Convert.ToByte(NewGreen);
            byte Red = Convert.ToByte(NewRed);

            return new Pixel(Blue, Green, Red);
        }
        #endregion

        #region Renforcement des bords (pas utilisé image pas pertinente)
        /// <summary>
        /// Toutes les fonctions à base de convolution aurait pu etre simplifier avec une méthode qui prend en paramètre le
        /// filtre à passer dans la matrice et aurait pu être découper en plusieurs méthode
        /// Code à améliorer...
        /// </summary>
        public void RenforcementBords()
        {
            Bitmap modified = new Bitmap(this.width, this.height);

            // on ne traite pas le contour 
            for (int y = 0; y < height; y++) // recopie première et dernière colonne
            {
                modified.image[0, y] = this.image[0, y];
                modified.image[width - 1, y] = this.image[width - 1, y];
            }

            for (int x = 0; x < width; x++) // recopie première et dernière ligne
            {
                modified.image[x, 0] = this.image[x, 0];
                modified.image[x, height - 1] = this.image[x, height - 1];
            }

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    modified.image[x, y] = ToPixelBords(x, y, image);
                }
            }

            modified.Save("RenforcementDesBords");
        }
        /// <summary>
        /// méthode qui passe le filtre de kernel au pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public Pixel ToPixelBords(int x, int y, Pixel[,] image)
        {
            // composante rouge
            byte[,] temp = { {image[x-1,y-1].Red , image[x-1,y  ].Red , image[x-1,y+1].Red },
                             {image[x  ,y-1].Red , image[x  ,y  ].Red , image[x  ,y+1].Red },
                             {image[x+1,y-1].Red , image[x+1,y  ].Red , image[x+1,y+1].Red } };

            int[,] Convolution = { { 0,0,0 },
                                   { -1,1,0 },
                                   { 0,0,0 } };

            int NewRed = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewRed += temp[i, j] * Convolution[i, j];
                }
            }

            // composante bleu
            byte[,] temp2 = { {image[x-1,y-1].Blue , image[x-1,y  ].Blue , image[x-1,y+1].Blue },
                              {image[x  ,y-1].Blue , image[x  ,y  ].Blue , image[x  ,y+1].Blue },
                              {image[x+1,y-1].Blue , image[x+1,y  ].Blue , image[x+1,y+1].Blue } };

            int NewBlue = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewBlue += temp2[i, j] * Convolution[i, j];
                }
            }

            // composante Verte
            byte[,] temp3 = { {image[x-1,y-1].Green , image[x-1,y  ].Green , image[x-1,y+1].Green },
                              {image[x  ,y-1].Green , image[x  ,y  ].Green , image[x  ,y+1].Green },
                              {image[x+1,y-1].Green , image[x+1,y  ].Green , image[x+1,y+1].Green } };

            int NewGreen = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewGreen += temp3[i, j] * Convolution[i, j];
                }
            }

            //valeurs absolue
            byte Blue = Convert.ToByte(Math.Abs(NewBlue));
            byte Green = Convert.ToByte(Math.Abs(NewGreen));
            byte Red = Convert.ToByte(Math.Abs(NewRed));

            return new Pixel(Blue, Green, Red);
        }
        #endregion

        #region Histogramme
        /// <summary>
        /// on vient lire pour sommer toutes les composantes de couleurs des pixels
        /// après une divisant par le nombre de pixel on peut dessiner un histogramme
        /// on a alors 3 valeurs comprises entre 0 Et 256 que l'on comparent en dessiner 3 colonnes
        /// </summary>
        public void Histogramme()
        {
            int SumRed = 0;
            int SumBlue = 0;
            int SumGreen = 0;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SumBlue += this.image[x, y].Blue;
                    SumGreen += this.image[x, y].Green;
                    SumRed += this.image[x, y].Red;
                }
            }

            int nbPixel = this.Height * this.Width;
            SumRed /= nbPixel;
            SumGreen /= nbPixel;
            SumBlue /= nbPixel;

            Bitmap histogramme = new Bitmap(252,256);

            for (int x = 36; x <= 72; x++)
            {
                for (int y = 0; y < SumBlue; y++)
                {
                    histogramme.image[x, y] = new Pixel(205, 115, 18);
                }
            }

            for (int x = 108; x <= 144; x++)
            {
                for (int y = 0; y < SumGreen; y++)
                {
                    histogramme.image[x, y] = new Pixel(38, 177, 47);
                }
            }

            for (int x = 180; x <= 216; x++)
            {
                for (int y = 0; y < SumRed; y++)
                {
                    histogramme.image[x, y] = new Pixel(10, 62, 212);
                }
            }

            histogramme.Save("Histogramme");

        }

        #endregion

        #region Fractale
        /// <summary>
        /// Algorithme de fractale basé sur la formule itérative Z (k+1) = Z (k) ^2 + c
        /// </summary>
        public void Fractales()
        {
            int height = 1000;
            int width = 1000;
            Bitmap fractale = new Bitmap(width, height);

            double x1 = -2.1;
            double x2 = 0.6;
            double y1 = -1.2;
            double y2 = 1.2;

            int nbIteration = 20;

            double x_zoom = width / (x2 - x1);
            double y_zoom = height / (y2 - y1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double c_r = x / x_zoom + x1;
                    double c_i = y / y_zoom + y1;
                    double z_r = 0;
                    double z_i = 0;
                    int i = 0;

                    do
                    {
                        double memory = z_r;
                        z_r = z_r * z_r - z_i * z_i + c_r;
                        z_i = 2 * z_i * memory + c_i;
                        i++;
                    }while((z_r*z_r + z_i*z_i) < 4 && i<nbIteration);

                    if( i == nbIteration)
                    {
                        fractale.image[x, y] = new Pixel(170, 127, 77);
                    }


                }
            }


            fractale.Save("Fractale");


        }
        #endregion

        #region QR CODE
        /// <summary>
        /// Comporte le dessin des 3 finders patterns et séparators
        /// timing patterns horizontale et verticale et le dark module
        /// </summary>
        public void Patern()
        {
            #region Carre en haut à gauche
            for (int j = 0; j <= 7; j++) // carré blanc de 8x8
            {
                for (int i = 0; i <= 7; i++)
                {
                    image[i, j] = new Pixel();
                }
            }
            for (int j = 0; j <= 6; j++) // carré noir de 7x7
            {
                for (int i = 0; i <= 6; i++)
                {
                    image[i, j] = new Pixel(0, 0, 0);
                }
            }
            for (int j = 1; j <= 5; j++) // carré blanc de 5x5
            {
                for (int i = 1; i <= 5; i++)
                {
                    image[i, j] = new Pixel();
                }
            }
            for (int j = 2; j <= 4; j++) // carré noir de 3x3
            {
                for (int i = 2; i <= 4; i++)
                {
                    image[i, j] = new Pixel(0, 0, 0); // pixel noir au centre
                }
            }
            #endregion
            #region Carre en haut à droite
            for (int j = 0; j <= 7; j++)
            {
                for (int i = image.GetLength(0)- 1; i >= image.GetLength(0)- 7 -1; i--)
                {
                    image[i - 3, j ] = new Pixel();
                }
            }
            for (int j = 0; j <= 6; j++)
            {
                for (int i = image.GetLength(0) -1 ; i >= image.GetLength(0) - 6 - 1; i--)
                {
                    image[i - 3, j] = new Pixel(0, 0, 0);
                }
            }
            for (int j = 1; j <= 5; j++)
            {
                for (int i = image.GetLength(0) - 2; i >= image.GetLength(0) - 5 - 1; i--)
                {
                    image[i - 3, j] = new Pixel();
                }
            }
            for (int j = 2; j <= 4; j++)
            {
                for (int i = image.GetLength(0) - 3; i >= image.GetLength(0) - 4 - 1; i--)
                {
                    image[i - 3, j] = new Pixel(0, 0, 0);
                }
            }
            #endregion
            #region Carre en bas à gauche
            for (int j = image.GetLength(1) - 1; j >= image.GetLength(1) - 1 - 7; j--)
            {
                for (int i = 0; i <= 7; i++)
                {
                    image[i, j] = new Pixel();
                }
            }
            for (int j = image.GetLength(1) - 1; j >= image.GetLength(1) - 1 - 6; j--)
            {
                for (int i = 0; i <= 6; i++)
                {
                    image[i, j] = new Pixel(0, 0, 0);
                }
            }
            for (int j = image.GetLength(1) - 1 - 1; j >= image.GetLength(1) - 1 - 5; j--)
            {
                for (int i = 1; i <= 5; i++)
                {
                    image[i, j] = new Pixel();
                }
            }
            for (int j = image.GetLength(1) - 1 - 2; j >= image.GetLength(1) - 1 - 4; j--)
            {
                for (int i = 2; i <= 4; i++)
                {
                    image[i, j] = new Pixel(0, 0, 0);
                }
            }
            #endregion
            #region timing pattern horizontale
            for (int j = 8; j<image.GetLength(1); j++)
            {
                if(image[6,j].Blue == 254)
                {
                    if(j%2 == 0)
                    {
                        image[6, j] = new Pixel(0, 0, 0);
                    }
                    else
                    {
                        image[6, j] = new Pixel();
                    }
                }
            }
            #endregion
            #region timing pattern verticale
            for (int i = 8; i < image.GetLength(0)- 3; i++)
            {
                if (image[i, 6].Blue == 254)
                {
                    if (i % 2 == 0)
                    {
                        image[i, 6] = new Pixel(0, 0, 0);
                    }
                    else
                    {
                        image[i, 6] = new Pixel();
                    }
                }
            }
            #endregion
            image[8, image.GetLength(1) - 8] = new Pixel(0, 0, 0); // dark module
        }
        /// <summary>
        /// Le pattern d'alignement pour le V2 
        /// </summary>
        public void AlignmentPatern()
        {
            for (int j = image.GetLength(1) - 1  - 4; j >= image.GetLength(1) - 1 - 4 - 4; j--) // carré noir de 5x5
            {
                for (int i = image.GetLength(1) - 1 - 4; i >= image.GetLength(1) - 1 - 4 - 4; i--)
                {
                    image[i, j] = new Pixel(0, 0, 0);
                }
            }
            for (int j = image.GetLength(1) - 1 - 5; j >= image.GetLength(1) - 1 - 4 - 3; j--) // carré blanc de 3x3
            {
                for (int i = image.GetLength(1) - 1 - 5; i >= image.GetLength(1) - 1 - 4 - 3; i--)
                {
                    image[i, j] = new Pixel();
                }
            }
            image[image.GetLength(0) - 1 - 9, image.GetLength(1) - 1 - 6] = new Pixel(0, 0, 0); // pixel noir au centre
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Masque"></param>
        public void Placement_Masque(List<int> Masque)
        {
            List<int> MasqueCopy = new List<int>();
            MasqueCopy.AddRange(Masque); // je duplique la list car je dois placer à 2 endroits le masquage
            #region Masquage 0 en bas 
            for (int i = image.GetLength(1) - 1; i > image.GetLength(1) - 1 - 7; i--)
            {
                if(Masque[0] == 1)
                {
                    image[8, i] = new Pixel(0, 0, 0);
                    Masque.RemoveAt(0);
                }
                else
                {
                    image[8, i] = new Pixel();
                    Masque.RemoveAt(0);
                } 
            }

            for (int i = image.GetLength(0) - 1 - 3 - 7 ; i < image.GetLength(0) - 1 - 2; i++)
            {
                if (Masque[0] == 1)
                {
                    image[i, 8] = new Pixel(0, 0, 0);
                    Masque.RemoveAt(0);
                }
                else
                {
                    image[i, 8] = new Pixel();
                    Masque.RemoveAt(0);
                }
            }
            #endregion
            #region Masque 0 en haut
            for (int i = 0; i < 9; i++)
            {
                if(image[i,8].Blue ==254)
                {
                    if (MasqueCopy[0] == 1)
                    {
                        image[i, 8] = new Pixel(0, 0, 0);
                        MasqueCopy.RemoveAt(0);
                    }
                    else
                    {
                        image[i, 8] = new Pixel();
                        MasqueCopy.RemoveAt(0);
                    }
                }
            }

            for (int i = 7; i >= 0; i--)
            {
                if(image[8,i].Blue == 254)
                {
                    if (MasqueCopy[0] == 1)
                    {
                        image[8, i] = new Pixel(0, 0, 0);
                        MasqueCopy.RemoveAt(0);
                    }
                    else
                    {
                        image[8, i] = new Pixel();
                        MasqueCopy.RemoveAt(0);
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// Placement des données selon la méthode conventionnel de monté et de déscente 
        /// </summary>
        /// <param name="Total"></param>
        public void Placement_Donnees(List<int> Total)
        {
            int hauteur = image.GetLength(1) - 1;
            int largeur = image.GetLength(0) - 1 - 3;
            Total.Add(0); Total.Add(0); Total.Add(0); Total.Add(0); Total.Add(0); Total.Add(0); Total.Add(0);
            while (largeur >= 0)
            {
                while(hauteur>=0)
                {
                    if(largeur >= 0 && image[largeur,hauteur].Blue == 254)
                    {
                        if((largeur + hauteur) % 2 != 0)
                        {
                            if (Total[0] == 1)
                            {
                                image[largeur, hauteur] = new Pixel(0, 0, 0);
                                Total.RemoveAt(0);
                            }
                            else
                            {
                                image[largeur, hauteur] = new Pixel();
                                Total.RemoveAt(0);
                            }
                        }
                        else
                        {
                            if (Total[0] == 0)
                            {
                                image[largeur, hauteur] = new Pixel(0, 0, 0);
                                Total.RemoveAt(0);
                            }
                            else
                            {
                                image[largeur, hauteur] = new Pixel();
                                Total.RemoveAt(0);
                            }
                        }
                        
                    }
                    largeur--;
                    if (largeur >=0 && image[largeur,hauteur].Blue == 254)
                    {
                        if((largeur + hauteur) % 2 != 0)
                        {
                            if (Total[0] == 1)
                            {
                                image[largeur, hauteur] = new Pixel(0, 0, 0);
                                Total.RemoveAt(0);
                            }
                            else
                            {
                                image[largeur, hauteur] = new Pixel();
                                Total.RemoveAt(0);
                            }
                        }
                        else
                        {
                            if (Total[0] == 0)
                            {
                                image[largeur, hauteur] = new Pixel(0, 0, 0);
                                Total.RemoveAt(0);
                            }
                            else
                            {
                                image[largeur, hauteur] = new Pixel();
                                Total.RemoveAt(0);
                            }
                        }
                        
                    }
                    largeur++;
                    hauteur--;
                }
                hauteur = 0;
                largeur -= 2;
                if (largeur == 6) largeur--;
                while (hauteur <= image.GetLength(1) - 1)
                {
                    if (largeur >= 0 && image[largeur, hauteur].Blue == 254)
                    {
                        if((largeur + hauteur) % 2 != 0)
                        {
                            if (Total[0] == 1)
                            {
                                image[largeur, hauteur] = new Pixel(0, 0, 0);
                                Total.RemoveAt(0);
                            }
                            else
                            {
                                image[largeur, hauteur] = new Pixel();
                                Total.RemoveAt(0);
                            }
                        }
                        else
                        {
                            if (Total[0] == 0)
                            {
                                image[largeur, hauteur] = new Pixel(0, 0, 0);
                                Total.RemoveAt(0);
                            }
                            else
                            {
                                image[largeur, hauteur] = new Pixel();
                                Total.RemoveAt(0);
                            }
                        }
                        
                    }
                    largeur--;
                    if (largeur >= 0 && image[largeur, hauteur].Blue == 254)
                    {
                        if((largeur + hauteur) % 2 != 0)
                        {
                            if (Total[0] == 1)
                            {
                                image[largeur, hauteur] = new Pixel(0, 0, 0);
                                Total.RemoveAt(0);
                            }
                            else
                            {
                                image[largeur, hauteur] = new Pixel();
                                Total.RemoveAt(0);
                            }
                        }
                        else
                        {
                            if (Total[0] == 0)
                            {
                                image[largeur, hauteur] = new Pixel(0, 0, 0);
                                Total.RemoveAt(0);
                            }
                            else
                            {
                                image[largeur, hauteur] = new Pixel();
                                Total.RemoveAt(0);
                            }
                        }
                        
                    }
                    largeur++;
                    hauteur++;
                }
                hauteur = image.GetLength(1) - 1;
                largeur -= 2;
            }
        }
        /// <summary>
        /// on vient mettre en blanc les 3 colonnes rajoutées pour esquiver le padding 
        /// </summary>
        public void Blanc()
        {
            for(int largeur = image.GetLength(0)  -3; largeur < image.GetLength(0); largeur ++)
            {
                for(int hauteur =0; hauteur < image.GetLength(1);hauteur ++)
                {
                    image[largeur, hauteur] = new Pixel();
                }
            }
        }
        /// <summary>
        /// méthode d'enregistrement du QRCode on vient l'agrandir
        /// </summary>
        public void SaveQRCODE()
        {
            //afficher dans les consoles la matrice de pixels pour des tests
            /*
            for (int j = 0; j < image.GetLength(1); j++)
            {
                for (int i = 0; i < image.GetLength(0); i++)
                {
                    if(image[i, j].Green >= 100)
                    {
                        Console.Write(image[i, j].Green);
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write(" ");
                        Console.Write(" ");
                        Console.Write(image[i, j].Green);
                        Console.Write(" ");
                    }
                    
                }
                Console.Write("\n");
            }
            */
            
            Bitmap QRCODE = new Bitmap(this.width, this.height);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    QRCODE.image[x, height - 1 - y] = this.image[x, y];
                }
            }
            
            int NewWidth = (int)(width * 16) + (int)(width * 16) % 4;
            int NewHeight = (int)(height * 16) + (int)(height * 16) % 4;

            Bitmap modified = new Bitmap(NewWidth, NewHeight);

            for (int y = 0; y < modified.height; y++)
            {
                for (int x = 0; x < modified.width; x++)
                {
                    modified.image[x, y] = QRCODE.image[(int)(x / 16), (int)(y / 16)];
                }
            }

            modified.Save("QRCODE");

        }
        #endregion
        
        #endregion
    }
}
