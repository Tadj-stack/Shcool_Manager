using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace MyShcool
{

    public partial class MainWindow : Window
    {
        // Creer deux liste ( collections d'objets Program et Car pour stocker les objets et verifier s'ils existent avant de les ajouter.
        List<Program> ProgramsList = new List<Program>();
        List<Stagiaire> StagiaireList = new List<Stagiaire>();




        public MainWindow()
        {
            InitializeComponent();

            //ouvrir la connexion a la base de donnee pour afficher les stagiaire dans la page consulter 
            Connection();

            //remplire les liste de programme dans les combox 
            PopulateProgramsList();

        }

        private void InsertProgram_Click(object sender, RoutedEventArgs e)
        {

            //gere les erreurs avec try catch
            try
            {
                // Check si le box et vide 
                if (string.IsNullOrEmpty(nump.Text) || string.IsNullOrEmpty(NomP.Text) || string.IsNullOrEmpty(duree.Text))
                {
                    MessageBox.Show("SVP!! Remplie toutes les champs");
                    return;
                }

                // Check si le numero est un entier avec les contrainte demander
                if (nump.Text.Length != 7 || !int.TryParse(nump.Text, out int numProgValue))
                {
                    MessageBox.Show("SVP !! Saisie un numero du programme valid");
                    nump.Text = "";
                    return;
                }
                string limitLettre = "^[a-zA-Z]+$";
                if (!Regex.IsMatch(NomP.Text, limitLettre))
                {
                    MessageBox.Show("SVP !! Saisie un nom du programme valid");
                    NomP.Text = "";
                    return;

                }
                // Check si lea duree est un entier moins de 60
                if (!int.TryParse(duree.Text, out int dureeValue) || dureeValue > 60 || dureeValue < 0)
                {

                    duree.Text = "";
                    MessageBox.Show("La duree est incorrecte. Elle doit être un entier positif inférieur à 60.");
                    return;
                }
                //creation dune instance programm
                Program newProgram = new Program(numProgValue.ToString(), NomP.Text, dureeValue.ToString());
                //verifier si le programme existe deja dans la liste des programme 

                try
                {

                    if (!ProgramsList.Contains(newProgram))

                    {

                        //ajouter le nouveau programme a la liste 
                        ProgramsList.Add(newProgram);

                        //inserer le programme dans a la base de donnees
                        InsertProgramIntoDatabase(newProgram);


                        //ajouter le programme a la data grid 
                        ProgTabl.Items.Add(new { NumProg = newProgram.NumProg, NomProg = newProgram.NomProg, Duree = newProgram.Duree });


                    }
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message);
                }




            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}");
            }

            //Vider les champs et assignier le combox 
            Programme.ItemsSource = null;
            Programme.DisplayMemberPath = "NomProg";
            Programme.ItemsSource = ProgramsList;
            nump.Text = "";
            NomP.Text = "";
            duree.Text = "";



        }

        //Inserer un programme dans la BD
        private void InsertProgramIntoDatabase(Program newProgram)
        {
            const string connectionString = "server=127.0.0.1;user=root;password=;database=my_school";

            using (var mySqlCn = new MySqlConnection(connectionString))
            {
                mySqlCn.Open();
                // Insert program data into the 'programme' table
                try
                {
                    using (var mySqlCmd = new MySqlCommand("INSERT INTO programme (num_prog, nom_prog, duree_prog) VALUES (@NumProg, @NomProg, @Duree)", mySqlCn))
                    {
                        mySqlCmd.Parameters.AddWithValue("@NumProg", newProgram.NumProg);
                        mySqlCmd.Parameters.AddWithValue("@NomProg", newProgram.NomProg);
                        mySqlCmd.Parameters.AddWithValue("@Duree", newProgram.Duree);

                        mySqlCmd.ExecuteNonQuery();
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Erreur dans la Base des donnee : {ex.Message}");
                }
            }
        }
        private void PopulateProgramsList()
        {
            ProgramsList.Clear(); // Clear existing programs
            cmbProgrammes.ItemsSource = null; // Clear ComboBox items source

            const string connectionString = "server=127.0.0.1;user=root;password=;database=my_school";
            try
            {
                using (var mySqlCn = new MySqlConnection(connectionString))
                {
                    mySqlCn.Open();

                    using (var mySqlCmd = new MySqlCommand("SELECT * FROM programme", mySqlCn))
                    {
                        using (MySqlDataReader mySqlReader = mySqlCmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            try
                            {
                                while (mySqlReader.Read())
                                {
                                    Program program = new Program(mySqlReader.GetString(1), mySqlReader.GetString(0), mySqlReader.GetString(2));
                                    ProgramsList.Add(program);
                                }
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show($"Erreur dans la Base des donnee : {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Erreur dans la Base des donnee : {ex.Message}");
            }


            // Update les ComboBox avec la liste des programmes
            cmbProgrammes.ItemsSource = ProgramsList;
            cmbProgrammes.DisplayMemberPath = "NomProg";
            Programme.ItemsSource = ProgramsList;
            Programme.DisplayMemberPath = "NomProg";
        }


        //button pour ajouter stagiaires
        private void AjouterStagiaire(object sender, RoutedEventArgs e)
        {

            Program selectedProg = Programme.SelectedItem as Program;
            //Verifier si les champs sont bien remplie 


            if (string.IsNullOrEmpty(Nom.Text) || string.IsNullOrEmpty(prenom.Text) || string.IsNullOrEmpty(NumeroEtud.Text) || string.IsNullOrEmpty(Sexe.Text) || string.IsNullOrEmpty(Programme.Text))
            {
                MessageBox.Show("Please fill in all textboxes.");

                return;
            }

            //contrainte pour le num d etudiant 

            if (NumeroEtud.Text.Length != 7)
            {
                MessageBox.Show("SVP !! Saisie un numero d'etudiant valid");
                NumeroEtud.Text = "";
                return;
            }
            //contrainte pour le nom d etudiant

            string limitLettre = "^[a-zA-Z]+$";
            if (!Regex.IsMatch(Nom.Text, limitLettre))
            {
                MessageBox.Show("SVP !! Saisie un nom d'etudiant valid");
                Nom.Text = "";
                return;

            }
            //contrainte pour le prenom d etudiant

            if (!Regex.IsMatch(prenom.Text, limitLettre))
            {
                MessageBox.Show("SVP !! Saisie un prenom d'etudiant valid");
                prenom.Text = "";
                return;

            }



            if (!datenai.SelectedDate.HasValue)
            {
                MessageBox.Show("Veuillez sélectionner une date de naissance.");

                return;
            }


            //obtention de la date 
            DateTime selectedDate = datenai.SelectedDate.Value.Date;


            string selectedGender = string.Empty;

            // verifier si le utilisateur a choisi un genre

            if (Sexe.SelectedItem != null)
            {
                // Extract le contenue de combobox 
                selectedGender = (Sexe.SelectedItem as ComboBoxItem)?.Content?.ToString();
            }


            // associer la liste des programme avec la case programme 
            Program selctedProg = Programme.SelectedItem as Program;

            //cree une instance de stagiaire
            Stagiaire newStagiaire = new Stagiaire(Nom.Text, prenom.Text, NumeroEtud.Text, selctedProg.NomProg, selectedDate.ToShortDateString(), selectedGender.ToUpper());



            if (!StagiaireList.Contains(newStagiaire))
            {
                StagiaireList.Add(newStagiaire);

                s.Items.Add(newStagiaire);


                //inserer un stagiaire dans la table stagiaire dans notre base de donnee


                InsertStagiaireIntoDatabase(newStagiaire);


            }
            else
            {
                MessageBox.Show("Stagiaire existe deja .");
            }

            //vider lesa cases de formulaire 
            Nom.Text = "";
            prenom.Text = "";
            NumeroEtud.Text = "";
            Sexe.SelectedItem = null; ;
            datenai.SelectedDate = null;
            Programme.SelectedItem = null;


        }

        /// <summary>
        //ajouter un stagiare a la table stagiaire

        private void InsertStagiaireIntoDatabase(Stagiaire newStagiaire)
        {
            const string connectionString = "server=127.0.0.1;user=root;password=;database=my_school";
            try
            {
                using (var mySqlCn = new MySqlConnection(connectionString))
                {
                    mySqlCn.Open();

                    Program selctedProg = Programme.SelectedItem as Program;

                    try
                    {
                        // inserer stagiaire data a la table 'stagiare' table avec la requete sql
                        using (var mySqlCmd = new MySqlCommand("INSERT INTO stagiare (nom, prenom, num_etudiant, sexe, nom_prog, date_naissance) VALUES (@Nom, @Prenom, @NumeroEtudiant, @Sexe, @NomProg, @DateNaissance)", mySqlCn))
                        {
                            //verification de genre
                            string selectedGender = Sexe.SelectedItem.ToString();

                            if (selectedGender.Contains("Male"))
                            {
                                newStagiaire.Sexe = "Male";
                            }
                            else if (selectedGender.Contains("Female"))
                            {
                                newStagiaire.Sexe = "Female";
                            }
                            else
                            {
                                newStagiaire.Sexe = "Other";
                            }

                            //insertion des colonne avec et des valeurs 
                            mySqlCmd.Parameters.AddWithValue("@Nom", newStagiaire.NomStage);
                            mySqlCmd.Parameters.AddWithValue("@Prenom", newStagiaire.PrenomStage);
                            mySqlCmd.Parameters.AddWithValue("@NumeroEtudiant", newStagiaire.NumeroEtudiant);
                            mySqlCmd.Parameters.AddWithValue("@Sexe", newStagiaire.Sexe);
                            mySqlCmd.Parameters.AddWithValue("@NomProg", selctedProg.NomProg);
                            mySqlCmd.Parameters.AddWithValue("@DateNaissance", newStagiaire.DateNaissance);

                            mySqlCmd.ExecuteNonQuery();
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show($"Erreur dans la Base des donnee : {ex.Message}");
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}");
            }
        }

        //une methode pour populer la table dans la fenetre consulter 
        public void Connection()
        {
            const string M_str_sqlcon = "server=127.0.0.1;user=root;password=;database=my_school";
            try
            {
                using (var mySqlCn = new MySqlConnection(M_str_sqlcon))
                {
                    using (var mySqlCmd = new MySqlCommand("SELECT  * FROM stagiare ", mySqlCn))
                    {
                        mySqlCn.Open();

                        using (MySqlDataReader mySqlReader = mySqlCmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (mySqlReader.Read())
                            {
                                Stagiaire newStagiaire = new Stagiaire(mySqlReader.GetString(0), mySqlReader.GetString(1), mySqlReader.GetString(2), mySqlReader.GetString(3), mySqlReader.GetString(4).ToString(), mySqlReader.GetString(5).ToString());
                                listeStagiaires.Items.Add(newStagiaire);


                            }
                        }
                    }

                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Erreur dans la Base des donnee : {ex.Message}");
            }
        }






        //methode de recherche dans la page consulter pour faire les recherches dans la base des donnee
        private void ChercherButton_Click(object sender, RoutedEventArgs e)
        {
            listeStagiaires.Items.Clear();

            // Get le programme de combobox

            Program selectedProgram = cmbProgrammes.SelectedItem as Program;

            if (selectedProgram == null)
            {
                MessageBox.Show("Veuillez sélectionner un programme.");
                return;
            }

            // Get le nom de stagiaire de TextBox

            string stagiaireName = stagiairenom.Text;

            const string connectionString = "server=127.0.0.1;user=root;password=;database=my_school";
            try
            {
                using (var mySqlCn = new MySqlConnection(connectionString))
                {
                    mySqlCn.Open();

                    //Execution de la requete SQl pour obtenir et filtrer les stagiaire de la base des donnees

                    using (var mySqlCmd = new MySqlCommand("SELECT * FROM stagiare WHERE nom_prog = @Programme AND nom LIKE @Nom", mySqlCn))
                    {
                        mySqlCmd.Parameters.AddWithValue("@Programme", selectedProgram.NomProg);
                        mySqlCmd.Parameters.AddWithValue("@Nom", $"%{stagiaireName}%");

                        using (MySqlDataReader mySqlReader = mySqlCmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {

                            //obtention et l affichage de stagiaire volue dans la datagride consulter 
                            while (mySqlReader.Read())
                            {
                                Stagiaire newStagiaire = new Stagiaire(mySqlReader.GetString(0), mySqlReader.GetString(1), mySqlReader.GetString(2), mySqlReader.GetString(3), mySqlReader.GetString(4), mySqlReader.GetString(5));
                                listeStagiaires.Items.Add(newStagiaire);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Erreur dans la Base des donnee : {ex.Message}");
            }
        }



        //3 methode pour vider les cases dans chaque fenetred
        private void SupprimerButton_Programme(object sender, RoutedEventArgs e)
        {
            nump.Text = "";
            NomP.Text = "";
            duree.Text = "";
        }

        private void SupprimerButton_Stagiaire(object sender, RoutedEventArgs e)
        {
            Nom.Text = "";
            prenom.Text = "";
            NumeroEtud.Text = "";
            Sexe.Items.Clear();
            datenai.SelectedDate = null;
            Programme.SelectedItem = null;


        }

        private void SupprimerButton_Consult(object sender, RoutedEventArgs e)
        {
            cmbProgrammes.SelectedItem = null;
            stagiairenom.Text = "";

        }



 
    }

    public class Program : IEquatable<Program>
    {
        //creation de la classe programme avec les proprite 
        public Program(string NumProg, string Nomprog, string Duree)
        {
            this.NumProg = NumProg;
            this.NomProg = Nomprog;
            this.Duree = Duree;
        }
        public string NumProg { get; set; }
        public string NomProg { get; set; }
        public string Duree { get; set; }


        //la methode equels pour comparer 2 programme si sont identique 
        public bool Equals(Program NewProgram)
        {
            if (this.NumProg == NewProgram.NumProg && this.NomProg == NewProgram.NomProg
                && this.Duree == NewProgram.Duree)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
    // Classe représentant un stagiaire avec implémentation de l'interface IEquatable pour la comparaison d'égalité.

    public class Stagiaire : IEquatable<Stagiaire>
    {
        // Constructeur de la classe Stagiaire.


        public Stagiaire(string NomStage, string PrenomStage, string NumeroEtudiant, string Programme, string DateNaissance, string Sexe)
        {
            // Initialisation des propriétés avec les valeurs passées en paramètres.
            this.NomStage = NomStage;
            this.PrenomStage = PrenomStage;
            this.Programme = Programme;
            this.NumeroEtudiant = NumeroEtudiant;
            this.Sexe = Sexe;
            this.DateNaissance = DateNaissance;
        }

        public string NomStage { get; set; }

        public string DateNaissance { get; set; }
        public string PrenomStage { get; set; }


        public string NumeroEtudiant { get; set; }


        public string Sexe { get; set; }

        public string Programme { get; set; }

        public bool Equals(Stagiaire NewStagiaire)
        {
            // Verifie si les proprietes spécifiques sont egales entre l'objet actuel et l'objet passe en parametre.
            if (this.NomStage == NewStagiaire.NomStage && this.PrenomStage == NewStagiaire.PrenomStage && this.NumeroEtudiant == NewStagiaire.NumeroEtudiant && this.Sexe == NewStagiaire.Sexe)
            {
                return true; // Les objets sont egaux.
            }
            else
            {
                return false; // Les objets ne sont pas egaux.
            }
        }
    }



}




