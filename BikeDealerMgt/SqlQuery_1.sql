CREATE DATABASE BikeDealerMgmtDB;

USE BikeDealerMgmtDB;


/*
This one is correct. No need to alter
CREATE TABLE Dealers (
    DealerId INT IDENTITY(1, 1) PRIMARY KEY,
    DealerName NVARCHAR(100) NOT NULL,
    Address NVARCHAR(200) NOT NULL,
    City VARCHAR(50) NULL,
    State VARCHAR(50) NULL,
    ZipCode VARCHAR(20) NULL,
    StorageCapacity INT NULL,
    Inventory INT NULL,
    UserId NVARCHAR(450) NOT NULL,

    -- Ensure one Dealer per Identity user
    CONSTRAINT UX_Dealers_UserId UNIQUE (UserId),

    -- Foreign key to AspNetUsers (with cascade delete)
    CONSTRAINT FK_Dealers_AspNetUsers_UserId FOREIGN KEY (UserId)
        REFERENCES AspNetUsers(Id)
        ON DELETE CASCADE,

    -- Keep non-negative checks when values are present
    CONSTRAINT CK_Dealers_StorageCapacity CHECK (StorageCapacity IS NULL OR StorageCapacity >= 0),
    CONSTRAINT CK_Dealers_Inventory CHECK (Inventory IS NULL OR Inventory >= 0)
);
*/

CREATE TABLE Dealers(
	DealerId INT IDENTITY(1, 1) PRIMARY KEY,
	DealerName NVARCHAR(100) NOT NULL,
	Address NVARCHAR(200) NOT NULL,
	City VARCHAR(50) NOT NULL,
	State VARCHAR(50) NOT NULL,
	ZipCode VARCHAR(20) NOT NULL,
	StorageCapacity INT CHECK(StorageCapacity>=0),
	Inventory INT CHECK(Inventory>=0),
	--CONSTRAINT UQ_Dealer_Name UNIQUE(DealerName)
);

-- Make optional fields nullable
ALTER TABLE Dealers
ALTER COLUMN City VARCHAR(50) NULL;
ALTER TABLE Dealers
ALTER COLUMN State VARCHAR(50) NULL;
ALTER TABLE Dealers
ALTER COLUMN ZipCode VARCHAR(20) NULL;
ALTER TABLE Dealers
ALTER COLUMN StorageCapacity INT NULL;
ALTER TABLE Dealers
ALTER COLUMN Inventory INT NULL;

DELETE FROM Dealers;
DELETE FROM DealerMaster;
DELETE FROM BikeStores;
-- Add linkage to Identity user
ALTER TABLE Dealers
ADD UserId NVARCHAR(450) NOT NULL;

-- Enforce 1:1 (one Dealer row per Identity user)
CREATE UNIQUE INDEX UX_Dealers_UserId ON Dealers(UserId);

-- FK to AspNetUsers
ALTER TABLE Dealers
ADD CONSTRAINT FK_Dealers_AspNetUsers_UserId
FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
ON DELETE CASCADE;

-- (Optional) keep the non-negative checks even when nullable
ALTER TABLE Dealers
ADD CONSTRAINT CK_Dealers_StorageCapacity
CHECK (StorageCapacity IS NULL OR StorageCapacity >= 0);

ALTER TABLE Dealers
ADD CONSTRAINT CK_Dealers_Inventory
CHECK (Inventory IS NULL OR Inventory >= 0);




CREATE TABLE BikeStores(
	BikeId INT IDENTITY(1, 1) PRIMARY KEY,
	ModelName VARCHAR(100) NOT NULL,
	ModelYear INT,
	EngineCC INT,
	Manufacturer VARCHAR(100),
	--CONSTRAINT UQ_BikeStores UNIQUE (ModelName, ModelYear, Manufacturer)
);

