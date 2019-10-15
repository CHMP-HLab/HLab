/*
 * TextBoxEx
 * 
 * Date : 21/05/2013
 * 
 * *********************************************************************************************************************************************************************************************************************************************************************/

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// A FAIRE : Téléphone, email, RCS, Siret, Siren... etc (masque ?)

namespace HLab.Base
{
   public class TextBoxEx : TextBox
   {
      //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      // Modes

      public enum Modes
      {
         /// <summary>
         /// Mode normal du TextBox
         /// </summary>
         Texte,

         /// <summary>
         /// N'accepte que des chiffres 
         /// </summary>
         Chiffres,

         /// <summary>
         /// Un nombre entier
         /// </summary>
         Entier,

         /// <summary>
         /// Un nombre décimal avec un nombre de chiffres après la virgule fixe
         /// </summary>
         Double,

         /// <summary>
         /// Un nombre décimal qui n'affiche les chiffres après la virgule (nombre fixe) que si la valeur n'est pas entière
         /// </summary>
         Nombre,

         /// <summary>
         /// Pour une date
         /// </summary>
         Date,

         /// <summary>
         /// Pour une heure
         /// </summary>
         Heure,

         /// <summary>
         /// Adresse e-mail
         /// </summary>
         Email
      }


      //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      // AffichageZero

      public enum AffichageZeros
      {
         Toujours,
         Jamais,
         Libre
      }


      //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      // Mode

      public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(Modes), typeof(TextBoxEx), new UIPropertyMetadata(Modes.Texte, ModeChanged));

      public Modes Mode
      {
         set { SetValue(ModeProperty, value); }
         get { return (Modes)GetValue(ModeProperty); }
      }

      public static void ModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         if(d == null)
            return;

