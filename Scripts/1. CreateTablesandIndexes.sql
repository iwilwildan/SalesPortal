-- CREATE DATABASE DigitalAvenuesTest

-- Table: Country
CREATE TABLE Country (
    CountryID INT IDENTITY(1,1) PRIMARY KEY,
    CountryName VARCHAR(255) NOT NULL
);

-- Table: Region
CREATE TABLE Region (
    RegionID INT IDENTITY(1,1) PRIMARY KEY,
    RegionName VARCHAR(255) NOT NULL,
    CountryID INT,
    FOREIGN KEY (CountryID) REFERENCES Country(CountryID)
);

-- Table: City
CREATE TABLE City (
    CityID INT IDENTITY(1,1) PRIMARY KEY,
    CityName VARCHAR(255) NOT NULL,
    RegionID INT,
    FOREIGN KEY (RegionID) REFERENCES Region(RegionID)
);

-- Table: Product
CREATE TABLE Product (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName VARCHAR(255) NOT NULL
);

-- Table: Sale
CREATE TABLE Sale (
    SaleID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName VARCHAR(255) NOT NULL,
    CountryID INT NOT NULL,
    RegionID INT,
    CityID INT,
    SaleDateTime DATETIME NOT NULL,
    ProductID INT,
    Quantity INT,
    FOREIGN KEY (CountryID) REFERENCES Country(CountryID),
    FOREIGN KEY (RegionID) REFERENCES Region(RegionID),
    FOREIGN KEY (CityID) REFERENCES City(CityID),
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);

CREATE TABLE ErrorLog (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    LogLevel VARCHAR(50) NOT NULL,
    LogMessage NVARCHAR(MAX) NOT NULL,
    LogDate DATETIME NOT NULL
);

-- Indexes
CREATE INDEX idx_Sale_SaleDateTime ON Sale (SaleDateTime);
CREATE INDEX idx_Sale_CountryID ON Sale (CountryID);
CREATE INDEX idx_Sale_RegionID ON Sale (RegionID);
CREATE INDEX idx_Sale_CityID ON Sale (CityID);
CREATE INDEX idx_Sale_ProductID ON Sale (ProductID);
