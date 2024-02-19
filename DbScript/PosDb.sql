-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               8.0.36 - MySQL Community Server - GPL
-- Server OS:                    Win64
-- HeidiSQL Version:             12.5.0.6677
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for postest
CREATE DATABASE IF NOT EXISTS `postest` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `postest`;

-- Dumping structure for table postest.products
CREATE TABLE IF NOT EXISTS `products` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Price` decimal(65,30) NOT NULL,
  `IsAlcohol` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table postest.products: ~6 rows (approximately)
INSERT INTO `products` (`Id`, `Name`, `Price`, `IsAlcohol`) VALUES
	('d107a600-cc9c-11ee-99a9-d8bbc1cfaf05', 'Tissue', 100.000000000000000000000000000000, 0),
	('d1090a4c-cc9c-11ee-99a9-d8bbc1cfaf05', 'Chicken', 3000.000000000000000000000000000000, 0),
	('d10a6501-cc9c-11ee-99a9-d8bbc1cfaf05', 'Bread', 2000.000000000000000000000000000000, 0),
	('d10b3a5a-cc9c-11ee-99a9-d8bbc1cfaf05', 'Beer', 2100.000000000000000000000000000000, 1),
	('d10bc33b-cc9c-11ee-99a9-d8bbc1cfaf05', 'Wine', 3800.000000000000000000000000000000, 1),
	('d10c6806-cc9c-11ee-99a9-d8bbc1cfaf05', 'Whiskey', 1000.000000000000000000000000000000, 1);

-- Dumping structure for table postest.purchases
CREATE TABLE IF NOT EXISTS `purchases` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Quantity` int NOT NULL,
  `Date` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Purchases_ProductId` (`ProductId`),
  KEY `IX_Purchases_UserId` (`UserId`),
  CONSTRAINT `FK_Purchases_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Purchases_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table postest.purchases: ~6 rows (approximately)
INSERT INTO `purchases` (`Id`, `UserId`, `ProductId`, `Quantity`, `Date`) VALUES
	('682ed14d-cc9e-11ee-99a9-d8bbc1cfaf05', '81a3033f-34a4-4926-9246-1522dbd23f47', 'd10c6806-cc9c-11ee-99a9-d8bbc1cfaf05', 1, '2024-02-13 14:06:01.000000'),
	('682fe587-cc9e-11ee-99a9-d8bbc1cfaf05', '81a3033f-34a4-4926-9246-1522dbd23f47', 'd10bc33b-cc9c-11ee-99a9-d8bbc1cfaf05', 2, '2024-02-14 15:07:01.000000'),
	('6830c6f0-cc9e-11ee-99a9-d8bbc1cfaf05', '81a3033f-34a4-4926-9246-1522dbd23f47', 'd10b3a5a-cc9c-11ee-99a9-d8bbc1cfaf05', 5, '2024-02-15 16:08:01.000000'),
	('a0050dd4-ccb1-11ee-99a9-d8bbc1cfaf05', '81a3033f-34a4-4926-9246-1522dbd23f47', 'd10a6501-cc9c-11ee-99a9-d8bbc1cfaf05', 10, '2024-02-10 14:06:01.000000'),
	('a00ef4c5-ccb1-11ee-99a9-d8bbc1cfaf05', '81a3033f-34a4-4926-9246-1522dbd23f47', 'd1090a4c-cc9c-11ee-99a9-d8bbc1cfaf05', 20, '2024-02-11 15:07:01.000000'),
	('a0100a83-ccb1-11ee-99a9-d8bbc1cfaf05', '81a3033f-34a4-4926-9246-1522dbd23f47', 'd107a600-cc9c-11ee-99a9-d8bbc1cfaf05', 50, '2024-02-12 16:08:01.000000');

