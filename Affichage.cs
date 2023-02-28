using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Media;
using System.Diagnostics;
using System.IO;

namespace TD2_test
{
    class Affichage
    {
        public static void Executer_Projet_PSI_Damien()
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            bool BonneRep = false;
            while(BonneRep == false)
            {
                Console.Clear();
                for (int i = 0; i < 70; i++)
                {
                    Console.Write(".");
                    System.Threading.Thread.Sleep(10);
                }
                Console.Write("Bienvenu dans le menu du projet de PSI de Damien BORIE");
                for (int i = 0; i < 70; i++)
                {
                    Console.Write(".");
                    System.Threading.Thread.Sleep(10);
                }
                Console.Write("\n\n\n\n\n");


                Console.Write("\tQue souhaitez-vous voir" +
                    "\n\n\t\ttapez 1 pour le traitement d'image" +
                    "\n\t\ttapez 2 pour la fractale" +
                    "\n\t\ttapez 3 pour le générateur de QR Code \n");

                string Answer = Console.ReadLine();
                switch (Answer)
                {
                    case "1":
                        LancementTraitementImage();
                        break;
                    case "2":
                        LancementFractale();
                        break;
                    case "3":
                        LancementQRCode();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Entrez un nombre valide !");
                        Console.ForegroundColor = ConsoleColor.White;
                        System.Threading.Thread.Sleep(1000);

                        break;
                }


            }
            Console.ReadKey();
        }
        public static void LancementTraitementImage()
        {
            bool choixbon = false;
            string path = "";
            while(choixbon == false)
            {
                Console.Clear();
                Console.Write("\n\n\n\n\n");

                Console.Write("\tChoisissez une image" +
                    "\n\n\t\ttapez 1 pour Lena" +
                    "\n\t\ttapez 2 pour Coco" +
                    "\n\t\ttapez 3 pour un chien" +
                    "\n\t\ttapez 4 pour la tour eiffel" + "\n");

                string Answer = Console.ReadLine();

                switch (Answer)
                {
                    case "1":
                        choixbon = true;
                        path += "lena.bmp";
                        break;
                    case "2":
                        choixbon = true;
                        path += "coco.bmp";
                        break;
                    case "3":
                        choixbon = true;
                        path += "chien.bmp";
                        break;
                    case "4":
                        choixbon = true;
                        path += "eiffel.bmp";
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Entrez un nombre valide !");
                        Console.ForegroundColor = ConsoleColor.White;
                        System.Threading.Thread.Sleep(1000);

                        break;
                }
            }
            bool choixtraitementbon = false;

            Bitmap MonImage = new Bitmap(path);

            Process.Start(path);

            while (choixtraitementbon == false)
            {
                Console.Clear();
                Console.Write("\n\n\n\n\n");
                Console.Write("\t\t Voici l'image d'origine");
                Console.Write("\n\tChoisissez un traitement" +
                    "\n\n\t\ttapez 1 pour Nuance de gris" +
                    "\n\t\ttapez 2 pour Noir et blanc" +
                    "\n\t\ttapez 3 pour Miroir horizontal" +
                    "\n\t\ttapez 4 pour Miroir vertical" +
                    "\n\t\ttapez 5 pour Rotation" +
                    "\n\t\ttapez 6 pour Réduire/Agrandir" +
                    "\n\t\ttapez 7 pour Flou" +
                    "\n\t\ttapez 8 pour Repoussage" +
                    "\n\t\ttapez 9 pour Détection des contours" +
                    "\n\t\ttapez 10 pour Histogramme \n");

                string Answer = Console.ReadLine();

                switch (Answer)
                {
                    case "1":
                        choixtraitementbon = true;
                        MonImage.ToGreyScale();
                        break;
                    case "2":
                        choixtraitementbon = true;
                        MonImage.ToBlackAndWhite();
                        break;
                    case "3":
                        choixtraitementbon = true;
                        MonImage.ToMirrorHorizontal();
                        break;
                    case "4":
                        choixtraitementbon = true;
                        MonImage.ToMirrorVertical();
                        break;
                    case "5":
                        choixtraitementbon = true;
                        Console.Clear();
                        Console.Write("\n\n\n\n\n");
                        Console.Write("\t\tChoisissez un angle quelconque : ");
                        double angle = Convert.ToDouble(Console.ReadLine());
                        Console.Write("\n\n\t\tPatientez");
                        MonImage.ToRotate(angle);
                        break;
                    case "6":
                        choixtraitementbon = true;
                        Console.Clear();
                        Console.Write("\n\n\n\n\n");
                        Console.Write("\t\tChoisissez un rapport de dimensionnement ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("(mettre une virgule pour les nombres décimaux et pas un point !) :  ");
                        Console.ForegroundColor = ConsoleColor.White;
                        double ratio = Convert.ToDouble(Console.ReadLine());
                        MonImage.ToScale(ratio);
                        break;
                    case "7":
                        choixtraitementbon = true;
                        MonImage.ToBlur();
                        break;
                    case "8":
                        choixtraitementbon = true;
                        MonImage.ToSharpen();
                        break;
                    case "9":
                        choixtraitementbon = true;
                        MonImage.ToEdgeDetection();
                        break;
                    case "10":
                        choixtraitementbon = true;
                        MonImage.Histogramme();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Entrez un nombre valide !");
                        Console.ForegroundColor = ConsoleColor.White;
                        System.Threading.Thread.Sleep(1000);

                        break;
                }
            }
            Process.Start("ImageDeSortie.bmp");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n\n\n\tAppuyer sur une touche pour revenir au menu principal");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }
        public static void LancementFractale()
        {
            Console.Clear();
            Bitmap MonImage = new Bitmap(10, 10);
            MonImage.Fractales();
            Console.Write("\n\n\n\n\n\n\n\n\t\tVoici la fractale Mandelbrot =>");
            Process.Start("ImageDeSortie.bmp");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n\n\n\tAppuyer sur une touche pour revenir au menu principal");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }
        public static void LancementQRCode()
        {
            Console.Clear();
            Console.Write("\n\n\n\n\n\n\n\n\t\tEntrez une chaine de caractère (inférieur à 47) : ");
            string MessageGenerer = Console.ReadLine();
            int taille = MessageGenerer.Length;
            while (taille > 47)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Entrez un nombre de caractère valide !");
                Console.ForegroundColor = ConsoleColor.White;
                MessageGenerer = Console.ReadLine();
                taille = MessageGenerer.Length;
            } 
            
            QRCode MonQRCode = new QRCode(MessageGenerer);
            Console.Write("\n\n\tSortez votre téléphone ;)");
            Process.Start("ImageDeSortie.bmp");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n\n\n\tAppuyer sur une touche pour revenir au menu principal");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();

        }
    }
}
