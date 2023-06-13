-- phpMyAdmin SQL Dump
-- version 4.9.4
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Generation Time: Jun 13, 2023 at 10:07 AM
-- Server version: 10.6.12-MariaDB
-- PHP Version: 7.4.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `erikdusschooten`
--

-- --------------------------------------------------------

--
-- Table structure for table `Games`
--

CREATE TABLE `Games` (
  `ID` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Games`
--

INSERT INTO `Games` (`ID`, `Name`) VALUES
(1, 'MiniBall'),
(2, 'MiniBlox'),
(3, 'MaxiBall'),
(4, 'MaxiBlox'),
(5, 'BallBlox');

-- --------------------------------------------------------

--
-- Table structure for table `Scores`
--

CREATE TABLE `Scores` (
  `ID` int(11) NOT NULL,
  `PlayerID` int(11) NOT NULL,
  `GameID` int(25) NOT NULL,
  `DateAchieved` datetime NOT NULL DEFAULT current_timestamp(),
  `Score` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Scores`
--

INSERT INTO `Scores` (`ID`, `PlayerID`, `GameID`, `DateAchieved`, `Score`) VALUES
(1, 2, 1, '2021-04-23 00:00:00', 666),
(2, 1, 5, '2021-04-21 00:00:00', 2484),
(3, 4, 3, '2021-04-13 00:00:00', 723),
(4, 3, 4, '2021-03-16 00:00:00', 6342),
(5, 5, 2, '2021-04-06 00:00:00', 621),
(6, 2, 1, '2021-04-23 00:00:00', 420),
(7, 2, 1, '2021-04-23 00:00:00', 999),
(8, 1, 1, '2021-04-30 12:10:55', 123),
(9, 1, 1, '2021-04-30 12:11:50', 444),
(10, 4, 1, '2021-05-12 11:34:08', 73);

-- --------------------------------------------------------

--
-- Table structure for table `Servers`
--

CREATE TABLE `Servers` (
  `id` int(11) NOT NULL,
  `password` varchar(15) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Servers`
--

INSERT INTO `Servers` (`id`, `password`) VALUES
(1, 'pw');

-- --------------------------------------------------------

--
-- Table structure for table `Users`
--

CREATE TABLE `Users` (
  `ID` int(11) NOT NULL,
  `Name` varchar(40) NOT NULL,
  `Email` varchar(60) NOT NULL,
  `Password` varchar(25) NOT NULL,
  `SignUpDate` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `Users`
--

INSERT INTO `Users` (`ID`, `Name`, `Email`, `Password`, `SignUpDate`) VALUES
(1, 'Bob', 'BobBobbie@gmail.com', 'BobTheBest01', '2021-04-23 12:25:07'),
(2, 'Dave', 'DaveDavie@gmail.com', 'DaDave', '2021-04-23 12:25:07'),
(3, 'Anna', 'AnAnnie@gmail.com', 'Annananas', '2021-04-23 12:25:07'),
(4, 'Steve', 'StevePieve@gmail.com', 'WordOfThePass', '2021-04-23 12:25:07'),
(5, 'Eve', 'EveePievee@gmail.com', 'NotAPokemon', '2021-04-23 12:25:07');

-- --------------------------------------------------------

--
-- Table structure for table `UsersLogin`
--

CREATE TABLE `UsersLogin` (
  `UserID` int(11) NOT NULL,
  `username` varchar(25) NOT NULL,
  `hash` varchar(1000) NOT NULL,
  `salt` varchar(1000) NOT NULL,
  `score` int(11) NOT NULL DEFAULT 0,
  `lastplayed` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `UsersLogin`
--

INSERT INTO `UsersLogin` (`UserID`, `username`, `hash`, `salt`, `score`, `lastplayed`) VALUES
(1, 'Erikduss', '$5$rounds=5000$MyEncryptionErik$xVdi07Eb4qDrezMpwecTmPLrP8w3BEmCloFYxRb0WR6', '$5$rounds=5000$MyEncryptionErikduss$', 858, '2023-06-05 16:53:10'),
(2, 'LaptopMe', '$5$rounds=5000$MyEncryptionLapt$23TKNp6KhyYWP1JQKaY0utxYtisCCeYa48lFUIFxpH.', '$5$rounds=5000$MyEncryptionLaptopMe$', 0, '2023-05-15 00:00:00'),
(3, 'ExtraAccount123', '$5$rounds=5000$MyEncryptionExtr$PgW23PJKUMHxbgtgdDu7XiROUApCUxgKkIBsQDgB8F5', '$5$rounds=5000$MyEncryptionExtraAccount123$', 0, '2023-05-15 00:00:00'),
(4, 'test', '$5$rounds=5000$MyEncryptiontest$JfixHY8A/jgS4co5ptTJKRCTDEoorrgE4D6MYMyn6G1', '$5$rounds=5000$MyEncryptiontest$', 700, '2023-06-05 15:03:00'),
(5, 'Testing1', '$5$rounds=5000$MyEncryptionTest$Bo6ksKZI17GYtHJ7V6iKdQ0VSFHkg70QTYD.fJV24g/', '$5$rounds=5000$MyEncryptionTesting1$', 0, '2023-06-13 12:04:03'),
(6, 'Testing2', '$5$rounds=5000$MyEncryptionTest$okDPfIOFicLyeqn1Yp1FEfY6KhyxyPW4F3lPvpxbyHB', '$5$rounds=5000$MyEncryptionTesting2$', 0, '2023-06-13 12:04:56'),
(7, 'Testing3', '$5$rounds=5000$MyEncryptionTest$oyAoh9KOWj/.Gis7samBeAmf.Q0FoCEOQP6x6KSr9K6', '$5$rounds=5000$MyEncryptionTesting3$', 0, '2023-06-13 12:05:18'),
(8, 'Testing4', '$5$rounds=5000$MyEncryptionTest$ifntR5NNhi0aKq.Qf5SvHySOgVygc/L/hD0irxKD2S5', '$5$rounds=5000$MyEncryptionTesting4$', 0, '2023-06-13 12:05:41');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `Games`
--
ALTER TABLE `Games`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `Scores`
--
ALTER TABLE `Scores`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `Servers`
--
ALTER TABLE `Servers`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `Users`
--
ALTER TABLE `Users`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `UsersLogin`
--
ALTER TABLE `UsersLogin`
  ADD PRIMARY KEY (`UserID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `Games`
--
ALTER TABLE `Games`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `Scores`
--
ALTER TABLE `Scores`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `Servers`
--
ALTER TABLE `Servers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `Users`
--
ALTER TABLE `Users`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `UsersLogin`
--
ALTER TABLE `UsersLogin`
  MODIFY `UserID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
