CREATE PROCEDURE spGetDisks
@PartnerId nvarchar(max)=NULL,
@PartnerPointId int=NULL ,
@ProducerId int=NULL,
@Diametr nvarchar(max),
@Width nvarchar(max)=NULL,
@Hole nvarchar(max)=NULL,
@DIA nvarchar(max)=NULL,
@PCD nvarchar(max)=NULL,
@ET nvarchar(max)=NULL

AS
BEGIN

IF OBJECT_ID('tempdb..#RestsOptimal') IS NOT NULL DROP TABLE #RestsOptimal
  CREATE TABLE #RestsOptimal
(   ProductId INT,  Rest INT,  DepartmentId INT,  DaysToDepartment Int) ;

   WITH CTE_RestsAndDays
  AS
  (SELECT RestOfProducts.ProductId ProductId, RestOfProducts.Rest, RestOfProducts.DepartmentId DepartmentId, DepartmentsAvailable.DaysToDepartment
FROM RestOfProducts
INNER JOIN
(
SELECT 5 DepartmentID, DaysToMainDepartment DaysToDepartment FROM PartnerPoints Where PartnerPointId=@PartnerPointId
UNION ALL
SELECT DepartmentID,DaysToDepartment FROM PartnerPoints Where PartnerPointId=@PartnerPointId AND IsNull(DepartmentID,-1)<>-1
) DepartmentsAvailable
ON RestOfProducts.DepartmentId=DepartmentsAvailable.DepartmentId )
INSERT INTO #RestsOptimal
SELECT CTE_RestsAndDays.ProductId, Rest, DepartmentId, CTE_RestsAndDays.DaysToDepartment FROM  CTE_RestsAndDays
INNER JOIN
( SELECT ProductId, MIN(DaysToDepartment) DaysToDepartment FROM CTE_RestsAndDays
GROUP BY ProductId ) RestsWithMinDelivery
ON CTE_RestsAndDays.ProductId=RestsWithMinDelivery.ProductId 
AND CTE_RestsAndDays.DaysToDepartment=RestsWithMinDelivery.DaysToDepartment 



SELECT row_number, ProductId, Name, ProducerId, 'disk' ProductType , ModelName,Rest, DepartmentId,DaysToDepartment, Price,Price1, PriceBase, Discount, PriceType,
CASE PriceType
WHEN 'zakup' THEN CAST (Price * (1+Discount/100)  AS DECIMAL(15,2)) -- zakup
WHEN 'base' THEN CAST (PriceBase * (1+Discount/100) AS DECIMAL (15,2)) -- base
WHEN 'dont_show_price' THEN 0 --dont_show_price
ELSE Price1 -- reccomended
END PriceOfClient
  FROM
(
SELECT row_number() OVER (ORDER BY Products.[Name] ASC) row_number, Products.ProductId ProductId, Products.ProducerId, Products.ProductType ProductType, ISNULL(Models.Name,'') ModelName, #RestsOptimal.Rest Rest, #RestsOptimal.DaysToDepartment DaysToDepartment, 
#RestsOptimal.DepartmentId DepartmentId,
Products.Name Name,  ISNULL(PriceOfPartners.Price,ISNULL(PriceOfProducts.Price,0)) Price, ISNULL(PriceOfProducts.Price1,0) Price1, ISNULL(PriceOfProducts.PriceBase,0) PriceBase,
IsNULL(PartnerPriceRules.Discount,0) Discount, PartnerPriceRules.PriceType
FROM Products
INNER JOIN #RestsOptimal ON Products.ProductId=#RestsOptimal.ProductId
LEFT JOIN (Select ProductId, Price from PriceOfPartners WHERE PartnerId=@PartnerId)  PriceOfPartners  ON Products.ProductId=PriceOfPartners.ProductId
LEFT JOIN PriceOfProducts ON Products.ProductId=PriceOfProducts.ProductId 
LEFT JOIN Models ON Products.ModelId=Models.ModelId
LEFT JOIN Producers ON Products.ProducerID=Producers.ProducerID
LEFT JOIN Tiporazmers ON Products.TiporazmerID=Tiporazmers.TiporazmerID
LEFT JOIN
(SELECT ProducerId, Discount, PriceType FROM PartnerPriceRules 
WHERE PartnerPointId=@PartnerPointId ) PartnerPriceRules
ON Products.ProducerId=PartnerPriceRules.ProducerId
 WHERE Products.ProductType = 'disk' AND Products.ProducerId IN (SELECT ProducerId FROM Producers WHERE Active=1 ) 
 AND ISNULL(PriceType,'recommended') <> 'dont_show_producer' AND (Products.ProducerId=@ProducerId OR @ProducerId IS NULL) 
 AND (ISNULL(Tiporazmers.Width,'')=@Width OR @Width IS NULL)
 AND (ISNULL(Tiporazmers.DIA,'')=@DIA OR @DIA IS NULL) 
 AND (Tiporazmers.Holes=@Hole OR @Hole IS NULL)
 AND (Tiporazmers.Diametr=@Diametr OR @Diametr IS NULL)
 AND (Tiporazmers.ET=@ET OR @ET IS NULL)
 AND (Tiporazmers.PCD=@PCD OR @PCD IS NULL)
) A

END