-- Dumping structure for table postest.usercupons
CREATE TABLE IF NOT EXISTS `usercupons` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Cupon` int NOT NULL,
  `Quantity` int NOT NULL,
  `ExchangedDate` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_UserCupons_UserId` (`UserId`),
  CONSTRAINT `FK_UserCupons_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table postest.usercupons: ~0 rows (approximately)
INSERT INTO `usercupons` (`Id`, `UserId`, `Cupon`, `Quantity`, `ExchangedDate`) VALUES
	('79f848cd-cecf-11ee-9533-d8bbc1cfaf05', '81a3033f-34a4-4926-9246-1522dbd23f47', 500, 3, '2024-02-19 09:04:59.000000'),
	('79f95f1f-cecf-11ee-9533-d8bbc1cfaf05', '81a3033f-34a4-4926-9246-1522dbd23f47', 1000, 1, '2024-02-19 09:04:59.000000');

-- Dumping structure for table postest.userotpcodes
CREATE TABLE IF NOT EXISTS `userotpcodes` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Mobile` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `OtpCode` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsUsed` tinyint(1) NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `ExpiredAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table postest.userotpcodes: ~5 rows (approximately)
INSERT INTO `userotpcodes` (`Id`, `Mobile`, `OtpCode`, `IsUsed`, `CreatedAt`, `ExpiredAt`) VALUES
	('212ffcf4-b77f-4a35-8b7b-dc3c84609aef', '9595131677', '857837', 1, '2024-02-16 06:26:19.909595', '2024-02-16 06:31:19.909595'),
	('2a673d57-4bd4-44d4-9f52-37bb3e2148df', '9595131677', '242440', 1, '2024-02-16 04:58:36.054897', '2024-02-16 05:03:36.054925'),
	('87422145-576a-4af0-a48d-0208e4d73a26', '9595131677', '929442', 0, '2024-02-16 04:46:10.519221', '2024-02-16 04:51:10.519222'),
	('8c1d5999-148d-4494-9794-4f13e49ecf6b', '9595131677', '619440', 1, '2024-02-16 06:22:10.519508', '2024-02-16 06:27:10.519541'),
	('a8c0acff-5d8e-4330-9073-f59bcc932160', '9595131677', '261212', 1, '2024-02-16 09:10:46.690928', '2024-02-16 09:15:46.690954');

-- Dumping structure for table postest.userpoints
CREATE TABLE IF NOT EXISTS `userpoints` (
  `Points` int NOT NULL,
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table postest.userpoints: ~0 rows (approximately)
INSERT INTO `userpoints` (`Points`, `Id`) VALUES
	(8500, '81a3033f-34a4-4926-9246-1522dbd23f47');

-- Dumping structure for table postest.users
CREATE TABLE IF NOT EXISTS `users` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FirstName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LastName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CountryCode` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Mobile` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Status` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table postest.users: ~1 rows (approximately)
INSERT INTO `users` (`Id`, `FirstName`, `LastName`, `CountryCode`, `Mobile`, `Status`, `CreatedAt`, `UpdatedAt`) VALUES
	('81a3033f-34a4-4926-9246-1522dbd23f47', 'Aung', 'Myint Than', '95', '95131677', 'Active', '2024-02-16 04:46:42.300161', NULL);

-- Dumping structure for table postest.usersessions
CREATE TABLE IF NOT EXISTS `usersessions` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SessionId` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table postest.usersessions: ~1 rows (approximately)
INSERT INTO `usersessions` (`Id`, `SessionId`) VALUES
	('81a3033f-34a4-4926-9246-1522dbd23f47', 'f6e95595-3bda-49be-abb6-7777f03d4a41');

-- Dumping structure for table postest.__efmigrationshistory
CREATE TABLE IF NOT EXISTS `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table postest.__efmigrationshistory: ~6 rows (approximately)
INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
	('20240215081617_InitialSetup', '8.0.2'),
	('20240216040306_UserUpdated', '8.0.2'),
	('20240216071702_ProductAndPurchaseAdded', '8.0.2'),
	('20240216071955_PurchaseUpdate', '8.0.2'),
	('20240218135032_UserPointsAndUserCuponsAdded', '8.0.2'),
	('20240218135644_UserPointsUpdated', '8.0.2');

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
