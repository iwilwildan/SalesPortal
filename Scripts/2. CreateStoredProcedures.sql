

-- Stored Procedure: GetFilteredSales
CREATE PROCEDURE sp_GetFilteredSales
    @StartDate DATETIME,
    @EndDate DATETIME,
    @SelectedCountry VARCHAR(MAX),
    @SelectedRegion VARCHAR(MAX),
    @SelectedCity VARCHAR(MAX),
    @PageSize INT,
    @PageNumber INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        S.SaleID,
        S.CustomerName,
        C.CountryName,
        ST.RegionName,
        CI.CityName,
        S.SaleDateTime, 
        P.ProductName,
        S.Quantity
    FROM
        Sale S
        INNER JOIN Country C ON S.CountryID = C.CountryID
        INNER JOIN Region ST ON S.RegionID = ST.RegionID
        INNER JOIN City CI ON S.CityID = CI.CityID
        INNER JOIN Product P ON S.ProductID = P.ProductID
    WHERE
        S.SaleDateTime BETWEEN @StartDate AND @EndDate
        AND (C.CountryName IN (SELECT value FROM STRING_SPLIT(@SelectedCountry, ',')) OR @SelectedCountry IS NULL)
        AND (ST.RegionName IN (SELECT value FROM STRING_SPLIT(@SelectedRegion, ',')) OR @SelectedRegion IS NULL)
        AND (CI.CityName IN (SELECT value FROM STRING_SPLIT(@SelectedCity, ',')) OR @SelectedCity IS NULL)
	ORDER BY S.SaleDateTime DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- Stored Procedure: GetCountries
CREATE PROCEDURE sp_GetCountries
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT CountryID, CountryName
    FROM Country;
END;
GO
-- Stored Procedure: GetRegionsByCountry
CREATE PROCEDURE sp_GetRegionsByCountry
    @CountryID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT RegionID, RegionName
    FROM Region
    WHERE CountryID = @CountryID;
END;
GO

CREATE PROCEDURE sp_GetCitiesByRegion
    @RegionID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CityID, CityName
    FROM City
    WHERE RegionID = @RegionID;
END;
GO

CREATE PROCEDURE sp_GetProducts
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ProductID,
        ProductName
    FROM
        Product;
END;
GO
-- Stored Procedure: AddSale
CREATE PROCEDURE sp_AddSale
    @CustomerName VARCHAR(255),
    @CountryID INT,
    @RegionID INT,
    @CityID INT,
    @SaleDateTime DATETIME,
    @ProductID INT,
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO Sale (CustomerName, CountryID, RegionID, CityID, SaleDateTime, ProductID, Quantity)
        VALUES (@CustomerName, @CountryID, @RegionID, @CityID, @SaleDateTime, @ProductID, @Quantity);

        SELECT @@ROWCOUNT AS 'RowCount';  -- Return the number of affected rows
    END TRY
    BEGIN CATCH
        
        SELECT -1 AS 'RowCount';  
    END CATCH;
END;
GO

CREATE PROCEDURE sp_GetTotalSalesPages
    @StartDate DATETIME,
    @EndDate DATETIME,
    @SelectedCountry VARCHAR(MAX),
    @SelectedRegion VARCHAR(MAX),
    @SelectedCity VARCHAR(MAX),
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalRecords INT;

    -- Calculate the total number of records in the Sale table
    SELECT @TotalRecords = COUNT(*)
    FROM Sale S
        INNER JOIN Country C ON S.CountryID = C.CountryID
        INNER JOIN Region ST ON S.RegionID = ST.RegionID
        INNER JOIN City CI ON S.CityID = CI.CityID
        INNER JOIN Product P ON S.ProductID = P.ProductID
    WHERE
        S.SaleDateTime BETWEEN @StartDate AND @EndDate
        AND (C.CountryName IN (SELECT value FROM STRING_SPLIT(@SelectedCountry, ',')) OR @SelectedCountry IS NULL)
        AND (ST.RegionName IN (SELECT value FROM STRING_SPLIT(@SelectedRegion, ',')) OR @SelectedRegion IS NULL)
        AND (CI.CityName IN (SELECT value FROM STRING_SPLIT(@SelectedCity, ',')) OR @SelectedCity IS NULL);

    -- Calculate the total number of pages
    DECLARE @TotalPages INT;
    SET @TotalPages = CEILING(CAST(@TotalRecords AS DECIMAL) / CAST(@PageSize AS DECIMAL));

    -- Return the total number of pages
    SELECT @TotalPages AS TotalPages;
END;
GO

-- Stored Procedure: LogError
CREATE PROCEDURE LogError
    @LogLevel VARCHAR(50),
    @LogMessage NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ErrorLog (LogLevel, LogMessage, LogDate)
    VALUES (@LogLevel, @LogMessage, GETUTCDATE());
END;