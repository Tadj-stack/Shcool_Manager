using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBDemoApp2;
using MongoDBDemoApp;

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

        string mongoConnectionString = "mongodb://127.0.0.1:27017";
        string databaseName = "school_db";
        string collectionNamePrograms = "programs";
        string collectionNameStagiaires = "stagiaires";

        IMongoCollection<Program> programCollection;
        IMongoCollection<Stagiaire> stagiaireCollection;
        List<Stagiaire> StagiaireList = new List<Stagiaire>();

        List<Program> ProgramsList = new List<Program>();





        public MainWindow()
        {
            InitializeComponent();
            try
            {
                InitializeDatabase();
                PopulateProgramsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void InitializeDatabase()
        {
            try
            {
                var client = new MongoClient(mongoConnectionString);
                var db = client.GetDatabase(databaseName);
                programCollection = db.GetCollection<Program>(collectionNamePrograms);
                stagiaireCollection = db.GetCollection<Stagiaire>(collectionNameStagiaires);
            }
            catch (Exception ex)
            {
                throw new Exception($"Database initialization error: {ex.Message}", ex);
            }
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
        private void DisplayProgramsList()
        {
            // Construct a string to hold the ProgramsList details
            string programsDetails = "Programs List:\n\n";

            // Iterate through ProgramsList and append details to the string
            foreach (var program in ProgramsList)
            {
                programsDetails += $"Program ID: {program.Id}\n";
                programsDetails += $"NumProg: {program.NumProg}\n";
                programsDetails += $"NomProg: {program.NomProg}\n";
                programsDetails += $"Duree: {program.Duree}\n\n";
            }

            // Display the programsDetails string in a MessageBox
            MessageBox.Show(programsDetails, "Programs List", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //Inserer un programme dans la BD
        private void InsertProgramIntoDatabase(Program newProgram)
        {

            try
            {
                programCollection.InsertOne(newProgram);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting program into database: {ex.Message}", ex);
            }
        }
        private void PopulateProgramsList()
        {
            try
            {
                ProgramsList.Clear();

                var programs = programCollection.Find(Builders<Program>.Filter.Empty).ToList();
                foreach (var program in programs)
                {
                    ProgramsList.Add(program);
                }
                DisplayProgramsList();

                cmbProgrammes.ItemsSource = ProgramsList;
                cmbProgrammes.DisplayMemberPath = "NomProg";
                Programme.ItemsSource = ProgramsList;
                Programme.DisplayMemberPath = "NomProg";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating programs list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //button pour ajouter stagiaires
        private void AjouterStagiaire(object sender, RoutedEventArgs e)
        {
            if (Programme.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un programme.");
                return;
            }

            if (string.IsNullOrEmpty(Nom.Text) || string.IsNullOrEmpty(prenom.Text) || string.IsNullOrEmpty(NumeroEtud.Text) || string.IsNullOrEmpty(Sexe.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }

            if (NumeroEtud.Text.Length != 7)
            {
                MessageBox.Show("SVP !! Saisie un numero d'etudiant valid");
                NumeroEtud.Text = "";
                return;
            }

            string limitLettre = "^[a-zA-Z]+$";
            if (!Regex.IsMatch(Nom.Text, limitLettre))
            {
                MessageBox.Show("SVP !! Saisie un nom d'etudiant valid");
                Nom.Text = "";
                return;
            }

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

            DateTime selectedDate = datenai.SelectedDate.Value.Date;

            string selectedGender = Sexe.SelectedItem != null ? (Sexe.SelectedItem as ComboBoxItem)?.Content?.ToString().ToUpper() : "";

            Program selectedProgram = Programme.SelectedItem as Program;

            Stagiaire newStagiaire = new Stagiaire(Nom.Text, prenom.Text, NumeroEtud.Text, selectedProgram.NomProg, selectedDate.ToShortDateString(), selectedGender);

            try
            {
                stagiaireCollection.InsertOne(newStagiaire);
                StagiaireList.Add(newStagiaire);
                s.Items.Add(newStagiaire);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}");
            }

            Nom.Text = "";
            prenom.Text = "";
            NumeroEtud.Text = "";
            Sexe.SelectedItem = null;
            datenai.SelectedDate = null;
            Programme.SelectedItem = null;
        }
        /// <summary>
        //ajouter un stagiare a la table stagiaire

        private void InsertStagiaireIntoDatabase(Stagiaire newStagiaire)
        {
            try
            {
                stagiaireCollection.InsertOne(newStagiaire);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting stagiaire into database: {ex.Message}", ex);
            }
        }

        //une methode pour populer la table dans la fenetre consulter 



        //methode de recherche dans la page consulter pour faire les recherches dans la base des donnee
        private void ChercherButton_Click(object sender, RoutedEventArgs e)
        {
            listeStagiaires.Items.Clear();

            Program selectedProgram = cmbProgrammes.SelectedItem as Program;

            if (selectedProgram == null)
            {
                MessageBox.Show("Veuillez sélectionner un programme.");
                return;
            }

            string stagiaireName = stagiairenom.Text;

            try
            {
                var filter = Builders<Stagiaire>.Filter.And(
                    Builders<Stagiaire>.Filter.Eq(s => s.Programme, selectedProgram.NomProg),
                    Builders<Stagiaire>.Filter.Regex(s => s.NomStage, new BsonRegularExpression(stagiaireName, "i"))
                );

                var results = stagiaireCollection.Find(filter).ToList();

                foreach (var stagiaire in results)
                {
                    listeStagiaires.Items.Add(stagiaire);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching for stagiaires: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
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
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
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




