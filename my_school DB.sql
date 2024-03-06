-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Hôte : 127.0.0.1
-- Généré le : lun. 11 déc. 2023 à 00:25
-- Version du serveur : 10.4.32-MariaDB
-- Version de PHP : 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données : `my_school`
--

-- --------------------------------------------------------

--
-- Structure de la table `programme`
--

CREATE TABLE `programme` (
  `nom_prog` varchar(255) NOT NULL,
  `duree_prog` int(2) NOT NULL,
  `num_prog` int(7) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `programme`
--

INSERT INTO `programme` (`nom_prog`, `duree_prog`, `num_prog`) VALUES
('administration', 13, 2711731),
('Programmation', 24, 3221723);

-- --------------------------------------------------------

--
-- Structure de la table `stagiare`
--

CREATE TABLE `stagiare` (
  `nom` text NOT NULL,
  `prenom` varchar(255) NOT NULL,
  `num_etudiant` int(7) NOT NULL,
  `nom_prog` varchar(255) NOT NULL,
  `date_naissance` text NOT NULL,
  `sexe` varchar(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `stagiare`
--

INSERT INTO `stagiare` (`nom`, `prenom`, `num_etudiant`, `nom_prog`, `date_naissance`, `sexe`) VALUES
('zoubeir', 'Belhadj', 2709869, 'Programmation', '1996-11-06', 'M'),
('Kiouas', 'Tadj', 2711731, 'Programmation', '1994-04-12', 'M'),
('Mostafa', 'laghrib', 2783312, 'administration', '1989-01-20', 'M');

--
-- Index pour les tables déchargées
--

--
-- Index pour la table `programme`
--
ALTER TABLE `programme`
  ADD PRIMARY KEY (`nom_prog`);

--
-- Index pour la table `stagiare`
--
ALTER TABLE `stagiare`
  ADD PRIMARY KEY (`num_etudiant`),
  ADD KEY `fk_stagiare_programme` (`nom_prog`);

--
-- Contraintes pour les tables déchargées
--

--
-- Contraintes pour la table `stagiare`
--
ALTER TABLE `stagiare`
  ADD CONSTRAINT `fk_stagiare_programme` FOREIGN KEY (`nom_prog`) REFERENCES `programme` (`nom_prog`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
