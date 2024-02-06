-- Insert into Country
INSERT INTO Country (CountryName)
VALUES
    ('USA'),
    ('Canada'),
    ('UK');

-- Insert into Region
INSERT INTO Region (RegionName, CountryID)
VALUES
    ('New York', 1),
    ('California', 1),
    ('Ontario', 2),
    ('London', 3);

-- Insert into City
INSERT INTO City (CityName, RegionID)
VALUES
    ('New York City', 1),
    ('Los Angeles', 2),
    ('Toronto', 3),
    ('London', 4);

-- Insert into Product
INSERT INTO Product (ProductName)
VALUES
    ('Laptop'),
    ('Smartphone'),
    ('Tablet');

-- Insert into Sale
INSERT INTO Sale  (CustomerName, CountryID, RegionID, CityID, SaleDateTime, ProductID, Quantity)
VALUES
    ('John Doe', 1, 1, 1, '2023-11-01T08:00:00', 1, 50),
    ('Jane Smith', 2, 3, 3, '2023-11-02T10:30:00', 2, 120),
    ('Bob Johnson', 3, 4, 4, '2023-11-03T15:45:00', 3, 30);