         if(d is TextBox)
         {
            TextBox tb = (d as TextBox);
            Modes mode = (Modes)e.NewValue;

            if(mode == Modes.Email)
            {
               tb.TextChanged += new TextChangedEventHandler(Ex_TextChanged);
            }
            else if(mode != Modes.Texte)
            {
               int decimales = (int)d.GetValue(DecimalesProperty);
               AffichageZeros affichageZero = (AffichageZeros)d.GetValue(AffichageZeroProperty);
               double valeurDouble = (double)d.GetValue(DoubleProperty);
               DateTime valeurDateTime = (DateTime)d.GetValue(DateProperty);
               bool valide = true;
               int debutSelection = tb.SelectionStart;
               int longueurSelection = tb.SelectionLength;

               String txt = Nettoye(tb.Text, mode, ref debutSelection, ref longueurSelection);
               tb.Text = Formate(mode, decimales, affichageZero, txt, ref debutSelection, ref valeurDouble, ref valeurDateTime, ref valide);

////               tb.PreviewKeyDown += Ex_PreviewKeyDown;
            }
////            else
////               tb.PreviewKeyDown -= Ex_PreviewKeyDown;

            // Sauve le BackGround de la TextBox
            //tb.SetValue(SavBackgroundProperty, tb.Background);
         }
      }


      //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      // AffichageZero

      public static readonly DependencyProperty AffichageZeroProperty = DependencyProperty.Register("AffichageZero", typeof(AffichageZeros), typeof(TextBoxEx), new UIPropertyMetadata(AffichageZeros.Toujours));

      public AffichageZeros AffichageZero
      {
         set { SetValue(AffichageZeroProperty, value); }
         get { return (AffichageZeros)GetValue(AffichageZeroProperty); }
      }


      //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      // Decimales

      public static readonly DependencyProperty DecimalesProperty = DependencyProperty.Register("Decimales", typeof(int), typeof(TextBoxEx), new UIPropertyMetadata(2));
      
      public int Decimales
      {
         set { SetValue(DecimalesProperty, value); }
         get { return (int)GetValue(DecimalesProperty); }
      }


      //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      // Date

      public static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(DateTime), typeof(TextBoxEx), new FrameworkPropertyMetadata(DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, DateChanged));//new UIPropertyMetadata(DateTime.MinValue, DateChanged));
      
      public DateTime Date
      {
         set { SetValue(DateProperty, value); }
         get { return (DateTime)GetValue(DateProperty); }
      }

      private static void DateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         TextBoxEx tb = (d as TextBoxEx);         
         if( !tb._bloqueChanged )
         {

            DateTime date = (DateTime)e.NewValue;

            if(date == DateTime.MinValue)
               tb.Text = "__ / __ / ____";
            else
               tb.Text = date.ToString("dd / MM / yyyy");
         }
      }


      public static readonly RoutedEvent DateChangeEvent = EventManager.RegisterRoutedEvent("DateChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextBoxEx));

      public event RoutedEventHandler DateChange
      {
         add { AddHandler(DateChangeEvent, value); }
         remove { RemoveHandler(DateChangeEvent, value); }
      }

      public void DateDuJour()
      {
         this.Date = DateTime.Now;
      }


      //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      // Double

      //[BindableAttribute(true, BindingDirection.TwoWay)]
      //public static readonly DependencyProperty DoubleProperty = DependencyProperty.RegisterAttached("Double", typeof(double), typeof(TextBoxEx), new UIPropertyMetadata(0.0, DoubleChanged));
      public static readonly DependencyProperty DoubleProperty = DependencyProperty.Register("Double", typeof(double), typeof(TextBoxEx), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, DoubleChanged));
      
      public double Double
      {
         set { SetValue(DoubleProperty, value); }
         get { return (double)GetValue(DoubleProperty); }
      }

      private static void DoubleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         TextBoxEx tb = (d as TextBoxEx);
         if(!tb._bloqueChanged)
         {

            Modes mode = (Modes)tb.GetValue(ModeProperty);
            int decimales = tb.Decimales;// GetDecimales(tb);
            AffichageZeros affichageZero = ((TextBoxEx)d).AffichageZero;// GetAffichageZero(tb);

            int debutSelection = 0;

            double valeurDouble = 0.0;
            DateTime valeurDateTime = DateTime.MinValue;
            bool valide = true;

            //if( mode == Modes.Chiffres )
               tb.Text = Formate(mode, decimales, affichageZero, e.NewValue.ToString(), ref debutSelection, ref valeurDouble, ref valeurDateTime, ref valide);
            //else
           //    tb.Text = Formate(mode, decimales, affichageZero, ((double)e.NewValue).ToString("N"+decimales), ref debutSelection, ref valeurDouble, ref valeurDateTime, ref valide);
         }
      }

      //[BindableAttribute(true, BindingDirection.TwoWay)]


      /*public static double GetDouble(DependencyObject obj)
      {
         return (double)obj.GetValue(DoubleProperty);
      }

      public static void SetDouble(DependencyObject obj, double value)
      {
         obj.SetValue(DoubleProperty, value);

         TextBox tb = (obj as TextBox);
         Modes mode = (Modes)tb.GetValue(ModeProperty);
         int decimales = GetDecimales(tb);
         AffichageZeros affichageZero = GetAffichageZero(tb);

         int debutSelection = 0;

         double valeurDouble = 0.0;
         DateTime valeurDateTime = DateTime.MinValue;
         bool valide = true;
         
         tb.Text = Formate(mode, decimales, affichageZero, value.ToString(), ref debutSelection, ref valeurDouble, ref valeurDateTime, ref valide);
      }*/


      public static readonly RoutedEvent DoubleChangeEvent = EventManager.RegisterRoutedEvent("DoubleChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextBoxEx));

      public event RoutedEventHandler DoubleChange
      {
         add { AddHandler(DoubleChangeEvent, value); }
         remove { RemoveHandler(DoubleChangeEvent, value); }
      }


      //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      // Valide

      public static readonly DependencyProperty ValideProperty = DependencyProperty.RegisterAttached("Valide", typeof(bool), typeof(TextBoxEx), new UIPropertyMetadata(true));

      public static bool GetValide(DependencyObject d)
      {
         return (bool)d.GetValue(ValideProperty);
      }

      public static void SetValide(DependencyObject d, bool value)
      {
         d.SetValue(ValideProperty, value);
      }

      public bool Valide
      {
         set { SetValue(ValideProperty, value); }
         get { return (bool)GetValue(ValideProperty); }
      }

      public static readonly RoutedEvent ValideChangeEvent = EventManager.RegisterRoutedEvent("ValideChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextBoxEx));

      public event RoutedEventHandler ValideChange
      {
         add { AddHandler(ValideChangeEvent, value); }
         remove { RemoveHandler(ValideChangeEvent, value); }
      }

      private bool _bloqueChanged = false;


      //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
      // Eclaire

      public static readonly DependencyProperty EclaireProperty = DependencyProperty.RegisterAttached("Eclaire", typeof(bool), typeof(TextBoxEx), new UIPropertyMetadata(false));

      public static bool GetEclaire(DependencyObject d)
      {
         return (bool)d.GetValue(EclaireProperty);
      }

      public static void SetEclaire(DependencyObject d, bool value)
      {
         d.SetValue(EclaireProperty, value);
      }


      /********************************************************************************************************************************************************************************************************************************************************************************
      * 
      * Texte qui change
      * 
      ***********************************************************************************************************************************************************************************************************************************************************************************/

      static void Ex_TextChanged(object sender, TextChangedEventArgs e)
      {
         TextBox tb = sender as TextBox;
         Modes mode = (Modes)tb.GetValue(ModeProperty);

         // Vérifie l'adresse e-mail
         if(mode == Modes.Email)
         {
            bool valide = ValideEmail(tb.Text);

            if((bool)tb.GetValue(ValideProperty) != valide)
            {
               tb.SetValue(ValideProperty, valide);
               tb.RaiseEvent(new RoutedEventArgs(TextBoxEx.ValideChangeEvent, tb));
            }
         }
      }


      /********************************************************************************************************************************************************************************************************************************************************************************
      * 
      * Touches
      * 
      ***********************************************************************************************************************************************************************************************************************************************************************************/

      protected override void OnPreviewKeyDown(KeyEventArgs e)
      {
         if(Mode == Modes.Email || Mode == Modes.Texte)
            return;

         e.Handled = true;

         // Si on est en mode lecture seule
         if(IsReadOnly)
            return;

         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Convertie la touche en lettre

         String lettre = "";
         switch(e.Key)
         {
            // 0
            case Key.NumPad0:
            case Key.D0: lettre = "0"; break;

            // 1
            case Key.NumPad1:
            case Key.D1: lettre = "1"; break;

            // 2
            case Key.NumPad2:
            case Key.D2: lettre = "2"; break;

            // 3
            case Key.NumPad3:
            case Key.D3: lettre = "3"; break;

            // 4
            case Key.NumPad4:
            case Key.D4: lettre = "4"; break;

            // 5
            case Key.NumPad5:
            case Key.D5: lettre = "5"; break;

            // 6
            case Key.NumPad6:
            case Key.D6: lettre = "6"; break;

            // 7
            case Key.NumPad7:
            case Key.D7: lettre = "7"; break;

            // 8
            case Key.NumPad8:
            case Key.D8: lettre = "8"; break;

            // 9
            case Key.NumPad9:
            case Key.D9: lettre = "9"; break;

            // Virgule
            case Key.Decimal:
            case Key.OemComma:
            case Key.OemPeriod: lettre = ","; break;

            // Retour
            case Key.Back: lettre = "R"; break;

            // Supprimer
            case Key.Delete: lettre = "S"; break;

            // Droite
            case Key.Right: lettre = "D"; break;

            // Gauche
            case Key.Left: lettre = "G"; break;

            // Touches a laisser passer
            case Key.Up:
            case Key.Down:
            case Key.Enter:
            case Key.Tab:
            case Key.PageUp:
            case Key.PageDown:
            case Key.Home:
            case Key.End: e.Handled = false; return;

            // Les autres touches à interdire
            default: return;
         }


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Paramètres existants

         int debutSelection    = SelectionStart;
         int longueurSelection = SelectionLength;


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Traite les cas particuliers 

         if(Mode == Modes.Entier && Text == "0" && debutSelection == 0 && longueurSelection == 0)
            debutSelection = 1;

         if((Mode == Modes.Double || Mode == Modes.Nombre) && debutSelection == 0 && longueurSelection == 0 && Text.Length > 0 && Text[0] == '0')
            debutSelection = 1;


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Nettoye la chaine de caractère en fonction du mode

         String txt = Nettoye(Text, Mode, ref debutSelection, ref longueurSelection);


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Traite la chaine nettoyée

         if(lettre == "D")
         {
            if(debutSelection < txt.Length)
               debutSelection++;
         }
         else if(lettre == "G")
         {
            if(debutSelection > 0)
               debutSelection--;
         }
         else if(lettre == "R")
         {
            // Remplace par des '_'
            if(Mode == Modes.Date || Mode == Modes.Heure)
            {
               if(longueurSelection == 0)
               {
                  if(debutSelection > 0)
                  {
                     debutSelection--;
                     txt = txt.Substring(0, debutSelection) + '_' + txt.Substring(debutSelection + 1, txt.Length - debutSelection - 1);
                  }
               }
               else
                  txt = txt.Substring(0, debutSelection) + String.Empty.PadRight(longueurSelection, '_') + txt.Substring(debutSelection + longueurSelection, txt.Length - debutSelection - longueurSelection);
            }

            // Les autres cas
            else
            {
               if(longueurSelection == 0)
                  if(debutSelection > 0)
                  {
                     debutSelection--;
                     if((Mode != Modes.Double && Mode != Modes.Nombre) || txt[debutSelection] != ',') // Ne supprime pas la virgule pour les doubles
                        longueurSelection = 1;
                  }

               txt = txt.Substring(0, debutSelection) + txt.Substring(debutSelection + longueurSelection, txt.Length - debutSelection - longueurSelection);
            }
         }
         else if(lettre == "S")
         {
            // Remplace par des '_'
            if(Mode == Modes.Date || Mode == Modes.Heure)
            {
               if(longueurSelection == 0)
               {
                  if(debutSelection < txt.Length)
                  {
                     txt = txt.Substring(0, debutSelection) + '_' + txt.Substring(debutSelection + 1, txt.Length - debutSelection - 1);
                     debutSelection++;
                  }
               }
               else
                  txt = txt.Substring(0, debutSelection) + String.Empty.PadRight(longueurSelection, '_') + txt.Substring(debutSelection + longueurSelection, txt.Length - debutSelection - longueurSelection);
            }

            // Les autres cas
            else
            {
               if(longueurSelection == 0)
                  if(debutSelection < txt.Length)
                  {
                     if((Mode == Modes.Double || Mode == Modes.Nombre) && txt[debutSelection] == ',') // Ne supprime pas la virgule pour les doubles
                        debutSelection++;
                     else
                        longueurSelection = 1;
                  }

               txt = txt.Substring(0, debutSelection) + txt.Substring(debutSelection + longueurSelection, txt.Length - debutSelection - longueurSelection);
            }
         }
         else if(lettre == ",")
         {
            // Ignore la virgule
            if(Mode == Modes.Chiffres || Mode == Modes.Entier || Mode == Modes.Date || Mode == Modes.Heure)
               return;

            // Ajoute la virgule
            txt = txt.Substring(0, debutSelection) + "," + txt.Substring(debutSelection + longueurSelection, txt.Length - debutSelection - longueurSelection);
            debutSelection++;
         }
         else
         {
            // Remplace les caractères tapés
            if(Mode == Modes.Date || Mode == Modes.Heure)
               if(debutSelection < txt.Length)
               {
                  if(longueurSelection == 0)
                     longueurSelection = 1;
                  else
                     lettre = lettre.PadRight(longueurSelection, '_');
               }

            // Insert les caractères
            txt = txt.Substring(0, debutSelection) + lettre + txt.Substring(debutSelection + longueurSelection, txt.Length - debutSelection - longueurSelection);
            debutSelection++;
         }

         // Supprime les éventuelles autres virgules
         txt = Nettoye(txt, Mode, ref debutSelection, ref longueurSelection);


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Reformate la chaine résultante

         double valeurDouble = 0.0;
         DateTime valeurDateTime = DateTime.MinValue;
         bool valide = true;
         Text = Formate(Mode, Decimales, AffichageZero, txt, ref debutSelection, ref valeurDouble, ref valeurDateTime, ref valide);
         SelectionStart = debutSelection;
         SelectionLength = 0;

         _bloqueChanged = true;

         if(Mode == Modes.Date || Mode == Modes.Heure)
         {
            if(this.Date != valeurDateTime)
            {
               SetValue(DateProperty, valeurDateTime);
               RaiseEvent(new RoutedEventArgs(DateChangeEvent, this));
            }
         }
         else
         {
            if(this.Double != valeurDouble)
            {
               SetValue(DoubleProperty, valeurDouble);
               RaiseEvent(new RoutedEventArgs(DoubleChangeEvent, this));
            }
         }

         if(this.Valide != valide)
         {
            SetValue(ValideProperty, valide);
            RaiseEvent(new RoutedEventArgs(ValideChangeEvent, this));
         }

         _bloqueChanged = false;
      }


/*
      static void Ex_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
      {
         if(!(sender is TextBox))
            return;

         e.Handled = true;
         TextBoxEx tb = sender as TextBoxEx;

         if(tb.IsReadOnly)
            return;

         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Convertie la touche en lettre

         String lettre = "";
         switch(e.Key)
         {
            // 0
            case Key.NumPad0:
            case Key.D0: lettre = "0"; break;

            // 1
            case Key.NumPad1:
            case Key.D1: lettre = "1"; break;

            // 2
            case Key.NumPad2:
            case Key.D2: lettre = "2"; break;

            // 3
            case Key.NumPad3:
            case Key.D3: lettre = "3"; break;

            // 4
            case Key.NumPad4:
            case Key.D4: lettre = "4"; break;

            // 5
            case Key.NumPad5:
            case Key.D5: lettre = "5"; break;

            // 6
            case Key.NumPad6:
            case Key.D6: lettre = "6"; break;

            // 7
            case Key.NumPad7:
            case Key.D7: lettre = "7"; break;

            // 8
            case Key.NumPad8:
            case Key.D8: lettre = "8"; break;

            // 9
            case Key.NumPad9:
            case Key.D9: lettre = "9"; break;

            // Virgule
            case Key.Decimal:
            case Key.OemComma:
            case Key.OemPeriod: lettre = ","; break;

            // Retour
            case Key.Back: lettre = "R"; break;

            // Supprimer
            case Key.Delete: lettre = "S"; break;

            // Droite
            case Key.Right: lettre = "D"; break;

            // Gauche
            case Key.Left: lettre = "G"; break;

            // Touches a laisser passer
            case Key.Up:
            case Key.Down:
            case Key.Enter:
            case Key.Tab:
            case Key.PageUp:
            case Key.PageDown:
            case Key.Home:
            case Key.End: e.Handled = false; return;

            // Les autres touches à interdire
            default: return;
         }


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Paramètres existants

         int debutSelection = tb.SelectionStart;
         int longueurSelection = tb.SelectionLength;
         Modes mode = (Modes)tb.GetValue(ModeProperty);
         int decimales = tb.Decimales;
         AffichageZeros affichageZero = ((TextBoxEx)tb).AffichageZero; //GetAffichageZero(tb);


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Traite les cas particuliers 

         if(mode == Modes.Entier && tb.Text == "0" && debutSelection == 0 && longueurSelection == 0)
            debutSelection = 1;

         if((mode == Modes.Double || mode == Modes.Nombre) && debutSelection == 0 && longueurSelection == 0 && tb.Text.Length > 0 && tb.Text[0] == '0')
            debutSelection = 1;


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Nettoye la chaine de caractère en fonction du mode

         String txt = Nettoye(tb.Text, mode, ref debutSelection, ref longueurSelection);


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Traite la chaine nettoyée

         if(lettre == "D")
         {
            if(debutSelection < txt.Length)
               debutSelection++;
         }
         else if(lettre == "G")
         {
            if(debutSelection > 0)
               debutSelection--;
         }
         else if(lettre == "R")
         {
            // Remplace par des '_'
            if(mode == Modes.Date || mode == Modes.Heure)
            {
               if(longueurSelection == 0)
               {
                  if(debutSelection > 0)
                  {
                     debutSelection--;
                     txt = txt.Substring(0, debutSelection) + '_' + txt.Substring(debutSelection + 1, txt.Length - debutSelection - 1);
                  }
               }
               else
                  txt = txt.Substring(0, debutSelection) + String.Empty.PadRight(longueurSelection, '_') + txt.Substring(debutSelection + longueurSelection, txt.Length - debutSelection - longueurSelection);
            }
               
            // Les autres cas
            else
            {
               if(longueurSelection == 0)
                  if(debutSelection > 0)
                  {
                     debutSelection--;
                     if((mode != Modes.Double && mode != Modes.Nombre) || txt[debutSelection] != ',' ) // Ne supprime pas la virgule pour les doubles
                        longueurSelection = 1;
                  }

               txt = txt.Substring(0, debutSelection) + txt.Substring(debutSelection + longueurSelection, txt.Length - debutSelection - longueurSelection);
            }
         }
         else if(lettre == "S")
         {
            // Remplace par des '_'
            if(mode == Modes.Date || mode == Modes.Heure)
            {
               if(longueurSelection == 0)
               {
                  if(debutSelection < txt.Length)
                  {
                        txt = txt.Substring(0, debutSelection) + '_' + txt.Substring(debutSelection + 1, txt.Length - debutSelection - 1);
                     debutSelection++;
                  }
               }
               else
                  txt = txt.Substring(0, debutSelection) + String.Empty.PadRight(longueurSelection, '_') + txt.Substring(debutSelection + longueurSelection, txt.Length - debutSelection - longueurSelection);
            }

            // Les autres cas
            else
            {
               if(longueurSelection == 0)
                  if(debutSelection < txt.Length)
                  {
                     if((mode == Modes.Double || mode == Modes.Nombre) && txt[debutSelection] == ',') // Ne supprime pas la virgule pour les doubles
                           debutSelection++;
                     else
                        longueurSelection = 1;
                  }

               txt = txt.Substring(0, debutSelection) + txt.Substring(debutSelection + longueurSelection, txt.Length - debutSelection - longueurSelection);
            }
         }
         else if(lettre == ",")
         {
            // Ignore la virgule
            if(mode == Modes.Chiffres || mode == Modes.Entier || mode == Modes.Date || mode == Modes.Heure)
               return;

            // Ajoute la virgule
            txt = txt.Substring(0, debutSelection) + "," + txt.Substring(debutSelection + longueurSelection, txt.Length - debutSelection - longueurSelection);
            debutSelection++;
         }
         else
         {
            // Remplace les caractères tapés
            if(mode == Modes.Date || mode == Modes.Heure)
               if(debutSelection < txt.Length)
               {
                  if(longueurSelection == 0)
                     longueurSelection = 1;
                  else
                     lettre = lettre.PadRight(longueurSelection, '_');
               }

            // Insert les caractères
            txt = txt.Substring(0, debutSelection) + lettre + txt.Substring(debutSelection + longueurSelection, txt.Length - debutSelection - longueurSelection);
            debutSelection++;
         }

         // Supprime les éventuelles autres virgules
         txt = Nettoye(txt, mode, ref debutSelection, ref longueurSelection);


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Reformate la chaine résultante
         
         double valeurDouble = 0.0;
         DateTime valeurDateTime = DateTime.MinValue;
         bool valide = true;
         tb.Text = Formate(mode, decimales, affichageZero, txt, ref debutSelection, ref valeurDouble, ref valeurDateTime, ref valide);
         tb.SelectionStart = debutSelection;
         tb.SelectionLength = 0;

         //_bloqueChanged = true;

         if(mode == Modes.Date || mode == Modes.Heure)
         {
            if((DateTime)tb.GetValue(DateProperty) != valeurDateTime)
            {
               tb.SetValue(DateProperty, valeurDateTime);
               tb.RaiseEvent(new RoutedEventArgs(DateChangeEvent, tb));
            }
         }
         else
         {
            if((double)tb.GetValue(DoubleProperty) != valeurDouble)
            {
               tb.SetValue(DoubleProperty, valeurDouble);
               tb.RaiseEvent(new RoutedEventArgs(DoubleChangeEvent, tb));
            }
         }

         if((bool)tb.GetValue(ValideProperty) != valide)
         {
            tb.SetValue(ValideProperty, valide);
            tb.RaiseEvent(new RoutedEventArgs(ValideChangeEvent, tb));
         }

         _bloqueChanged = false;
      }
      */

      /********************************************************************************************************************************************************************************************************************************************************************************
      * 
      * Nettoye la chaine de caractères originale
      * 
      ***********************************************************************************************************************************************************************************************************************************************************************************/

      public static String Nettoye(String texte, Modes mode, ref int debutSelection, ref int longueurSelection)
      {
         String resultat = "";
         int debutResultat = debutSelection;
         int longeurResultat = longueurSelection;

         // Replace l'eventuelle point par une virgule
         texte = texte.Replace( "." , "," );

         // Supprime tous sauf les nombres et la première virgule
         char[] caracteres = texte.ToCharArray();
         int positionTexte = 0;
         int positionVirgule = -1;
         foreach(char c in caracteres)
         {
            if((c >= '0' && c <= '9') || c == '_' )
               resultat += c;
            else if( (mode == Modes.Double || mode == Modes.Nombre ) && c == ',' && positionVirgule < 0)
            {
               resultat += c;
               positionVirgule = positionTexte;
            }
            else
            {
               if(positionTexte < debutSelection)
                  debutResultat--;
               else if(positionTexte - debutSelection < longueurSelection)
                  longeurResultat--;
            }

            positionTexte++;
         }

         debutSelection = debutResultat;
         longueurSelection = longeurResultat;
         return resultat;
      }


      /********************************************************************************************************************************************************************************************************************************************************************************
      * 
      * Formate la chaine de caractère intermédiaire pour l'affichage final et détermination des valeurs Double et DateTime
      * 
      ***********************************************************************************************************************************************************************************************************************************************************************************/

      public static String Formate(Modes mode, int decimales, AffichageZeros affichageZero, String texte, ref int positionCurseur, ref double valeurDouble, ref DateTime valeurDateTime, ref bool valide)
      {
         String resultat = "";
         valide = true;

         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Chiffres

         if(mode == Modes.Chiffres)
         {
            resultat = texte;
            double.TryParse(resultat, out valeurDouble);
         }


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Entier

         if(mode == Modes.Entier)
         {
            String temp = texte.TrimStart(new char[] { '0' });
            positionCurseur -= texte.Length - temp.Length;
            double.TryParse(temp, out valeurDouble);

            if(positionCurseur < 0)
               positionCurseur = 0;

            // Partie entière
            int chiffre = 0;
            int i = temp.Length - 1;
            while(i >= 0)
            {
               if(chiffre != 0 && chiffre % 3 == 0)
               {
                  resultat = " " + resultat;
                  if(positionCurseur > i)
                     positionCurseur++;
               }
               resultat = temp[i] + resultat;
               i--;
               chiffre++;
            }

            // Traite les valeurs vides
            if(resultat == "")
            {
               if(affichageZero == AffichageZeros.Toujours)
               {
                  resultat = "0";
                  positionCurseur = 1;
               }
               else if(affichageZero == AffichageZeros.Libre)
               {
                  if(texte != "")
                     positionCurseur = 1;
                  else
                     positionCurseur = 0;
               }
            }
         }


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Double

         else if(mode == Modes.Double)
         {
            valeurDouble = FormateDouble(texte, decimales, ref positionCurseur, ref resultat);

            // A FAIRE AffichageZero
         }


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Nombre

         else if(mode == Modes.Nombre)
         {
            valeurDouble = FormateDouble(texte, decimales, ref positionCurseur, ref resultat);

            if(valeurDouble == Math.Truncate(valeurDouble))
            {
               int positionVirgule = resultat.IndexOf(',');
               if(positionCurseur <= positionVirgule)
                  resultat = resultat.Substring(0, positionVirgule);
            }

            // A FAIRE AffichageZero
         }


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Date

         else if(mode == Modes.Date)
         {
            // Prépare le découpage de la date            
            texte = texte.PadRight(8, '_');

            // Jour
            String jour = texte.Substring(0, 2);

            if(jour[0] != '_' && jour[1] == '_')
            {
               if(jour[0] > '3')
               {
                  jour = "0" + jour[0];
                  if(positionCurseur == 1)
                     positionCurseur = 2;
               }
               else
                  jour = jour[0] + "0";
            }

            jour = jour.Replace('_', '0');
            int iJour = int.Parse(jour);

            if(iJour > 31)
            {
               iJour = 31;
               jour = "31";
            }

            if(iJour < 1)
            {
               jour = "__";
               valide = false;
            }


            // Mois
            String mois = texte.Substring(2, 2);

            if(mois[0] != '_' && mois[1] == '_')
            {
               if(mois[0] > '1')
               {
                  mois = "0" + mois[0];
                  if(positionCurseur == 3)
                     positionCurseur = 4;
               }
               else
                  mois = mois[0] + "0";
            }

            mois = mois.Replace('_', '0');
            int iMois = int.Parse(mois);

            if(iMois > 12)
            {
               iMois = 12;
               mois = "12";
            }

            if(iMois < 1)
            {
               mois = "__";
               valide = false;
            }


            //Année
            String annee = texte.Substring(4, 4);

            // Que le premier chiffre
            if(annee[0] != '_' && annee[1] == '_' && annee[2] == '_' && annee[3] == '_')
            {
               if(annee[0] == '0') // Années >= 2000
               {
                  annee = "200_";
                  if(positionCurseur == 5)
                     positionCurseur = 7;
               }
            }

            // Les deux premiers chiffres entrés
            if(annee[0] != '_' && annee[1] != '_' && annee[2] == '_' && annee[3] == '_')
            {
               // Cas ou la date ne sera qu'à partir de 2010
               if(annee[0] == '1')
               {
                  if(annee[1] < '9')
                  {
                     annee = "20" + annee[0] + annee[1];
                     if(positionCurseur == 6)
                        positionCurseur = 8;
                  }
               }

               else if(annee[0] > '2')
               {
                  annee = "19" + annee[0] + annee[1];
                  if(positionCurseur == 6)
                     positionCurseur = 8;
               }
            }

            int iAnnee = int.Parse(annee.Replace('_', '0'));

            if(iAnnee > 2050)
               valide = false;

            if(iAnnee < 1900)
               valide = false;

            if(iAnnee < 1)
               annee = "____";


            // Validation de la date
            resultat = jour + " / " + mois + " / " + annee;

            if( resultat.Contains("_") )
               valide = false;            
            
            valeurDateTime = DateTime.MinValue;
            valeurDouble = 0.0;
            if(valide)
            {
               try
               {
                  valeurDateTime = new DateTime(iAnnee, iMois, iJour);
                  valeurDouble = valeurDateTime.Year * 10000.0 + valeurDateTime.Month * 100.0 + valeurDateTime.Day;
               }
               catch(Exception)
               {
                  valide = false;
               }
            }

            //if(valeurDateTime.Day != iJour || valeurDateTime.Month != iMois)
            //   valide = false;

            if(resultat == "__ / __ / ____")
               valide = true;

            // Modifie la position du curseur
            if(positionCurseur >= 4)
               positionCurseur += 6;
            else if(positionCurseur >= 2)
               positionCurseur += 3;
         }


         //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
         // Heure

         else if(mode == Modes.Heure)
         {
            // Prépare le découpage de la date            
            texte = texte.PadRight(6, '_');

            // Heure
            String heure = texte.Substring(0, 2);
            int iHeure = int.Parse(heure.Replace('_', '0'));


            // Minutes
            String minute = texte.Substring(2, 2);

            if(minute[0] != '_' && minute[1] == '_')
            {
               if(minute[0] > '5')
                  minute = "5" + minute[1];
            }

            int iMinute = int.Parse(minute.Replace('_', '0'));

            if(iMinute > 59)
            {
               iMinute = 59;
               minute = "59";
            }

            // Secondes
            String seconde = texte.Substring(4, 2);

            if(seconde[0] != '_' && seconde[1] == '_')
            {
               if(seconde[0] > '5')
                  seconde = "5" + seconde[1];
            }

            int iSeconde = int.Parse(seconde.Replace('_', '0'));

            if(iSeconde > 59)
            {
               iSeconde = 59;
               seconde = "59";
            }

            // Validation de la date
            valeurDateTime = DateTime.MinValue;
            valeurDouble = 0.0;
            if(valide)
            {
               try
               {
                  valeurDateTime = new DateTime(1, 1, 1, iHeure, iMinute, iSeconde);
                  valeurDouble = valeurDateTime.Hour * 10000.0 + valeurDateTime.Minute * 100.0 + valeurDateTime.Second;
               }
               catch(Exception)
               {
                  valide = false;
               }
            }

            //if(valeurDateTime.Day != iJour || valeurDateTime.Month != iMois)
            //   valide = false;

            resultat = heure + " : " + minute + " : " + seconde;
            if(resultat == "__ : __ : __")
               valide = true;

            // Modifie la position du curseur
            if(positionCurseur >= 4)
               positionCurseur += 6;
            else if(positionCurseur >= 2)
               positionCurseur += 3;
         }

         return resultat;
      }


      /********************************************************************************************************************************************************************************************************************************************************************************
      * 
      * Formate la chaine de caractère intermédiaire en Double
      * 
      ***********************************************************************************************************************************************************************************************************************************************************************************/

      private static double FormateDouble(String texte, int decimales, ref int positionCurseur, ref String resultat)
      {
         String temp = texte.TrimStart(new char[] { '0' });
         int tailleTemp = temp.Length;
         positionCurseur -= texte.Length - tailleTemp;

         if(positionCurseur < 0)
            positionCurseur = 0;

         int positionVirgule = temp.IndexOf(',');

         if(positionVirgule == -1)
            positionVirgule = tailleTemp;

         // Partie entière
         int chiffre = 0;
         int i = positionVirgule - 1;
         while(i >= 0)
         {
            if(chiffre != 0 && chiffre % 3 == 0)
            {
               resultat = " " + resultat;
               if(positionCurseur > i)
                  positionCurseur++;
            }
            resultat = temp[i] + resultat;
            i--;
            chiffre++;
         }

         // Partie décimale
         chiffre = 0;
         i = positionVirgule;
         while(i < tailleTemp && chiffre <= decimales)
         {
            resultat += temp[i];
            //if(chiffre != 0 && chiffre % 3 == 0)
            //{
            //   resultat += " ";
            //   if(positionCurseur > i)
            //      positionCurseur++;
            //}
            i++;
            chiffre++;
         }

         if(positionCurseur > resultat.Length)
            positionCurseur = resultat.Length;

         // Remplie après la virgule
         positionVirgule = resultat.IndexOf(',');
         if(positionVirgule == -1)
         {
            resultat += ",";
            positionVirgule = resultat.Length - 1; 
         }
         resultat = resultat.PadRight(positionVirgule + 1 + decimales, '0');

         // Si il n'y a pas de zéro devant
         if(resultat[0] == ',')
         {
            resultat = "0" + resultat;
            positionCurseur++;
         }

         // Valeur double
         return double.Parse(resultat.Replace(" ", ""));
      }


      /********************************************************************************************************************************************************************************************************************************************************************************
      * 
      * Vérifie si l'adresse email est valide
      * 
      ***********************************************************************************************************************************************************************************************************************************************************************************/

     /* private static bool ValideEmail(String email)
      {
         return IsValidEmail(email);

         if(email.Length <= 0)
            return true;

         String[] parties = email.Split(new char[] { '@' });

         if(parties.GetLength(0) != 2)
            return false;

         if(parties[0].Length < 1)
            return false;

         if(parties[1].Length < 3)
            return false;

         String[] partiesDomaine = parties[1].Split(new char[] { '.' });

         if(partiesDomaine.GetLength(0) < 2)
            return false;

         if(partiesDomaine[0].Length < 1)
            return false;

         if(partiesDomaine[partiesDomaine.GetLength(0) - 1].Length < 1)
            return false;

         // Vérifie qu'il n'y a que des caractères autorisés
         // Partie utilisateur
         foreach(char c in parties[0])
         {
            if( c 
         
            }
        // Partie domaine

         return true;
      }*/

      //static bool invalid = false;
      /*
      private static bool ValideEmail(String email)
      {
         //return Regex.IsMatch(email, @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])");
         /*
         return Regex.IsMatch(email, @"^(?'localPart'((((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c"
+@"\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?(([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)|(""(([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?(([\u0021\u0023-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u"
+@"007f])|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?""))((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u00"
+@"0e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?)(\.(((\((((?'paren'\()|(?'-paren'\))|"
+@"([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+"
+@"((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?(([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)|(""(([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?(([\u0021\u0023-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-"
+@"\u0008\u000b\u000c\u000e-\u001f\u007f])))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?""))((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)["
+@"\t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?))*))@(?'domain'((((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\"
+@"u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?("
+@"([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)|(""(([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?(([\u0021\u0023-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007"
+@"f])))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?""))((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\"
+@"r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?)(\.(((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u0"
+@"07f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[\t]+)+))*?(([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)|(""(([ \t]+((\r\"
+@"n)[ \t]+)?|((\r\n)[ \t]+)+)?(([\u0021\u0023-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?""))"
+@"((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u0"
+@"07f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?))*)|(((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\"
+@"([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?\[(([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?([!-Z^-~]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f"
+@"]))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?\]((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n"
+@"\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?))$", RegexOptions.IgnoreCase);
      //return Regex.IsMatch(email, "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\\])");
         */
         //return Regex.IsMatch(email, "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\\])", RegexOptions.IgnoreCase);


      public static bool ValideEmail(String email)
      {
         if(String.IsNullOrEmpty(email))
            return true;

         // Use IdnMapping class to convert Unicode domain names.
         email = Regex.Replace(email, @"(@)(.+)$", DomainMapper);
         if(email == "")
            return false;

         // Return true if strIn is in valid e-mail format.
         return Regex.IsMatch(email, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", RegexOptions.IgnoreCase);
      }

      private static string DomainMapper(Match match)
      {
         // IdnMapping class with default property values.
         IdnMapping idn = new IdnMapping();

         string domainName = match.Groups[2].Value;
         try
         {
            domainName = idn.GetAscii(domainName);
         }
         catch(ArgumentException)
         {
            return "";
            //invalid = true;
         }
         return match.Groups[1].Value + domainName;
      }
   }
}