CREATE TABLE DealerMaster (
	DealerMasterId INT IDENTITY(1, 1) PRIMARY KEY,
	DealerId INT NOT NULL,
	BikeId INT NOT NULL,
	BikesDelivered INT,
	DeliveryDate DATETIME2,
	CONSTRAINT fk_DM_Dealer FOREIGN KEY (DealerId) REFERENCES Dealers(DealerId),
	CONSTRAINT fk_DM_Bike FOREIGN KEY (BikeId) REFERENCES BikeStores(BikeId)
);


INSERT INTO Dealers (DealerName, Address, City, State, ZipCode, StorageCapacity, Inventory)
VALUES
('Speed Wheels', '123 Main St', 'Hyderabad', 'Telangana', '500001', 100, 50),
('Rider’s Hub', '45 Lake View Rd', 'Bengaluru', 'Karnataka', '560001', 120, 60),
('Moto World', '89 Park Lane', 'Chennai', 'Tamil Nadu', '600001', 80, 40),
('Highway Motors', '67 MG Road', 'Pune', 'Maharashtra', '411001', 150, 70),
('Urban Bikes', '22 City Center', 'Delhi', 'Delhi', '110001', 200, 90);


INSERT INTO BikeStores (ModelName, ModelYear, EngineCC, Manufacturer)
VALUES
('Pulsar 220F', 2023, 220, 'Bajaj'),
('Royal Enfield Classic 350', 2024, 350, 'Royal Enfield'),
('Apache RTR 200', 2023, 200, 'TVS'),
('KTM Duke 390', 2024, 390, 'KTM'),
('Yamaha R15 V4', 2024, 155, 'Yamaha');


INSERT INTO DealerMaster (DealerId, BikeId, BikesDelivered, DeliveryDate)
VALUES
(1, 1, 10, '2024-08-01'),
(2, 2, 8, '2024-08-05'),
(3, 3, 12, '2024-08-10'),
(4, 4, 6, '2024-08-15'),
(5, 5, 15, '2024-08-20');


SELECT * FROM Dealers;
SELECT * FROM DealerMaster;
SELECT * FROM BikeStores;

----Insert Admin role (if not exists):
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());

--Insert Admin User
--Password Admin@123
-- Insert Admin User (Password = Admin@123)
INSERT INTO AspNetUsers
(Id, UserName, NormalizedUserName, Email, NormalizedEmail,
EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
Address, StoreName, GSTNumber, IsDealerVerified, CompanyName, RegistrationNumber, IsManufacturerVerified)
VALUES
(NEWID(), 'admin@bikedekho.com', 'ADMIN@BIKEDEKHO.COM', 'admin@bikedekho.com', 'ADMIN@BIKEDEKHO.COM',
1, 'AQAAAAIAAYagAAAAECJaakNn631n18QLyu6Et+3PBA1Tp5WrM1PHgQXbY/PWK6NAB66BqjHuo6KWxkq9HQ==',
NEWID(), NEWID(), 0, 0, 1, 0,
NULL, NULL, NULL, 0, NULL, NULL, 0);


SELECT * FROM AspNetUsers WHERE UserName = 'admin@bikedekho.com';
SELECT * FROM AspNetRoles WHERE Name = 'Admin';

--Assign role id of admin from above to the admin user
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES ('CA1CFCFB-116E-4944-8627-DFB9B512D2A6', '6e783e54-12c0-435a-bdcb-ef2adc699f48');


SELECT * FROM AspNetRoleClaims;
SELECT * FROM AspNetRoles;
SELECT * FROM AspNetUserClaims;
SELECT * FROM AspNetUserLogins;
SELECT * FROM AspNetUserRoles;
SELECT * FROM AspNetUsers;
SELECT * FROM AspNetUserTokens;

SELECT DealerId, DealerName, UserId FROM Dealers WHERE DealerId = 19;
SELECT Id, UserName FROM AspNetUsers WHERE Id = '745808fe-fa01-44b1-ae8b-65257ccac69f';
select * from aspNetUsers;