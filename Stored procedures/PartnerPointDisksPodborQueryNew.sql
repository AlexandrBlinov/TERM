/*SELECT TOP 50 cast(row_number as integer) row_number , ProductId, ProductType, ModelName, ModelId, Name, 
ProducerName, Rest, Price, PriceIM, Season FROM ( SELECT row_number() OVER (ORDER BY Products.[Name] ASC) AS [row_number],  Products.ProductId ProductId, Products.ProductType ProductType, ISNULL(Models.Name,'') ModelName,Models.ModelId ModelId,ISNULL(RestOfProducts.Rest,0) Rest, Products.Name Name,  ISNULL(PriceOfPartners.Price,ISNULL(PriceOfProducts.Price,0)) Price, ISNULL(PriceOfProducts.Price1,0) PriceIM,
  ISNULL(Models.Season,'') Season, ISNULL(Producers.Name,'') ProducerName  FROM Products
    LEFT JOIN (Select ProductId, Price from PriceOfPartners WHERE PartnerId=@PartnerId)  PriceOfPartners  ON Products.ProductId=PriceOfPartners.ProductId
	  LEFT JOIN PriceOfProducts ON Products.ProductId=PriceOfProducts.ProductId 
	        INNER JOIN (SELECT  ProductId, CASE WHEN Rest>50 THEN 50 ELSE Rest End Rest FROM RestOfProducts WHERE DepartmentId=@DepartmentId) RestOfProducts ON Products.ProductId=RestOfProducts.ProductId
			  LEFT JOIN Models ON Products.ModelId=Models.ModelId
			      LEFT JOIN Producers ON Products.ProducerID=Producers.ProducerID
				  LEFT JOIN Tiporazmers ON Products.TiporazmerID=Tiporazmers.TiporazmerID
				   WHERE (Products.ProductType='disk' AND Producers.Active=1) -- AND (Products.ProducerID=@ProducerID) AND (Tiporazmers.width=@width) AND (Tiporazmers.Holes=@Hole) AND (Tiporazmers.diametr=@diametr)  
				   )
*/


use YstTerminal

--SELECT GETDATE()


IF OBJECT_ID('tempdb..#RestsOptimal') IS NOT NULL DROP TABLE #RestsOptimal

  CREATE TABLE #RestsOptimal
(   ProductId INT,  Rest INT,  DepartmentId INT,  DaysToDepartment Int) 

 GO

  WITH CTE_RestsAndDays
  AS
  (SELECT RestOfProducts.ProductId ProductId, RestOfProducts.Rest, RestOfProducts.DepartmentId DepartmentId, DepartmentsAvailable.DaysToDepartment
FROM RestOfProducts
INNER JOIN
(
SELECT TOP 1 5 DepartmentID, DaysToMainDepartment DaysToDepartment FROM PartnerPoints Where PartnerPointId=23
UNION ALL 
SELECT TOP 1 DepartmentID, DaysToDepartment FROM PartnerPoints Where PartnerPointId=26 
AND ISNULL(DepartmentID,-1)<>-1 ) DepartmentsAvailable
ON RestOfProducts.DepartmentId=DepartmentsAvailable.DepartmentId )
INSERT INTO #RestsOptimal
SELECT CTE_RestsAndDays.ProductId, Rest, DepartmentId, CTE_RestsAndDays.DaysToDepartment FROM  CTE_RestsAndDays
INNER JOIN
( SELECT ProductId, MIN(DaysToDepartment) DaysToDepartment FROM CTE_RestsAndDays
GROUP BY ProductId ) RestsWithMinDelivery
ON CTE_RestsAndDays.ProductId=RestsWithMinDelivery.ProductId 
AND CTE_RestsAndDays.DaysToDepartment=RestsWithMinDelivery.DaysToDepartment 

GO

SELECT ProductId, Name, ProducerId, Rest, DepartmentId,DaysToDepartment, Price,Price1, PriceBase, Discount, PriceType,
CASE PriceType
WHEN 'zakup' THEN CAST (Price * (1+Discount/100)  AS DECIMAL(15,2)) -- zakup
WHEN 'base' THEN CAST (PriceBase * (1+Discount/100) AS DECIMAL (15,2)) -- base
WHEN 'dont_show_price' THEN -1 --dont_show_price
ELSE Price1 -- reccomended
END PriceOfClient
  FROM
(
 SELECT row_number() OVER (ORDER BY Products.[Name] ASC) AS [row_number],Products.ProductId ProductId, Products.ProducerId, Products.ProductType ProductType, #RestsOptimal.Rest Rest, #RestsOptimal.DaysToDepartment DaysToDepartment, 
#RestsOptimal.DepartmentId DepartmentId,
Products.Name Name,  ISNULL(PriceOfPartners.Price,ISNULL(PriceOfProducts.Price,0)) Price, ISNULL(PriceOfProducts.Price1,0) Price1, ISNULL(PriceOfProducts.PriceBase,0) PriceBase,
IsNULL(PartnerPriceRules.Discount,0) Discount, PartnerPriceRules.PriceType
FROM Products
LEFT JOIN (Select ProductId, Price from PriceOfPartners WHERE PartnerId='92226')  PriceOfPartners  ON Products.ProductId=PriceOfPartners.ProductId
LEFT JOIN PriceOfProducts ON Products.ProductId=PriceOfProducts.ProductId 
LEFT JOIN Models ON Products.ModelId=Models.ModelId
LEFT JOIN Producers ON Products.ProducerID=Producers.ProducerID
INNER JOIN #RestsOptimal 
ON Products.ProductId=#RestsOptimal.ProductId
LEFT JOIN
(SELECT ProducerId, Discount, PriceType FROM PartnerPriceRules 
WHERE PartnerPointId=26 ) PartnerPriceRules
ON Products.ProducerId=PartnerPriceRules.ProducerId
WHERE Products.ProductType = 'disk' AND Products.ProducerId IN (SELECT ProducerId FROM Producers WHERE Active=1 ) 
AND IsNull(PartnerPriceRules.PriceType,'reccommended') <> 'dont_show_producer'
) A





